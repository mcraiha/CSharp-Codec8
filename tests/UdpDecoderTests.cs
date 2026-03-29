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

	[Test, Description("First sample from GitHub, https://github.com/alim-zanibekov/teltonika/blob/1465a609effa2716b015afb3ebfe279213663b46/teltonika_test.go#L383")]
	public void DecodeSecondExampleCodec8UdpTest()
	{
		// Arrange
		string input = "01E4CAFE0126000F333532303934303839333937343634080400000163C803B420010A259E1A1D4A057D00DA0128130057421B0A4503F00150051503EF01510052005900BE00C1000AB50008B60005427025CD79D8CE605A5400005500007300005A0000C0000007C700000018F1000059D910002D32C85300000000570000000064000000F7BF000000000000000163C803AC50010A25A9D21D4A01B600DB0128130056421B0A4503F00150051503EF01510052005900BE00C1000AB50008B6000542702ECD79D8CE605A5400005500007300005A0000C0000007C700000017F1000059D910002D32B05300000000570000000064000000F7BF000000000000000163C803A868010A25B5581D49FE5400DB0127130057421B0A4503F00150051503EF01510052005900BE00C1000AB50008B60005427039CD79D8CE605A5400005500007300005A0000C0000007C700000017F1000059D910002D32995300000000570000000064000000F7BF000000000000000163C803A4B2010A25CC861D49F75C00DB0124130058421B0A4503F00150051503EF01510052005900BE00C1000AB50008B6000542703CCD79D8CE605A5400005500007300005A0000C0000007C700000018F1000059D910002D32695300000000570000000064000000F7BF000000000004";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8UdpDecoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8Udp, result, $"Expected success, but got: {valueOrError}");

		(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8FrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8FrameNoCRC c))valueOrError;

		Assert.AreEqual(484, header.packetLengthInBytes);
		Assert.AreEqual(65226, header.packetId);
		Assert.AreEqual(1, header.notUsableByte);

		Assert.AreEqual(38, avlDataEncapsulated.avlPacketId);
		Assert.AreEqual(15, avlDataEncapsulated.imeiLengthInBytes);
		Assert.AreEqual("352094089397464", avlDataEncapsulated.GetImei());

		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement = avlDatas[0].GetIOElement();

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(4, frame.numberOfData1);
		Assert.AreEqual(4, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x63, 0xC8, 0x03, 0xB4, 0x20 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2018, month: 6, day: 3, hour: 23, minute: 37, second: 56, TimeSpan.Zero), avlDatas[0].GetTimestamp());

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data
		CollectionAssert.AreEqual(new byte[] { 10, 37, 158, 26 }, gpsElement.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 29, 74, 5, 125 }, gpsElement.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 218 }, gpsElement.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 1, 40 }, gpsElement.angleBytes);

		Assert.AreEqual(19, gpsElement.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 87 }, gpsElement.speedBytes);
		Assert.IsTrue(gpsElement.IsGPSValid(), "GPS value should be valid");

		// IO element data
		Assert.AreEqual(66, ioElement.eventIoId);
		Assert.AreEqual(27, ioElement.totalCount);

		Assert.AreEqual(10, ioElement.oneByteValuesCount);
		Assert.AreEqual(10, ioElement.oneByteIdValuePairs.Count);

		Assert.AreEqual(0x45, ioElement.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0x03, ioElement.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(0xF0, ioElement.oneByteIdValuePairs[1].Id);
		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[1].Value);

		Assert.AreEqual(10, ioElement.twoByteValuesCount);
		Assert.AreEqual(10, ioElement.twoByteIdValuePairs.Count);

		Assert.AreEqual(0xB5, ioElement.twoByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x08 }, ioElement.twoByteIdValuePairs[0].Value);

		Assert.AreEqual(7, ioElement.fourByteValuesCount);
		Assert.AreEqual(7, ioElement.fourByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement.eightByteValuesCount);
		Assert.AreEqual(0, ioElement.eightByteIdValuePairs.Count);
	}
}

public class Codec8ExtendedUdpDecoderTests
{
	[SetUp]
	public void Setup()
	{
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

	[Test, Description("Second sample from GitHub, https://github.com/alim-zanibekov/teltonika/blob/1465a609effa2716b015afb3ebfe279213663b46/teltonika_test.go#L381")]
	public void DecodeSecondExampleCodec8ExtendedUdpTest()
	{
		// Arrange
		string input = "0086CAFE0101000F3335323039333038353639383230368E0100000167EFA919800200000000000000000000000000000000FC0013000800EF0000F00000150500C80000450200010000710000FC00000900B5000000B600000042305600CD432A00CE6064001100090012FF22001303D1000F0000000200F1000059D90010000000000000000001";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8ExtendedUdpDecoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8ExtendedUdp, result, $"Expected success, but got: {valueOrError}");

		(UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8ExtendedFrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8ExtendedFrameNoCRC c))valueOrError;

		Assert.AreEqual(134, header.packetLengthInBytes);
		Assert.AreEqual(65226, header.packetId);
		Assert.AreEqual(1, header.notUsableByte);

		Assert.AreEqual(1, avlDataEncapsulated.avlPacketId);
		Assert.AreEqual(15, avlDataEncapsulated.imeiLengthInBytes);
		Assert.AreEqual("352093085698206", avlDataEncapsulated.GetImei());

		IReadOnlyList<AvlDataCodec8Extended> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8Extended ioElement = avlDatas[0].GetIOElement();

		Assert.AreEqual(Codec8ExtendedUdpDecoder.codec8ExtendedId, frame.codecId, "Should be Codec8 extended");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x67, 0xEF, 0xA9, 0x19, 0x80 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2018, month: 12, day: 27, hour: 12, minute: 34, second: 56, TimeSpan.Zero), avlDatas[0].GetTimestamp());

		Assert.AreEqual(2, avlDatas[0].priority);

		// GPS element data
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.angleBytes);

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement.speedBytes);
		Assert.IsFalse(gpsElement.IsGPSValid(), "GPS value should be invalid");

		// IO element data
		CollectionAssert.AreEqual(new byte[] { 0, 252 }, ioElement.eventIoId);
		CollectionAssert.AreEqual(new byte[] { 0, 19 }, ioElement.totalCount);

		CollectionAssert.AreEqual(new byte[] { 0, 8 }, ioElement.oneByteValuesCountBytes);
		Assert.AreEqual(8, ioElement.oneByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 239 }, ioElement.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(0, ioElement.oneByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 9 }, ioElement.twoByteValuesCountBytes);
		Assert.AreEqual(9, ioElement.twoByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 181 }, ioElement.twoByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00 }, ioElement.twoByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 2 }, ioElement.fourByteValuesCountBytes);
		Assert.AreEqual(2, ioElement.fourByteIdValuePairs.Count);

		CollectionAssert.AreEqual(new byte[] { 0, 241 }, ioElement.fourByteIdValuePairs[0].Id);
		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x59, 0xD9 }, ioElement.fourByteIdValuePairs[0].Value);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, ioElement.eightByteValuesCountBytes);
		Assert.AreEqual(0, ioElement.eightByteIdValuePairs.Count);
	}
}