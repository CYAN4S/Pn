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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Pn
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        PaintCanvas paintCanvas;
        ToolController toolController;

        public MainWindow()
        {
            InitializeComponent();

            paintCanvas = new PaintCanvas(MainCanvas);
            toolController = new ToolController();

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            paintCanvas.MouseLeftButtonDown(sender, e, toolController);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            paintCanvas.MouseMove(sender, e, toolController);
        }
        
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            paintCanvas.MouseLeftButtonUp(sender, e, toolController);
        }

        private void PenButtonClick(object sender, RoutedEventArgs e)
        {
            toolController.pick = 1;
        }

        private void EraserButtonClick(object sender, RoutedEventArgs e)
        {
            toolController.pick = 2;
        }

        private void ColorSelectOKButton(object sender, RoutedEventArgs e)
        {
            //toolController.color = ColorSelector
            var cl = ColorSelector.SelectedColor;
            Color color = new Color();
            color.R = ColorSelector.R;
            color.G = ColorSelector.G;
            color.B = ColorSelector.B;
            color.A = ColorSelector.A;
            toolController.color = new SolidColorBrush(color);
            ColorSelectorGrid.Visibility = Visibility.Hidden;

        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

        }

        private void PenWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            toolController.penWidth = (int)PenWidth.Value;
        }

        private void PenWidthButton(object sender, RoutedEventArgs e)
        {
            if (PenWidthGrid.Visibility == Visibility.Visible)
            {
                PenWidthGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                PenWidthGrid.Visibility = Visibility.Visible;
            }
        }

        private void ColorSelectorButton(object sender, RoutedEventArgs e)
        {
            ColorSelectorGrid.Visibility = Visibility.Visible;
        }
    }
}
