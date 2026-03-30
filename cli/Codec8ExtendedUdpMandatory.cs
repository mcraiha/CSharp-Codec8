using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8ExtendedUdpMandatory
{
	private readonly UdpChannelHeader _header;
	private readonly AvlDataEncapsulated _avlDataEncapsulated;
	private readonly Codec8ExtendedFrameNoCRC _frame;

	public Codec8ExtendedUdpMandatory(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8ExtendedFrameNoCRC frame)
	{
		_header = header;
		_avlDataEncapsulated = avlDataEncapsulated;
		_frame = frame;
	}

	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"Packet lenght in bytes\t{_header.packetLengthInBytes.ToString("X2")}\t{_header.packetLengthInBytes}");
		sb.AppendLine($"Packet Id\t{Common.GetByteArrayAsSplittedHex(_header.packetId, false)}\t{_header.GetPacketIdAsUshort()}");
		sb.AppendLine($"Not usable byte\t{_header.notUsableByte.ToString("X2")}\t{_header.notUsableByte}");

		sb.AppendLine($"AVL packet Id\t{_avlDataEncapsulated.avlPacketId.ToString("X2")}\t{_avlDataEncapsulated.avlPacketId}");
		sb.AppendLine($"IMEI\t{Common.GetByteArrayAsSplittedHex(_avlDataEncapsulated.imei, false)}\t{_avlDataEncapsulated.GetImei()}");

		sb.AppendLine("Codec ID\t8E\t142 (Codec 8 Extended)");
		sb.AppendLine($"Number of Data 1 (Records)\t{_frame.numberOfData1.ToString("X2")}\t{_frame.numberOfData1}");
		sb.AppendLine($"Number of Data 2 (Records)\t{_frame.numberOfData2.ToString("X2")}\t{_frame.numberOfData2}");
		return sb.ToString();
	}
}