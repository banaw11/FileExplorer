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
using System.Threading;

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
            }
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
                        TreeViewItem tvi = explorerMenu.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
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
            Paths.CheckElement(Paths.CurrentPath);
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
            Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
            searchText.Text = "Searching in : " + new DirectoryInfo(Paths.CurrentPath).Name;

        }

        public void LoadThisPc()
        {
            if (mainBrowser.Visibility == Visibility.Hidden)
            {
                mainBrowser.Visibility = Visibility.Visible;
                listExplorer.Visibility = Visibility.Hidden;

            }
            Paths.CurrentPath = null;
            if (logicalDrives.Items.IsEmpty && userFolder.Items.IsEmpty) Paths.ThisPcElements(logicalDrives, userFolder);
            if (explorerMenu.Items.IsEmpty) Paths.LoadMenuExplorer(explorerMenu);
            Paths.ShowCurrentPath(pathControl, Paths.CurrentPath) ;
        }

        private void TypePath(object sender, MouseButtonEventArgs e)
        {
            pathText.Visibility = Visibility.Visible;
            pathControl.Visibility = Visibility.Hidden;
            pathText.Text = Paths.CurrentPath;
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
                Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
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
                    Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
                    if (mainBrowser.Visibility == Visibility.Visible)
                    {
                        mainBrowser.Visibility = Visibility.Hidden;
                        listExplorer.Visibility = Visibility.Visible;

                    }
                }
                
                pathText.Visibility = Visibility.Hidden;
                pathControl.Visibility = Visibility.Visible;
                Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
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
            button.Content = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/down.png", UriKind.Absolute)) };
            if(button.Tag != null && button.ContextMenu.Items.IsEmpty)
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
            else if( button.ContextMenu.Items.IsEmpty)
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
            if(button.ContextMenu.Items.IsEmpty is false) button.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// Method for ContextMenu Item clicked. Return path of selected item and show his content
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Paths.CurrentPath = item.Tag.ToString();
            Paths.CheckElement(Paths.CurrentPath);
            RefreshFileBrowser();
            Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
            if (mainBrowser.Visibility == Visibility.Visible)
            {
                mainBrowser.Visibility = Visibility.Hidden;
                listExplorer.Visibility = Visibility.Visible;

            }

        }

        private void ThisPcViewSelectedItem(object sender, MouseButtonEventArgs e)
        {
            Button item = sender as Button;
            Paths.CurrentPath = item.Tag.ToString();
            Paths.CheckElement(Paths.CurrentPath);
            RefreshFileBrowser();
        }

        public void GoToDirectory(object sender, MouseButtonEventArgs e)
        {
            Button directory = sender as Button;
            if (directory.Tag != null)
            {
                Paths.CurrentPath = directory.Tag.ToString();
                RefreshFileBrowser();
            }
            else
            {
                LoadThisPc();
            }
            
        }

        /// <summary>
        /// Method which move to previous path
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e"></param>
        public void GoToPreviousPath(object sender, EventArgs e)
        {
            if(Paths.previousPaths.Count > 0)
            {
                if(Paths.previousPaths[Paths.previousPaths.Count - 1] != null)
                {
                    Paths.CheckElement(Paths.previousPaths[Paths.previousPaths.Count - 1]);
                    RefreshFileBrowser();
                }
                else
                {
                    LoadThisPc();
                }
                Paths.nextPaths.Add(Paths.previousPaths[Paths.previousPaths.Count - 1]);
                Paths.previousPaths.RemoveAt(Paths.previousPaths.Count - 1);
                Paths.previousPaths.RemoveAt(Paths.previousPaths.Count - 1);
                Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
                
            }
        }

        /// <summary>
        /// Method which move to earlier path of previous
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e"></param>
        public void GoToNextPath (object sender, EventArgs e)
        {
            if(Paths.nextPaths.Count > 0)
            {
                if (Paths.nextPaths[Paths.nextPaths.Count - 1] != null)
                {
                    Paths.CheckElement(Paths.nextPaths[Paths.nextPaths.Count - 1]);
                    RefreshFileBrowser();
                }
                else
                {
                    LoadThisPc();
                }
                Paths.nextPaths.RemoveAt(Paths.nextPaths.Count - 1);
                Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
            }
        }
        
        public void SearchElement (object sender, KeyEventArgs e)
        {
            int counter = 0;
            TextBox text = sender as TextBox;
            string searchingFile = text.Text.ToLower();
            searchList.Visibility = Visibility.Hidden;
            searchList.Items.Clear();
            if(e.Key == Key.Enter)
            {
                elements.Clear();
                listExplorer.Items.Clear();
                Paths.ShowFoundedElements();
                foreach (var i in elements)
                {
                    listExplorer.Items.Add(i);
                }
            }
            else
            {
                if (searchingFile.Length > 1)
            {
                Paths.GetSearchingElement(searchingFile, Paths.CurrentPath);
                if (elements.Count > 0)
                {
                    foreach (var element in Paths.elements)
                    {
                         searchList.Items.Add(element);
                        if (counter++ == 8) break;
                    }
                    searchList.Visibility = Visibility.Visible;
                }
                else
                {
                    searchList.Visibility = Visibility.Hidden;
                }

            }
            }
            
            

        }

        public void GoToSelectedElement(object sender, EventArgs e)
        {
           
            searchList.Visibility = Visibility.Hidden;
            searchText.Text = null;
            ListView searchedList = sender as ListView;
            Element searchedElement = searchedList.SelectedItem as Element;
            if (Paths.CheckElement(searchedElement.Tag))
            {
                RefreshFileBrowser();
                Paths.ShowCurrentPath(pathControl, Paths.CurrentPath);
            }


        }

        public void TypeElement (object sender, MouseButtonEventArgs e)
        {
            searchText.Text = null;
        }
     
        public void ContextMenuExplorer (object sender, RoutedEventArgs e)
        {
            ListView list = sender as ListView;
            ElementOfDirectory element = list.SelectedItem as ElementOfDirectory;
            if(element!= null)
            {
                MessageBox.Show("tekst");
            }
        }
        public void RefreshView(object sender, RoutedEventArgs e)
        {
            RefreshFileBrowser();
        }

        
    }

}
