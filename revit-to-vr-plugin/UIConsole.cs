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
        public delegate void OnTextChanged(string text);

        // properties
        private string text_;
        public OnTextChanged onTextChanged;
        public string Text => text_;

        // methods
        public void Clear()
        {
            text_ = "";
            onTextChanged?.Invoke(text_);
        }
        
        public void Log(string text)
        {
            text_ += text + "\n";
            onTextChanged?.Invoke(text_);
            Debug.WriteLine(text);
        }
    }
}
