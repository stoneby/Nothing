using System;
using System.IO;
using System.Text;

namespace KXSGCodec
{
    /// <summary>
    /// all message need extend this class
    /// </summary>
    public abstract class BaseMessage
    {
        public const int MsgTypeLen = 2;
        public const int MsgSizeLen = 2;
        public const int MinMsgLen = MsgTypeLen + MsgSizeLen;
        public const int MaxMsgLen = 1024 * 32;

        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /** reader */
        private BinaryReader reader;
        private MemoryStream readStream;

        /** writer */
        private BinaryWriter writer;
        private MemoryStream writeStream;
        private short msgLen;

        /// <summary>
        /// encode message to byte array
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {
            writeStream = new MemoryStream();
            writer = new BinaryWriter(writeStream, DefaultEncoding);

            // Header, length + MsgId
            WriteShort(0);
            WriteShort(GetMsgType());

            // Write message...
            WriteImpl();

            var bytes = writeStream.ToArray();
            // Rewrite message length
            msgLen = (short)bytes.Length;
            var lenBytes = ConvertLenToArry(msgLen);
            bytes[0] = lenBytes[0];
            bytes[1] = lenBytes[1];

            ResetWriter();

            return bytes;
        }

        /// <summary>
        /// decode byte array data to message object
        /// </summary>
        /// <param name="bytes"></param>
        public void Decode(byte[] bytes)
        {
            readStream = new MemoryStream();
            reader = new BinaryReader(readStream, DefaultEncoding);

            readStream.Write(bytes, 0, bytes.Length);
            readStream.Position = 0;

            msgLen = ReadShort();
            //skip type
            ReadShort();

            // Read message
            ReadImpl();

            ResetReader();

        }

        protected virtual int GetReadPos()
        {
            return (int)readStream.Position;
        }

        protected short GetMsgLength()
        {
            return msgLen;
        }

        protected virtual int GetWritePos()
        {
            return (int)writeStream.Position;
        }

        private void ResetWriter()
        {
            writer.Close();
        }

        private void ResetReader()
        {
            reader.Close();
        }

        protected abstract void WriteImpl();

        protected abstract void ReadImpl();

        public abstract short GetMsgType();

        protected void WriteByte(int data)
        {
            writer.Write((byte)data);
        }

        protected byte ReadByte()
        {
            return reader.ReadByte();
        }

        protected byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        protected void WriteBytes(byte[] bytes)
        {
            writer.Write(bytes);
        }

        protected void WriteInt(int data)
        {
            var bytes = BitConverter.GetBytes(data);
            Array.Reverse(bytes);
            writer.Write(bytes);
        }

        protected int ReadInt()
        {
            var arry = reader.ReadBytes(4);
            Array.Reverse(arry);
            return BitConverter.ToInt32(arry, 0);
        }

        protected void WriteString(String data)
        {
            if (string.IsNullOrEmpty(data))
            {
                WriteShort(0);
                return;
            }

            var bytes = DefaultEncoding.GetBytes(data);
            var len = bytes.Length;
            var lenBytes = ConvertLenToArry((short)len);
            writer.Write(lenBytes);
            writer.Write(bytes);
        }

        protected string ReadString()
        {
            var lenArry = reader.ReadBytes(2);
            var len = (lenArry[0] << 8) | lenArry[1];
            if (len <= 0)
            {
                return "";
            }
            var bytes = reader.ReadBytes(len);
            return DefaultEncoding.GetString(bytes);
        }

        protected void WriteDouble(double data)
        {
            var bytes = BitConverter.GetBytes(data);
            ReverseWrite(bytes);
        }

        protected double ReadDouble()
        {
            var arry = reader.ReadBytes(8);
            Array.Reverse(arry);
            return BitConverter.ToDouble(arry, 0);
        }

        protected void WriteBoolean(bool data)
        {
            var bytes = BitConverter.GetBytes(data);
            ReverseWrite(bytes);
        }

        protected bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        protected void WriteLong(Int64 data)
        {
            var bytes = BitConverter.GetBytes(data);
            ReverseWrite(bytes);
        }

        protected long ReadLong()
        {
            var arry = reader.ReadBytes(8);
            Array.Reverse(arry);
            return BitConverter.ToInt64(arry, 0);
        }

        protected void WriteShort(Int16 data)
        {
            var bytes = BitConverter.GetBytes(data);
            ReverseWrite(bytes);
        }

        protected short ReadShort()
        {
            var arry = reader.ReadBytes(2);
            Array.Reverse(arry);
            return BitConverter.ToInt16(arry, 0);
        }

        private void ReverseWrite(byte[] bytes)
        {
            Array.Reverse(bytes);
            writer.Write(bytes);
        }

        private byte[] ConvertLenToArry(short len)
        {
            var lenArry = new byte[2];
            lenArry[0] = (byte)(len >> 8);
            lenArry[1] = (byte)(len & 0x00FF);
            return lenArry;
        }
    }
}
