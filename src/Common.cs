using System;

namespace Codec8
{
    public enum GenericDecodeResult
	{
		SuccessCodec8,
		InputNullOrEmpty,
		OddNumberOfHexValues,
		WrongPreamble,
		NumberOfDataMismatch
	}

    public sealed class GPSElement
	{
		public byte[] longitudeBytes;
		public byte[] latitudeBytes;
		public byte[] altitudeBytes;
		public byte[] angleBytes;
		public byte visibleSatellites;
		public byte[] speedBytes;

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