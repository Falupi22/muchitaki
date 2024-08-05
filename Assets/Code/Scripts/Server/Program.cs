using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

/// <summary>
/// Change destination card
/// </summary>
namespace Assets.Code.Scripts.Server
{
    public class Program : MonoBehaviour
    {
        #region Fields

        private static List<Player> connectedPlayers;
        private static GameManager gameManager;

        #endregion

        #region Methods

        private void Start()
        {
            connectedPlayers = new List<Player>();
            gameManager = GameManager.Instance;
        }

        private void OnPlayersCollectionChanged()
        {
            if (connectedPlayers.Count >= GameManager.MIN_PLAYERS)
            {
                gameManager.StartNew(connectedPlayers);
            }
        }

        #endregion
    }
}