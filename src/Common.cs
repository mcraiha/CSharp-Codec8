using System;

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
		/// Decode failed, input has odd number of hex values
		/// </summary>
		OddNumberOfHexValues,

		/// <summary>
		/// Decode failed, wrong preamble
		/// </summary>
		WrongPreamble,

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
		NumberOfDataMismatch
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
	}
}