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

        private static GameManager gameManager;

        #endregion

        #region Methods

        private void Start()
        {
            gameManager = GameManager.Instance;
        }

        #endregion
    }
}