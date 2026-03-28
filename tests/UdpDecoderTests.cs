namespace tests;

#pragma warning disable NUnit2005
#pragma warning disable NUnit2049
public class Codec8UdpDecoderTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test, Description("First sample from Wiki, https://wiki.teltonika-gps.com/view/Codec#Codec_8 Codec8 protocol sending over UDP")]
	public void DecodeFirstExampleCodec8UdpTest()
	{
		// Arrange
		string input = "003DCAFE0105000F33353230393330383634303336353508010000016B4F815B30010000000000000000000000000000000103021503010101425DBC000001";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8UdpDecoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8Udp, result, $"Expected success, but got: {valueOrError}");

		(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8FrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8FrameNoCRC c))valueOrError;

		Assert.AreEqual(61, header.packetLengthInBytes);
		Assert.AreEqual(65226, header.packetId);
		Assert.AreEqual(1, header.notUsableByte);

		Assert.AreEqual(5, avlDataEncapsulated.avlPacketId);
		Assert.AreEqual(15, avlDataEncapsulated.imeiLengthInBytes);
		Assert.AreEqual("352093086403655", avlDataEncapsulated.GetImei());

		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement = avlDatas[0].GetIOElement();

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x4F, 0x81, 0x5B, 0x30 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 13, hour: 6, minute: 23, second: 26, TimeSpan.Zero), avlDatas[0].GetTimestamp());

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.angleBytes);

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.speedBytes);
		Assert.IsFalse(gpsElement.IsGPSValid(), "GPS value should be invalid");

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
		CollectionAssert.AreEqual(new byte[] { 0x5D, 0xBC }, ioElement.twoByteIdValuePairs[0].Value);
	}

	[Test, Description("First sample from Wiki, https://wiki.teltonika-gps.com/view/Codec#Codec_8_Extended Codec8 Extended protocol sending over UDP")]
	public void DecodeFirstExampleCodec8ExtendedUdpTest()
	{
		// Arrange
		string input = "005FCAFE0107000F3335323039333038363430333635358E010000016B4F831C680100000000000000000000000000000000010005000100010100010011009D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A000001";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8ExtendedUdpDecoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8ExtendedUdp, result, $"Expected success, but got: {valueOrError}");

		(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8ExtendedFrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8ExtendedFrameNoCRC c))valueOrError;

		Assert.AreEqual(95, header.packetLengthInBytes);
		Assert.AreEqual(65226, header.packetId);
		Assert.AreEqual(1, header.notUsableByte);

		Assert.AreEqual(7, avlDataEncapsulated.avlPacketId);
		Assert.AreEqual(15, avlDataEncapsulated.imeiLengthInBytes);
		Assert.AreEqual("352093086403655", avlDataEncapsulated.GetImei());

		IReadOnlyList<AvlDataCodec8Extended> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8Extended ioElement = avlDatas[0].GetIOElement();

		Assert.AreEqual(Codec8ExtendedUdpDecoder.codec8ExtendedId, frame.codecId, "Should be Codec8 extended");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x4F, 0x83, 0x1C, 0x68 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 13, hour: 6, minute: 25, second: 21, TimeSpan.Zero), avlDatas[0].GetTimestamp());

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.angleBytes);

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.speedBytes);
		Assert.IsFalse(gpsElement.IsGPSValid(), "GPS value should be invalid");

		// IO element data
		CollectionAssert.AreEqual(new byte[] { 0, 1 }, ioElement.eventIoId);
		CollectionAssert.AreEqual(new byte[] { 0, 5 }, ioElement.totalCount);

		CollectionAssert.AreEqual(new byte[] { 0, 1 }, ioElement.oneByteValuesCountBytes);
		Assert.AreEqual(1, ioElement.oneByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 1 }, ioElement.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 1 }, ioElement.twoByteValuesCountBytes);
		Assert.AreEqual(1, ioElement.twoByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 0x11 }, ioElement.twoByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x9D }, ioElement.twoByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 1 }, ioElement.fourByteValuesCountBytes);
		Assert.AreEqual(1, ioElement.fourByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 0x10 }, ioElement.fourByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x01, 0x5E, 0x2C, 0x88 }, ioElement.fourByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 2 }, ioElement.eightByteValuesCountBytes);
		Assert.AreEqual(2, ioElement.eightByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 0x0B }, ioElement.eightByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x35, 0x44, 0xC8, 0x7A }, ioElement.eightByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 0x0E }, ioElement.eightByteIdValuePairs[1].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x1D, 0xD7, 0xE0, 0x6A }, ioElement.eightByteIdValuePairs[1].Value);
	}
}