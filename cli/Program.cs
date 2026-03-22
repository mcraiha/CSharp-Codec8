using Codec8;

namespace Codec8Cli;

class Program
{
	/// <summary>
	/// Bold
	/// </summary>
	/// <remarks>From https://davidjones.sportronics.com.au/coding/ConsoleTextFormat-Formatting_Console_App_Text-coding.html</remarks>
	public const string b = "\u001b[1m";

	/// <summary>
	/// Not Bold
	/// </summary>
	public const string _b = "\u001b[22m";

	static void Main(string[] args)
	{
		if (args.Length < 1)
		{
			PrintHelp();
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
			}
			else if (result == GenericDecodeResult.SuccessCodec8Extended)
			{
				Codec8ExtendedFrame frameExtended = (Codec8ExtendedFrame)valueOrError;

			}
			else
			{
				string possibleError = (string)valueOrError;
			}
		}
	}

	static void PrintHelp()
	{
		Console.WriteLine("You have to give some parameters for Codec8Cli.");
		Console.WriteLine("e.g.");
		Console.WriteLine($"{b}codec8cli 000000000000002808010000016B40D9AD80010000000000000000000000000000000103021503010101425E100000010000F22A{_b}");
		Console.WriteLine("or");
		Console.WriteLine($"{b}codec8cli inputfile.txt{_b}");
	}
}
