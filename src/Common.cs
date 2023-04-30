using System;
using System.Buffers.Binary;

namespace Codec8
{
	/// <summary>
	/// Generic decode result
	/// </summary>
	public enum GenericDecodeResult
	{
		/// <summary>
		/// Decode was success as Codec8
		/// </summary>
		SuccessCodec8,

		/// <summary>
		/// Decode was success as Codec8 Extended
		/// </summary>
		SuccessCodec8Extended,

		/// <summary>
		/// Decode failed, input is null or empty
		/// </summary>
		InputNullOrEmpty,

		/// <summary>
		/// Decode failed, input has non hex value(s)
		/// </summary>
		ContainsNonHexValues,

		/// <summary>
		/// Decode failed, input has odd number of hex values
		/// </summary>
		OddNumberOfHexValues,

		/// <summary>
		/// Decode failed, wrong preamble
		/// </summary>
		WrongPreamble,

		/// <summary>
		/// Data Field Length is too big
		/// </summary>
		DataFieldLengthTooBig,

		/// <summary>
		/// Decode failed, incorrect codec id
		/// </summary>
		IncorrectCodecId,

		/// <summary>
		/// Decode failed, incorrect priority value
		/// </summary>
		IncorrectPriority,

		/// <summary>
		/// Decode failed, number of data values don't match
		/// </summary>
		NumberOfDataMismatch,

		/// <summary>
		/// Data field amount of bytes do not contain needed amount of data elements 
		/// </summary>
		DataFieldLengthAndNumberOfDataMismatch,

		/// <summary>
		/// Crc values don't match
		/// </summary>
		CrcMismatch
	}

	/// <summary>
	/// GPS Element
	/// </summary>
	public sealed class GPSElement
	{
		/// <summary>
		/// Longitude as four bytes
		/// </summary>
		public byte[] longitudeBytes;

		/// <summary>
		/// Latitude as four bytes
		/// </summary>
		public byte[] latitudeBytes;

		/// <summary>
		/// Altitude as two bytes
		/// </summary>
		public byte[] altitudeBytes;

		/// <summary>
		/// Angle as two bytes
		/// </summary>
		public byte[] angleBytes;

		/// <summary>
		/// Visible satellites amount
		/// </summary>
		public byte visibleSatellites;

		/// <summary>
		/// Speed as two bytes
		/// </summary>
		public byte[] speedBytes;

		/// <summary>
		/// Constructor (only one)
		/// </summary>
		/// <param name="bytes">Bytes to turn into GPSElement, all bytes might not be read</param>
		public GPSElement(ReadOnlySpan<byte> bytes)
		{
			int currentIndex = 0;

			this.longitudeBytes = bytes.Slice(currentIndex, 4).ToArray();
			currentIndex += 4;

			this.latitudeBytes = bytes.Slice(currentIndex, 4).ToArray();
			currentIndex += 4;

			this.altitudeBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			this.angleBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			this.visibleSatellites = bytes[currentIndex];
			currentIndex++;

			this.speedBytes = bytes.Slice(currentIndex, 2).ToArray();
		}

		/// <summary>
		/// Is GPS data valid, only invalid value is speed == 0x0000
		/// </summary>
		/// <returns>True if it is; False otherwise</returns>
		public bool IsGPSValid()
		{
			return speedBytes[0] != 0 || speedBytes[1] != 0;
		}

		/// <summary>
		/// Get unmodified longitude
		/// </summary>
		/// <returns>Longitude as int</returns>
		public int GetLongitude()
		{
			return BytesToNumbers.GetInt32(this.longitudeBytes);
		}

		private const float precisionDivider = 10_000_000;

		/// <summary>
		/// Get longitude angle
		/// </summary>
		/// <returns>Float between -180.0 ... 180.0</returns>
		public float GetLongitudeAngle()
		{
			return GetLongitude() / precisionDivider;
		}

		/// <summary>
		/// Get unmodified latitude
		/// </summary>
		/// <returns>Latitude as int</returns>
		public int GetLatitude()
		{
			return BytesToNumbers.GetInt32(this.latitudeBytes);
		}

		/// <summary>
		/// Get latitude angle
		/// </summary>
		/// <returns>Float between -90.0 ... 90.0</returns>
		public float GetLatitudeAngle()
		{
			return GetLatitude() / precisionDivider;
		}

		/// <summary>
		/// Get as GeoJSON point
		/// </summary>
		/// <param name="prettyPrint">Should the position be pretty printed</param>
		/// <returns>String containing the GeoJSON point </returns>
		public string GetAsGeoJSONPoint(bool prettyPrint)
		{
			if (prettyPrint)
			{
				return $$"""
				{
					"type": "Point",
					"coordinates": [{{GetLongitudeAngle().ToString(System.Globalization.CultureInfo.InvariantCulture)}}, {{GetLatitudeAngle().ToString(System.Globalization.CultureInfo.InvariantCulture)}}]
				}
				""";
			}

			return $$"""{"type":"Point","coordinates":[{{GetLongitudeAngle().ToString(System.Globalization.CultureInfo.InvariantCulture)}},{{GetLatitudeAngle().ToString(System.Globalization.CultureInfo.InvariantCulture)}}]}""";
		}

		/// <summary>
		/// Get altitude (meters above sea level)
		/// </summary>
		/// <returns>Altitude as short</returns>
		public short GetAltitude()
		{
			return BytesToNumbers.GetInt16(this.altitudeBytes);
		}

		/// <summary>
		/// Get angle
		/// </summary>
		/// <returns>Angle as ushort</returns>
		public ushort GetAngle()
		{
			return BytesToNumbers.GetUInt16(this.angleBytes);
		}

		/// <summary>
		/// Get speed
		/// </summary>
		/// <returns>Speed as ushort</returns>
		public ushort GetSpeed()
		{
			return BytesToNumbers.GetUInt16(this.speedBytes);
		}

		/// <summary>
		/// Get values in one string
		/// </summary>
		/// <returns>String that contains all values</returns>
		public override string ToString()
		{
			return $"Longitude: {GetLongitude()} Latitude: {GetLatitude()} Altitude: {GetAltitude()} Angle: {GetAngle()} Visible satellites: {this.visibleSatellites} Speed: {GetSpeed()}";
		}
	}

	/// <summary>
	/// Generic decoder, for cases when you do not know the encoding
	/// </summary>
	public static class GenericDecoder
	{
		/// <summary>
		/// Try to parse Codec8Frame or Codec8ExtendedFrame from given hexadecimal string
		/// </summary>
		/// <param name="hexadecimal">Hexadecimal input string</param>
		/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either Codec8Frame, Codec8ExtendedFrame or error string</returns>
		public static (GenericDecodeResult result, object valueOrError) ParseHexadecimalString(string hexadecimal)
		{
			(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(hexadecimal);
			if (result == GenericDecodeResult.IncorrectCodecId)
			{
				(result, valueOrError) = Codec8ExtendedDecoder.ParseHexadecimalString(hexadecimal);
				if (result == GenericDecodeResult.SuccessCodec8Extended)
				{
					return (result, valueOrError);
				}
			}

			return (result, valueOrError);			
		}
	}

	/// <summary>
	/// Tools for hex encoding / decoding
	/// </summary>
	public static class HexTools
	{
		/// <summary>
		/// Check if string contains only hex chars
		/// </summary>
		/// <param name="hexadecimal">Hexadecimal string</param>
		/// <returns>True if it contains; False otherwise</returns>
		public static bool CheckIfHexOnly(string hexadecimal)
		{
			ReadOnlySpan<char> chars = hexadecimal;
			foreach (char c in chars)
			{
				bool isHex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
				if (!isHex)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Find first index of non hex value
		/// </summary>
		/// <param name="hexadecimal">Hexadecimal string</param>
		/// <returns>Index position, -1 in case all chars are hex chars</returns>
		public static int FindFirstNonHexPos(string hexadecimal)
		{
			ReadOnlySpan<char> chars = hexadecimal;
			int index = -1;
			for (int i = 0; i < chars.Length; i++)
			{
				char c = chars[i];
				bool isHex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
				if (!isHex)
				{
					index = i;
					break;
				}
			}

			return index;
		}
	}

	/// <summary>
	/// Static class for calculating CRC16 as defined in https://wiki.teltonika-gps.com/view/Codec#CRC-16
	/// </summary>
	public static class Crc16
	{
		/// <summary>
		/// Calculate Crc16 from given bytes (result is big-endian)
		/// </summary>
		/// <param name="bytes">Byte array</param>
		/// <returns>4 byte value of Crc</returns>
		public static byte[] Calculate(ReadOnlySpan<byte> bytes)
		{
			uint crc = 0;

			for (int i = 0; i < bytes.Length; i++)
			{
				crc = crc ^ bytes[i];
				int bitNumber = 0;
				
				while (bitNumber != 8)
				{
					uint carry = crc & 1;
					crc >>= 1;
					if (carry == 1)
					{
						crc = crc ^ 0xA001;
					}
					bitNumber++;
				}	
			}

			if (BitConverter.IsLittleEndian)
			{
				crc = BinaryPrimitives.ReverseEndianness(crc);
			}

			return BitConverter.GetBytes(crc);
		}
	}
}