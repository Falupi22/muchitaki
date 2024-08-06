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
        public const int INITIAL_HAND_AMOUNT = 8;

        #endregion

        #region Singleton

        private static readonly Lazy<GameManager> lazy = new Lazy<GameManager>(() => new GameManager());

        #endregion

        #region Fields

        private Stack<Card> cardDeck;
        private PlayerManager playerManager;
        private Player currentPlayer;

        #endregion

        #region Constructor

        private GameManager()
        {
            cardDeck = new Stack<Card>();
            playerManager = PlayerManager.Instance;

            playerManager.PlayerConnected += HandlePlayerConnected;
        }

        private void HandlePlayerConnected(Player player)
        {
            Console.WriteLine($"{player.Name} has joined!");

            if (PlayerManager.Instance.Players.Count >= MIN_PLAYERS)
            {
                StartNew();
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

        public void StartNew()
        {
            SetDeck();

            PlayerManager.Instance.Players.Shuffle();

            for (int playerIndex = 0; playerIndex < PlayerManager.Instance.Players.Count; playerIndex++)
            {
                PlayerManager.Instance.Players[playerIndex].Hand.AddRange(cardDeck.ToList().GetRange(INITIAL_HAND_AMOUNT * playerIndex, INITIAL_HAND_AMOUNT));
                PlayerManager.Instance.SetPlayers(cardDeck.Peek(), playerIndex == 0);
            }

            currentPlayer = PlayerManager.Instance.Players.First();
        }

        private void SetDeck()
        {
            try
            {
                (SpecialCard[] specialCards, CountryCard[] countryCards, DestinationCard[] destinationCards) = CardCreator.Create();

                cardDeck.Clear();
                List<Card> cards = new List<Card>();
                cards.AddRange(specialCards);
                cards.AddRange(countryCards);
                cards.AddRange(destinationCards);

                cards.Shuffle();

                foreach (Card card in cards)
                {
                    cardDeck.Push(card);
                }
            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}
