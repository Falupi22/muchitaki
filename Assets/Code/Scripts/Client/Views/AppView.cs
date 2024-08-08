using Assets.Code.Scripts.Client.Popups;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.Scripts.Client {

    public class AppView : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private BoardView board;

        [SerializeField]
        private ConnectionView view;

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void HandleConnected()
        {
            MessageBox.Show("Nice");
        }

        // Start is called before the first frame update
        void Start()
        {
            view.Connected += HandleConnected;
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion
    }
}