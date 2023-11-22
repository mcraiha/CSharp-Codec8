namespace tests;

#pragma warning disable NUnit2002
#pragma warning disable NUnit2003
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

	[Test, Description("Check that hex detection works correctly")]
	public void CheckHexTest()
	{
		// Arrange
		string valid1 = "abc0463";
		string valid2 = "1234567890";
		string valid3 = "abcdeF";

		string invalid1 = "abcdeFG";
		string invalid2 = "xabc0463";
		string invalid3 = "abc0463-";

		// Act
		bool shouldBeTrue1 = HexTools.CheckIfHexOnly(valid1);
		bool shouldBeTrue2 = HexTools.CheckIfHexOnly(valid2);
		bool shouldBeTrue3 = HexTools.CheckIfHexOnly(valid3);

		bool shouldBeFalse1 = HexTools.CheckIfHexOnly(invalid1);
		bool shouldBeFalse2 = HexTools.CheckIfHexOnly(invalid2);
		bool shouldBeFalse3 = HexTools.CheckIfHexOnly(invalid3);

		// ASsert
		Assert.IsTrue(shouldBeTrue1);
		Assert.IsTrue(shouldBeTrue2);
		Assert.IsTrue(shouldBeTrue3);

		Assert.IsFalse(shouldBeFalse1);
		Assert.IsFalse(shouldBeFalse2);
		Assert.IsFalse(shouldBeFalse3);
	}

	[Test, Description("Check that find first non hex works correctly")]
	public void FindFirstNonHexTest()
	{
		// Arrange
		string valid1 = "abc0463";

		string invalid1 = "abcdeFG";
		string invalid2 = "xabc0463";
		string invalid3 = "abc0463-";

		// Act
		int minusOne = HexTools.FindFirstNonHexPos(valid1);

		int index1 = HexTools.FindFirstNonHexPos(invalid1);
		int index2 = HexTools.FindFirstNonHexPos(invalid2);
		int index3 = HexTools.FindFirstNonHexPos(invalid3);

		// ASsert
		Assert.AreEqual(-1, minusOne);
		
		Assert.AreEqual(6, index1);
		Assert.AreEqual(0, index2);
		Assert.AreEqual(7, index3);
	}
}