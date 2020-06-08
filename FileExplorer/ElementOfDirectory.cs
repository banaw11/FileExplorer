using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FileExplorer
{
    class ElementOfDirectory
    {
        public BitmapImage Icon { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public string Type  { get; set; }

        public string Size { get; set; }

        public string Path { get; set; }

        
        
    }
}
