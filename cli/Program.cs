using System.Reflection;
using Codec8;

namespace Codec8Cli;

public class Program
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

	public static void Main(string[] args)
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
			
			GenericParseResult result = GenericDecoder.GenericParseHexadecimalString(input.Replace("-", string.Empty));
			HandleGenericParseResult(result);
		}
	}

	private static int HandleGenericParseResult(GenericParseResult result) => result switch
	{
		Codec8Frame frame => HandleCodec8Frame(frame),
		Codec8ExtendedFrame frameExtended => HandleCodec8ExtendedFrame(frameExtended),
		Codec8Udp udp => HandleCodec8Udp(udp),
		Codec8ExtendedUdp udpExtended => HandleCodec8ExtendedUdp(udpExtended),
		ParseErrorInfo error => HandleParseError(error),
	};

	private static int HandleCodec8Frame(Codec8Frame frame)
	{
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

		return recordNumber;
	}

	private static int HandleCodec8ExtendedFrame(Codec8ExtendedFrame frame)
	{
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

		return recordNumber;
	}

	private static int HandleCodec8Udp(Codec8Udp udp)
	{
		Codec8UdpMandatory mandatory = new Codec8UdpMandatory(udp.udpChannelHeader, udp.avlDataEncapsulated, udp.frame);
		Console.WriteLine(mandatory.ToString());
		int recordNumber = 1;
		foreach (AvlDataCodec8 avlData in udp.frame.GetAvlDatas())
		{
			GPSElement gps = avlData.GetGPSElement();
			IOElementCodec8 ioElement = avlData.GetIOElement();
			
			Codec8Avl codec8Avl = new Codec8Avl(avlData, gps, ioElement, recordNumber);
			Console.WriteLine(codec8Avl.ToString());
			recordNumber++;
		}

		return recordNumber;
	}

	private static int HandleCodec8ExtendedUdp(Codec8ExtendedUdp udp)
	{
		Codec8ExtendedUdpMandatory mandatory = new Codec8ExtendedUdpMandatory(udp.udpChannelHeader, udp.avlDataEncapsulated, udp.frame);
		Console.WriteLine(mandatory.ToString());
		int recordNumber = 1;
		foreach (AvlDataCodec8Extended avlData in udp.frame.GetAvlDatas())
		{
			GPSElement gps = avlData.GetGPSElement();
			IOElementCodec8Extended ioElement = avlData.GetIOElement();
			
			Codec8ExtendedAvl codec8Avl = new Codec8ExtendedAvl(avlData, gps, ioElement, recordNumber);
			Console.WriteLine(codec8Avl.ToString());
			recordNumber++;
		}

		return recordNumber;
	}

	private static int HandleParseError(ParseErrorInfo error)
	{
		Console.WriteLine($"Cannot parse input, error: {error.error} - {error.errorText}");
		return 0;
	}

	private static void PrintHelp()
	{
		PrintVersion();
		Console.WriteLine("You have to give some parameters for Codec8Cli.");
		Console.WriteLine("e.g.");
		Console.WriteLine($"{b}codec8cli 000000000000002808010000016B40D9AD80010000000000000000000000000000000103021503010101425E100000010000F22A{_b}");
		Console.WriteLine("or");
		Console.WriteLine($"{b}codec8cli inputfile.txt{_b}");
	}

	private static void PrintVersion()
	{
		Console.WriteLine($"Codec8Cli v{Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion}");
	}
}
