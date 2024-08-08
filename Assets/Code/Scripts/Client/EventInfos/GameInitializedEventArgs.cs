using Assets.Code.Scripts.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Scripts.Client.EventInfos
{
    public class GameInitializedEventArgs : EventArgs
    {
        #region Constructors
        
        public GameInitializedEventArgs(List<Card> selectableCards, List<Player> allPlayers, bool isCurrentTurn)
        {
            SelectableCards = selectableCards;
            AllPlayers = allPlayers;
            IsCurrentTurn = isCurrentTurn;  
        }

        #endregion

        #region Properties

        public List<Card> SelectableCards { get; set; }

        public List<Player> AllPlayers { get; set; }

        public bool IsCurrentTurn { get; set; }

        #endregion
    }
}
