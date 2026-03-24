using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8EightByteIdValuePairs
{
	private readonly List<(byte Id, byte[] Value)> _eightByteIdValuePairs;

	public Codec8EightByteIdValuePairs(List<(byte Id, byte[] Value)> EightByteIdValuePairs)
	{
		_eightByteIdValuePairs = EightByteIdValuePairs;
	}
	
	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		int ioCount = 1;
		foreach ((byte Id, byte[] Value) in _eightByteIdValuePairs)
		{
			string ordinalNumber = Common.TurnToOrdinal(ioCount);
			sb.AppendLine($"↳ {ordinalNumber} IO ID\t{Id.ToString("X2")}\t{Id}");
			sb.AppendLine($"↳ {ordinalNumber} IO Value\t{Common.GetByteArrayAsSplittedHex(Value, false)}\t{BytesToNumbers.GetUInt64(Value)} / {BytesToNumbers.GetInt64(Value)}");
			ioCount++;
		}

		return sb.ToString();
	}
}