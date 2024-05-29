using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RevitToVR
{
    public class SettingsPanel : MonoBehaviour
    {
        private VRApplication _vrApplication;

        [SerializeField] private TextMeshProUGUI connectedMessage;
        [SerializeField] private GameObject connectedVisual;
        [SerializeField] private GameObject notConnectedVisual;
        
        [SerializeField] private GameObject connectButton;
        [SerializeField] private GameObject disconnectButton;

        [SerializeField] private TextMeshProUGUI messageCountMessage;

        [SerializeField] private TMP_InputField ipAddressInputField;
        
        private bool _connected = false;
        private bool connected
        {
            get => _connected;
            set
            {
                _connected = value;
                UpdateConnectedVisual();
            }
        }

        private void UpdateConnectedVisual()
        {
            connectedVisual.SetActive(connected);
            notConnectedVisual.SetActive(!connected);
            connectButton.SetActive(!connected);
            disconnectButton.SetActive(connected);

            connectedMessage.text = connected ? 
                $"Connected to server at ip address {ipAddress}" : 
                "Not connected to server";
        }
        
        // ip address
        private string ipAddress; // gets set using player prefs

        private string ipAddressPlayerPrefsKey = "IP_ADDRESS"; 
        
        private void Start()
        {
            ipAddress = PlayerPrefs.GetString(ipAddressPlayerPrefsKey);
            ipAddressInputField.text = ipAddress;
            
            _vrApplication = VRApplication.instance;
            _vrApplication.onOpen += OnOpen;
            _vrApplication.onClose += OnClose;
            _vrApplication.onMessage += OnMessage;

            connected = false;
            messageCount = 0;
            
            ipAddressInputField.onValueChanged.AddListener(OnInputFieldIpAddressChanged);
        }

        private void OnInputFieldIpAddressChanged(string newValue)
        {
            ipAddress = newValue;
            PlayerPrefs.SetString(ipAddressPlayerPrefsKey, ipAddress);
        }

        private void OnDestroy()
        {
            _vrApplication.onOpen -= OnOpen;
            _vrApplication.onClose -= OnClose;
            _vrApplication.onMessage -= OnMessage;
            
            ipAddressInputField.onValueChanged.RemoveListener(OnInputFieldIpAddressChanged);
        }

        // should be called by the connect button
        public void RequestConnect()
        {
            if (ipAddress != "")
            {
                _vrApplication.RequestConnect(ipAddress);                
            }
        }

        // should be called by the disconnect button
        public void RequestDisconnect()
        {
            _vrApplication.RequestDisconnect();
        }

        private void OnOpen()
        {
            connected = true;
        }

        private void OnClose()
        {
            connected = false;
        }

        private int _messageCount = 0;

        private int messageCount
        {
            get => _messageCount;
            set
            {
                _messageCount = value;
                UpdateMessageCountMessage();
            }
        }

        private void UpdateMessageCountMessage()
        {
            messageCountMessage.text = $"Amount of messages received from server: {messageCount}";
        }
        
        private void OnMessage()
        {
            messageCount++;
        }
    }
}
