using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8Mandatory
{
	private readonly Codec8Frame _frame;

	public Codec8Mandatory(Codec8Frame frame)
	{
		_frame = frame;
	}

	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("Zero bytes\t00 00 00 00\t0");
		sb.AppendLine($"Data Field Length\t{Common.GetByteArrayAsSplittedHex(_frame.dataFieldLengthBytes, false)}\t{BytesToNumbers.GetUInt32(_frame.dataFieldLengthBytes)}");
		sb.AppendLine("Codec ID\t08\t8 (Codec 8)");
		sb.AppendLine($"Number of Data 1 (Records)\t{_frame.numberOfData1.ToString("X2")}\t{_frame.numberOfData1}");
		sb.AppendLine($"Number of Data 2 (Records)\t{_frame.numberOfData2.ToString("X2")}\t{_frame.numberOfData2}");
		sb.AppendLine($"CRC-16\t{Common.GetByteArrayAsSplittedHex(_frame.crc16, false)}\t{BytesToNumbers.GetUInt32(_frame.crc16)}");
		return sb.ToString();
	}
}