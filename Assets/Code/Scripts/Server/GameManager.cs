using Assets.Code.Scripts.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Networking.PlayerConnection;

namespace Assets.Code.Scripts.Server
{
    internal class GameManager
    {
        #region Constants

        public const int MIN_PLAYERS = 2;

        #endregion

        #region Singleton

        private static readonly Lazy<GameManager> lazy = new Lazy<GameManager>(() => new GameManager());

        #endregion

        #region Fields

        private List<Card> cardDeck;
        private PlayerManager playerManager;
        private Player currentPlayer;

        #endregion

        #region Constructor

        private GameManager()
        {
            cardDeck = new List<Card>();
            playerManager = PlayerManager.Instance;

            playerManager.PlayerConnected += HandlePlayerConnected;
        }

        private void HandlePlayerConnected(Player player)
        {
            Console.WriteLine($"{player.Name} has joined!");

            if (PlayerManager.Instance.Players.Count >= MIN_PLAYERS)
            {
                StartNew(PlayerManager.Instance.Players);
            }
        }

        #endregion

        #region Properties

        public static GameManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        #endregion

        #region Methods

        public void StartNew(List<Player> players)
        {
            players.Clear();
            players.AddRange(players);

            SetDeck();
            currentPlayer = players.First();
        }

        private void SetDeck()
        {
            try
            {
                (SpecialCard[] specialCards, CountryCard[] countryCards, DestinationCard[] destinationCards) = CardCreator.Create();

                cardDeck.Clear();
                cardDeck.AddRange(specialCards);
                cardDeck.AddRange(countryCards);
                cardDeck.AddRange(destinationCards);

                cardDeck.Shuffle();

            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}
