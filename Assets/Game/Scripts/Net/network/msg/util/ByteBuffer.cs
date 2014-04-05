using System;
using System.IO;

namespace KXSGCodec
{
    /// <summary>
    /// implements same function according to mina bytebuffer
    /// </summary>
    public class ByteBuffer
    {
        private MemoryStream leftRecvMsgBuf;

        public ByteBuffer(int maxSize)
        {
            this.leftRecvMsgBuf = new MemoryStream(maxSize);
        }

        public void Put(byte[] data, int length)
        {
            leftRecvMsgBuf.Write(data, 0, length);
        }

        public void Put(byte[] data)
        {
            leftRecvMsgBuf.Write(data, 0, data.Length);
        }

        public void Get(byte[] data)
        {
            leftRecvMsgBuf.Read(data, 0, data.Length);
        }

        public void Flip()
        {
            this.leftRecvMsgBuf.Seek(0, SeekOrigin.Begin);
        }

        public void Compact()
        {
            if (leftRecvMsgBuf.Position == 0)
            {
                return;
            }

            long _remaining = this.Remaining();
            if (_remaining <= 0)
            {
                this.Clear();
                return;
            }

            byte[] _leftData = new byte[_remaining];
            this.Get(_leftData);
            this.Clear();
            this.Put(_leftData);
        }

        public void Clear()
        {
            leftRecvMsgBuf.Seek(0, SeekOrigin.Begin);
            leftRecvMsgBuf.SetLength(0);
        }

        public long Remaining()
        {
            return leftRecvMsgBuf.Length - leftRecvMsgBuf.Position;
        }

        public Boolean HasRemaining()
        {
            return leftRecvMsgBuf.Length > leftRecvMsgBuf.Position;
        }

        public long Position()
        {
            return leftRecvMsgBuf.Position;
        }

        public long Length()
        {
            return leftRecvMsgBuf.Length;
        }

        public void SetPosition(long position)
        {
            leftRecvMsgBuf.Seek(position, SeekOrigin.Begin);
        }

    }
}
