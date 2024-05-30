using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public class UIPanelsController : MonoBehaviour
    {
        public enum Panel
        {
            Properties = 0,
            Create,
            Document,
            Collaboration,
            Settings,
            Console
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

        // UIConsole
        [SerializeField] private GameObject consolePanel;

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

        public void SetActivePanel(Panel panel)
        {
            activePanel = panel;
        }

        public void SetPropertiesPanel()
        {
            activePanel = Panel.Properties;
        }

        public void SetCreatePanel()
        {
            activePanel = Panel.Create;
        }

        public void SetDocumentPanel()
        {
            activePanel = Panel.Document;
        }

        public void SetCollaborationPanel()
        {
            activePanel = Panel.Collaboration;
        }

        public void SetSettingsPanel()
        {
            activePanel = Panel.Settings;
        }

        public void SetConsolePanel()
        {
            activePanel = Panel.Console;
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
            consolePanel.SetActive(activePanel == Panel.Console);
        }

        private void Start()
        {
            activePanel = (Panel)PlayerPrefs.GetInt(activePanelKey, (int)Panel.Settings);

            GameObject[] objects = new GameObject[6];
            objects[0] = propertiesPanel;
            objects[1] = createPanel;
            objects[2] = documentPanel;
            objects[3] = collaborationPanel;
            objects[4] = settingsPanel;
            objects[5] = consolePanel;

            foreach (GameObject obj in objects)
            {
                obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }
}
