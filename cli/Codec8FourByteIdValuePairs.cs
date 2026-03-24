using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8FourByteIdValuePairs
{
	private readonly List<(byte Id, byte[] Value)> _fourByteIdValuePairs;

	public Codec8FourByteIdValuePairs(List<(byte Id, byte[] Value)> FourByteIdValuePairs)
	{
		_fourByteIdValuePairs = FourByteIdValuePairs;
	}
	
	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		int ioCount = 1;
		foreach ((byte Id, byte[] Value) in _fourByteIdValuePairs)
		{
			string ordinalNumber = Common.TurnToOrdinal(ioCount);
			sb.AppendLine($"↳ {ordinalNumber} IO ID\t{Id.ToString("X2")}\t{Id}");
			sb.AppendLine($"↳ {ordinalNumber} IO Value\t{Common.GetByteArrayAsSplittedHex(Value, false)}\t{BytesToNumbers.GetUInt32(Value)} / {BytesToNumbers.GetInt32(Value)}");
			ioCount++;
		}

		return sb.ToString();
	}
}