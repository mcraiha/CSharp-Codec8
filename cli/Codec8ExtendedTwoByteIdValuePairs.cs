using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8ExtendedTwoByteIdValuePairs
{
	private readonly List<(byte[] Id, byte[] Value)> _twoByteIdValuePairs;

	public Codec8ExtendedTwoByteIdValuePairs(List<(byte[] Id, byte[] Value)> TwoByteIdValuePairs)
	{
		_twoByteIdValuePairs = TwoByteIdValuePairs;
	}
	
	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		int ioCount = 1;
		foreach ((byte[] Id, byte[] Value) in _twoByteIdValuePairs)
		{
			string ordinalNumber = Common.TurnToOrdinal(ioCount);
			sb.AppendLine($"↳ {ordinalNumber} IO ID\t{Common.GetByteArrayAsSplittedHex(Id, false)}\t{BytesToNumbers.GetUInt16(Id)}");
			sb.AppendLine($"↳ {ordinalNumber} IO Value\t{Common.GetByteArrayAsSplittedHex(Value, false)}\t{BytesToNumbers.GetUInt16(Value)} / {BytesToNumbers.GetInt16(Value)}");
			ioCount++;
		}

		return sb.ToString();
	}
}