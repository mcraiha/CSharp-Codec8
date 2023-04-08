using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Buffers.Binary;

namespace Codec8
{
	/// <summary>
	/// Single IO element of Codec8
	/// </summary>
	public sealed class IOElementCodec8
	{
		/// <summary>
		/// Event IO ID – if data is acquired on event – this field defines which IO property has changed and generated an event. For example, when if Ignition state changed and it generate event, Event IO ID will be 0xEF (AVL ID: 239). If it’s not eventual record – the value is 0.
		/// </summary>
		public byte eventIoId;

		/// <summary>
		/// A total number of properties coming with record (N = N1 + N2 + N4 + N8)
		/// </summary>
		public byte totalCount;

		/// <summary>
		/// Number of properties, which length is 1 byte
		/// </summary>
		public byte oneByteValuesCount;

		/// <summary>
		/// All property pairs where value is 1 byte
		/// </summary>
		public List<(byte Id, byte Value)> oneByteIdValuePairs;

		/// <summary>
		/// Number of properties, where value length is 2 bytes
		/// </summary>
		public byte twoByteValuesCount;

		/// <summary>
		/// All property pairs where value is 2 bytes
		/// </summary>
		public List<(byte Id, byte[] Value)> twoByteIdValuePairs;

		/// <summary>
		/// Number of properties, where value length is 4 bytes
		/// </summary>
		public byte fourByteValuesCount;

		/// <summary>
		/// All property pairs where value is 4 bytes
		/// </summary>
		public List<(byte Id, byte[] Value)> fourByteIdValuePairs;

		/// <summary>
		/// Number of properties, where value length is 8 bytes
		/// </summary>
		public byte eightByteValuesCount;

		/// <summary>
		/// All property pairs where value is 8 bytes
		/// </summary>
		public List<(byte Id, byte[] Value)> eightByteIdValuePairs;

		/// <summary>
		/// How many bytes the IO element takes
		/// </summary>
		public int sizeInBytes;

		/// <summary>
		/// Constructor (only one)
		/// </summary>
		/// <param name="bytes">Bytes to turn into IOElementCodec8, all bytes might not be read</param>
		public IOElementCodec8(ReadOnlySpan<byte> bytes)
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

	/// <summary>
	/// Codec8 AVL Data structure
	/// </summary>
	public sealed class AvlDataCodec8
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
		/// <param name="bytes">Bytes to turn into AvlDataCodec8, all bytes might not be read</param>
		public AvlDataCodec8(ReadOnlySpan<byte> bytes)
		{
			int currentIndex = 0;

			this.timestampBytes = bytes.Slice(currentIndex, 8).ToArray();
			currentIndex += 8;

			this.priority = bytes[currentIndex];
			currentIndex++;

			this.gpsElementBytes = bytes.Slice(currentIndex, 15).ToArray();
			currentIndex += 15;

			IOElementCodec8 ioElement = new IOElementCodec8(bytes.Slice(currentIndex));
			this.ioElementBytes = bytes.Slice(currentIndex, ioElement.sizeInBytes).ToArray();
			currentIndex += ioElement.sizeInBytes;

			this.sizeInBytes = currentIndex;
		}

		/// <summary>
		/// Get timestamp as DateTimeOffset
		/// </summary>
		/// <returns>DateTimeOffset</returns>
		public DateTimeOffset GetTimestamp()
		{
			ulong totalMilliseconds = BytesToNumbers.GetUInt64(this.timestampBytes);

			return DateTimeOffset.FromUnixTimeMilliseconds((long)totalMilliseconds);
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
		/// <returns>IOElementCodec8</returns>
		public IOElementCodec8 GetIOElement()
		{
			return new IOElementCodec8(this.ioElementBytes);
		}
	}

	/// <summary>
	/// Codec8 frame
	/// </summary>
	public sealed class Codec8Frame
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
		/// Codec ID, in Codec8 it is always 0x08
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

		/// <summary>
		/// Get data field length
		/// </summary>
		/// <returns>How many bytes are between Codec ID and Number Of Data 2</returns>
		public uint GetDataFieldLength()
		{
			return BytesToNumbers.GetUInt32(this.dataFieldLengthBytes);
		}

		/// <summary>
		/// Get all AvlDataCodec8 structures
		/// </summary>
		/// <returns>List of AvlDataCodec8 structures</returns>
		public IReadOnlyList<AvlDataCodec8> GetAvlDatas()
		{
			List<AvlDataCodec8> returnList = new List<AvlDataCodec8>();
			foreach (byte[] bytes in this.avlDataBytesList)
			{
				returnList.Add(new AvlDataCodec8(bytes));
			}

			return returnList;
		}
	}

	/// <summary>
	/// Codec8 decoder (static)
	/// </summary>
	public static class Codec8Decoder
	{
		/// <summary>
		/// Expected preamble value
		/// </summary>
		public const uint preambleExpected = 0;

		/// <summary>
		/// Expected codec ID
		/// </summary>
		public const byte codec8Id = 0x08;

		/// <summary>
		/// Valid priority values
		/// </summary>
		/// <returns></returns>
		public static readonly ImmutableHashSet<byte> validPriorities = ImmutableHashSet.Create<byte> ( 0, 1, 2 );

		/// <summary>
		/// Try to parse Codec8Frame from given hexadecimal string
		/// </summary>
		/// <param name="hexadecimal">Hexadecimal input string (e.g. 00000000012 ... or 00-00-00-00-00-12 ..)</param>
		/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either Codec8Frame or error string</returns>
		public static (GenericDecodeResult result, object valueOrError) ParseHexadecimalString(string hexadecimal)
		{
			if (string.IsNullOrEmpty(hexadecimal))
			{
				return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
			}

			hexadecimal = hexadecimal.Replace("-", "");

			if (!HexTools.CheckIfHexOnly(hexadecimal))
			{
				return (GenericDecodeResult.ContainsNonHexValues, $"Input contains non-hexadecimal value");
			}

			if (hexadecimal.Length % 2 == 1)
			{
				return (GenericDecodeResult.OddNumberOfHexValues, $"Input has odd number of hex values");
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
		/// Try to parse Codec8Frame from given byte array
		/// </summary>
		/// <param name="bytes">Byte array input</param>
		/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either Codec8Frame or error string</returns>
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
			uint dataFieldLength = BytesToNumbers.GetUInt32(dataFieldLengthBytes);

			currentIndex += sizeof(uint);

			//Console.WriteLine($"{currentIndex + dataFieldLength + 4} {bytes.Length}");

			if (currentIndex + dataFieldLength + 4 > bytes.Length)
			{
				return (GenericDecodeResult.DataFieldLengthTooBig, $"Datafield says there should be {dataFieldLength} bytes, but not that many bytes exist");
			}

			int crcStartOffset = currentIndex;
			int crcEndPos = (int)(currentIndex + dataFieldLength);

			byte codecId = bytes[currentIndex];
			currentIndex += sizeof(byte);

			if (codecId != codec8Id)
			{
				return (GenericDecodeResult.IncorrectCodecId, $"Expected {codec8Id:X2} as codec ID, but got {codecId:X2} instead");
			}

			byte numberOfData1 = bytes[currentIndex];
			currentIndex += sizeof(byte);

			List<byte[]> avlDataBytesList = new List<byte[]>();
			while (currentIndex < crcEndPos - 1)
			{
				AvlDataCodec8 avlData = new AvlDataCodec8(bytes.Slice(currentIndex));
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

			if (numberOfData1 != avlDataBytesList.Count)
			{
				return (GenericDecodeResult.DataFieldLengthAndNumberOfDataMismatch, $"Cannot find {numberOfData1} elements from given data field that is {dataFieldLength} bytes, count is: {avlDataBytesList.Count}");
			}

			ReadOnlySpan<byte> calculateCrcFrom = Crc16.Calculate(bytes.Slice(crcStartOffset, crcEndPos - crcStartOffset));

			ReadOnlySpan<byte> crcBytes = bytes.Slice(currentIndex, 4);

			if (!crcBytes.SequenceEqual(calculateCrcFrom))
			{
				return (GenericDecodeResult.CrcMismatch, $"Mismatch in CRC: {BitConverter.ToString(calculateCrcFrom.ToArray()).Replace("-","")} vs. {BitConverter.ToString(crcBytes.ToArray()).Replace("-","")}");
			}
			
			Codec8Frame frame = new Codec8Frame(preambleBytes, dataFieldLengthBytes, codecId, numberOfData1, numberOfData2, avlDataBytesList, crcBytes);

			return (GenericDecodeResult.SuccessCodec8, frame);
		}
	}
}
