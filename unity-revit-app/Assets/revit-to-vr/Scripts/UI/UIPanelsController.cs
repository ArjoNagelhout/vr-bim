using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public class UIPanelsController : MonoBehaviour
    {
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
    }
}
