using System;
using System.Buffers.Binary;

namespace Codec8
{
	/// <summary>
	/// Turn bytes to numbers (static class)
	/// </summary>
	public static class BytesToNumbers
	{
		/// <summary>
		/// Get Int16 from two bytes
		/// </summary>
		/// <param name="bytes">Bytes</param>
		/// <returns>Int16</returns>
		public static short GetInt16(ReadOnlySpan<byte> bytes)
		{
			short returnValue = BitConverter.ToInt16(bytes);

			// Change endianness if needed
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Get UInt16 from two bytes
		/// </summary>
		/// <param name="bytes">Bytes</param>
		/// <returns>UInt16</returns>
		public static ushort GetUInt16(ReadOnlySpan<byte> bytes)
		{
			ushort returnValue = BitConverter.ToUInt16(bytes);

			// Change endianness if needed
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Get Int32 from four bytes
		/// </summary>
		/// <param name="bytes">Bytes</param>
		/// <returns>Int32</returns>
		public static int GetInt32(ReadOnlySpan<byte> bytes)
		{
			int returnValue = BitConverter.ToInt32(bytes);

			// Change endianness if needed
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Get UInt32 from four bytes
		/// </summary>
		/// <param name="bytes">Bytes</param>
		/// <returns>UInt32</returns>
		public static uint GetUInt32(ReadOnlySpan<byte> bytes)
		{
			uint returnValue = BitConverter.ToUInt32(bytes);

			// Change endianness if needed
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Get Int64 from eight bytes
		/// </summary>
		/// <param name="bytes">Bytes</param>
		/// <returns>Int64</returns>
		public static long GetInt64(ReadOnlySpan<byte> bytes)
		{
			long returnValue = BitConverter.ToInt64(bytes);

			// Change endianness if needed
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Get UInt64 from eight bytes
		/// </summary>
		/// <param name="bytes">Bytes</param>
		/// <returns>UInt64</returns>
		public static ulong GetUInt64(ReadOnlySpan<byte> bytes)
		{
			ulong returnValue = BitConverter.ToUInt64(bytes);

			// Change endianness if needed
			if (BitConverter.IsLittleEndian)
			{
				returnValue = BinaryPrimitives.ReverseEndianness(returnValue);
			}

			return returnValue;
		}
	}
}