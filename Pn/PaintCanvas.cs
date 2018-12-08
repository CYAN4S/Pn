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


namespace Pn
{
    class PaintCanvas
    {
        public Canvas MainCanvas { get; set; }

        bool clicked = false;
        Point _pos;
        int linesCount = 0;
        public List<int> linesCounts = new List<int>();

        public PaintCanvas(Canvas c)
        {
            MainCanvas = c;
        }


        public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e, ToolController tool)
        {
            switch (tool.pick)
            {
                case 1:
                    clicked = true;
                    _pos = e.GetPosition(MainCanvas);
                    linesCount = 0;
                    break;

                case 2:
                    break;

                default:
                    break;
            }

        }

        public void MouseMove(object sender, MouseEventArgs e, ToolController tool)
        {
             if (!clicked)
            {
                return;
            }
            switch (tool.pick)
            {
                case 1:
                    var pos = e.GetPosition(MainCanvas);
                    if (clicked)
                    {
                        Line myLine = new Line
                        {
                            Stroke = tool.color,
                            Fill = tool.color,
                            X1 = _pos.X,
                            X2 = pos.X,
                            Y1 = _pos.Y,
                            Y2 = pos.Y,
                            StrokeThickness = tool.penWidth,
                        };

                        Ellipse myEllipse = new Ellipse
                        {
                            Width = tool.penWidth,
                            Height = tool.penWidth,
                            Stroke = tool.color,
                            Fill = tool.color,
                        };
                        myEllipse.Margin = new Thickness(_pos.X - tool.penWidth * 0.5, _pos.Y - tool.penWidth * 0.5, 0, 0);
                        
                        MainCanvas.Children.Add(myLine);
                        MainCanvas.Children.Add(myEllipse);
                        _pos = pos;
                        linesCount++;
                    }
                    break;

                case 2:
                    break;

                default:
                    break;
            }
        }

        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e, ToolController tool)
        {
            clicked = false;
            switch (tool.pick)
            {
                case 1:
                    linesCounts.Add(linesCount);
                    break;

                case 2:
                    break;

                default:
                    break;
            }
        }
    }
}
