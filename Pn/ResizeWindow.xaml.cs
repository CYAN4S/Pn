using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Pn
{
    /// <summary>
    /// ResizeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ResizeWindow : Window
    {
        public delegate void OnChildTextInputHandler(string Parameters1, string Parameters2);
        public event OnChildTextInputHandler OnChildTextInputEvent;

        public ResizeWindow(double w, double h)
        {
            InitializeComponent();
            WidthTextBox.Text = ((int)w).ToString();
            HeightTextBox.Text = ((int)h).ToString();
        }

        private void OKButton(object sender, RoutedEventArgs e)
        {
            OnChildTextInputEvent?.Invoke(WidthTextBox.Text, HeightTextBox.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnChildTextInputEvent?.Invoke(null, null);
        }
    }
}
