using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8Avl
{
	private readonly AvlDataCodec8 _avlData;

	private readonly GPSElement _gps;

	private readonly IOElementCodec8 _ioElement;

	private readonly int _recordNumber;

	public Codec8Avl(AvlDataCodec8 AVLData, GPSElement GPS, IOElementCodec8 IOElement, int RecordNumber)
	{
		_avlData = AVLData;
		_gps = GPS;
		_ioElement = IOElement;
		_recordNumber = RecordNumber;
	}

	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"Timestamp\t{Common.GetByteArrayAsSplittedHex(_avlData.timestampBytes, false)}\t{_avlData.GetTimestamp().ToString("o")}");
		sb.AppendLine($"Priority\t{_avlData.priority.ToString("X2")}\t{_avlData.priority}");
		sb.AppendLine(new Gps(_gps).ToString());
		sb.AppendLine(new Codec8IOElement(_ioElement).ToString());
		return sb.ToString();
	}
}