using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
