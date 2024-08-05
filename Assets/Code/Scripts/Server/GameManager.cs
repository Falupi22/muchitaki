using Assets.Code.Scripts.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        private List<Player> players;
        private Player currentPlayer;

        #endregion

        #region Constructor

        private GameManager()
        {
            cardDeck = new List<Card>();
            players = new List<Player>();
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
