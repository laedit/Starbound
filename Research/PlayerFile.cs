using System;
using System.IO;

namespace Research
{
    class PlayerFile
    {
        public static void Test()
        {
            // .player
            using (BinaryReader reader = new BinaryReader(File.OpenRead("b089205d214ca0f80b188e52f2859cf2.player")))
            {
                var signature = reader.ReadString(8); // SBPFV1.1
                var version = reader.ReadUInt32BE(); // 428
                var playerDataLength = reader.DecodeInt32();//1740 //14698 Yak
                var hasuuid = reader.ReadBoolean(); // true
                var uuid = reader.ReadStringFromBytes(16).ToLowerInvariant(); // B089205D214CA0F80B188E52F2859CF2
                var name = ReadString(reader); // Laedit
                var race = ReadString(reader); // glitch
                var gender = reader.ReadByte(); // 0 => male
                var hairGroup = ReadString(reader); // hair
                var hairType = ReadString(reader); // 27
                var hairDirectives = ReadString(reader);
                var bodyDirectives = ReadString(reader);
                var facialHairGroup = reader.ReadString();
                var facialHairType = reader.ReadString();
                var facialHairDirectives = reader.ReadString();
                var facialMaskGroup = reader.ReadString();
                var facialMaskType = reader.ReadString();
                var facialMaskDirectives = reader.ReadString();

            }
        }

        private static string ReadString(BinaryReader reader)
        {
            var resultLength = reader.DecodeInt32();
            var result = new string(reader.ReadChars(resultLength));
            return result;
        }
    }


    public static class Helpers
    {
        // thanks to Tim Williams: http://stackoverflow.com/a/15274591/424072
        // Note this MODIFIES THE GIVEN ARRAY then returns a reference to the modified array.
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).Reverse(), 0);
        }

        public static Int16 ReadInt16BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).Reverse(), 0);
        }

        public static UInt32 ReadUInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).Reverse(), 0);
        }

        public static Int32 ReadInt32BE(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).Reverse(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }
    }

    public static class BinaryReaderExtensions
    {
        public static string ReadString(this BinaryReader reader, int length)
        {
            return new string(reader.ReadChars(length));
        }

        public static string ReadStringFromBytes(this BinaryReader reader, int count)
        {
            return BitConverter.ToString(reader.ReadBytes(count)).Replace("-", string.Empty);
        }
    }

    public static class Extensions
    {
        // thanks to Lasse V. Karlsen: http://stackoverflow.com/a/3564685/424072

        /// <summary>
        /// Encodes the specified <see cref="Int32"/> value with a variable number of
        /// bytes, and writes the encoded bytes to the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="BinaryWriter"/> to write the encoded value to.
        /// </param>
        /// <param name="value">
        /// The <see cref="Int32"/> value to encode and write to the <paramref name="writer"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="writer"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="value"/> is less than 0.</para>
        /// </exception>
        /// <remarks>
        /// See <see cref="DecodeInt32"/> for how to decode the value back from
        /// a <see cref="BinaryReader"/>.
        /// </remarks>
        public static void EncodeInt32(this BinaryWriter writer, int value)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", value, "value must be 0 or greater");

            bool first = true;
            while (first || value > 0)
            {
                first = false;
                byte lower7bits = (byte)(value & 0x7f);
                value >>= 7;
                if (value > 0)
                    lower7bits |= 128;
                writer.Write(lower7bits);
            }
        }

        /// <summary>
        /// Decodes a <see cref="Int32"/> value from a variable number of
        /// bytes, originally encoded with <see cref="EncodeInt32"/> from the specified reader.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="BinaryReader"/> to read the encoded value from.
        /// </param>
        /// <returns>
        /// The decoded <see cref="Int32"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="reader"/> is <c>null</c>.</para>
        /// </exception>
        public static int DecodeInt32(this BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            bool more = true;
            int value = 0;
            int shift = 0;
            while (more)
            {
                byte lower7bits = reader.ReadByte();
                more = (lower7bits & 128) != 0;
                value |= (lower7bits & 0x7f) << shift;
                shift += 7;
            }
            return value;
        }
    }
}
