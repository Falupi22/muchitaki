using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Scripts.Server
{
    internal class Player
    {
        #region Fields

        private List<Card> hand;
        private bool isCurrentTurn;
        private string name;
        private int score;

        #endregion

        #region Constructors

        public Player(List<Card> Hand, string name)
        {
            hand.AddRange(Hand);
            this.name = name;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Card> Hand
        {
            get { return hand; }
            set { hand = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        #endregion

        #region Methods

        #endregion
    }
}
