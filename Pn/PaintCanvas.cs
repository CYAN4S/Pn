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
        public Grid CanvasGrid { get; set; }

        bool clicked = false;
        Point _pos;

        public PaintCanvas(Grid canvasGrid)
        {
            CanvasGrid = canvasGrid;
        }


        public void PenStart(object sender, MouseButtonEventArgs e)
        {
            clicked = true;
            _pos = e.GetPosition(CanvasGrid);
        }

        public void PenUpdate(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(CanvasGrid);
            if (clicked)
            {
                Line myLine = new Line
                {
                    Stroke = System.Windows.Media.Brushes.LightSteelBlue,
                    X1 = _pos.X,
                    X2 = pos.X,
                    Y1 = _pos.Y,
                    Y2 = pos.Y,
                    StrokeThickness = 2
                };
                CanvasGrid.Children.Add(myLine);
                _pos = pos;
            }
        }

        public void PenEnd(object sender, MouseButtonEventArgs e)
        {
            clicked = false;
        }
    }
}
