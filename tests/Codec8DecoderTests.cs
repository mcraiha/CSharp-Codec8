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
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result, $"Expected success, but got: {valueOrError}");

		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement = avlDatas[0].GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x36 }, frame.dataFieldLengthBytes);
		Assert.AreEqual(54, frame.GetDataFieldLength());

		Assert.AreEqual(51, avlDatas[0].sizeInBytes, "Only AvlDataCodec8 should be 3 bytes less than data field length");

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xC7, 0xCF }, frame.crc16);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD8, 0xEA, 0x30 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 10, hour: 10, minute: 4, second: 46, TimeSpan.Zero), avlDatas[0].GetTimestamp());

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
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result, $"Expected success, but got: {valueOrError}");

		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement = avlDatas[0].GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x28 }, frame.dataFieldLengthBytes);
		Assert.AreEqual(40, frame.GetDataFieldLength());

		Assert.AreEqual(37, avlDatas[0].sizeInBytes, "Only AvlDataCodec8 should be 3 bytes less than data field length");

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xF2, 0x2A }, frame.crc16);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD9, 0xAD, 0x80 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 10, hour: 10, minute: 5, second: 36, TimeSpan.Zero), avlDatas[0].GetTimestamp());

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
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result, $"Expected success, but got: {valueOrError}");

		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();

		GPSElement gpsElement1 = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement1 = avlDatas[0].GetIOElement();

		GPSElement gpsElement2 = avlDatas[1].GetGPSElement();
		IOElementCodec8 ioElement2 = avlDatas[1].GetIOElement();

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0x43 }, frame.dataFieldLengthBytes);
		Assert.AreEqual(67, frame.GetDataFieldLength());

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(2, frame.numberOfData1);
		Assert.AreEqual(2, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x25, 0x2C }, frame.crc16);

		// First AVL

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x40, 0xD5, 0x7B, 0x48 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 10, hour: 10, minute: 1, second: 1, TimeSpan.Zero), avlDatas[0].GetTimestamp());

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data of first AVL
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement1.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement1.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement1.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement1.angleBytes);

		Assert.AreEqual(0, gpsElement1.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement1.speedBytes);
		Assert.IsFalse(gpsElement1.IsGPSValid(), "GPS value should be invalid");

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
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 10, hour: 10, minute: 1, second: 19, TimeSpan.Zero), avlDatas[1].GetTimestamp());

		Assert.AreEqual(1, avlDatas[1].priority);

		// GPS element data of first AVL
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement2.longitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, gpsElement2.latitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement2.altitudeBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement2.angleBytes);

		Assert.AreEqual(0, gpsElement2.visibleSatellites);

		CollectionAssert.AreEqual(new byte[] { 0, 0 }, gpsElement2.speedBytes);
		Assert.IsFalse(gpsElement2.IsGPSValid(), "GPS value should be invalid");

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

	
	[Test, Description("Random input from the internet")]
	public void DecodeRandomExampleCodec8Test()
	{
		// Arrange
		string input = "00000000000001CB080400000163c803eb02010a2524c01d4a377d00d3012f130032421b0a4503f00150051503ef01510052005900be00c1000ab50008b60006426fd8cd3d1ece605a5400005500007300005a0000c0000007c70000000df1000059d910002d33c65300000000570000000064000000f7bf000000000000000163c803e6e8010a2530781d4a316f00d40131130031421b0a4503f00150051503ef01510052005900be00c1000ab50008b60005426fcbcd3d1ece605a5400005500007300005a0000c0000007c70000000ef1000059d910002d33b95300000000570000000064000000f7bf000000000000000163c803df18010a2536961d4a2e4f00d50134130033421b0a4503f00150051503ef01510052005900be00c1000ab50008b6000542702bcd3d1ece605a5400005500007300005a0000c0000007c70000001ef1000059d910002d33aa5300000000570000000064000000f7bf000000000000000163c8039ce2010a25d8d41d49f42c00dc0123120058421b0a4503f00150051503ef01510052005900be00c1000ab50009b60005427031cd79d8ce605a5400005500007300005a0000c0000007c700000019f1000059d910002d32505300000000570000000064000000f7bf00000000000400003379";
		
		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(input);
		Codec8Frame frame = (Codec8Frame)valueOrError;
		IReadOnlyList<AvlDataCodec8> avlDatas = frame.GetAvlDatas();

		GPSElement gpsElement1 = avlDatas[0].GetGPSElement();
		IOElementCodec8 ioElement1 = avlDatas[0].GetIOElement();

		GPSElement gpsElement2 = avlDatas[1].GetGPSElement();
		IOElementCodec8 ioElement2 = avlDatas[1].GetIOElement();

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result, $"Expected success, but got: {valueOrError}");	

		CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, frame.preambleBytes);
		CollectionAssert.AreEqual(new byte[] { 0, 0, 0x01, 0xCB }, frame.dataFieldLengthBytes);
		Assert.AreEqual(459, frame.GetDataFieldLength());

		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");

		Assert.AreEqual(4, frame.numberOfData1);
		Assert.AreEqual(4, frame.numberOfData2);

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x33, 0x79 }, frame.crc16);

		// First AVL

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x63, 0xC8, 0x03, 0xEB, 0x02 }, avlDatas[0].timestampBytes);
		Assert.AreEqual(DateTimeOffset.FromUnixTimeMilliseconds(1528069090050), avlDatas[0].GetTimestamp());

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data of first AVL
		Assert.AreEqual(170206400, gpsElement1.GetLongitude());
		Assert.AreEqual(491403133, gpsElement1.GetLatitude());
		Assert.AreEqual(211, gpsElement1.GetAltitude());
		Assert.AreEqual(303, gpsElement1.GetAngle());

		Assert.AreEqual(19, gpsElement1.visibleSatellites);

		Assert.AreEqual(50, gpsElement1.GetSpeed());
		Assert.IsTrue(gpsElement1.IsGPSValid(), "GPS value should be valid");

		// IO element data of first AVL
		Assert.AreEqual(66, ioElement1.eventIoId);
		Assert.AreEqual(27, ioElement1.totalCount);

		Assert.AreEqual(10, ioElement1.oneByteValuesCount);
		Assert.AreEqual(10, ioElement1.oneByteIdValuePairs.Count);

		Assert.AreEqual(69, ioElement1.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(3, ioElement1.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(10, ioElement1.twoByteValuesCount);
		Assert.AreEqual(10, ioElement1.twoByteIdValuePairs.Count);

		Assert.AreEqual(7, ioElement1.fourByteValuesCount);
		Assert.AreEqual(7, ioElement1.fourByteIdValuePairs.Count);

		Assert.AreEqual(0, ioElement1.eightByteValuesCount);
		Assert.AreEqual(0, ioElement1.eightByteIdValuePairs.Count);


		// Second AVL

		CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0x01, 0x63, 0xC8, 0x03, 0xE6, 0xE8 }, avlDatas[1].timestampBytes);
		Assert.AreEqual(DateTimeOffset.FromUnixTimeMilliseconds(1528069089000), avlDatas[1].GetTimestamp());

		Assert.AreEqual(1, avlDatas[1].priority);

		// GPS element data of second AVL
		Assert.AreEqual(170209400, gpsElement2.GetLongitude());
		Assert.AreEqual(491401583, gpsElement2.GetLatitude());
		Assert.AreEqual(212, gpsElement2.GetAltitude());
		Assert.AreEqual(305, gpsElement2.GetAngle());

		Assert.AreEqual(19, gpsElement2.visibleSatellites);

		Assert.AreEqual(49, gpsElement2.GetSpeed());
		Assert.IsTrue(gpsElement2.IsGPSValid(), "GPS value should be valid");

		// IO element data of second AVL
		Assert.AreEqual(66, ioElement2.eventIoId);
		Assert.AreEqual(27, ioElement2.totalCount);

		Assert.AreEqual(10, ioElement2.oneByteValuesCount);
		Assert.AreEqual(10, ioElement2.oneByteIdValuePairs.Count);

		Assert.AreEqual(69, ioElement2.oneByteIdValuePairs[0].Id);
		Assert.AreEqual(3, ioElement2.oneByteIdValuePairs[0].Value);

		Assert.AreEqual(10, ioElement2.twoByteValuesCount);
		Assert.AreEqual(10, ioElement2.twoByteIdValuePairs.Count);

		Assert.AreEqual(7, ioElement2.fourByteValuesCount);
		Assert.AreEqual(7, ioElement2.fourByteIdValuePairs.Count);

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
			(GenericDecodeResult.ContainsNonHexValues, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CG"),
			(GenericDecodeResult.OddNumberOfHexValues, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF1"),
			(GenericDecodeResult.WrongPreamble, "000100000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF"),
			(GenericDecodeResult.DataFieldLengthTooBig, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7"),
			(GenericDecodeResult.IncorrectCodecId, "000000000000003609010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF"),
			(GenericDecodeResult.IncorrectPriority, "000000000000003608010000016B40D8EA30030000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF"),
			(GenericDecodeResult.NumberOfDataMismatch, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000020000C7CF"),
			(GenericDecodeResult.DataFieldLengthAndNumberOfDataMismatch, "000000000000003608020000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000020000C7CF"),
			(GenericDecodeResult.CrcMismatch, "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7DF"),
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