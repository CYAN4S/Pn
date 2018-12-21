using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
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
            ListboxList = new List<string>
            {
                "TESTING"
            };
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
                    Filter = "Image files (*.png)|*.png"
                };

                bool result = (bool)(saveFileDialog1.ShowDialog());

                if (!result)
                    return;
                
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
                    Content = gridItem,
                    Height = 45,
                    Tag = saveFileDialog1.FileName,
                };
                listBox.Items.Insert(0, item);
                MainWindow.currentFilePath = saveFileDialog1.FileName;
            }
            else
            {
                ExportToPng(MainWindow.currentFilePath, MainCanvas, CanvasGrid);
            }
            MessageBox.Show("저장되었습니다.", "알림");

            MainWindow.isNewFile = false;
            
        }

        public void ExportToPng(string path, Canvas surface, Grid grid)
        {
            if (path == null) return;
            
            Transform transform = surface.LayoutTransform;
            surface.LayoutTransform = null;
            
            Size size = new Size((int)surface.Width, (int)surface.Height);
            surface.Measure(size);
            surface.Arrange(new Rect(size));
            
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(surface);
            
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
            
            surface.LayoutTransform = transform;
        }
    }
}
