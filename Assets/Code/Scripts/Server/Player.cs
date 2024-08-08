using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Scripts.Server
{
    public class Player
    {
        #region Fields

        private Guid id;
        private List<Card> hand;
        private bool isCurrentTurn;
        private string name;
        private int score;

        #endregion

        #region Constructors

        public Player(string name, Guid id, List<Card> Hand = null)
        {
            this.id = id;
            this.name = name;

            if (Hand != null)
            {
                hand.AddRange(Hand);
            }
            else
            {
                Hand = new List<Card>();
            }
        }

        #endregion

        #region Properties

        public Guid ID
        {
            get { return id; }
            set { id = value; }
        }


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
