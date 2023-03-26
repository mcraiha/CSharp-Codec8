using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Buffers.Binary;

namespace Codec8
{

	/// <summary>
	/// Single IO element of Codec8 Extended
	/// </summary>
	public sealed class IOElementCodec8Extended
	{
		/// <summary>
		/// Event IO ID – if data is acquired on event – this field defines which IO property has changed and generated an event. For example, when if Ignition state changed and it generate event, Event IO ID will be 0xEF (AVL ID: 239). If it’s not eventual record – the value is 0.
		/// </summary>
		public byte[] eventIoId;

		/// <summary>
		/// A total number of properties (as big-endian bytes) coming with record (N = N1 + N2 + N4 + N8 + NX)
		/// </summary>
		public byte[] totalCount;

		/// <summary>
		/// Number of properties (as big-endian bytes), where value length is 1 byte
		/// </summary>
		public byte[] oneByteValuesCountBytes;

		/// <summary>
		/// All property pairs where value is 1 byte
		/// </summary>
		public List<(byte[] Id, byte Value)> oneByteIdValuePairs;

		/// <summary>
		/// Number of properties (as big-endian bytes), where value length is 2 bytes
		/// </summary>
		public byte[] twoByteValuesCountBytes;

		/// <summary>
		/// All property pairs where value is 2 bytes
		/// </summary>
		public List<(byte[] Id, byte[] Value)> twoByteIdValuePairs;

		/// <summary>
		/// Number of properties (as big-endian bytes), where value length is 4 bytes
		/// </summary>
		public byte[] fourByteValuesCountBytes;

		/// <summary>
		/// All property pairs where value is 4 bytes
		/// </summary>
		public List<(byte[] Id, byte[] Value)> fourByteIdValuePairs;

		/// <summary>
		/// Number of properties (as big-endian bytes), where value length is 8 bytes
		/// </summary>
		public byte[] eightByteValuesCountBytes;

		/// <summary>
		/// All property pairs where value is 8 bytes
		/// </summary>
		public List<(byte[] Id, byte[] Value)> eightByteIdValuePairs;

		/// <summary>
		/// Number of properties, where value length is X bytes
		/// </summary>
		public byte[] xByteValuesCountBytes;

		/// <summary>
		/// All property pairs where value is X bytes
		/// </summary>
		public List<(byte[] Id, byte[] Value)> xByteIdValuePairs;

		/// <summary>
		/// How many bytes the IO element takes
		/// </summary>
		public int sizeInBytes;

		/// <summary>
		/// Constructor (only one)
		/// </summary>
		/// <param name="bytes">Bytes to turn into IOElementCodec8Extended, all bytes might not be read</param>
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

	/// <summary>
	/// Codec8 Extended AVL Data structure
	/// </summary>
	public sealed class AvlDataCodec8Extended
	{

		/// <summary>
		/// Byte array for a difference, in milliseconds, between the current time and midnight, January, 1970 UTC (UNIX time). 8 bytes
		/// </summary>		
		public byte[] timestampBytes;

		/// <summary>
		/// Field which define AVL data priority
		/// </summary>
		public byte priority;

		/// <summary>
		/// GPS Element as bytes. 15 bytes
		/// </summary>
		public byte[] gpsElementBytes;

		/// <summary>
		/// IO Element as bytes
		/// </summary>
		public byte[] ioElementBytes;

		/// <summary>
		/// How many bytes the AVL Data structure takes
		/// </summary>
		public int sizeInBytes;

		/// <summary>
		/// Constructor (only one)
		/// </summary>
		/// <param name="bytes">Bytes to turn into AvlDataCodec8Extended, all bytes might not be read</param>
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

		/// <summary>
		/// Get the GPS Element
		/// </summary>
		/// <returns>GPSElement</returns>
		public GPSElement GetGPSElement()
		{
			return new GPSElement(this.gpsElementBytes);
		}

		/// <summary>
		/// Gets the IO Element
		/// </summary>
		/// <returns>IOElementCodec8Extended</returns>
		public IOElementCodec8Extended GetIOElement()
		{
			return new IOElementCodec8Extended(this.ioElementBytes);
		}
	}

	/// <summary>
	/// Codec8 Extended frame
	/// </summary>
	public sealed class Codec8ExtendedFrame
	{
		/// <summary>
		/// The packet start bytes, this should ALWAYS be four zero values
		/// </summary>
		public byte[] preambleBytes;

		/// <summary>
		/// Size is calculated starting from Codec ID to Number of Data 2, big-endian four byte array
		/// </summary>
		public byte[] dataFieldLengthBytes;

		/// <summary>
		/// Codec ID, in Codec8 Extended it is always 0x8E
		/// </summary>
		public byte codecId;

		/// <summary>
		/// A number which defines how many records is in the packet
		/// </summary>
		public byte numberOfData1;

		/// <summary>
		/// A number which defines how many records is in the packet. This number must be the same as “Number of Data 1”
		/// </summary>
		public byte numberOfData2;

		/// <summary>
		/// Actual data bytes in the packet
		/// </summary>
		public List<byte[]> avlDataBytesList;

		/// <summary>
		/// Calculated from Codec ID to the Second Number of Data. CRC (Cyclic Redundancy Check) is an error-detecting code using for detect accidental changes to RAW data. For calculation we are using CRC-16/IBM. 4 bytes.
		/// </summary>
		public byte[] crc16;

		/// <summary>
		/// Constructor (only one)
		/// </summary>
		/// <param name="preamble">Preamble as bytes</param>
		/// <param name="dataFieldLength">Data field length as big-endian bytes</param>
		/// <param name="id">Codec ID as byte</param>
		/// <param name="records1">Number of records (first)</param>
		/// <param name="records2">Number of records (second)</param>
		/// <param name="avlData">AVL data elements as list of bytes</param>
		/// <param name="crc">CRC as bytes</param>
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

		/// <summary>
		/// Get data field length
		/// </summary>
		/// <returns>How many bytes are between Codec ID and Number Of Data 2</returns>
		public uint GetDataFieldLength()
		{
			uint returnValue = BitConverter.ToUInt32(this.dataFieldLengthBytes);
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Get all AvlDataCodec8Extended structures
		/// </summary>
		/// <returns>List of AvlDataCodec8Extended structures</returns>
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

	/// <summary>
	/// Codec8 Extended decoder (static)
	/// </summary>
	public static class Codec8ExtendedDecoder
	{
		/// <summary>
		/// Expected preamble value
		/// </summary>
		public const uint preambleExpected = 0;

		/// <summary>
		/// Expected codec ID
		/// </summary>
		public const byte codec8ExtendedId = 0x8E;

		/// <summary>
		/// Valid priority values
		/// </summary>
		/// <returns></returns>
		public static readonly ImmutableHashSet<byte> validPriorities = ImmutableHashSet.Create<byte> ( 0, 1, 2 );

		/// <summary>
		/// Try to parse Codec8ExtendedFrame from given hexadecimal string
		/// </summary>
		/// <param name="hexadecimal">Hexadecimal input string</param>
		/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either Codec8ExtendedFrame or error string</returns>
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

		/// <summary>
		/// Try to parse Codec8ExtendedFrame from given byte array
		/// </summary>
		/// <param name="bytes">Byte array input</param>
		/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either Codec8ExtendedFrame or error string</returns>
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
				if (!validPriorities.Contains(avlData.priority))
				{
					return (GenericDecodeResult.IncorrectPriority, $"Priority value {avlData.priority} is not valid");
				}
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
