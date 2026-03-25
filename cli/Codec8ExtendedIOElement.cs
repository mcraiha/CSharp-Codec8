using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8ExtendedIOElement
{
	private readonly IOElementCodec8Extended _ioElement;

	public Codec8ExtendedIOElement(IOElementCodec8Extended IOElement)
	{
		_ioElement = IOElement;
	}

	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"Event IO ID\t{Common.GetByteArrayAsSplittedHex(_ioElement.eventIoId, false)}\t{BytesToNumbers.GetUInt16(_ioElement.eventIoId)}");
		sb.AppendLine($"Total number of IO\t{Common.GetByteArrayAsSplittedHex(_ioElement.totalCount, false)}\t{BytesToNumbers.GetUInt16(_ioElement.totalCount)}");

		sb.AppendLine($"Number of One Byte IO\t{Common.GetByteArrayAsSplittedHex(_ioElement.oneByteValuesCountBytes, false)}\t{BytesToNumbers.GetUInt16(_ioElement.oneByteValuesCountBytes)}");
		Codec8ExtendedOneByteIdValuePairs codec8OneByteIdValuePairs = new Codec8ExtendedOneByteIdValuePairs(_ioElement.oneByteIdValuePairs);
		sb.AppendLine(codec8OneByteIdValuePairs.ToString());

		sb.AppendLine($"Number of Two Byte IO\t{Common.GetByteArrayAsSplittedHex(_ioElement.twoByteValuesCountBytes, false)}\t{BytesToNumbers.GetUInt16(_ioElement.twoByteValuesCountBytes)}");
		Codec8ExtendedTwoByteIdValuePairs codec8TwoByteIdValuePairs = new Codec8ExtendedTwoByteIdValuePairs(_ioElement.twoByteIdValuePairs);
		sb.AppendLine(codec8TwoByteIdValuePairs.ToString());

		sb.AppendLine($"Number of Four Byte IO\t{Common.GetByteArrayAsSplittedHex(_ioElement.fourByteValuesCountBytes, false)}\t{BytesToNumbers.GetUInt16(_ioElement.fourByteValuesCountBytes)}");
		Codec8ExtendedFourByteIdValuePairs codec8FourByteIdValuePairs = new Codec8ExtendedFourByteIdValuePairs(_ioElement.fourByteIdValuePairs);
		sb.AppendLine(codec8FourByteIdValuePairs.ToString());

		sb.AppendLine($"Number of Eight Byte IO\t{Common.GetByteArrayAsSplittedHex(_ioElement.eightByteValuesCountBytes, false)}\t{BytesToNumbers.GetUInt16(_ioElement.eightByteValuesCountBytes)}");
		Codec8ExtendedEightByteIdValuePairs codec8EightByteIdValuePairs = new Codec8ExtendedEightByteIdValuePairs(_ioElement.eightByteIdValuePairs);
		sb.AppendLine(codec8EightByteIdValuePairs.ToString());

		return sb.ToString();
	}
}