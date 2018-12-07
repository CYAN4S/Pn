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
        public Brush color;
        public int penWidth;
        
        public ToolController()
        {
            pick = 1;
            color = Brushes.Black;
            penWidth = 0;
        }

    }
}
