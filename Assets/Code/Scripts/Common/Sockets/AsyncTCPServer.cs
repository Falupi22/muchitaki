using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using UnityEditor.Experimental.GraphView;
using Assets.Code.Scripts.Common.Extensions;

namespace Assets.Code.Scripts.Common.Sockets
{
    internal class AsyncTCPServer
    {
        #region Constants

        public const int PORT = 8765;

        #endregion

        #region Fields

        private TcpListener listener;
        private List<AsyncTCPClient> clients;

        #endregion

        #region Constructors

        public AsyncTCPServer()
        {
            listener = new TcpListener(IPAddressHelper.GetLocal(), PORT);
            clients = new List<AsyncTCPClient>();
        }

        #endregion

        #region Events

        public event Action<AsyncTCPClient> ClientConnected;

        #endregion

        #region Methods

        public async void Start()
        {
            listener.Start();

            while (true) 
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                AsyncTCPClient asyncTCPClient = new AsyncTCPClient(client);
                clients.Add(asyncTCPClient);

                _ = asyncTCPClient.StartReceivingAsync();
                ClientConnected?.Invoke(asyncTCPClient);
            }
        }

        #endregion
    }
}
