using System.Reflection;
using Codec8;

namespace Codec8Cli;

class Program
{
	/// <summary>
	/// Bold
	/// </summary>
	/// <remarks>From https://davidjones.sportronics.com.au/coding/ConsoleTextFormat-Formatting_Console_App_Text-coding.html</remarks>
	private const string b = "\u001b[1m";

	/// <summary>
	/// Not Bold
	/// </summary>
	private const string _b = "\u001b[22m";

	static void Main(string[] args)
	{
		if (args.Length < 1)
		{
			PrintHelp();
			return;
		}
		else if (args.Length == 1)
		{
			if (args[0] == "--version" || args[0] == "-v")
			{
				PrintVersion();
				return;
			}
		}
		
		foreach (string arg in args)
		{
			string input = File.Exists(arg) ? File.ReadAllText(arg) : arg;
			
			(GenericDecodeResult result, object valueOrError) = GenericDecoder.ParseHexadecimalString(input.Replace("-", string.Empty));

			if (result == GenericDecodeResult.SuccessCodec8)
			{
				Codec8Frame frame = (Codec8Frame)valueOrError;
				Codec8Mandatory mandatory = new Codec8Mandatory(frame);
				Console.WriteLine(mandatory.ToString());
				int recordNumber = 1;
				foreach (AvlDataCodec8 avlData in frame.GetAvlDatas())
				{
					GPSElement gps = avlData.GetGPSElement();
					IOElementCodec8 ioElement = avlData.GetIOElement();
					
					Codec8Avl codec8Avl = new Codec8Avl(avlData, gps, ioElement, recordNumber);
					Console.WriteLine(codec8Avl.ToString());
					recordNumber++;
				}
			}
			else if (result == GenericDecodeResult.SuccessCodec8Extended)
			{
				Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
				Codec8ExtendedMandatory mandatory = new Codec8ExtendedMandatory(frame);
				Console.WriteLine(mandatory.ToString());
				int recordNumber = 1;
				foreach (AvlDataCodec8Extended avlData in frame.GetAvlDatas())
				{
					GPSElement gps = avlData.GetGPSElement();
					IOElementCodec8Extended ioElement = avlData.GetIOElement();
					
					Codec8ExtendedAvl codec8Avl = new Codec8ExtendedAvl(avlData, gps, ioElement, recordNumber);
					Console.WriteLine(codec8Avl.ToString());
					recordNumber++;
				}
			}
			else if (result == GenericDecodeResult.SuccessCodec8Udp)
			{
				(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8FrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8FrameNoCRC c))valueOrError;
				Codec8UdpMandatory mandatory = new Codec8UdpMandatory(header, avlDataEncapsulated, frame);
				Console.WriteLine(mandatory.ToString());
				int recordNumber = 1;
				foreach (AvlDataCodec8 avlData in frame.GetAvlDatas())
				{
					GPSElement gps = avlData.GetGPSElement();
					IOElementCodec8 ioElement = avlData.GetIOElement();
					
					Codec8Avl codec8Avl = new Codec8Avl(avlData, gps, ioElement, recordNumber);
					Console.WriteLine(codec8Avl.ToString());
					recordNumber++;
				}
			}
			else if (result == GenericDecodeResult.SuccessCodec8ExtendedUdp)
			{
				(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8ExtendedFrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8ExtendedFrameNoCRC c))valueOrError;
				Codec8ExtendedUdpMandatory mandatory = new Codec8ExtendedUdpMandatory(header, avlDataEncapsulated, frame);
				Console.WriteLine(mandatory.ToString());
				int recordNumber = 1;
				foreach (AvlDataCodec8Extended avlData in frame.GetAvlDatas())
				{
					GPSElement gps = avlData.GetGPSElement();
					IOElementCodec8Extended ioElement = avlData.GetIOElement();
					
					Codec8ExtendedAvl codec8Avl = new Codec8ExtendedAvl(avlData, gps, ioElement, recordNumber);
					Console.WriteLine(codec8Avl.ToString());
					recordNumber++;
				}
			}
			else
			{
				string possibleError = (string)valueOrError;
				Console.WriteLine($"Cannot parse input, error: {possibleError}");
			}
		}
	}

	static void PrintHelp()
	{
		PrintVersion();
		Console.WriteLine("You have to give some parameters for Codec8Cli.");
		Console.WriteLine("e.g.");
		Console.WriteLine($"{b}codec8cli 000000000000002808010000016B40D9AD80010000000000000000000000000000000103021503010101425E100000010000F22A{_b}");
		Console.WriteLine("or");
		Console.WriteLine($"{b}codec8cli inputfile.txt{_b}");
	}

	static void PrintVersion()
	{
		Console.WriteLine($"Codec8Cli v{Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion}");
	}
}
