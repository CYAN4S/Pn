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
        bool isNewFile = true;
        string currentFilePath = null;

        public MainWindow()
        {
            InitializeComponent();

            paintCanvas = new PaintCanvas(MainCanvas);
            toolController = new ToolController();
            directoryController = new DirectoryController();
            redos = new Stack<List<UIElement>>();

            MainCanvas.Width = CanvasGrid.Width;
            MainCanvas.Height = CanvasGrid.Height;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            CursorPosLable.Content = "(" + (int)e.GetPosition(CanvasGrid).X + ", " + (int)e.GetPosition(CanvasGrid).Y + ")";
            paintCanvas.MouseMove(sender, e, toolController);
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            paintCanvas.MouseLeftButtonUp(sender, e, toolController);
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            redos.Clear();
            paintCanvas.MouseLeftButtonDown(sender, e, toolController);
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

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

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files | *.png",
                CheckFileExists = true,
                CheckPathExists = true
            };
            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                var path = openFileDialog.FileName.Split('\\');
                MessageBox.Show("불러왔습니다.", "알림");

                ListBoxItem remove = null;
                foreach (ListBoxItem i in listBox.Items)
                {
                    if ((string)i.Tag == openFileDialog.FileName)
                    {
                        remove = i;
                    }
                }

                if (remove != null)
                {
                    listBox.Items.Remove(remove);
                }

                ListBoxItem item = new ListBoxItem
                {
                    Content = path[path.Length - 1] + "\n" + openFileDialog.FileName,
                    Height = 45,
                    Tag = openFileDialog.FileName
                };
                listBox.Items.Insert(0, item);

                Stream imageStreamSource = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                Image myImage = new Image
                {
                    Source = bitmapSource,
                    Width = bitmapSource.Width,
                    Height = bitmapSource.Height,

                    Tag = System.IO.Path.GetFullPath(openFileDialog.FileName)
                };

                MainCanvas.Children.Clear();
                CanvasGrid.Width = myImage.Width;
                CanvasGrid.Height = myImage.Height;
                
                MainCanvas.Width = CanvasGrid.Width;
                MainCanvas.Height = CanvasGrid.Height;
                MainCanvas.Children.Add(myImage);

                isNewFile = false;
                currentFilePath = openFileDialog.FileName;

                //imageStreamSource.Close();
            }

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            }
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

        private void NewFile(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            if (isNewFile)
            {
                //Stream myStream;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog
                {

                    //saveFileDialog1.Filter = "XAML files|*.xaml|Image files (*.png)|*.png|All files (*.*)|*.*";
                    Filter = "Image files (*.png)|*.png"
                };
                //saveFileDialog1.FilterIndex = 2;
                //saveFileDialog1.RestoreDirectory = true;

                var result = saveFileDialog1.ShowDialog();
                if (result == true)
                {
                    // File.WriteAllText(saveFileDialog1.FileName, XamlWriter.Save(MainCanvas.Children));
                    ExportToPng(saveFileDialog1.FileName, MainCanvas, CanvasGrid);
                }
                /*
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        // Code to write the stream goes here.
                        myStream.Close();
                    }
                }*/

            }
            else
            {
                ExportToPng(currentFilePath, MainCanvas, CanvasGrid);
            }

            MessageBox.Show("저장되었습니다.", "알림");


        }

        public void ExportToPng(string path, Canvas surface, Grid grid)
        {
            if (path == null) return;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size((int)surface.Width, (int)surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;
        }

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

        private void RectButton(object sender, RoutedEventArgs e)
        {
            toolController.pick = 3;
        }

        ResizeWindow resizeWindow = null;
        private void ResizeButton(object sender, RoutedEventArgs e)
        {
            //Application.Current.Properties["Width"] = ;
            //Application.Current.Properties["Height"] = ;

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
    }
}
