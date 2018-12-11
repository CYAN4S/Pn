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
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Markup;


namespace Pn
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        PaintCanvas paintCanvas;
        ToolController toolController;
        DirectoryController directoryController;
        Stack<List<UIElement>> redos;
        public static bool isNewFile = true;
        public static string currentFilePath = null;
        ScaleTransform st = new ScaleTransform();
        

        public MainWindow()
        {
            InitializeComponent();

            paintCanvas = new PaintCanvas(MainCanvas);
            toolController = new ToolController();
            directoryController = new DirectoryController();
            redos = new Stack<List<UIElement>>();

            MainCanvas.Width = CanvasGrid.Width;
            MainCanvas.Height = CanvasGrid.Height;

            MainCanvas.RenderTransform = st;
            MainCanvas.Children.Clear();
        }

        #region Event

        // WINDOW
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PenWidthGrid.Visibility = Visibility.Hidden;
            HelpLabel.Content = "Window";
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //CursorPosLable.Content = "(" + (int)e.GetPosition(CanvasGrid).X + ", " + (int)e.GetPosition(CanvasGrid).Y + ")";
            paintCanvas.MouseMove(sender, e, toolController);
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            paintCanvas.MouseLeftButtonUp(sender, e, toolController);
            //HelpLabel.Content = "LAB_";
        }

        //MAINCANVAS
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            CursorPosLable.Content = "(" + (int)e.GetPosition(CanvasGrid).X + ", " + (int)e.GetPosition(CanvasGrid).Y + ")";
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HelpLabel.Content = "MainCanvas";
            PenWidthGrid.Visibility = Visibility.Hidden;
            redos.Clear();
            paintCanvas.MouseLeftButtonDown(sender, e, toolController);
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void PaintGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HelpLabel.Content = "PaintGrid";
        }

        #endregion

        #region Drawing Tool Selection

        private void PenButtonClick(object sender, RoutedEventArgs e)
        {
            toolController.pick = 1;
        }

        private void EraserButtonClick(object sender, RoutedEventArgs e)
        {
            toolController.pick = 2;
        }

        private void RectButton(object sender, RoutedEventArgs e)
        {
            toolController.pick = 3;
        }

        private void LineButton(object sender, RoutedEventArgs e)
        {
            toolController.pick = 4;
        }

        private void button_Copy6_Click(object sender, RoutedEventArgs e)
        {
            toolController.pick = 5;
        }

        #endregion

        #region Color Tool

        private void ColorSelectorButton(object sender, RoutedEventArgs e)
        {
            ColorSelectorGrid.Visibility = Visibility.Visible;
        }

        private void ColorSelectOKButton(object sender, RoutedEventArgs e)
        {
            //toolController.color = ColorSelector
            var cl = ColorSelector.SelectedColor;
            Color color = new Color
            {
                R = ColorSelector.R,
                G = ColorSelector.G,
                B = ColorSelector.B,
                A = ColorSelector.A
            };
            toolController.strokeColor = new SolidColorBrush(color);
            ColorSelectorGrid.Visibility = Visibility.Hidden;
        }


        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            ColorSelectorGrid_.Visibility = Visibility.Visible;
        }

        private void button2__Click(object sender, RoutedEventArgs e)
        {
            var cl = ColorSelector_.SelectedColor;
            Color color = new Color
            {
                R = ColorSelector_.R,
                G = ColorSelector_.G,
                B = ColorSelector_.B,
                A = ColorSelector_.A
            };
            toolController.fillColor = new SolidColorBrush(color);
            ColorSelectorGrid_.Visibility = Visibility.Hidden;
        }

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            
            if (lbi != null)
            {
                Stream imageStreamSource = new FileStream((string)lbi.Tag, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                Image myImage = new Image
                {
                    Source = bitmapSource,
                    Width = bitmapSource.Width,
                    Height = bitmapSource.Height,

                    Tag = System.IO.Path.GetFullPath((string)lbi.Tag)
                };

                MainCanvas.Children.Clear();
                CanvasGrid.Width = myImage.Width;
                CanvasGrid.Height = myImage.Height;

                MainCanvas.Width = CanvasGrid.Width;
                MainCanvas.Height = CanvasGrid.Height;
                MainCanvas.Children.Add(myImage);

                isNewFile = false;
                currentFilePath = (string)(lbi.Tag);
            }
        }

        #region Pen Width Tool

        private void PenWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (toolController != null)
            {
                toolController.penWidth = (int)PenWidth.Value;
            }
        }

        private void PenWidthButton(object sender, RoutedEventArgs e)
        {
            PenWidthGrid.Visibility = PenWidthGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        
        #endregion

        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.Children.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("내용", "타이틀", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    directoryController.SaveFile(listBox, MainCanvas, CanvasGrid);
                }
                else if (result == MessageBoxResult.No)
                {
                    // skip.
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            MainCanvas.Children.Clear();
            currentFilePath = null;
            isNewFile = true;
        }

        #region Load & Save

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            directoryController.LoadFile(listBox, MainCanvas, CanvasGrid);
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            directoryController.SaveFile(listBox, MainCanvas, CanvasGrid);
            //if (isNewFile)
            //{
            //    //Stream myStream;
            //    SaveFileDialog saveFileDialog1 = new SaveFileDialog
            //    {
            //        //saveFileDialog1.Filter = "XAML files|*.xaml|Image files (*.png)|*.png|All files (*.*)|*.*";
            //        Filter = "Image files (*.png)|*.png"
            //    };
            //    //saveFileDialog1.FilterIndex = 2;
            //    //saveFileDialog1.RestoreDirectory = true;
            //    var result = saveFileDialog1.ShowDialog();
            //    if (result == true)
            //    {
            //        // File.WriteAllText(saveFileDialog1.FileName, XamlWriter.Save(MainCanvas.Children));
            //        ExportToPng(saveFileDialog1.FileName, MainCanvas, CanvasGrid);
            //    }
            //    /*
            //    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //    {
            //        if ((myStream = saveFileDialog1.OpenFile()) != null)
            //        {
            //            // Code to write the stream goes here.
            //            myStream.Close();
            //        }
            //    }*/
            //    var path = saveFileDialog1.FileName.Split('\\');
            //    ListBoxItem item = new ListBoxItem
            //    {
            //        Content = path[path.Length - 1] + "\n" + saveFileDialog1.FileName,
            //        Height = 45,
            //        Tag = saveFileDialog1.FileName
            //    };
            //    listBox.Items.Insert(0, item);
            //}
            //else
            //{
            //    ExportToPng(currentFilePath, MainCanvas, CanvasGrid);
            //}
            //MessageBox.Show("저장되었습니다.", "알림");
        }
        #endregion

        #region Undo & Redo
        private void UndoButton(object sender, RoutedEventArgs e)
        {
            if (paintCanvas.LinesCounts.Count != 0)
            {
                List<UIElement> l = new List<UIElement>();
                for (int i = 0; i < paintCanvas.LinesCounts[paintCanvas.LinesCounts.Count - 1]; i++)
                {
                    if (MainCanvas.Children.Count != 0)
                    {
                        l.Add(MainCanvas.Children[MainCanvas.Children.Count - 1]);
                        MainCanvas.Children.RemoveAt(MainCanvas.Children.Count - 1);
                    }
                }
                redos.Push(l);
                paintCanvas.LinesCounts.RemoveAt(paintCanvas.LinesCounts.Count - 1);
            }
            
        }

        private void RedoButton(object sender, RoutedEventArgs e)
        {
            if (redos.Count != 0)
            {
                List<UIElement> l = redos.Pop();
                paintCanvas.LinesCounts.Add(l.Count);
                foreach (var item in l)
                {
                    MainCanvas.Children.Add(item);
                }
            }
        }
        #endregion
        
        #region Resize Window
        ResizeWindow resizeWindow = null;
        private void ResizeButton(object sender, RoutedEventArgs e)
        {
            if (resizeWindow == null)
            {
                resizeWindow = new ResizeWindow(CanvasGrid.Width, CanvasGrid.Height);
                resizeWindow.OnChildTextInputEvent += new ResizeWindow.OnChildTextInputHandler(OKEvent);
                resizeWindow.Show();
            }
        }

        void OKEvent(string a, string b)
        {
            if (a != null && b != null)
            {
                CanvasGrid.Width = int.Parse(a);
                CanvasGrid.Height = int.Parse(b);
                MainCanvas.Width = CanvasGrid.Width;
                MainCanvas.Height = CanvasGrid.Height;
                resizeWindow.Close();
            }
            
            if (resizeWindow != null)
            {
                resizeWindow.OnChildTextInputEvent -= new ResizeWindow.OnChildTextInputHandler(OKEvent);
                resizeWindow = null;
            }
        }
        #endregion

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            CanvasGrid.Width *= 2;
            CanvasGrid.Height *= 2;

            st.ScaleX *= 2;
            st.ScaleY *= 2;

            RangeSlider.Value = Math.Log(st.ScaleX, 2);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            CanvasGrid.Width /= 2;
            CanvasGrid.Height /= 2;

            st.ScaleX /= 2;
            st.ScaleY /= 2;

            RangeSlider.Value = Math.Log(st.ScaleX, 2);
        }

        private void RangeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CanvasGrid.Width = MainCanvas.ActualWidth * Math.Pow(2, RangeSlider.Value);
            CanvasGrid.Height = MainCanvas.ActualHeight * Math.Pow(2, RangeSlider.Value);

            st.ScaleX = Math.Pow(2, RangeSlider.Value);
            st.ScaleY = Math.Pow(2, RangeSlider.Value);

            RangeTextBox.Text = ((int)(100 * Math.Pow(2, RangeSlider.Value))).ToString() + "%";
        }

        private void RangeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //string inputText = RangeTextBox.Text;
            //char[] percent = { '%' };
            //inputText = inputText.TrimEnd(percent);
            //int percentage =


        }

        
    }
}
