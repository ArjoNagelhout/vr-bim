using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace revit_to_vr_plugin
{
    /// <summary>
    /// Interaction logic for DockablePane.xaml
    /// </summary>
    public partial class RevitToVRDockablePane : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RevitToVRDockablePane()
        {
            InitializeComponent();
            DataContext = this;
            UIConsole.Instance.onTextChanged += OnTextChanged;
            OnTextChanged(UIConsole.Instance.Text);
        }

        ~RevitToVRDockablePane()
        {
            UIConsole.Instance.onTextChanged -= OnTextChanged;
        }

        void OnTextChanged(string text)
        {
            ConsoleOutput = text;
        }

        private string consoleOutput;
        public string ConsoleOutput
        {
            get { return consoleOutput; }
            set
            {
                consoleOutput = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConsoleOutput)));
            }
        }
    }
}
