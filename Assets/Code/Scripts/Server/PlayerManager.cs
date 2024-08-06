using Assets.Code.Scripts.Common.Commands;
using Assets.Code.Scripts.Common.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;

namespace Assets.Code.Scripts.Server
{
    internal class PlayerManager
    {
        #region Singleton

        private static readonly Lazy<PlayerManager> lazy = new Lazy<PlayerManager>(() => new PlayerManager());

        #endregion

        #region Fields

        private AsyncTCPServer server;
        private readonly ConcurrentDictionary<Player, AsyncTCPClient> players;
        private Dictionary<CommandType, Func<Command, AsyncTCPClient, Task>> commandCallbacks;

        #endregion

        #region Constructor

        private PlayerManager()
        {
            server = new AsyncTCPServer();
            players = new ConcurrentDictionary<Player, AsyncTCPClient>();
            commandCallbacks = new Dictionary<CommandType, Func<Command, AsyncTCPClient, Task>>();

            commandCallbacks.Add(CommandType.NameResp, HandleNameResp);
            commandCallbacks.Add(CommandType.Disconnect, HandleDisconnect);

            server.Start();
            server.ClientConnected += HandleClientConnected;
        }

        #endregion

        #region Events

        public event Action<Player> PlayerConnected;

        #endregion

        #region Properties

        public static PlayerManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public List<Player> Players
        {
            get { return players.Keys.ToList(); }
        }

        #endregion

        #region Methods

        private void HandlePlayerAction(Player player)
        {

        }

        private async void HandleClientConnected(AsyncTCPClient client)
        {
            client.DataReceived += HandleDataReceived;
            client.ErrorOccurred += HandleClientError;

            await client.SendAsync(new Command(CommandType.Name, null));
        }

        private async void HandleClientError(string exception, AsyncTCPClient client)
        {
            Player player = players.FirstOrDefault(item => item.Value == client).Key;
            client.DataReceived -= HandleDataReceived;
            client.ErrorOccurred -= HandleClientError;
            client.Disconnect();

            await BroadcastExcept(new Command(CommandType.InformPlayerLeft, player.ToString()), client);
        }

        private async void HandleDataReceived(string message, AsyncTCPClient client)
        {
            Command command = Command.FromString(message);
            await commandCallbacks[command.CommandType]?.Invoke(command, client);
        }

        #region Command Handlers

        public async Task HandleNameResp(Command command, AsyncTCPClient client)
        {
            string name = command.Data.ToString();
            Player player = new Player(name);
            players[player] = client;

            await BroadcastExcept(new Command(CommandType.InformPlayerJoined, player.ToString()), client);
            PlayerConnected?.Invoke(player);
        }

        public async Task HandleDisconnect(Command command, AsyncTCPClient client)
        {
            client.DataReceived -= HandleDataReceived;
            client.ErrorOccurred -= HandleClientError;
            client.Disconnect();

            Player player = players.FirstOrDefault(item => item.Value == client).Key;
            players[player] = client;

            await BroadcastExcept(new Command(CommandType.InformPlayerLeft, player.ToString()), client);
            PlayerConnected?.Invoke(player);
        }

        public async Task BroadcastExcept(Command command, params AsyncTCPClient[] clients)
        {
            foreach (AsyncTCPClient client in players.Values)
            {
                if (!clients.Contains(client))
                {
                    await client.SendAsync(command);
                }
            }
        }

        #endregion

        #endregion
    }
}
