using System;
using System.Text;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Buffers.Binary;

namespace Codec8;

/// <summary>
/// UDP Datagram header
/// </summary>
public sealed class UdpChannelHeader
{
	/// <summary>
	/// Packet length (excluding this field) in bytes
	/// </summary>
	public readonly ushort packetLengthInBytes;

	/// <summary>
	/// Packet ID unique for this channel
	/// </summary>
	/// <remarks>Always 2 bytes</remarks>
	public readonly byte[] packetId;

	/// <summary>
	/// Not usable byte
	/// </summary>
	public readonly byte notUsableByte;

	/// <summary>
	/// Only constructor
	/// </summary>
	/// <param name="bytes">Bytes to turn into UdpChannelHeader</param>
	public UdpChannelHeader(ReadOnlySpan<byte> bytes)
	{
		int currentIndex = 0;
		
		this.packetLengthInBytes = BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(bytes.Slice(currentIndex)));
		currentIndex += 2;

		this.packetId = new byte[2];
		bytes.Slice(currentIndex, 2).CopyTo(this.packetId);
		currentIndex += 2;
	
		this.notUsableByte = bytes[currentIndex];
		currentIndex++;
	}

	/// <summary>
	/// Get Packet ID as Ushort
	/// </summary>
	/// <returns>Packet ID as Ushort</returns>
	public ushort GetPacketIdAsUshort()
	{
		return BitConverter.ToUInt16(this.packetId);
	}
}

/// <summary>
/// AVL data encapsulated in UDP channel packet
/// </summary>
public sealed class AvlDataEncapsulated
{
	/// <summary>
	/// ID identifying this AVL packet
	/// </summary>
	public readonly byte avlPacketId;

	/// <summary>
	/// How many bytes are in IMEI array (this should always be 15)
	/// </summary>
	public readonly ushort imeiLengthInBytes;

	/// <summary>
	/// IMEI as byte array (numbers are ASCII bytes)
	/// </summary>
	/// <remarks>Use GetImei() instead of this</remarks>
	public readonly byte[] imei;

	/// <summary>
	/// Only constructor
	/// </summary>
	/// <param name="bytes"></param>
	public AvlDataEncapsulated(ReadOnlySpan<byte> bytes)
	{
		int currentIndex = 0;

		this.avlPacketId = bytes[currentIndex];
		currentIndex++;

		this.imeiLengthInBytes = BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(bytes.Slice(currentIndex)));
		currentIndex += 2;

		this.imei = new byte[this.imeiLengthInBytes];
		bytes.Slice(currentIndex, this.imeiLengthInBytes).CopyTo(this.imei);
	}

	/// <summary>
	/// Get IMEI as string
	/// </summary>
	/// <example>352093086403655</example>
	/// <returns>IMEI as string</returns>
	public string GetImei()
	{
		return Encoding.UTF8.GetString(this.imei);
	}
}

/// <summary>
/// Codec8 UDP decoder (static)
/// </summary>
public static class Codec8UdpDecoder
{
	/// <summary>
	/// Expected codec ID
	/// </summary>
	public const byte codec8Id = 0x08;

	/// <summary>
	/// Valid priority values
	/// </summary>
	/// <returns></returns>
	public static readonly FrozenSet<byte> validPriorities = new HashSet<byte>(){ 0, 1, 2 }.ToFrozenSet();

	/// <summary>
	/// Try to parse UdpChannelHeader + AvlDataEncapsulated + Codec8FrameNoCRC tuple from given hexadecimal string
	/// </summary>
	/// <param name="hexadecimal">Hexadecimal input string (e.g. 003DCAFE0105 ... or 00-3D-CA-FE-01-05 ...)</param>
	/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either <c>UdpChannelHeader</c> + <c>AvlDataEncapsulated</c> + <c>Codec8FrameNoCRC</c> tuple or error string</returns>
	public static (GenericDecodeResult result, object valueOrError) ParseHexadecimalString(string hexadecimal)
	{
		if (string.IsNullOrEmpty(hexadecimal))
		{
			return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
		}

		hexadecimal = hexadecimal.Replace("-", "");

		if (!HexTools.CheckIfHexOnly(hexadecimal))
		{
			int index = HexTools.FindFirstNonHexPos(hexadecimal);
			return (GenericDecodeResult.ContainsNonHexValues, $"Input contains non-hexadecimal char '{hexadecimal[index]}' at pos {index}");
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
	/// Try to parse UdpChannelHeader + AvlDataEncapsulated + Codec8FrameNoCRC tuple from given byte array
	/// </summary>
	/// <param name="bytes">Byte array input</param>
	/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either <c>UdpChannelHeader</c> + <c>AvlDataEncapsulated</c> + <c>Codec8FrameNoCRC</c> tuple or error string</returns>
	public static (GenericDecodeResult result, object valueOrError) ParseByteArray(ReadOnlySpan<byte> bytes)
	{
		if (bytes.Length < 1)
		{
			return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
		}

		int currentIndex = 0;

		// Check that given length and given amount of bytes match
		ushort firstTwoBytesWithEndiannessSwap = BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(bytes));
		if (firstTwoBytesWithEndiannessSwap + 2 != bytes.Length)
		{
			return (GenericDecodeResult.PacketLengthMismatch, $"Packet length is: {firstTwoBytesWithEndiannessSwap + 2} but actual data length is {bytes.Length}");
		}

		UdpChannelHeader udpChannelHeader = new UdpChannelHeader(bytes);
		currentIndex += 5;

		AvlDataEncapsulated avlDataEncapsulated = new AvlDataEncapsulated(bytes.Slice(currentIndex));
		currentIndex += 18;

		if (avlDataEncapsulated.imeiLengthInBytes != 15)
		{
			return (GenericDecodeResult.WrongImeiLength, $"Only valid IMEI length is: 15 bytes but data says {avlDataEncapsulated.imeiLengthInBytes} bytes");
		}

		foreach (byte b in avlDataEncapsulated.imei)
		{
			if (b < (byte)'0' || b > (byte)'9')
			{
				return (GenericDecodeResult.ImeiContainsNonNumbers, $"IMEI contains non-number characters");
			}
		}

		byte codecId = bytes[currentIndex];
		currentIndex += sizeof(byte);

		if (codecId != codec8Id)
		{
			return (GenericDecodeResult.IncorrectCodecId, $"Expected {codec8Id:X2} as codec ID, but got {codecId:X2} instead");
		}

		byte numberOfData1 = bytes[currentIndex];
		currentIndex += sizeof(byte);

		List<byte[]> avlDataBytesList = new List<byte[]>();
		while (currentIndex < bytes.Length - 1)
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
		
		Codec8FrameNoCRC frame = new Codec8FrameNoCRC(codecId, numberOfData1, numberOfData2, avlDataBytesList);

		return (GenericDecodeResult.SuccessCodec8Udp, (udpChannelHeader, avlDataEncapsulated, frame));
	}
}

/// <summary>
/// Codec8 Extended UDP decoder (static)
/// </summary>
public static class Codec8ExtendedUdpDecoder
{
	/// <summary>
	/// Expected codec ID
	/// </summary>
	public const byte codec8ExtendedId = 0x8E;

	/// <summary>
	/// Valid priority values
	/// </summary>
	/// <returns></returns>
	public static readonly FrozenSet<byte> validPriorities = new HashSet<byte>(){ 0, 1, 2 }.ToFrozenSet();

	/// <summary>
	/// Try to parse UdpChannelHeader + AvlDataEncapsulated + Codec8ExtendedFrameNoCRC tuple from given hexadecimal string
	/// </summary>
	/// <param name="hexadecimal">Hexadecimal input string</param>
	/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either <c>UdpChannelHeader</c> + <c>AvlDataEncapsulated</c> + <c>Codec8ExtendedFrameNoCRC</c> tuple or error string</returns>
	public static (GenericDecodeResult result, object valueOrError) ParseHexadecimalString(string hexadecimal)
	{
		if (string.IsNullOrEmpty(hexadecimal))
		{
			return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
		}

		hexadecimal = hexadecimal.Replace("-", "");

		if (!HexTools.CheckIfHexOnly(hexadecimal))
		{
			int index = HexTools.FindFirstNonHexPos(hexadecimal);
			return (GenericDecodeResult.ContainsNonHexValues, $"Input contains non-hexadecimal char '{hexadecimal[index]}' at pos {index}");
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
	/// Try to parse UdpChannelHeader + AvlDataEncapsulated + Codec8ExtendedFrameNoCRC tuple from given byte array
	/// </summary>
	/// <param name="bytes">Byte array input</param>
	/// <returns>GenericDecodeResult to indicate if parse was success, and valueOrError that contains either <c>UdpChannelHeader</c> + <c>AvlDataEncapsulated</c> + <c>Codec8ExtendedFrameNoCRC</c> tuple or error string</returns>
	public static (GenericDecodeResult result, object valueOrError) ParseByteArray(ReadOnlySpan<byte> bytes)
	{
		if (bytes.Length < 1)
		{
			return (GenericDecodeResult.InputNullOrEmpty, $"Input is null or empty");
		}

		int currentIndex = 0;

		// Check that given length and given amount of bytes match
		ushort firstTwoBytesWithEndiannessSwap = BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(bytes));
		if (firstTwoBytesWithEndiannessSwap + 2 != bytes.Length)
		{
			return (GenericDecodeResult.PacketLengthMismatch, $"Packet length is: {firstTwoBytesWithEndiannessSwap + 2} but actual data length is {bytes.Length}");
		}

		UdpChannelHeader udpChannelHeader = new UdpChannelHeader(bytes);
		currentIndex += 5;

		AvlDataEncapsulated avlDataEncapsulated = new AvlDataEncapsulated(bytes.Slice(currentIndex));
		currentIndex += 18;

		if (avlDataEncapsulated.imeiLengthInBytes != 15)
		{
			return (GenericDecodeResult.WrongImeiLength, $"Only valid IMEI length is: 15 bytes but data says {avlDataEncapsulated.imeiLengthInBytes} bytes");
		}

		foreach (byte b in avlDataEncapsulated.imei)
		{
			if (b < (byte)'0' || b > (byte)'9')
			{
				return (GenericDecodeResult.ImeiContainsNonNumbers, $"IMEI contains non-number characters");
			}
		}

		byte codecId = bytes[currentIndex];
		currentIndex += sizeof(byte);

		if (codecId != codec8ExtendedId)
		{
			return (GenericDecodeResult.IncorrectCodecId, $"Expected {codec8ExtendedId:X2} as codec ID, but got {codecId:X2} instead");
		}

		byte numberOfData1 = bytes[currentIndex];
		currentIndex += sizeof(byte);

		List<byte[]> avlDataBytesList = new List<byte[]>();
		while (currentIndex < bytes.Length - 1)
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
		
		Codec8ExtendedFrameNoCRC frame = new Codec8ExtendedFrameNoCRC(codecId, numberOfData1, numberOfData2, avlDataBytesList);

		return (GenericDecodeResult.SuccessCodec8ExtendedUdp, (udpChannelHeader, avlDataEncapsulated, frame));
	}
}