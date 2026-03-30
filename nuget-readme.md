## About

Decoder for **Codec 8** and **Codec 8 Extended** (TCP and UDP supported) formats that are used by certain **Teltonika** devices.

## How to use

To decode Codec8 TCP
```csharp
using Codec8;

string input = "000000000000003608010000016B40D8EA30010000000000000000000000000000000105021503010101425E0F01F10000601A014E0000000000000000010000C7CF";
(GenericDecodeResult result, object valueOrError) = Codec8Decoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8)
{
    Codec8Frame frame = (Codec8Frame)valueOrError;
}
```

To decode Codec8 Extended TCP
```csharp
using Codec8;

string input = "000000000000004A8E010000016B412CEE000100000000000000000000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994";
(GenericDecodeResult result, object valueOrError) = Codec8ExtendedDecoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8Extended)
{
    Codec8ExtendedFrame frame = (Codec8ExtendedFrame)valueOrError;
}
```

To decode Codec8 UDP
```csharp
using Codec8;

string input = "003DCAFE0105000F33353230393330383634303336353508010000016B4F815B30010000000000000000000000000000000103021503010101425DBC000001";
(GenericDecodeResult result, object valueOrError) = Codec8UdpDecoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8Udp)
{
    (UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8FrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8FrameNoCRC c))valueOrError;
}
```

To decode Codec8 Extended UDP
```csharp
using Codec8;

string input = "005FCAFE0107000F3335323039333038363430333635358E010000016B4F831C680100000000000000000000000000000000010005000100010100010011009D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A000001";
(GenericDecodeResult result, object valueOrError) = Codec8ExtendedUdpDecoder.ParseHexadecimalString(input);
if (result == GenericDecodeResult.SuccessCodec8ExtendedUdp)
{
    (UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8ExtendedFrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8ExtendedFrameNoCRC c))valueOrError;
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
else if (result == GenericDecodeResult.SuccessCodec8Udp)
{
    (UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8FrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8FrameNoCRC c))valueOrError;
}
else if (result == GenericDecodeResult.SuccessCodec8ExtendedUdp)
{
    (UdpChannelHeader header, AvlDataEncapsulated avlDataEncapsulated, Codec8ExtendedFrameNoCRC frame) = ((UdpChannelHeader a, AvlDataEncapsulated b, Codec8ExtendedFrameNoCRC c))valueOrError;
}
else
{
    Console.Writeline($"Expected success, but got: {valueOrError}");
}
```