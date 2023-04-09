# CSharp-Codec8

Decoder for **Codec 8** and **Codec 8 Extended** (currently only TCP, no UDP support) formats that are used by certain **Teltonika** devices.

## Build status
[![.NET](https://github.com/mcraiha/CSharp-Codec8/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mcraiha/CSharp-Codec8/actions/workflows/dotnet.yml)

## Nuget
[LibCodec8](https://www.nuget.org/packages/LibCodec8)

## Specs

See [Codec 8](https://wiki.teltonika-gps.com/view/Codec#Codec_8) and [Codec 8 Extended](https://wiki.teltonika-gps.com/view/Codec#Codec_8_Extended) wiki

## How to use

To decode Codec8
```csharp
using Codec8;

string input = "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF";
(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8)
{
    Codec8Frame frame = (Codec8Frame)valueOrError;
}
```

To decode Codec8 Extended
```csharp
using Codec8;

string input = "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994";
(GenericDecodeResult result, object valueOrError) = Codec8ExtendedDecoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8Extended)
{
    Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
}
```

If you don't know what kind of data you have, you can use generic decoder
```csharp
using Codec8;

string input = "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994";
(GenericDecodeResult result, object valueOrError) = GenericDecoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8)
{
    Codec8Frame frame = (Codec8Frame)valueOrError;
}
else if (result == GenericDecodeResult.SuccessCodec8Extended)
{
    Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
}
else
{
    Console.Writeline($"Expected success, but got: {valueOrError}");
}
```

## License

This document and source code files are released into the public domain. See [LICENSE](LICENSE) file