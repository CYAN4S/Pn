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
        public List<int> LinesCounts { get; set; }

        Rectangle tempRectangle = new Rectangle();
        Ellipse tempEllipse = new Ellipse();
        Line tempinLine = new Line();

        public PaintCanvas(Canvas c)
        {
            MainCanvas = c;
            LinesCounts = new List<int>();
        }

        public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e, ToolController tool)
        {
            clicked = true;
            _pos = e.GetPosition(MainCanvas);
            linesCount = 1;
            switch (tool.pick)
            {
                case 1:
                    Ellipse myEllipse = new Ellipse
                    {
                        Width = tool.penWidth,
                        Height = tool.penWidth,
                        Fill = tool.strokeColor,
                    };
                    myEllipse.Margin = new Thickness(_pos.X - tool.penWidth * 0.5, _pos.Y - tool.penWidth * 0.5, 0, 0);

                    MainCanvas.Children.Add(myEllipse);
                    //polyline = new Polyline
                    //{
                    //    Stroke = tool.color,
                    //    Fill = tool.color,
                    //    StrokeThickness = tool.penWidth
                    //};
                    //polyline.Points.Add(_pos);
                    //MainCanvas.Children.Add(polyline);
                    break;

                case 2:
                    Ellipse myEllipseEra = new Ellipse
                    {
                        Width = tool.penWidth,
                        Height = tool.penWidth,
                        Fill = Brushes.White
                    };
                    myEllipseEra.Margin = new Thickness(_pos.X - tool.penWidth * 0.5, _pos.Y - tool.penWidth * 0.5, 0, 0);

                    MainCanvas.Children.Add(myEllipseEra);
                    break;
                    

                default:
                    break;
            }

        }

        public void MouseMove(object sender, MouseEventArgs e, ToolController tool)
        {
            if (!clicked)
            {
                tempRectangle = null;
                tempinLine = null;
                tempEllipse = null;
                return;
            }

            if (e.MouseDevice.LeftButton != MouseButtonState.Pressed)
            {
                tempRectangle = null;
                tempinLine = null;
                tempEllipse = null;
                LinesCounts.Add(1);
                return;
            }

            double left = 0, top = 0, width = 0, height = 0;

            var pos = e.GetPosition(MainCanvas);
            switch (tool.pick)
            {
                case 1:
                    Line myLine = new Line
                    {
                        Stroke = tool.strokeColor,
                        X1 = _pos.X,
                        X2 = pos.X,
                        Y1 = _pos.Y,
                        Y2 = pos.Y,
                        StrokeThickness = tool.penWidth,
                    };

                    Ellipse myEllipse_ = new Ellipse
                    {
                        Width = tool.penWidth,
                        Height = tool.penWidth,
                        //Stroke = tool.strokeColor,
                        Fill = tool.strokeColor,
                    };
                    myEllipse_.Margin = new Thickness(_pos.X - tool.penWidth * 0.5, _pos.Y - tool.penWidth * 0.5, 0, 0);

                    MainCanvas.Children.Add(myLine);
                    MainCanvas.Children.Add(myEllipse_);
                    _pos = pos;
                    linesCount += 2;
                    //polyline.Points.Add(pos);
                    break;

                case 2:
                    Line _myLine = new Line
                    {
                        Stroke = Brushes.White,
                        X1 = _pos.X,
                        X2 = pos.X,
                        Y1 = _pos.Y,
                        Y2 = pos.Y,
                        StrokeThickness = tool.penWidth,
                    };

                    Ellipse _myEllipse = new Ellipse
                    {
                        Width = tool.penWidth,
                        Height = tool.penWidth,
                        //Stroke = tool.strokeColor,
                        Fill = Brushes.White
                    };
                    _myEllipse.Margin = new Thickness(_pos.X - tool.penWidth * 0.5, _pos.Y - tool.penWidth * 0.5, 0, 0);

                    MainCanvas.Children.Add(_myLine);
                    MainCanvas.Children.Add(_myEllipse);
                    _pos = pos;
                    linesCount += 2;
                    break;

                case 3:
                    left = _pos.X;
                    top = _pos.Y;

                    width = pos.X - _pos.X;
                    height = pos.Y - _pos.Y;

                    if (pos.X < _pos.X)
                    {
                        left = pos.X;
                        width *= -1;
                    }
                    if (pos.Y < _pos.Y)
                    {
                        top = pos.Y;
                        height *= -1;
                    }

                    Rectangle myRectangle = new Rectangle
                    {
                        Stroke = tool.strokeColor,
                        Fill = tool.fillColor,
                        Margin = new Thickness(left, top, 0, 0),
                        Width = width,
                        Height = height,
                        StrokeThickness = tool.penWidth
                    };

                    MainCanvas.Children.Remove(tempRectangle);
                    tempRectangle = myRectangle;
                    MainCanvas.Children.Add(myRectangle);

                    break;
                case 4:
                    Line inLine = new Line
                    {
                        Stroke = tool.strokeColor,
                        X1 = _pos.X,
                        X2 = pos.X,
                        Y1 = _pos.Y,
                        Y2 = pos.Y,
                        StrokeThickness = tool.penWidth,
                    };
                    MainCanvas.Children.Remove(tempinLine);
                    tempinLine = inLine;
                    MainCanvas.Children.Add(inLine);
                    break;

                case 5:
                    left = _pos.X;
                    top = _pos.Y;

                    width = pos.X - _pos.X;
                    height = pos.Y - _pos.Y;

                    if (pos.X < _pos.X)
                    {
                        left = pos.X;
                        width *= -1;
                    }
                    if (pos.Y < _pos.Y)
                    {
                        top = pos.Y;
                        height *= -1;
                    }

                    Ellipse myEllipse = new Ellipse
                    {
                        Stroke = tool.strokeColor,
                        Fill = tool.fillColor,
                        Margin = new Thickness(left, top, 0, 0),
                        Width = width,
                        Height = height,
                        StrokeThickness = tool.penWidth
                    };

                    MainCanvas.Children.Remove(tempEllipse);
                    tempEllipse = myEllipse;
                    MainCanvas.Children.Add(myEllipse);

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
                    LinesCounts.Add(linesCount);
                    break;

                case 2:
                    LinesCounts.Add(linesCount);
                    break;

                case 3:
                    LinesCounts.Add(1);
                    break;

                case 4:
                    LinesCounts.Add(1);
                    break;

                case 5:
                    LinesCounts.Add(1);
                    break;

                default:
                    break;
            }
        }
    }
}
