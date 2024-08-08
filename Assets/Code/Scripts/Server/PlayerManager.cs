using Assets.Code.Scripts.Common.Commands;
using Assets.Code.Scripts.Common.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine.XR;
using Assets.Code.Scripts.Client.Popups;

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
            commandCallbacks.Add(CommandType.PlayDone, HandlePlayDone);

            server.Start();
            server.ClientConnected += HandleClientConnected;
        }

        #endregion

        #region Events

        public event Action<Player> PlayerConnected;
        public event Action<Player> PlayerDisconnected;
        public event Action<Player, List<Card>, List<Card>> TurnPlayed;

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

        /// <summary>
        /// Informs the players about their hand, the exposed card and if they're first
        /// </summary>
        /// <param name="exposedCard"> Exposed card</param>
        /// <param name="isFirst"> Are them first</param>
        public async void SetPlayers(Card exposedCard, Player firstTurn)
        {
            var data = new
            {
                ExposedCard = exposedCard,
                Players,
                FirstPlayerID = firstTurn.ID
            };

            string dataString = JsonConvert.SerializeObject(data);
            await BroadcastExcept(new Command(CommandType.InformClientGameInit, dataString));
        }

        public async Task InformStatus(Player winner, Card exposedCard, Player currentPlayer, Player nextPlayer)
        {
            var data = new
            {
                Winner = winner,
                ExposedCard = exposedCard,
                Players,
                CurrentTurnPlayerID = nextPlayer.ID,
                SpecialInfo = new object()
            };

            string dataString = JsonConvert.SerializeObject(data);
            await BroadcastExcept(new Command(CommandType.InformStatus, dataString), players[currentPlayer]);
        } 

        public async Task InformTurnResult(Player player, Card retrieveCard, Card exposedCard)
        {
            var data = new
            {
                RetrieveCard = retrieveCard,
                ExposedCard = exposedCard
            };

            string dataString = JsonConvert.SerializeObject(data);
            await players[player].SendAsync(new Command(CommandType.PlayDoneResp, dataString));
        }

        private async void HandleClientConnected(AsyncTCPClient client)
        {
            client.DataReceived += HandleDataReceived;
            client.ErrorOccurred += HandleClientError;

            MessageBox.Show("TCP SERVER ACCEPT - SEND NAME PROT");
            await client.SendAsync(new Command(CommandType.Name, Guid.NewGuid().ToString()));
        }

        private async void HandleClientError(string exception, AsyncTCPClient client)
        {
            Player player = players.FirstOrDefault(item => item.Value == client).Key;
            client.DataReceived -= HandleDataReceived;
            client.ErrorOccurred -= HandleClientError;
            client.Disconnect();

            Debug.WriteLine($"{player}:{exception}");
            await BroadcastExcept(new Command(CommandType.InformClientPlayerLeft, JsonConvert.SerializeObject(player)), client);
            
            // Tries to remove (not immediately) because if it crashes during name fetching, it is not yet considered a connected client
            players.TryRemove(player, out _);
            PlayerDisconnected?.Invoke(player);
        }

        private async void HandleDataReceived(string message, AsyncTCPClient client)
        {
            Command command = Command.FromString(message);
            await commandCallbacks[command.CommandType]?.Invoke(command, client);
        }

        #region Command Handlers

        private async Task HandlePlayDone(Command command, AsyncTCPClient client)
        {
            await Task.Run(() => {
            
                (List<Card> cardsPlayed, List<Card> hand) = JsonConvert.DeserializeObject<(List<Card> cardsPlayed, List<Card> hand)>(command.Data.ToString());
                Player player = players.FirstOrDefault(item => item.Value == client).Key;

                TurnPlayed?.Invoke(player, cardsPlayed, hand);
            });
        }

        public async Task HandleNameResp(Command command, AsyncTCPClient client)
        {
            MessageBox.Show("RESP received: " + command.Data.ToString());

            var template = new { Name = "", ID = "" };
            var result = JsonConvert.DeserializeAnonymousType(command.Data.ToString(), template);
            MessageBox.Show("RESP proccessing: " + result.Name + " " + result.ID);

            Player player = new Player(result.Name, new Guid(result.ID));
            players[player] = client;

            MessageBox.Show("RESP processd");

            await BroadcastExcept(new Command(CommandType.InformClientPlayerJoined, JsonConvert.SerializeObject(player)), client);

            Debug.WriteLine(result.Name);
            PlayerConnected?.Invoke(player);
        }

        public async Task HandleDisconnect(Command command, AsyncTCPClient client)
        {
            client.DataReceived -= HandleDataReceived;
            client.ErrorOccurred -= HandleClientError;
            client.Disconnect();

            Player player = players.FirstOrDefault(item => item.Value == client).Key;

            await BroadcastExcept(new Command(CommandType.InformClientPlayerLeft, player.ToString()), client);
            
            players.Remove(player, out _);
            PlayerDisconnected?.Invoke(player);
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
