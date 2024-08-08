using Assets.Code.Scripts.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Scripts.Client.EventInfos
{
    public class InformStatusEventArgs : EventArgs
    {
        #region Constructors

        public InformStatusEventArgs(Player winner, Card exposedCard, List<Player> players,bool isCurrentTurn, List<Card> selectableCards, object specialInfo)
        {
            Winner = winner;
            SelectableCards = selectableCards;
            Players = players;
            IsCurrentTurn = isCurrentTurn;
            ExposedCard = exposedCard;
            SpecialInfo = specialInfo;
        }

        #endregion

        #region Properties

        public Player Winner { get; set; }

        public List<Card> SelectableCards { get; set; }

        public List<Player> Players { get; set; }

        public bool IsCurrentTurn { get; set; }

        public Card ExposedCard { get; set; }

        public object SpecialInfo { get; set; }

        #endregion
    }
}
