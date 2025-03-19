using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GameFrameSync;
using Google.Protobuf;
using UnityEngine;

namespace ChaosBall.Net
{
    public class UdpListener
    {
        private UdpClient _mUdpClient;
        private int _mCurrentDataSequence = 0;
        public int UdpListenPort { get; set; }
        public IPEndPoint RemoteEp { get; private set; }

        public event Action<ResFrameSyncData> OnReceiveFrameSync;
        public UdpListener()
        {
            
        }

        public int StartListen()
        {
            try
            {
                _mUdpClient = new UdpClient(UdpListenPort, AddressFamily.InterNetwork);
            }
            catch (SocketException e)
            {
                Debug.LogError("UDP SocketError:" + e);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception:" + e);
            }
            Debug.Log($"UDP listen port: {UdpListenPort}");
            StartReceive();
            return UdpListenPort;
        }

        private void StartReceive()
        {
            try
            {
                _mUdpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (SocketException e)
            {
                Debug.LogError("UDP SocketError:" + e);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception:" + e);
            }
        }

        private void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _mUdpClient.EndReceive(iar, ref remoteEp);
                RemoteEp = remoteEp;
                // Debug.Log($"远端监听：{RemoteEp.Address}:{RemoteEp.Port}");
                ResFrameSyncData resFrameSyncData = Deserialize(data);
                if (resFrameSyncData.MessageType is MessageType.FrameSync)
                {
                    // Debug.Log($"收到服务端udp消息:{resFrameSyncData.FrameId}");
                    OnReceiveFrameSync?.Invoke(resFrameSyncData);
            
                    SendAck(resFrameSyncData.MessageHead.Index);
                }
                StartReceive();
            }
            catch (SocketException e)
            {
                Debug.LogError("UDP SocketError:" + e);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception:" + e);
            }
        }
        
        public void Send(ResFrameSyncData resFrameSyncData)
        {
            MessageHead messageHead = new MessageHead
            {
                Index = _mCurrentDataSequence,
            };
            resFrameSyncData.MessageHead = messageHead;
            resFrameSyncData.MessageType = MessageType.FrameSync;

            try
            {
                byte[] data = Serialize(resFrameSyncData);

                _mCurrentDataSequence++;
            
                _mUdpClient.Send(data, data.Length, RemoteEp);
            }
            catch (SocketException e)
            {
                Debug.LogError("UDP SocketError:" + e);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception:" + e);
            }
        }

        private void SendAck(int index)
        {
            MessageHead messageHead = new MessageHead
            {
                Index = index,
                Ack = true,
                ClientIp = GameInterface.Interface.TcpClient.ClientIp,
            };
            ResFrameSyncData resFrameSyncData = new ResFrameSyncData
            {
                MessageHead = messageHead,
                MessageType = MessageType.Ack,
            };
            try
            {
                byte[] data = Serialize(resFrameSyncData);
                _mUdpClient.Send(data, data.Length, RemoteEp);
            }
            catch (SocketException e)
            {
                Debug.LogError("UDP SocketError:" + e);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception:" + e);
            }
        }

        private byte[] Serialize(ResFrameSyncData resFrameSyncData)
        {
            byte[] data = resFrameSyncData.ToByteArray();
            return data;
        }

        private ResFrameSyncData Deserialize(byte[] data)
        {
            ResFrameSyncData resFrameSyncData = ResFrameSyncData.Parser.ParseFrom(data);
            return resFrameSyncData;
        }
    }
}