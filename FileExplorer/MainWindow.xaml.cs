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
using System.IO;
using System.Linq.Expressions;

namespace FileExplorer
{
    
    public partial class MainWindow : Window
    {
        

        internal static List<ElementOfDirectory> elements = new List<ElementOfDirectory>();
        public MainWindow()
        {
            
            InitializeComponent();

            LoadThisPc();
            
        }

        /// <summary>
        /// Method of performig actions after selection item of ListView. 
        /// </summary>
        /// <param name="sender">ListView</param>
        /// <param name="e">Exception</param>
        public void FileBrowser(object sender, EventArgs e)
        {

            ListView list = (ListView)sender;

            ElementOfDirectory element = (ElementOfDirectory)list.SelectedItem;
            Paths.ShowCurrentPath(pathControl, element.Path);
            if (Paths.CheckElement(element.Path))
            {
                RefreshFileBrowser();
                TreeViewItem item = new TreeViewItem { Header = element.Name, Tag = element.Path };
                TreeViewItem tvi = FindTreeViewItem(item);
                if (tvi != null)
                {
                    
                    tvi.IsExpanded = true;
                }
            }

        }

        private TreeViewItem FindTreeViewItem(object o)
        {
            TreeViewItem item = explorerMenu.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;

            if (item != null)
            {
                
                item.IsExpanded = true;
                return item;
            }
            
            foreach (var itemTVI in item.Items)
            {
                TreeViewItem item2 = explorerMenu.ItemContainerGenerator.ContainerFromItem(itemTVI) as TreeViewItem;

                item2= FindTreeViewItem(item2);

                if (item2!= null)
                {
                    
                    item2.IsExpanded = true;
                }
                return item2;
            }
            return null;
        }
        private void SelectedMenuItem(object sender, EventArgs e)
        {
            try
            {
                TreeView menuitem = (TreeView)sender;
                TreeViewItem item = (TreeViewItem)menuitem.SelectedItem;
                if(item.Tag != null)
                {
                    Paths.ShowCurrentPath(pathControl, item.Tag.ToString());
                    if (Paths.CheckElement(item.Tag.ToString()))
                    {
                        if(listExplorer.Visibility == Visibility.Hidden)
                        {
                            listExplorer.Visibility = Visibility.Visible;
                            mainBrowser.Visibility = Visibility.Hidden;
                        }

                        RefreshFileBrowser();
                    }
                }
                else
                {
                    Paths.ShowCurrentPath(pathControl, null);
                    LoadThisPc();
                    
                }
                
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Method to load File and Directory View in FileBrowser
        /// </summary>
        public void RefreshFileBrowser()
        {
            listExplorer.Items.Clear();
            foreach (var item in elements)
            {
                listExplorer.Items.Add(item);
            }
            if(listExplorer.Visibility is Visibility.Hidden)
            {
                listExplorer.Visibility = Visibility.Visible;
                mainBrowser.Visibility = Visibility.Hidden;
            }
            Paths.ShowCurrentPath(pathControl, Paths.currentPath);
        }

        public void LoadThisPc()
        {
            if (mainBrowser.Visibility == Visibility.Hidden)
            {
                mainBrowser.Visibility = Visibility.Visible;
                listExplorer.Visibility = Visibility.Hidden;

            }
            Paths.currentPath = null; 
            Paths.ThisPcElements(logicalDrives, userFolder);
            Paths.LoadMenuExplorer(explorerMenu);
            Paths.ShowCurrentPath(pathControl, null) ;
        }

        private void TypePath(object sender, MouseButtonEventArgs e)
        {
            pathText.Visibility = Visibility.Visible;
            pathControl.Visibility = Visibility.Hidden;

            pathText.Text = Paths.currentPath;
        }

        private void PathGrid(object sender, MouseButtonEventArgs w)
        {
            if (pathText.Visibility is Visibility.Visible)
            {
                pathText.Visibility = Visibility.Hidden;
                pathControl.Visibility = Visibility.Visible;

            }
            else
            {
                Paths.ShowCurrentPath(pathControl, Paths.currentPath);
            }

           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToPath(object sender, KeyEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if(e.Key == Key.Enter)
            { 
                if (Paths.CheckPath(text.Text))
                {
                    listExplorer.Items.Clear();
                    foreach (var i in elements)
                    {
                        listExplorer.Items.Add(i);
                    }
                    Paths.ShowCurrentPath(pathControl, Paths.currentPath);
                    if (mainBrowser.Visibility == Visibility.Visible)
                    {
                        mainBrowser.Visibility = Visibility.Hidden;
                        listExplorer.Visibility = Visibility.Visible;

                    }
                }
                
                pathText.Visibility = Visibility.Hidden;
                pathControl.Visibility = Visibility.Visible;
                Paths.ShowCurrentPath(pathControl, Paths.currentPath);
            }
        }

        /// <summary>
        /// Method which added sub directories of selected directory to ContextMenu as MenuItem
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e"></param>
        private void ShowSubDirectory (object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.ContextMenu.Items.Clear();
            button.Content = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/down.png", UriKind.Absolute)) };
            if(button.Tag != null)
            {
                string[] directories = Directory.GetDirectories(button.Tag.ToString());
                foreach (var dir in directories)
                {
                    MenuItem item = new MenuItem();
                    item.Header = new DirectoryInfo(dir).Name;
                    item.Tag = dir;
                    item.Icon = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/folder.png", UriKind.Absolute)) };
                    item.Click += ItemSelected;
                    button.ContextMenu.Items.Add(item);
                }
            }
            else
            {
                foreach (var element in Paths.elementsThisPC)
                {
                    MenuItem item = new MenuItem();
                    item.Header = element.NameE;
                    item.Tag = element.Tag;
                    item.Icon = new Image { Source = element.Icon };
                    item.Click += ItemSelected;
                    button.ContextMenu.Items.Add(item);
                }
            }
            if(button.ContextMenu.Items.IsEmpty is false)
            {
                button.ContextMenu.IsOpen = true;
            }
            

           
        }

        /// <summary>
        /// Method for ContextMenu Item clicked. Return path of selected item and show his content
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Paths.currentPath = item.Tag.ToString();
            Paths.CheckElement(Paths.currentPath);
            RefreshFileBrowser();
            Paths.ShowCurrentPath(pathControl, Paths.currentPath);
            if (mainBrowser.Visibility == Visibility.Visible)
            {
                mainBrowser.Visibility = Visibility.Hidden;
                listExplorer.Visibility = Visibility.Visible;

            }

        }

        private void ThisPcViewSelectedItem(object sender, MouseButtonEventArgs e)
        {
            Button item = sender as Button;
            Paths.currentPath = item.Tag.ToString();
            Paths.CheckElement(Paths.currentPath);
            RefreshFileBrowser();
        }
    }
}
