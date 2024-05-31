using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RevitToVR
{
    public class UIConsole : MonoBehaviour
    {
        public static UIConsole Instance => instance_;
        private static UIConsole instance_;
        
        private string text_ = "";
        public string Text
        {
            get => text_;
            set
            {
                text_ = value;
                textMesh.text = text_; // update text mesh
            }
        }

        [SerializeField]
        private TextMeshProUGUI textMesh;
        
        private void Awake()
        {
            if (instance_ != null)
            {
                Destroy(instance_.gameObject);
            }
            instance_ = this;
        }

        public static void Log(string text)
        {
            //Clear();
            Instance.Text += text + "\n";
            Debug.Log("UIConsole: " + text);
            
        }

        public static void Clear()
        {
            Instance.Text = "";
        }
    }
}
