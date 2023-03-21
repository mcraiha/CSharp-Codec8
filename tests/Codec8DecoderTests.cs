namespace tests;

#pragma warning disable NUnit2005
public class Codec8DecoderTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test, Description("First sample from Wiki, https://wiki.teltonika-gps.com/view/Codec#Codec_8")]
	public void DecodeFirstExampleCodec8Test()
	{
		// Arrange
		string input = "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result);

		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement = avlDatas[0].GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x36 }, frame.dataFieldLengthBytes);

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xC7, 0xCF }, frame.crc16);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD8, 0xEA, 0x30 }, avlDatas[0].timestampBytes);

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.angleBytes);

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.speedBytes);

		// IO element data
		Assert.AreEqual(1, ioElement.eventIoId);
		Assert.AreEqual(5, ioElement.totalCount);

		Assert.AreEqual(2, ioElement.oneByteValuesCount);
		Assert.AreEqual(2, ioElement.oneByteIdValuePairs.Count);

		Assert.AreEqual(0x15, ioElement.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0x03, ioElement.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[1].Id);
		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[1].Value);

		Assert.AreEqual(1, ioElement.twoByteValuesCount);
		Assert.AreEqual(1, ioElement.twoByteIdValuePairs.Count);

		Assert.AreEqual(0x42, ioElement.twoByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x5E, 0x0F }, ioElement.twoByteIdValuePairs[0].Value);

		Assert.AreEqual(1, ioElement.fourByteValuesCount);
		Assert.AreEqual(1, ioElement.fourByteIdValuePairs.Count);

		Assert.AreEqual(0xF1, ioElement.fourByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x60, 0x1A }, ioElement.fourByteIdValuePairs[0].Value);

		Assert.AreEqual(1, ioElement.eightByteValuesCount);
		Assert.AreEqual(1, ioElement.eightByteIdValuePairs.Count);

		Assert.AreEqual(0x4E, ioElement.eightByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, ioElement.eightByteIdValuePairs[0].Value);
	}

	[Test, Description("Second sample from Wiki, https://wiki.teltonika-gps.com/view/Codec#Codec_8")]
	public void DecodeSecondExampleCodec8Test()
	{
		// Arrange
		string input = "000000000000002808010000016B40D9AD80010000000000000000000000000000000103021503010101425E100000010000F22A";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result);

		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement = avlDatas[0].GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x28 }, frame.dataFieldLengthBytes);

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xF2, 0x2A }, frame.crc16);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD9, 0xAD, 0x80 }, avlDatas[0].timestampBytes);

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.angleBytes);

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.speedBytes);

		// IO element data
		Assert.AreEqual(1, ioElement.eventIoId);
		Assert.AreEqual(3, ioElement.totalCount);

		Assert.AreEqual(2, ioElement.oneByteValuesCount);
		Assert.AreEqual(2, ioElement.oneByteIdValuePairs.Count);

		Assert.AreEqual(0x15, ioElement.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0x03, ioElement.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[1].Id);
		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[1].Value);

		Assert.AreEqual(1, ioElement.twoByteValuesCount);
		Assert.AreEqual(1, ioElement.twoByteIdValuePairs.Count);

		Assert.AreEqual(0x42, ioElement.twoByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x5E, 0x10 }, ioElement.twoByteIdValuePairs[0].Value);

		Assert.AreEqual(0, ioElement.fourByteValuesCount);
		Assert.AreEqual(0, ioElement.fourByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement.eightByteValuesCount);
		Assert.AreEqual(0, ioElement.eightByteIdValuePairs.Count);
	}

	[Test, Description("Third sample from Wiki, https://wiki.teltonika-gps.com/view/Codec#Codec_8")]
	public void DecodeThirdExampleCodec8Test()
	{
		// Arrange
		string input = "000000000000004308020000016B40D57B480100000000000000000000000000000001010101000000000000016B40D5C198010000000000000000000000000000000101010101000000020000252C";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result);

		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();

		GPSElement gpsElement1 = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement1 = avlDatas[0].GetIOElement();

		GPSElement gpsElement2 = avlDatas[1].GetGPSElement();
		IOElementCodec8 ioElement2 = avlDatas[1].GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x43 }, frame.dataFieldLengthBytes);

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(2, frame.numberOfData1);
		Assert.AreEqual(2, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x25, 0x2C }, frame.crc16);

		// First AVL

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD5, 0x7B, 0x48 }, avlDatas[0].timestampBytes);

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data of first AVL
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement1.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement1.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement1.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement1.angleBytes);

		Assert.AreEqual(0, gpsElement1.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement1.speedBytes);

		// IO element data  of first AVL
		Assert.AreEqual(1, ioElement1.eventIoId);
		Assert.AreEqual(1, ioElement1.totalCount);

		Assert.AreEqual(1, ioElement1.oneByteValuesCount);
		Assert.AreEqual(1, ioElement1.oneByteIdValuePairs.Count);

		Assert.AreEqual(0x01, ioElement1.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0x00, ioElement1.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(0, ioElement1.twoByteValuesCount);
		Assert.AreEqual(0, ioElement1.twoByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement1.fourByteValuesCount);
		Assert.AreEqual(0, ioElement1.fourByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement1.eightByteValuesCount);
		Assert.AreEqual(0, ioElement1.eightByteIdValuePairs.Count);

		// Second AVL

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD5, 0xC1, 0x98 }, avlDatas[1].timestampBytes);

		Assert.AreEqual(1, avlDatas[1].priority);

		// GPS element data of first AVL
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement2.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement2.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement2.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement2.angleBytes);

		Assert.AreEqual(0, gpsElement2.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement2.speedBytes);

		// IO element data  of first AVL
		Assert.AreEqual(1, ioElement2.eventIoId);
		Assert.AreEqual(1, ioElement2.totalCount);

		Assert.AreEqual(1, ioElement2.oneByteValuesCount);
		Assert.AreEqual(1, ioElement2.oneByteIdValuePairs.Count);

		Assert.AreEqual(0x01, ioElement2.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0x01, ioElement2.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(0, ioElement2.twoByteValuesCount);
		Assert.AreEqual(0, ioElement2.twoByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement2.fourByteValuesCount);
		Assert.AreEqual(0, ioElement2.fourByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement2.eightByteValuesCount);
		Assert.AreEqual(0, ioElement2.eightByteIdValuePairs.Count);
	}

	[Test, Description("Invalid inputs")]
	public void DecodeInvalidCodec8Test()
	{
		// Arrange
		List<(GenericDecodeResult expectedResult, string input)> invalids = new List<(GenericDecodeResult result, string input)>()
		{
			(GenericDecodeResult.InputNullOrEmpty, ""),
			(GenericDecodeResult.OddNumberOfHexValues, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF1"),
			(GenericDecodeResult.WrongPreamble, "000100000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF"),
			(GenericDecodeResult.IncorrectCodecId, "000000000000003609010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF"),
			(GenericDecodeResult.NumberOfDataMismatch, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000020000C7CF"),
		};

		// Act

		// Assert
		foreach (var pair in invalids)
		{
			(GenericDecodeResult result, _) = Codec8Decoder.ParseHexadecimalString(pair.input);
			Assert.AreEqual(pair.expectedResult, result);
		}
	}
}