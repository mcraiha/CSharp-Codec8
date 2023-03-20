using System;
using System.Collections.Generic;

namespace Codec8
{

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

	public sealed class IOElement
	{
		public byte eventIoId;
		public byte totalCount;

		public byte oneByteValuesCount;
		public List<(byte Id, byte Value)> oneByteIdValuePairs;

		public byte twoByteValuesCount;
		public List<(byte Id, byte[] Value)> twoByteIdValuePairs;

		public byte fourByteValuesCount;
		public List<(byte Id, byte[] Value)> fourByteIdValuePairs;

		public byte eightByteValuesCount;
		public List<(byte Id, byte[] Value)> eightByteIdValuePairs;

		public int sizeInBytes;

		public IOElement(ReadOnlySpan<byte> bytes)
		{
			int currentIndex = 0;

			this.eventIoId = bytes[currentIndex];
			currentIndex++;

			this.totalCount = bytes[currentIndex];
			currentIndex++;

			this.oneByteValuesCount = bytes[currentIndex];
			currentIndex++;

			this.oneByteIdValuePairs = new List<(byte, byte)>(this.oneByteValuesCount);

			for (int i = 0; i < this.oneByteValuesCount; i++)
			{
				this.oneByteIdValuePairs.Add((bytes[currentIndex], bytes[currentIndex + 1]));
				currentIndex += 2;
			}

			this.twoByteValuesCount = bytes[currentIndex];
			currentIndex++;

			this.twoByteIdValuePairs = new List<(byte, byte[])>(this.twoByteValuesCount);

			for (int i = 0; i < this.twoByteValuesCount; i++)
			{
				this.twoByteIdValuePairs.Add((bytes[currentIndex], bytes.Slice(currentIndex + 1, 2).ToArray()));
				currentIndex += 3;
			}

			this.fourByteValuesCount = bytes[currentIndex];
			currentIndex++;

			this.fourByteIdValuePairs = new List<(byte, byte[])>(this.fourByteValuesCount);

			for (int i = 0; i < this.fourByteValuesCount; i++)
			{
				this.fourByteIdValuePairs.Add((bytes[currentIndex], bytes.Slice(currentIndex + 1, 4).ToArray()));
				currentIndex += 5;
			}

			this.eightByteValuesCount = bytes[currentIndex];
			currentIndex++;

			this.eightByteIdValuePairs = new List<(byte, byte[])>(this.eightByteValuesCount);

			for (int i = 0; i < this.eightByteValuesCount; i++)
			{
				this.eightByteIdValuePairs.Add((bytes[currentIndex], bytes.Slice(currentIndex + 1, 8).ToArray()));
				currentIndex += 9;
			}

			this.sizeInBytes = currentIndex;
		}
	}

	public sealed class AvlData
	{
		public byte[] timestampBytes;
		public byte priority;
		public byte[] gpsElementBytes;
		public byte[] ioElementBytes;
		public int sizeInBytes;

		public AvlData(ReadOnlySpan<byte> bytes)
		{
			int currentIndex = 0;

			this.timestampBytes = bytes.Slice(currentIndex, 8).ToArray();
			currentIndex += 8;

			this.priority = bytes[currentIndex];
			currentIndex++;

			this.gpsElementBytes = bytes.Slice(currentIndex, 15).ToArray();
			currentIndex += 15;

			IOElement ioElement = new IOElement(bytes.Slice(currentIndex));
			this.ioElementBytes = bytes.Slice(currentIndex, ioElement.sizeInBytes).ToArray();
			currentIndex += ioElement.sizeInBytes;

			this.sizeInBytes = currentIndex;
		}

		public GPSElement GetGPSElement()
		{
			return new GPSElement(this.gpsElementBytes);
		}

		public IOElement GetIOElement()
		{
			return new IOElement(this.ioElementBytes);
		}
	}

	public sealed class Codec8Frame
	{
		public byte[] preambleBytes;
		public byte[] dataFieldLengthBytes;
		public byte codecId;
		public byte numberOfData1; // How many records are included
		public byte numberOfData2; // How many records are included
		public List<byte[]> avlDataBytesList;
		public byte[] crc16;

		public Codec8Frame(ReadOnlySpan<byte> preamble, ReadOnlySpan<byte> dataFieldLength, byte id, byte records1, byte records2, List<byte[]> avlData, ReadOnlySpan<byte> crc)
		{
			this.preambleBytes = preamble.ToArray();
			this.dataFieldLengthBytes = dataFieldLength.ToArray();
			this.codecId = id;
			this.numberOfData1 = records1;
			this.numberOfData2 = records2;
			this.avlDataBytesList = avlData;
			this.crc16 = crc.ToArray();
		}

		public uint GetDataFieldLength()
		{
			return BitConverter.ToUInt32(this.dataFieldLengthBytes);
		}

		public IReadOnlyList<AvlData> GetAvlDatas()
		{
			List<AvlData> returnList = new List<AvlData>();
			foreach (byte[] bytes in this.avlDataBytesList)
			{
				returnList.Add(new AvlData(bytes));
			}

			return returnList;
		}
	}

	public enum GenericDecodeResult
	{
		SuccessCodec8,
		InputNullOrEmpty,
		OddNumberOfHexValues,
		WrongPreamble,
		NumberOfDataMismatch
	}

	public static class Codec8Decoder
	{
		public const uint preambleExpected = 0;
		public const byte codec8Id = 8;

		public static readonly HashSet<byte> validPriorities = new HashSet<byte>() { 0, 1, 2 }; 

		public static (GenericDecodeResult result, object valueOrError) ParseHexadecimalString(string hexadecimal)
		{
			if (string.IsNullOrEmpty(hexadecimal))
			{
				return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
			}

			hexadecimal = hexadecimal.Replace("-", "");

			if (hexadecimal.Length % 2 == 1)
			{
				return (GenericDecodeResult.OddNumberOfHexValues, $"Odd number of hex values");
			}

			ReadOnlySpan<char> chars = hexadecimal;

			// Two hex chars become one byte
			byte[] inputAsBytes = new byte[chars.Length / 2];

			for (int i = 0; i < inputAsBytes.Length; i++)
			{
				inputAsBytes[i] = byte.Parse(chars.Slice(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
			}

			return ParseByteArray(inputAsBytes);
		}

		public static (GenericDecodeResult result, object valueOrError) ParseByteArray(ReadOnlySpan<byte> bytes)
		{
			if (bytes.Length < 1)
			{
				return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
			}

			int currentIndex = 0;

			ReadOnlySpan<byte> preambleBytes = bytes.Slice(currentIndex, sizeof(uint));
			uint preamble = BitConverter.ToUInt32(preambleBytes);
			currentIndex += sizeof(uint);

			if (preamble != preambleExpected)
			{
				return (GenericDecodeResult.WrongPreamble, $"Expected {preambleExpected} but got {preamble} instead");
			}

			ReadOnlySpan<byte> dataFieldLengthBytes = bytes.Slice(currentIndex, sizeof(uint));
			uint dataFieldLength = BitConverter.ToUInt32(dataFieldLengthBytes);
			currentIndex += sizeof(uint);

			byte codecId = bytes[currentIndex];
			currentIndex += sizeof(byte);

			byte numberOfData1 = bytes[currentIndex];
			currentIndex += sizeof(byte);

			List<byte[]> avlDataBytesList = new List<byte[]>();
			for (byte b = 0; b < numberOfData1; b++)
			{
				AvlData avlData = new AvlData(bytes.Slice(currentIndex));
				avlDataBytesList.Add(bytes.Slice(currentIndex, avlData.sizeInBytes).ToArray());
				currentIndex += avlData.sizeInBytes;
			}

			byte numberOfData2 = bytes[currentIndex];
			currentIndex += sizeof(byte);

			if (numberOfData1 != numberOfData2)
			{
				return (GenericDecodeResult.WrongPreamble, $"{numberOfData1} vs. {numberOfData2}");
			}

			ReadOnlySpan<byte> crcBytes = bytes.Slice(currentIndex, 4);
			
			Codec8Frame frame = new Codec8Frame(preambleBytes, dataFieldLengthBytes, codecId, numberOfData1, numberOfData2, avlDataBytesList, crcBytes);

			return (GenericDecodeResult.SuccessCodec8, frame);
		}
	}
}
