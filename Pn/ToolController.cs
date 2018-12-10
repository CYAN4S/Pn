using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pn
{
    class ToolController
    {
        public int pick;
        public Brush strokeColor;
        public Brush fillColor;
        public int penWidth;
        
        public ToolController()
        {
            pick = 1;
            strokeColor = Brushes.Black;
            fillColor = Brushes.Black;
            penWidth = 1;
        }

    }
}
