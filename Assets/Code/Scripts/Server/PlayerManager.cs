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
        public event Action<Player, Card, List<Card>> TurnPlayed;

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
                currentTurnPlayerID = nextPlayer.ID,
                SpecialInfo = new object()
            };

            string dataString = JsonConvert.SerializeObject(data);
            await BroadcastExcept(new Command(CommandType.InformClientGameInit, dataString), players[currentPlayer]);
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

            await client.SendAsync(new Command(CommandType.Name, null));
        }

        private async void HandleClientError(string exception, AsyncTCPClient client)
        {
            Player player = players.FirstOrDefault(item => item.Value == client).Key;
            client.DataReceived -= HandleDataReceived;
            client.ErrorOccurred -= HandleClientError;
            client.Disconnect();

            Debug.WriteLine($"{player}:{exception}");
            await BroadcastExcept(new Command(CommandType.InformClientPlayerLeft, JsonConvert.SerializeObject(player)), client);
            
            players.Remove(player, out _);
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
            
                (Card cardPlayed, List<Card> hand) = JsonConvert.DeserializeObject<(Card exposedCard, List<Card> hand)>(command.Data.ToString());
                Player player = players.FirstOrDefault(item => item.Value == client).Key;

                TurnPlayed?.Invoke(player, cardPlayed, hand);
            });
        }

        public async Task HandleNameResp(Command command, AsyncTCPClient client)
        {
            (string name, Guid id) = JsonConvert.DeserializeObject<(string name, Guid id)>(command.Data.ToString());
            Player player = new Player(name, id);
            players[player] = client;

            await BroadcastExcept(new Command(CommandType.InformClientPlayerJoined, JsonConvert.SerializeObject(player)), client);

            Debug.WriteLine(name);
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
