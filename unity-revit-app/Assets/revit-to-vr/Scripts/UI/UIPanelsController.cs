using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public class UIPanelsController : MonoBehaviour
    {
        enum Panel
        {
            Properties = 0,
            Create,
            Document,
            Collaboration,
            Settings
        }
        
        // properties for the selected Element
        // contains metadata such as the name etc.
        // and contains Edit Buttons for the Toposolid
        [SerializeField] private GameObject propertiesPanel;

        // create functionality (dummy)
        [SerializeField] private GameObject createPanel;

        // document scale
        [SerializeField] private GameObject documentPanel;

        // collaboration functionality (dummy)
        [SerializeField] private GameObject collaborationPanel;
        
        // set ip address
        [SerializeField] private GameObject settingsPanel;

        private Panel _activePanel;

        private Panel activePanel
        {
            get => _activePanel;
            set
            {
                _activePanel = value;
                OnActivePanelChanged();                
            }
        }

        private readonly string activePanelKey = "ACTIVE_PANEL";

        private void OnActivePanelChanged()
        {
            PlayerPrefs.SetInt(activePanelKey, (int)activePanel);
            propertiesPanel.SetActive(activePanel == Panel.Properties);
            createPanel.SetActive(activePanel == Panel.Create);
            documentPanel.SetActive(activePanel == Panel.Document);
            collaborationPanel.SetActive(activePanel == Panel.Collaboration);
            settingsPanel.SetActive(activePanel == Panel.Settings);
        }

        private void Start()
        {
            activePanel = (Panel)PlayerPrefs.GetInt(activePanelKey, (int)Panel.Settings);
        }
    }
}
