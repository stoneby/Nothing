using System;
using System.IO;
using Thrift.Protocol;
using Thrift.Transport;

namespace KXSGCodec
{
    /// <summary>
    /// thrift message serialization util, FIXME: serializer depended stream has a problem that it's expand larger and larger（while limited to 32k）
    /// </summary>
    public class ThriftMsgSerialize
    {
        /** serializer depended stream */
        private static Stream OUTPUT_STREAM = new MemoryStream(64);

        /** deserializer depended stream */
        private static Stream INPUT_STREAM = new MemoryStream(64);

        /** serializer protocol */
        private static TProtocol SERIALIZE_PROTOCOL;

        /** deserializer protocol */
        private static TProtocol DESERIALIZE_PROTOCOL;

        private static ThriftMsgSerialize serialize = new ThriftMsgSerialize();

        private ThriftMsgSerialize()
        {
            TStreamTransport serializeTransport = new TStreamTransport(null, OUTPUT_STREAM);
            SERIALIZE_PROTOCOL = new TCompactProtocol(serializeTransport);

            TStreamTransport deserializeTransport = new TStreamTransport(INPUT_STREAM, null);
            DESERIALIZE_PROTOCOL = new TCompactProtocol(deserializeTransport);
        }

        /// <summary>
        /// serialize thrift msg object to byte array
        /// </summary>
        /// <param name="tbase"></param>
        /// <returns></returns>
        public static byte[] Serialize(TBase tbase)
        {
            if (tbase == null)
            {
                return null;
            }
            OUTPUT_STREAM.Seek(0, SeekOrigin.Begin);
            OUTPUT_STREAM.SetLength(0);

            tbase.Write(SERIALIZE_PROTOCOL);
			byte[] bytes = new byte[OUTPUT_STREAM.Length];
            OUTPUT_STREAM.Position = 0;
            OUTPUT_STREAM.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// deSerialize msg data to thrift msg object
        /// </summary>
        /// <param name="tbase"></param>
        /// <param name="bytes"></param>
        public static void DeSerialize(TBase tbase, byte[] bytes)
        {
            if (tbase == null || bytes == null)
            {
                return;
            }

            INPUT_STREAM.Seek(0, SeekOrigin.Begin);
            INPUT_STREAM.SetLength(0);

            INPUT_STREAM.Write(bytes, 0, bytes.Length);
            INPUT_STREAM.Seek(0, SeekOrigin.Begin);

            tbase.Read(DESERIALIZE_PROTOCOL);
        }

        /// <summary>
        /// serialize thrift msg to string
        /// </summary>
        /// <param name="tbase"></param>
        /// <returns></returns>
        public static string SerializeToBase64String(TBase tbase)
        {
            string szMsg = null;
            try
            {
                byte[] _bytes = Serialize(tbase);
                szMsg = Convert.ToBase64String(_bytes);
            }
            catch (Exception e)
            {
                Logger.LogError("SerializeToBase64String is exception, error : " + e.ToString());
            }
            return szMsg;
        }

        /// <summary>
        /// deserializ string msg data to thrift msg object
        /// </summary>
        /// <param name="tbase"></param>
        /// <param name="szMsg"></param>
        public static void DeSerializeFromBase64String(TBase tbase, string szMsg)
        {
            try
            {
                byte[] _bytes = Convert.FromBase64String(szMsg);
                DeSerialize(tbase, _bytes);
            }
            catch (Exception e)
            {
                Logger.LogError("DeSerializeFromBase64String is exception, error : " + e.ToString());
            }
        }
    }
}
