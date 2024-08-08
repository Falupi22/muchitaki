using Assets.Code.Scripts.Client.EventInfos;
using Assets.Code.Scripts.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityBasedTimer = Assets.Code.Scripts.Common.Utils.Timers.Timer;

namespace Assets.Code.Scripts.Client
{
    public class BoardView : MonoBehaviour
    {
        #region Constants

        public const int TIMER_INTERVAL_IN_MILLISECONDS = 1000;
        public const int TURN_TIME_IN_MILLISECONDS = 15000;

        #endregion

        #region Fields

        [SerializeField]
        private List<PlayerView> playerViews;

        [SerializeField]
        private PlayerView currentPlayerView;

        [SerializeField]
        private CardView currentCard;

        [SerializeField]
        private DeckView deckView;

        [SerializeField]
        private UnityBasedTimer timer;

        private int secondsCounter;
        private string time;
        private GameManager gameManager;
        private List<Card> selectableCards;
        private List<Card> selectedCards;
        private Player currentTurnPlayer;
        private Player currentPlayer;
        private Card exposedCard;

        #endregion

        #region Constructors

        public BoardView()
        {
            gameManager = GameManager.Instance;
            gameManager.GameInitialized += HandleGameInitialized;
            gameManager.InformStatus += HandleStatus;
            gameManager.Reset += HandleReset;

            timer = null;
            selectableCards = new List<Card>();
            selectedCards = new List<Card>();
            playerViews = new List<PlayerView>();
        }

        #endregion

        #region Properties

        public int SecondsConunter 
        { 
            get 
            { 
                return secondsCounter; 
            } 
        }

        #endregion

        #region Methods 

        public void ToggleSelectedCard(Card card)
        {
            // Card can be selected only if it is selectable
            if (selectableCards.Contains(card))
            {
                if (selectedCards.Contains(card))
                {
                    selectedCards.Remove(card);
                }
                else
                {
                    selectedCards.Add(card);
                }
            }
        }

        public async void PlayTurn()
        {
            // Player can choose to take a card even if they can play a card.
            if (selectedCards.Count == 0) 
            {
                await GameManager.Instance.TakeCard();
            }
            else
            {
                await GameManager.Instance.PlayCards(selectedCards);
            }
        }

        /// <summary>
        /// Gets all the cards that can be played on the current exposed card
        /// </summary>
        /// <returns></returns>
        private List<Card> GetSelectableCards()
        {
            List<Card> currentSelectableCards = new List<Card>();

            currentSelectableCards.AddRange(currentPlayer.Hand.Where(card => card.CanBePutOn(exposedCard)));

            return currentSelectableCards;
        }

        private void HandleReset()
        {
            // Updates all
        }

        private void HandleTimerElapsed(UnityBasedTimer timer) 
        {
            secondsCounter--;

            if (secondsCounter == 0)
            {
                timer.End();
                secondsCounter = TURN_TIME_IN_MILLISECONDS;

                // Auto play since the player has not done anything
                PlayTurn();
            }
            else 
            { 
                // Updates seconds counter
            }
        }

        private void DisplaySelectableCards(List<Card> selectableCards)
        {
            selectableCards.AddRange(selectableCards);

            // Graphic logic
        }

        #region Command Handlers

        private void HandleGameInitialized(GameInitializedEventArgs args)
        {
            currentPlayer.Hand.AddRange(args.AllPlayers
                .First(player => player.ID.Equals(currentPlayer.ID)).Hand);

            // CreateOpponents(args.Players);

            // The current turn is the player's turn
            if (args.IsCurrentTurn)
            {
                if (args.SelectableCards.Count > 0)
                {
                    DisplaySelectableCards(args.SelectableCards);
                }
                else
                {
                    // IndicateTakingCard();
                }

                timer.Begin();
                secondsCounter = TURN_TIME_IN_MILLISECONDS;
            }
        }

        private void HandleStatus(InformStatusEventArgs args)
        {
            currentPlayer.Hand.AddRange(args.Players
                .First(player => player.ID.Equals(currentPlayer.ID)).Hand);

            if (args.Winner == null)
            {
                // CreateOpponents(args.Players);

                // The current turn is the player's turn
                if (args.IsCurrentTurn)
                {
                    if (args.SelectableCards.Count > 0)
                    {
                        DisplaySelectableCards(args.SelectableCards);
                    }
                    else
                    {
                        // IndicateTakingCard();
                    }

                    timer.Begin();
                    secondsCounter = TURN_TIME_IN_MILLISECONDS;
                }
            }
            else 
            {
                if (!args.Winner.ID.Equals(currentPlayer.ID))
                {
                    // IndicateLost();
                }     
            }
        }

        #endregion

        #endregion
    }
}
