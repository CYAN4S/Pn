using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pn
{
    class DirectoryController
    {
        public List<string> ListboxList { get; set; }

        public DirectoryController()
        {
            ListboxList = new List<string>();
            ListboxList.Add("TESTING");
        }

        public void AddData(string data)
        {
            ListboxList.Add(data);
        }

        public void LoadFile(ListBox listBox, Canvas MainCanvas, Grid CanvasGrid)
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


                // 리스트 박스 중복 검사
                ListBoxItem remove = null;
                foreach (ListBoxItem i in listBox.Items)
                {
                    if ((string)i.Tag == openFileDialog.FileName)
                        remove = i;
                }

                if (remove != null)
                    listBox.Items.Remove(remove);

                // 리스트 박스 아이템 생성
                Grid gridItem = new Grid()
                {
                    Height = 45
                };

                Label titleLabel = new Label()
                {
                    Content = path[path.Length - 1],
                    Padding = new Thickness(5, 0, 5, 0),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };

                Label pathLabel = new Label()
                {
                    Content = openFileDialog.FileName,
                    Padding = new Thickness(5, 0, 5, 0),
                    Margin = new Thickness(0, 23, 0, 0),
                    BorderThickness = new Thickness(0, 0, 0, 5),
                    BorderBrush = Brushes.White,
                    Foreground = Brushes.White
                };

                gridItem.Children.Add(titleLabel);
                gridItem.Children.Add(pathLabel);
                
                ListBoxItem item = new ListBoxItem
                {
                    //Content = path[path.Length - 1] + "\n" + openFileDialog.FileName,
                    Content = gridItem,
                    Height = 45,
                    Tag = openFileDialog.FileName
                };
                listBox.Items.Insert(0, item);

                // 화면에 이미지 출력
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

                // 저장 기능 값 변경
                MainWindow.isNewFile = false;
                MainWindow.currentFilePath = openFileDialog.FileName;
            }
        }

        public void SaveFile(ListBox listBox, Canvas MainCanvas, Grid CanvasGrid)
        {
            if (MainWindow.isNewFile)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog
                {
                    //saveFileDialog1.Filter = "XAML files|*.xaml|Image files (*.png)|*.png|All files (*.*)|*.*";
                    Filter = "Image files (*.png)|*.png"
                };

                bool result = (bool)(saveFileDialog1.ShowDialog());

                if (!result)
                    return;

                // File.WriteAllText(saveFileDialog1.FileName, XamlWriter.Save(MainCanvas.Children));
                ExportToPng(saveFileDialog1.FileName, MainCanvas, CanvasGrid);
                var path = saveFileDialog1.FileName.Split('\\');

                Grid gridItem = new Grid()
                {
                    Height = 45
                };

                Label titleLabel = new Label()
                {
                    Content = path[path.Length - 1],
                    Padding = new Thickness(5, 0, 5, 0),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };

                Label pathLabel = new Label()
                {
                    Content = saveFileDialog1.FileName,
                    Padding = new Thickness(5, 0, 5, 0),
                    Margin = new Thickness(0, 23, 0, 0),
                    BorderThickness = new Thickness(0, 0, 0, 5),
                    BorderBrush = Brushes.White,
                    Foreground = Brushes.White
                };

                gridItem.Children.Add(titleLabel);
                gridItem.Children.Add(pathLabel);

                ListBoxItem item = new ListBoxItem
                {
                    //Content = path[path.Length - 1] + "\n" + saveFileDialog1.FileName,
                    Content = gridItem,
                    Height = 45,
                    Tag = saveFileDialog1.FileName,
                };
                listBox.Items.Insert(0, item);
            }
            else
            {
                ExportToPng(MainWindow.currentFilePath, MainCanvas, CanvasGrid);
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
    }
}
