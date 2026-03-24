using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Codec8IOElement
{
	private readonly IOElementCodec8 _ioElement;

	public Codec8IOElement(IOElementCodec8 IOElement)
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
		sb.AppendLine($"Event IO ID\t{_ioElement.eventIoId.ToString("X2")}\t{_ioElement.eventIoId}");
		sb.AppendLine($"Total number of IO\t{_ioElement.totalCount.ToString("X2")}\t{_ioElement.totalCount}");

		sb.AppendLine($"Number of One Byte IO\t{_ioElement.oneByteValuesCount.ToString("X2")}\t{_ioElement.oneByteValuesCount}");
		Codec8OneByteIdValuePairs codec8OneByteIdValuePairs = new Codec8OneByteIdValuePairs(_ioElement.oneByteIdValuePairs);
		sb.AppendLine(codec8OneByteIdValuePairs.ToString());

		sb.AppendLine($"Number of Two Byte IO\t{_ioElement.twoByteValuesCount.ToString("X2")}\t{_ioElement.twoByteValuesCount}");
		Codec8TwoByteIdValuePairs codec8TwoByteIdValuePairs = new Codec8TwoByteIdValuePairs(_ioElement.twoByteIdValuePairs);
		sb.AppendLine(codec8TwoByteIdValuePairs.ToString());

		sb.AppendLine($"Number of Four Byte IO\t{_ioElement.fourByteValuesCount.ToString("X2")}\t{_ioElement.fourByteValuesCount}");
		Codec8FourByteIdValuePairs codec8FourByteIdValuePairs = new Codec8FourByteIdValuePairs(_ioElement.fourByteIdValuePairs);
		sb.AppendLine(codec8FourByteIdValuePairs.ToString());

		sb.AppendLine($"Number of Eight Byte IO\t{_ioElement.eightByteValuesCount.ToString("X2")}\t{_ioElement.eightByteValuesCount}");
		Codec8EightByteIdValuePairs codec8EightByteIdValuePairs = new Codec8EightByteIdValuePairs(_ioElement.eightByteIdValuePairs);
		sb.AppendLine(codec8EightByteIdValuePairs.ToString());

		return sb.ToString();
	}
}