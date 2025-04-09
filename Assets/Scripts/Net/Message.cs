using System;
using System.Linq;
using Google.Protobuf;
using SocketProtocol;
using UnityEngine;

// using SocketProtocol;

namespace ChaosBall.Net
{
    public class Message
    {
        /// <summary>
        /// 消息头长度
        /// </summary>
        public const int MESSAGE_HEADER_LEN = 4;

        private byte[] _buffer;
        private int _messageLen;

        public Message() {
            _buffer = new byte[1024];
            _messageLen = 0;
        }

        public byte[] Buffer => _buffer;

        public int MessageLen => _messageLen;

        public int RemainSize => _buffer.Length - _messageLen;
        
        /// <summary>
        /// 解析从对端发送来的消息
        /// </summary>
        /// <param name="len">消息长度</param>
        public void ReadBuffer(int len, Action<MainPack> onMainPackDeserialize) {
            _messageLen += len;
            // 消息长度 <= 4，说明这个消息只有消息头
            if (len <= MESSAGE_HEADER_LEN) {
                return;
            }
            // 将字节数组中前4个字节（从startIndex开始,第二个参数）转换为 int，刚好是消息头大小，存储的是消息体长度
            int bodyLen = BitConverter.ToInt32(_buffer, 0);
            while (true) {
                if (_messageLen >= bodyLen + MESSAGE_HEADER_LEN) {

                    try
                    {
                        MainPack pack = MainPack.Parser.ParseFrom(_buffer, MESSAGE_HEADER_LEN, bodyLen);
                        onMainPackDeserialize?.Invoke(pack);
                        // System.Buffer.BlockCopy(_mBuffer, bodyLen + MESSAGE_HEADER_LEN,
                        //     _mBuffer, 0, _mMessageLen - (bodyLen + MESSAGE_HEADER_LEN));
                        // _mMessageLen -= (bodyLen + MESSAGE_HEADER_LEN);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Message Parse Exception: {ex}");
                    }
                    finally
                    {
                        System.Buffer.BlockCopy(_buffer, bodyLen + MESSAGE_HEADER_LEN,
                            _buffer, 0, _messageLen - (bodyLen + MESSAGE_HEADER_LEN));
                        _messageLen -= (bodyLen + MESSAGE_HEADER_LEN);
                    }
                }
                else
                {
                    _messageLen = 0;
                    Array.Fill<byte>(_buffer, 0);
                    break;
                }
            }
        }

        public static byte[] GetPackData(MainPack pack) {
            byte[] body = pack.ToByteArray();
            byte[] head = BitConverter.GetBytes(body.Length);
            return head.Concat(body).ToArray();
        }
    }
}