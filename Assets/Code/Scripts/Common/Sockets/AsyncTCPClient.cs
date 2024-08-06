using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Assets.Code.Scripts.Common.Sockets
{
    internal class AsyncTCPClient
    {
        #region Fields

        private TcpClient client;

        #endregion

        #region Constructors

        public AsyncTCPClient(TcpClient client)
        {
            this.client = client;
        }

        #endregion

        #region Events

        public event Action<string, AsyncTCPClient> DataReceived;
        public event Action<string, AsyncTCPClient> ErrorOccurred;

        #endregion

        #region Methods

        /// <summary>
        /// Connects the client to an endpoint.
        /// </summary>
        /// <param name="hostIP"></param>
        /// <param name="port"></param>
        /// <remarks>
        /// Use in client-side only
        /// </remarks>
        public async Task ConnectAsync(IPAddress hostIP, int port)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(hostIP.ToString(), port);
            }
            catch (Exception exception)
            {
                ErrorOccurred?.Invoke(exception.ToString(), this);
            }
        }

        public void Disconnect()
        {
            try
            {
                client.Close();
            }
            catch (Exception exception)
            {
                ErrorOccurred?.Invoke(exception.ToString(), this);
            }
        }

        public async Task SendAsync(object message)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message.ToString());

                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(bytes);
                await stream.FlushAsync();
            }
            catch (Exception exception)
            {
                ErrorOccurred?.Invoke(exception.ToString(), this);
            }
        }

        public async Task StartReceivingAsync()
        {
            bool isError = false;
            bool hasConnectionEnded = false;

            while (!isError && !hasConnectionEnded)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    NetworkStream stream = client.GetStream();
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        await stream.FlushAsync();

                        DataReceived?.Invoke(data, this);
                    }
                    else { 
                        hasConnectionEnded = true;
                        Disconnected?.Invoke(this);
                    }
                }
                catch (Exception exception)
                {
                    isError = true;
                    ErrorOccurred?.Invoke(exception.ToString(), this);
                }
            }
        }

        #endregion
    }
}
