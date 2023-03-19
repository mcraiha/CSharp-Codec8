namespace tests;

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
		AvlData avlData = frame.GetAvlData();
		GPSElement gpsElement = avlData.GetGPSElement();
		IOElement ioElement = avlData.GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x36 }, frame.dataFieldLengthBytes);

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xC7, 0xCF }, frame.crc16);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD8, 0xEA, 0x30 }, avlData.timestampBytes);

		Assert.AreEqual(1, avlData.priority);

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
}