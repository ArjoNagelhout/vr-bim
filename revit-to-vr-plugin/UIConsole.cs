using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace revit_to_vr_plugin
{
    public class UIConsole
    {
        // singleton
        private static UIConsole instance_;
        public static UIConsole Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new UIConsole();
                }
                return instance_;
            }
        }

        public delegate void OnTextChanged(string text);

        // properties
        private string text_;
        public string Text
        {
            get => text_;
            set
            {
                text_ = value;
                onTextChanged?.Invoke(text_);
            }
        }

        public OnTextChanged onTextChanged;

        // methods
        public static void Clear()
        {
            Instance.text_ = "";
        }
        
        public static void Log(string text)
        {
            Instance.Text += text + "\n";
            Debug.WriteLine(text);
        }

    }
}
