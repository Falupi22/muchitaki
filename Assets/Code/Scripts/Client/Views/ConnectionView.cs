using Assets.Code.Scripts.Client.Popups;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Scripts.Client
{
    public class ConnectionView : MonoBehaviour
    {
        #region Constants

        public const string ERROR_MESSAGE_IP_INVALID = "Invalid input: IP is invalid";
        public const string ERROR_MESSAGE_PORT_INVALID = "Invalid input: Port is invalid";

        #endregion

        #region Fields

        [SerializeField]
        private TMP_InputField playerNameText;

        [SerializeField]
        private TMP_InputField ipText;

        [SerializeField]
        private TMP_InputField portText;

        [SerializeField]
        private Button connectButton;

        #endregion

        #region Constructions

        #endregion

        #region Events

        public event Action Connected;

        #endregion

        #region Methods

        [SerializeField]
        public async Task Connect()
        {
            if (IPAddress.TryParse(ipText.text, out IPAddress parsedIP))
            {
                if (int.TryParse(portText.text, out int parsedPort))
                {
                    await GameManager.Instance.Connect(parsedIP, parsedPort, playerNameText.text);
                }
                else
                {
                    MessageBox.Show(ERROR_MESSAGE_PORT_INVALID);
                }
            }
            else
            {
                MessageBox.Show(ipText.text);
                MessageBox.Show(ERROR_MESSAGE_IP_INVALID);
            }
        }

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.Connected += HandleConnected;
            connectButton.onClick.AddListener(async () => await Connect());
        }

        private void HandleConnected()
        {
            Connected?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}