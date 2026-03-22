namespace Codec8Cli;

public static class Common
{
	public static string GetByteArrayAsSplittedHex(byte[] bytes, bool splitWIthDashes)
	{
		return String.Join( splitWIthDashes ? "-" :" ", Array.ConvertAll(bytes, x => x.ToString("X2")));
	}

	public static string TurnToOrdinal(int number)
	{
		int remainder = number % 100;
		return remainder switch
		{
			11 => "11th",
			12 => "12th",
			13 => "13th",

			1 => "1st",
			2 => "2nd",
			3 => "3rd",

			_ => $"{number}th"
		};
	}
}