using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using UnityEditor.Experimental.GraphView;
using Assets.Code.Scripts.Common.Extensions;
using System.Diagnostics;

namespace Assets.Code.Scripts.Common.Sockets
{
    internal class AsyncTCPServer
    {
        #region Constants

        public const int PORT = 8765;

        #endregion

        #region Fields

        private TcpListener listener;

        #endregion

        #region Constructors

        public AsyncTCPServer()
        {
            try 
            {
                listener = new TcpListener(IPAddress.Any, PORT);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        #endregion

        #region Events

        public event Action<AsyncTCPClient> ClientConnected;

        #endregion

        #region Methods

        public async void Start()
        {
            Debug.WriteLine("Server is running");
            listener.Start();

            while (true) 
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                AsyncTCPClient asyncTCPClient = new AsyncTCPClient(client);

                _ = asyncTCPClient.StartReceivingAsync();
                ClientConnected?.Invoke(asyncTCPClient);
            }
        }

        #endregion
    }
}
