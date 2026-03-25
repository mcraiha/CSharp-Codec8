using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8ExtendedOneByteIdValuePairs
{
	private readonly List<(byte[] Id, byte Value)> _oneByteIdValuePairs;

	public Codec8ExtendedOneByteIdValuePairs(List<(byte[] Id, byte Value)> OneByteIdValuePairs)
	{
		_oneByteIdValuePairs = OneByteIdValuePairs;
	}
	
	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		int ioCount = 1;
		foreach ((byte[] Id, byte Value) in _oneByteIdValuePairs)
		{
			string ordinalNumber = Common.TurnToOrdinal(ioCount);
			sb.AppendLine($"↳ {ordinalNumber} IO ID\t{Common.GetByteArrayAsSplittedHex(Id, false)}\t{BytesToNumbers.GetUInt16(Id)}");
			sb.AppendLine($"↳ {ordinalNumber} IO Value\t{Value.ToString("X2")}\t{Value}");
			ioCount++;
		}

		return sb.ToString();
	}
}