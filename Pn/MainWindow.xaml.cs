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
        bool isNewFile = true;

        public MainWindow()
        {
            InitializeComponent();

            paintCanvas = new PaintCanvas(MainCanvas);
            toolController = new ToolController();
            directoryController = new DirectoryController();

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {


        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            paintCanvas.MouseLeftButtonUp(sender, e, toolController);
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            CursorPosLable.Content = "(" + (int)e.GetPosition(CanvasGrid).X + ", " + (int)e.GetPosition(CanvasGrid).Y + ")";
            paintCanvas.MouseMove(sender, e, toolController);
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            paintCanvas.MouseLeftButtonDown(sender, e, toolController);
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files | *.png";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                var path = openFileDialog.FileName.Split('\\');
                MessageBox.Show("불러왔습니다.", "알림");
                ListBoxItem item = new ListBoxItem
                {
                    Content = path[path.Length - 1],
                    Height = 37
                };
                listBox.Items.Add(item);

                Stream imageStreamSource = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];

                Image myImage = new Image();
                myImage.Source = bitmapSource;
                //myImage.Width = 200;

                myImage.Tag = System.IO.Path.GetFullPath(openFileDialog.FileName);

                MainCanvas.Children.Clear();
                CanvasGrid.Width = myImage.Width;
                CanvasGrid.Height = myImage.Height;
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
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                //saveFileDialog1.Filter = "XAML files|*.xaml|Image files (*.png)|*.png|All files (*.*)|*.*";
                saveFileDialog1.Filter = "Image files (*.png)|*.png";
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

            }


        }

        public void ExportToPng(string path, Canvas surface, Grid grid)
        {
            if (path == null) return;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size((int)grid.Width, (int)grid.Height);
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
            for (int i = 0; i < paintCanvas.linesCounts[paintCanvas.linesCounts.Count - 1]; i++)
            {
                MainCanvas.Children.RemoveAt(MainCanvas.Children.Count - 1);
            }
            paintCanvas.linesCounts.RemoveAt(paintCanvas.linesCounts.Count - 1);
        }
    }
}
