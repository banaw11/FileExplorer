using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;

namespace FileExplorer
{
    class ElementOfDirectory
    {
        public BitmapImage Icon { get; set; }
        public TextBox Name { get; set; }
        public DateTime Date { get; set; }

        public string Type  { get; set; }

        public string Size { get; set; }

        public string Path { get; set; }

        public bool Value { get; set; }

        
        
    }
}
