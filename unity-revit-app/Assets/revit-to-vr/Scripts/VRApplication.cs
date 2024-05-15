using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevitToVR
{
    public class VRApplication : MonoBehaviour
    {
        private MainServiceClient mainServiceClient;
        
        // Start is called before the first frame update
        void Start()
        {
            mainServiceClient = new MainServiceClient();
            UIConsole.Log("Started VRApplication");
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnDestroy()
        {
            mainServiceClient = null;
        }
    }
}
