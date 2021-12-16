using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoCaro
{
    class SocketManager
    {
        #region Server
        Socket server;
        public void creatServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);
            Thread acceptClient = new Thread(() => 
            {
                client = server.Accept();
            });
            acceptClient.IsBackground = true;
            acceptClient.Start();
        }
        #endregion
        #region Client
        Socket client;
        public bool connect()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(iep);
                return true;
            }
            catch
            {
                return false; 
            }
        }
        #endregion
        #region Both
        public string IP = "127.0.0.1";
        public int port = 9999;
        public const int BUFFER = 1024;
        public bool isServer = true;

        public bool Send(object obj)
        {
            byte[] sendData = SerializeData(obj);
            return SendData(client, sendData);
        }
        public object Receive()
        {
            byte[] receiveData = new byte[BUFFER];
            bool isOK = ReceiveData(client, receiveData);
            return DeserializeData(receiveData);
        }
        private bool SendData(Socket socket, byte[] data)
        {
            return socket.Send(data) == 1 ? true : false;
        }
        private bool ReceiveData(Socket socket,byte[] data)
        {
            return socket.Receive(data) == 1 ? true : false;
        }
        private byte[] SerializeData(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
        private object DeserializeData(byte[] theByteArray)
        {
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf = new BinaryFormatter();
            ms.Position = 0;
            return bf.Deserialize(ms);
        }
        public string getLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses )
                    {
                        output = ip.Address.ToString();
                    }    
                }    

            }
            return output;
        }


        #endregion
    }
}
