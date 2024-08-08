using Assets.Code.Scripts.Common.Commands;
using Assets.Code.Scripts.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Assets.Code.Scripts.Server
{
    internal class GameManager
    {
        #region Constants

        public const int MIN_PLAYERS = 2;
        public const int MAX_PLAYERS = 4;
        public const int WAIT_INTERVAL_BEFORE_PLAY = 10000;
        public const int INITIAL_HAND_AMOUNT = 8;

        #endregion

        #region Singleton

        private static readonly Lazy<GameManager> lazy = new Lazy<GameManager>(() => new GameManager());

        #endregion

        #region Fields

        private Timer timer;
        private bool isGamePending;
        private Stack<Card> cardDeck;
        private Stack<Card> cardExposedPile;
        private PlayerManager playerManager;
        private Player currentPlayer;
 

        #endregion

        #region Constructors

        private GameManager()
        {
            timer = new Timer(HandleTimerElapsed);
            cardDeck = new Stack<Card>();
            cardExposedPile = new Stack<Card>();
            playerManager = PlayerManager.Instance;

            playerManager.PlayerConnected += HandlePlayerConnected;
            playerManager.TurnPlayed += HandleTurnPlayed;
            playerManager.PlayerDisconnected += HandlePlayerDisconnected;
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

        private void Reset()
        {
            cardDeck.Clear();
            cardExposedPile.Clear();
        }

        private async void HandleTurnPlayed(Player player, List<Card> cardsPlayed, List<Card> hand)
        {
            Player winner = null;

            if (hand.Count == 0)
            {
                // Tells the player that they won.
                await PlayerManager.Instance.InformTurnResult(player, null, cardDeck.Pop());

                winner = player;
            }
            else
            {
                if (cardsPlayed == null)
                {
                    // In case there are no cards left in the deck
                    if (cardDeck.Count == 0)
                    {
                        cardDeck.Spill(cardExposedPile);
                    }

                    // Tells the player that they should take a card from the deck. Exposed card remains the same
                    await PlayerManager.Instance.InformTurnResult(player, cardDeck.Pop(), null);
                }
                else
                {
                    foreach (Card card in cardsPlayed)
                    {
                        cardExposedPile.Push(card);
                    }

                    // Normal turn. No card is taken, exposed card changes
                    await PlayerManager.Instance.InformTurnResult(player, null, cardDeck.Pop());
                }

                int currentPlayerIndex = PlayerManager.Instance.Players.IndexOf(currentPlayer);

                // Proceeds to the next turn
                currentPlayer = currentPlayerIndex == PlayerManager.Instance.Players.Count - 1 ?
                    PlayerManager.Instance.Players[0] :
                    PlayerManager.Instance.Players[currentPlayerIndex + 1];

            }

            await PlayerManager.Instance.InformStatus(winner, cardExposedPile.Peek(), player, currentPlayer);
        }

        private void HandlePlayerConnected(Player player)
        {
            Console.WriteLine($"{player.Name} has joined!");

            if (PlayerManager.Instance.Players.Count == MAX_PLAYERS)
            {
                if (isGamePending)
                {
                    CancelTimer();
                }

                StartNew();
            }
            else if (PlayerManager.Instance.Players.Count >= MIN_PLAYERS) 
            { 
                if (!isGamePending)
                {
                    timer.Change(WAIT_INTERVAL_BEFORE_PLAY, 1);
                    isGamePending = true;
                }
            }

            isGamePending = false;
        }

        private void HandleTimerElapsed(object state)
        {
            if (PlayerManager.Instance.Players.Count >= MIN_PLAYERS)
            {
                StartNew();
            }
        }

        private void HandlePlayerDisconnected(Player player)
        {
            if (PlayerManager.Instance.Players.Count < MIN_PLAYERS)
            {
                if (isGamePending)
                {
                    CancelTimer();
                }
            }
        }

        private void CancelTimer()
        {
            isGamePending = false;
            timer.Dispose();

            timer = new Timer(HandleTimerElapsed);
        }

        public void StartNew()
        {
            SetDeck();

            PlayerManager.Instance.Players.Shuffle();

            for (int playerIndex = 0; playerIndex < PlayerManager.Instance.Players.Count; playerIndex++)
            {
                PlayerManager.Instance.Players[playerIndex].Hand.AddRange(cardDeck.ToList().GetRange(INITIAL_HAND_AMOUNT * playerIndex, INITIAL_HAND_AMOUNT));
            }

            currentPlayer = PlayerManager.Instance.Players.First();
            PlayerManager.Instance.SetPlayers(cardDeck.Peek(), currentPlayer);
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
