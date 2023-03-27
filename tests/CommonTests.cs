namespace tests;

#pragma warning disable NUnit2005
public class GenericDecoderTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test, Description("Check that Codec8 decoding works")]
	public void DecodeCodec8Test()
	{
		// Arrange
		string input = "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF";

		// Act
		(GenericDecodeResult result, object valueOrError) = GenericDecoder.ParseHexadecimalString(input);
		Codec8Frame frame = (Codec8Frame)valueOrError;

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8, result, $"Expected success, but got: {valueOrError}");
		Assert.AreEqual(0x08, frame.codecId, "Should be Codec8");
	}

	[Test, Description("Check that Codec8 Extended decoding works")]
	public void DecodeCodec8ExtendedTest()
	{
		// Arrange
		string input = "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994";

		// Act
		(GenericDecodeResult result, object valueOrError) = GenericDecoder.ParseHexadecimalString(input);
		Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;

		// Assert
		Assert.AreEqual(GenericDecodeResult.SuccessCodec8Extended, result, $"Expected success, but got: {valueOrError}");
		Assert.AreEqual(0x8E, frame.codecId, "Should be Codec8 Extended");
	}
}