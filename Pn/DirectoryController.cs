using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Pn
{
    class DirectoryController
    {
        List<string> ListboxList;

        public DirectoryController()
        {
            ListboxList = new List<string>();
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
                ListBoxItem item = new ListBoxItem
                {
                    Content = path[path.Length - 1] + "\n" + openFileDialog.FileName,
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
    }
}
