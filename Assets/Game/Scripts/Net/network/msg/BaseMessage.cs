using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KXSGCodec
{
    /// <summary>
    /// all message need extend this class
    /// </summary>
    public abstract class BaseMessage
    {
        public const int MSG_TYPE_LEN = 2;
        public const int MSG_SIZE_LEN = 2;
        public const int MIN_MSG_LEN = MSG_TYPE_LEN + MSG_SIZE_LEN;
		public const int MAX_MSG_LEN = 1024 * 32;
        
        private static Encoding DEFAULT_ENCODING = Encoding.UTF8;

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
            writer = new BinaryWriter(writeStream, DEFAULT_ENCODING);

            // Header, length + MsgId
            this.WriteShort(0);
            this.WriteShort(GetMsgType());

            // Write message...
            this.WriteImpl();

            byte[] bytes = writeStream.ToArray();
            // Rewrite message length
            this.msgLen = (short)bytes.Length;
            byte[] _lenBytes = this.ConvertLenToArry(this.msgLen);
            bytes[0] = _lenBytes[0];
            bytes[1] = _lenBytes[1];

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
            reader = new BinaryReader(readStream, DEFAULT_ENCODING);

            readStream.Write(bytes, 0, bytes.Length);
            readStream.Position = 0;

            this.msgLen = this.ReadShort();
            //skip type
            this.ReadShort();

            // Read message
            ReadImpl();

            ResetReader();
            
        }

        protected virtual int GetReadPos()
        {
            return (int)readStream.Position;
        }

        protected short getMsgLength()
        {
            return this.msgLen;
        }

        protected virtual int GetWritePos()
        {
            return (int)writeStream.Position;
        }

        private void ResetWriter()
        {
            this.writer.Close();
        }

        private void ResetReader()
        {
            this.reader.Close();
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
            byte[] _bytes = BitConverter.GetBytes(data);
            Array.Reverse(_bytes);
            writer.Write(_bytes);
        }

        protected int ReadInt()
        {
            byte[] _arry = reader.ReadBytes(4);
            Array.Reverse(_arry);
            return BitConverter.ToInt32(_arry, 0);
        }

        protected void WriteString(String data)
        {
            if (data == null || data.Length == 0)
            {
                this.WriteShort(0);
                return;
            }

            byte[] _bytes = DEFAULT_ENCODING.GetBytes(data);
            int _len = _bytes.Length;
            byte[] _lenBytes = this.ConvertLenToArry((short)_len);
            writer.Write(_lenBytes);
            writer.Write(_bytes);
        }

        protected string ReadString()
        {
            byte[] _lenArry = reader.ReadBytes(2);
            int _len = (_lenArry[0] << 8) | _lenArry[1];
            if (_len <= 0)
            {
                return "";
            }
            byte[] _bytes = reader.ReadBytes(_len);
            return DEFAULT_ENCODING.GetString(_bytes);
        }

        protected void WriteDouble(double data)
        {
            byte[] _bytes = BitConverter.GetBytes(data);
            ReverseWrite(_bytes);
        }

        protected double ReadDouble()
        {
            byte[] _arry = reader.ReadBytes(8);
            Array.Reverse(_arry);
            return BitConverter.ToDouble(_arry, 0);
        }

        protected void WriteBoolean(bool data)
        {
            byte[] _bytes = BitConverter.GetBytes(data);
            ReverseWrite(_bytes);
        }

        protected bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        protected void WriteLong(Int64 data)
        {
            byte[] _bytes = BitConverter.GetBytes(data);
            ReverseWrite(_bytes);
        }

        protected long ReadLong()
        {
            byte[] _arry = reader.ReadBytes(8);
            Array.Reverse(_arry);
            return BitConverter.ToInt64(_arry, 0);
        }

        protected void WriteShort(Int16 data)
        {
            byte[] _bytes = BitConverter.GetBytes(data);
            ReverseWrite(_bytes);
        }

        protected short ReadShort()
        {
            byte[] _arry = reader.ReadBytes(2);
            Array.Reverse(_arry);
            return BitConverter.ToInt16(_arry, 0);
        }

        private void ReverseWrite(byte[] bytes)
        {
            Array.Reverse(bytes);
            writer.Write(bytes);
        }

        private byte[] ConvertLenToArry(short len)
        {
            byte[] _lenArry = new byte[2];
            _lenArry[0] = (byte)(len >> 8);
            _lenArry[1] = (byte)(len & 0x00FF);
            return _lenArry;
        }
    }
}
