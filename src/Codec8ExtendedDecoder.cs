using System;
using System.Collections.Generic;
using System.Buffers.Binary;

namespace Codec8
{

	public sealed class IOElementCodec8Extended
	{
		public byte[] eventIoId;
		public byte[] totalCount;

		public byte[] oneByteValuesCountBytes;
		public List<(byte[] Id, byte Value)> oneByteIdValuePairs;

		public byte[] twoByteValuesCountBytes;
		public List<(byte[] Id, byte[] Value)> twoByteIdValuePairs;

		public byte[] fourByteValuesCountBytes;
		public List<(byte[] Id, byte[] Value)> fourByteIdValuePairs;

		public byte[] eightByteValuesCountBytes;
		public List<(byte[] Id, byte[] Value)> eightByteIdValuePairs;

		public byte[] xByteValuesCountBytes;
		public List<(byte[] Id, byte[] Value)> xByteIdValuePairs;

		public int sizeInBytes;

		public IOElementCodec8Extended(ReadOnlySpan<byte> bytes)
		{
			int currentIndex = 0;

			this.eventIoId = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			this.totalCount = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			this.oneByteValuesCountBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			ushort oneByteValuesCount = BitConverter.ToUInt16(this.oneByteValuesCountBytes);
			if (BitConverter.IsLittleEndian)
			{
				oneByteValuesCount = BinaryPrimitives.ReverseEndianness(oneByteValuesCount);
			}
			this.oneByteIdValuePairs = new List<(byte[], byte)>((int)oneByteValuesCount);

			for (int i = 0; i < oneByteValuesCount; i++)
			{
				this.oneByteIdValuePairs.Add((bytes.Slice(currentIndex, 2).ToArray(), bytes[currentIndex + 2]));
				currentIndex += 3;
			}

			this.twoByteValuesCountBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			ushort twoByteValuesCount = BitConverter.ToUInt16(this.twoByteValuesCountBytes);
			if (BitConverter.IsLittleEndian)
			{
				twoByteValuesCount = BinaryPrimitives.ReverseEndianness(twoByteValuesCount);
			}
			this.twoByteIdValuePairs = new List<(byte[], byte[])>((int)twoByteValuesCount);

			for (int i = 0; i < twoByteValuesCount; i++)
			{
				this.twoByteIdValuePairs.Add((bytes.Slice(currentIndex, 2).ToArray(), bytes.Slice(currentIndex + 2, 2).ToArray()));
				currentIndex += 4;
			}

			this.fourByteValuesCountBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			ushort fourByteValuesCount = BitConverter.ToUInt16(this.fourByteValuesCountBytes);
			if (BitConverter.IsLittleEndian)
			{
				fourByteValuesCount = BinaryPrimitives.ReverseEndianness(fourByteValuesCount);
			}
			this.fourByteIdValuePairs = new List<(byte[], byte[])>(fourByteValuesCount);

			for (int i = 0; i < fourByteValuesCount; i++)
			{
				this.fourByteIdValuePairs.Add((bytes.Slice(currentIndex, 2).ToArray(), bytes.Slice(currentIndex + 2, 4).ToArray()));
				currentIndex += 6;
			}

			this.eightByteValuesCountBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			ushort eightByteValuesCount = BitConverter.ToUInt16(this.eightByteValuesCountBytes);
			if (BitConverter.IsLittleEndian)
			{
				eightByteValuesCount = BinaryPrimitives.ReverseEndianness(eightByteValuesCount);
			}
			this.eightByteIdValuePairs = new List<(byte[], byte[])>(eightByteValuesCount);

			for (int i = 0; i < eightByteValuesCount; i++)
			{
				this.eightByteIdValuePairs.Add((bytes.Slice(currentIndex, 2).ToArray(), bytes.Slice(currentIndex + 2, 8).ToArray()));
				currentIndex += 10;
			}

			this.xByteValuesCountBytes = bytes.Slice(currentIndex, 2).ToArray();
			currentIndex += 2;

			ushort xByteValuesCount = BitConverter.ToUInt16(this.xByteValuesCountBytes);
			if (BitConverter.IsLittleEndian)
			{
				xByteValuesCount = BinaryPrimitives.ReverseEndianness(xByteValuesCount);
			}
			this.xByteIdValuePairs = new List<(byte[], byte[])>(xByteValuesCount);

			for (int i = 0; i < xByteValuesCount; i++)
			{
				ushort xValueLengthInBytes = BitConverter.ToUInt16(bytes.Slice(currentIndex + 2, 2));
				this.xByteIdValuePairs.Add((bytes.Slice(currentIndex, 2).ToArray(), bytes.Slice(currentIndex + 4, xValueLengthInBytes).ToArray()));
				currentIndex += (4 + xValueLengthInBytes);
			}

			this.sizeInBytes = currentIndex;
		}
	}

	public sealed class AvlDataCodec8Extended
	{
		public byte[] timestampBytes;
		public byte priority;
		public byte[] gpsElementBytes;
		public byte[] ioElementBytes;
		public int sizeInBytes;

		public AvlDataCodec8Extended(ReadOnlySpan<byte> bytes)
		{
			int currentIndex = 0;

			this.timestampBytes = bytes.Slice(currentIndex, 8).ToArray();
			currentIndex += 8;

			this.priority = bytes[currentIndex];
			currentIndex++;

			this.gpsElementBytes = bytes.Slice(currentIndex, 15).ToArray();
			currentIndex += 15;

			IOElementCodec8Extended ioElement = new IOElementCodec8Extended(bytes.Slice(currentIndex));
			this.ioElementBytes = bytes.Slice(currentIndex, ioElement.sizeInBytes).ToArray();
			currentIndex += ioElement.sizeInBytes;

			this.sizeInBytes = currentIndex;
		}

		public GPSElement GetGPSElement()
		{
			return new GPSElement(this.gpsElementBytes);
		}

		public IOElementCodec8Extended GetIOElement()
		{
			return new IOElementCodec8Extended(this.ioElementBytes);
		}
	}

	public sealed class Codec8ExtendedFrame
	{
		public byte[] preambleBytes;
		public byte[] dataFieldLengthBytes;
		public byte codecId;
		public byte numberOfData1; // How many records are included
		public byte numberOfData2; // How many records are included
		public List<byte[]> avlDataBytesList;
		public byte[] crc16;

		public Codec8ExtendedFrame(ReadOnlySpan<byte> preamble, ReadOnlySpan<byte> dataFieldLength, byte id, byte records1, byte records2, List<byte[]> avlData, ReadOnlySpan<byte> crc)
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

		public IReadOnlyList<AvlDataCodec8Extended> GetAvlDatas()
		{
			List<AvlDataCodec8Extended> returnList = new List<AvlDataCodec8Extended>();
			foreach (byte[] bytes in this.avlDataBytesList)
			{
				returnList.Add(new AvlDataCodec8Extended(bytes));
			}

			return returnList;
		}
	}

	public static class Codec8ExtendedDecoder
	{
		public const uint preambleExpected = 0;
		public const byte codec8ExtendedId = 0x8E;

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

			if (codecId != codec8ExtendedId)
			{
				return (GenericDecodeResult.IncorrectCodecId, $"Expected {codec8ExtendedId:X2} as codec ID, but got {codecId:X2} instead");
			}

			byte numberOfData1 = bytes[currentIndex];
			currentIndex += sizeof(byte);

			List<byte[]> avlDataBytesList = new List<byte[]>();
			for (byte b = 0; b < numberOfData1; b++)
			{
				AvlDataCodec8Extended avlData = new AvlDataCodec8Extended(bytes.Slice(currentIndex));
				avlDataBytesList.Add(bytes.Slice(currentIndex, avlData.sizeInBytes).ToArray());
				currentIndex += avlData.sizeInBytes;
			}

			byte numberOfData2 = bytes[currentIndex];
			currentIndex += sizeof(byte);

			if (numberOfData1 != numberOfData2)
			{
				return (GenericDecodeResult.NumberOfDataMismatch, $"Mismatch in number of data values: {numberOfData1} vs. {numberOfData2}");
			}

			ReadOnlySpan<byte> crcBytes = bytes.Slice(currentIndex, 4);
			
			Codec8ExtendedFrame frame = new Codec8ExtendedFrame(preambleBytes, dataFieldLengthBytes, codecId, numberOfData1, numberOfData2, avlDataBytesList, crcBytes);

			return (GenericDecodeResult.SuccessCodec8Extended, frame);
		}
	}
}
