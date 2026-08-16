namespace tests;

public class Codec8ExtendedDecoderTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test, Description("First sample from Wiki, https://wiki.teltonika-gps.com/view/Codec#Codec_8_Extended")]
	public void DecodeFirstExampleCodec8ExtendedTest()
	{
		// Arrange
		string input = "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8ExtendedDecoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8Extended, result, $"Expected success, but got: {valueOrError}");

		Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
		IReadOnlyList<AvlDataCodec8Extended> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8Extended ioElement = avlDatas[0].GetIOElement();

		Assert.That(frame.preambleBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
		Assert.That(frame.dataFieldLengthBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0x4A }));
		Assert.AreEqual(74, frame.GetDataFieldLength());

		Assert.AreEqual(71, avlDatas[0].sizeInBytes, "Only AvlDataCodec8Extended should be 3 bytes less than data field length");

		Assert.AreEqual(0x8E, frame.codecId, "Should be Codec8 Extended");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		Assert.That(frame.crc16, Is.EqualTo(new byte[] { 0x00, 0x00, 0x29, 0x94 }));

		Assert.That(avlDatas[0].timestampBytes, Is.EqualTo(new byte[] { 0x00, 0x00, 0x01, 0x6B, 0x41, 0x2C, 0xEE, 0x00 }));
		Assert.AreEqual(new DateTimeOffset (year: 2019, month: 6, day: 10, hour: 11, minute: 36, second: 32, TimeSpan.Zero), avlDatas[0].GetTimestamp());

		Assert.AreEqual(1, avlDatas[0].priority);

		// GPS element data
		Assert.That(gpsElement.longitudeBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
		Assert.That(gpsElement.latitudeBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
		Assert.That(gpsElement.altitudeBytes, Is.EqualTo(new byte[] { 0, 0 }));
		Assert.That(gpsElement.angleBytes, Is.EqualTo(new byte[] { 0, 0 }));

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		Assert.That(gpsElement.speedBytes, Is.EqualTo(new byte[] { 0, 0 }));

		// IO element data
		Assert.That(ioElement.eventIoId, Is.EqualTo(new byte[] { 0, 1 }));
		Assert.That(ioElement.totalCount, Is.EqualTo(new byte[] { 0, 5 }));

		Assert.That(ioElement.oneByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 1 }));
		Assert.AreEqual(1, ioElement.oneByteIdValuePairs.Count);

		Assert.That(ioElement.oneByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 1 }));
		Assert.AreEqual(0x01, ioElement.oneByteIdValuePairs[0].Value);

		Assert.That(ioElement.twoByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 1 }));
		Assert.AreEqual(1, ioElement.twoByteIdValuePairs.Count);

		Assert.That(ioElement.twoByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 0x11 }));
		Assert.That(ioElement.twoByteIdValuePairs[0].Value, Is.EqualTo(new byte[] { 0x00, 0x1D }));

		Assert.That(ioElement.fourByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 1 }));
		Assert.AreEqual(1, ioElement.fourByteIdValuePairs.Count);

		Assert.That(ioElement.fourByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 0x10 }));
		Assert.That(ioElement.fourByteIdValuePairs[0].Value, Is.EqualTo(new byte[] { 0x01, 0x5E, 0x2C, 0x88 }));

		Assert.That(ioElement.eightByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 2 }));
		Assert.AreEqual(2, ioElement.eightByteIdValuePairs.Count);

		Assert.That(ioElement.eightByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 0x0B }));
		Assert.That(ioElement.eightByteIdValuePairs[0].Value, Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x35, 0x44, 0xC8, 0x7A }));

        Assert.That(ioElement.eightByteIdValuePairs[1].Id, Is.EqualTo(new byte[] { 0, 0x0E }));
		Assert.That(ioElement.eightByteIdValuePairs[1].Value, Is.EqualTo(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x1D, 0xD7, 0xE0, 0x6A }));
	}

	[Test, Description("Random input from the internet")]
	public void DecodeRandomExampleCodec8ExtendedTest()
	{
		// Arrange
		string input = "00000000000000718e0100000167efa919800200000000000000000000000000000000fc0013000800ef0000f00000150500c80000450200010000710000fc00000900b5000000b600000042305600cd432a00ce6064001100090012ff22001303d1000f0000000200f1000059d900100000000000000000010000E1B8";

		// Act
		(GenericDecodeResult result, object valueOrError) = Codec8ExtendedDecoder.ParseHexadecimalString(input);

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8Extended, result, $"Expected success, but got: {valueOrError}");

		Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
		IReadOnlyList<AvlDataCodec8Extended> avlDatas = frame.GetAvlDatas();
		GPSElement gpsElement = avlDatas[0].GetGPSElement();
		IOElementCodec8Extended ioElement = avlDatas[0].GetIOElement();

		Assert.That(frame.preambleBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
		Assert.That(frame.dataFieldLengthBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0x71 }));
		Assert.AreEqual(113, frame.GetDataFieldLength());

		Assert.AreEqual(110, avlDatas[0].sizeInBytes, "Only AvlDataCodec8Extended should be 3 bytes less than data field length");

		Assert.AreEqual(0x8E, frame.codecId, "Should be Codec8 Extended");

		Assert.AreEqual(1, frame.numberOfData1);
		Assert.AreEqual(1, frame.numberOfData2);

		Assert.That(frame.crc16, Is.EqualTo(new byte[] { 0x00, 0x00, 0xE1, 0xB8 }));

		Assert.AreEqual(DateTimeOffset.FromUnixTimeMilliseconds(1545914096000), avlDatas[0].GetTimestamp());

		Assert.AreEqual(2, avlDatas[0].priority);

		// GPS element data
		Assert.That(gpsElement.longitudeBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
		Assert.That(gpsElement.latitudeBytes, Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
		Assert.That(gpsElement.altitudeBytes, Is.EqualTo(new byte[] { 0, 0 }));
		Assert.That(gpsElement.angleBytes, Is.EqualTo(new byte[] { 0, 0 }));

		Assert.AreEqual(0, gpsElement.visibleSatellites);

		Assert.That(gpsElement.speedBytes, Is.EqualTo(new byte[] { 0, 0 }));

		// IO element data
		Assert.That(ioElement.eventIoId, Is.EqualTo(new byte[] { 0, 252 }));
		Assert.That(ioElement.totalCount, Is.EqualTo(new byte[] { 0, 19 }));

		Assert.That(ioElement.oneByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 8 }));
		Assert.AreEqual(8, ioElement.oneByteIdValuePairs.Count);

		Assert.That(ioElement.oneByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 239 }));
		Assert.AreEqual(0, ioElement.oneByteIdValuePairs[0].Value);

		Assert.That(ioElement.twoByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 9 }));
		Assert.AreEqual(9, ioElement.twoByteIdValuePairs.Count);

		Assert.That(ioElement.twoByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 181 }));
		Assert.That(ioElement.twoByteIdValuePairs[0].Value, Is.EqualTo(new byte[] { 0x00, 0x00 }));

		Assert.That(ioElement.fourByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 2 }));
		Assert.AreEqual(2, ioElement.fourByteIdValuePairs.Count);

		Assert.That(ioElement.fourByteIdValuePairs[0].Id, Is.EqualTo(new byte[] { 0, 241 }));
		Assert.That(ioElement.fourByteIdValuePairs[0].Value, Is.EqualTo(new byte[] { 0x00, 0x00, 89, 217 }));

		Assert.That(ioElement.eightByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 0 }));
		Assert.AreEqual(0, ioElement.eightByteIdValuePairs.Count);

		Assert.That(ioElement.xByteValuesCountBytes, Is.EqualTo(new byte[] { 0, 0 }));
		Assert.AreEqual(0, ioElement.xByteIdValuePairs.Count);
	}



	[Test, Description("Invalid inputs")]
	public void DecodeInvalidCodec8ExtendedTest()
	{
		// Arrange
		List<(GenericDecodeResult expectedResult, string input)> invalids = new List<(GenericDecodeResult result, string input)>()
		{
			(GenericDecodeResult.InputNullOrEmpty, ""),
			(GenericDecodeResult.InputNotEnoughBytes, "000000000000002108E10000016B40D8EA3001000000000000000000000000000000010000000000010000C7"),
			(GenericDecodeResult.ContainsNonHexValues, "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A0000010000299G"),
			(GenericDecodeResult.OddNumberOfHexValues, "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A000001000029941"),
			(GenericDecodeResult.WrongPreamble, "000100000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994"),
			(GenericDecodeResult.DataFieldLengthTooBig, "000000000000004B8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994"),
			(GenericDecodeResult.IncorrectCodecId, "000000000000004A8F010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994"),
			(GenericDecodeResult.IncorrectPriority, "000000000000004A8E010000016B412CEE000300000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994"),
			(GenericDecodeResult.NumberOfDataMismatch, "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000200002994"),
			(GenericDecodeResult.DataFieldLengthAndNumberOfDataMismatch, "000000000000004A8E020000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000200002994"),
			(GenericDecodeResult.CrcMismatch, "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002995"),
		};

		// Act

		// Assert
		foreach (var pair in invalids)
		{
			(GenericDecodeResult result, _) = Codec8ExtendedDecoder.ParseHexadecimalString(pair.input);
			Assert.AreEqual(pair.expectedResult, result);
		}
	}
}