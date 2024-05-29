using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

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

        private string permanentText_;
        public string PermanentText
        {
            get => permanentText_;
            set
            {
                permanentText_ = value;
                onTextChanged?.Invoke(permanentText_ + text_);
            }
        }

        // properties
        private string text_;
        public string Text
        {
            get => text_;
            set
            {
                text_ = value;
                onTextChanged?.Invoke(permanentText_ + text_);
            }
        }

        public OnTextChanged onTextChanged;

        // methods
        public static void Clear()
        {
            Instance.Text = "";
        }

        public static void ClearPermanent()
        {
            Instance.PermanentText = "";
        }

        public static void Log(string text)
        {
            string formatted = $"[{DateTime.Now}] {text}";
            Instance.Text = formatted + "\n";
            Debug.WriteLine(formatted);
        }

        // replaces the permanent text
        public static void LogPermanent(string text)
        {
            string formatted = $"[{DateTime.Now}] {text}";
            Instance.PermanentText = formatted + "\n";
            Debug.WriteLine(formatted);
        }

        private static string logFileName = "revit_to_vr_plugin.log";

        private static string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), logFileName);

        // Constructor to set up TraceListener
        private UIConsole()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));
            Trace.AutoFlush = true;
        }
    }
}