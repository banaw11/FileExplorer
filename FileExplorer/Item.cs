using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FileExplorer
{
    class Item : TreeViewItem
    {
        
        public string NameT { get; set; }
        public  BitmapImage Icon { get; set; }

    }

    public class Element
    {
        
        public string NameE { get; set; }
        public BitmapImage Icon { get; set; }

        public string Tag { get; set; }
    }

    public class ElementOfPath
    {
        public string NameE { get; set; }
        public BitmapImage Icon { get; set; }

        public string Tag { get; set; }
    }

    
}
