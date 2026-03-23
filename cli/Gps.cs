using System.Text;
using Codec8;

namespace Codec8Cli;

public sealed class Gps
{
	private readonly GPSElement _gps;

	public Gps(GPSElement GPS)
	{
		_gps = GPS;
	}

	/// <summary>
	/// For basic printing
	/// </summary>
	/// <returns>String</returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine($"Longitude\t{Common.GetByteArrayAsSplittedHex(_gps.longitudeBytes, false)}\t{_gps.GetLongitudeAngle()}");
		sb.AppendLine($"Latitude\t{Common.GetByteArrayAsSplittedHex(_gps.latitudeBytes, false)}\t{_gps.GetLatitudeAngle()}");
		sb.AppendLine($"Altitude\t{Common.GetByteArrayAsSplittedHex(_gps.altitudeBytes, false)}\t{_gps.GetAltitude()}");
		sb.AppendLine($"Angle\t{Common.GetByteArrayAsSplittedHex(_gps.angleBytes, false)}\t{_gps.GetAngle()}");
		sb.AppendLine($"Satellites\t{_gps.visibleSatellites.ToString("X2")}\t{_gps.visibleSatellites}");
		sb.AppendLine($"Speed\t{Common.GetByteArrayAsSplittedHex(_gps.speedBytes, false)}\t{_gps.GetSpeed()}");
		return sb.ToString();
	}
}