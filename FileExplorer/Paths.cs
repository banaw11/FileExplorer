using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using FileExplorer.Properties;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.Remoting.Messaging;
using System.Linq.Expressions;

namespace FileExplorer
{
    class Paths
    {
        public static  string[] currentPathsFiles;
        public static string[] currentPathsDirectories;
        static string currentPath;
        public static List<Element> elementsThisPC = new List<Element>();
        public static object treeNode;
        public static TreeViewItem currentTreeViewItem = new TreeViewItem();
        public static List<string> previousPaths = new List<string>();
        public static List<string> nextPaths = new List<string>();
        public static List<Element> elements = new List<Element>();
        public static bool searchingDone = false;
        public  static string test = "";
        public Paths()
        {
            
        }
        
        public static string CurrentPath
        {
            get { return currentPath; }

            set
            {
                previousPaths.Add(currentPath);
                currentPath = value;
            }

        }
        

        /// <summary>
        /// Method returning elements from current selected path
        /// </summary>
        private static void GetElementsOfPath()
        {
            try
            {
                
                currentPathsFiles = Directory.GetFiles(CurrentPath);
                currentPathsDirectories = Directory.GetDirectories(CurrentPath);
                MainWindow.elements.Clear();

                foreach (var item in currentPathsDirectories)
                {
                    if(CheckAcessPath(item))
                    {
                         MainWindow.elements.Add(new ElementOfDirectory { Icon = new BitmapImage(new Uri(@"Icons\folder.png",UriKind.Relative)),  Name = new DirectoryInfo(item).Name, Date = new DirectoryInfo(item).LastWriteTime, Type = "directory", Size = null, Path = item });
                    }
                   
                }

                foreach (var item in currentPathsFiles)
                {
                    if(CheckAcessPath(item))
                    {
                        MainWindow.elements.Add(new ElementOfDirectory {Icon = GetIconOfFile(new FileInfo(item).Extension),  Name = Path.GetFileNameWithoutExtension(item), Date = new FileInfo(item).LastWriteTime, Type = new FileInfo(item).Extension, Size = (new FileInfo(item).Length / 1024).ToString() + " KB", Path = item });
                    }
                    
                }

                
            }
            catch (Exception e)
            {

            }
        }

        public static bool CheckElement(string path)
        {
            FileAttributes file = File.GetAttributes(path);
            if(file.HasFlag(FileAttributes.Directory))
            {
               if(CurrentPath != path)
                {
                    CurrentPath = path;
                }
                
                GetElementsOfPath();
                return true;
            }
            else
            {
                
                OpenFile(path);
                return false;
            }
        }
        /// <summary>
        /// Method opening file from selected path.
        /// </summary>
        /// <param name="path">Path of file</param>
        private static void OpenFile(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception e)
            {

            }
        }
        /// <summary>
        /// Method which select Icon to extension of file
        /// </summary>
        /// <param name="extension of file"></param>
        /// <returns></returns>
        private static BitmapImage GetIconOfFile(string extension)
        {
            extension = extension.Trim('.').ToLower();
            if (Resources.ResourceManager.GetObject(extension) is null)
            {
               return new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/file.png", UriKind.Absolute));
            }
            else
            {
                return new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/"+ extension +".png", UriKind.Absolute));
            }

        }

        /// <summary>
        /// Method which return all drives as List
        /// </summary>
        /// <returns>List(string)</returns>
        public static List<string> GetDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> allDrives = new List<string>();
            foreach (var item in drives)
            {
                allDrives.Add(item.Name);
            }

            return allDrives;
            
        }

        /// <summary>
        /// Method loading menu FileExplorer's. Show all drives and directory as TreView
        /// </summary>
        /// <param name="explorerMenu">TreeView</param>
        public static void LoadMenuExplorer(TreeView explorerMenu)
        {
            TreeViewItem thisPc = new TreeViewItem { Header = new Item() { NameT = "ThisPC", Icon = new BitmapImage(new Uri(@"Icons\pc.png", UriKind.Relative)), Tag =null } };
            foreach (var element in GetDrives())
            {
                TreeViewItem item =new TreeViewItem { Header = new Item() {NameT = element, Icon = new BitmapImage(new Uri(@"Icons\drive.png", UriKind.Relative)) }, Tag = element };
                item.Items.Add(treeNode);
                item.Expanded += new RoutedEventHandler(GetChild);
                thisPc.Items.Add(item);
            }
            explorerMenu.Items.Add(thisPc);
        }

        private static void GetChild(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == treeNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string element in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        if(CheckAcessPath(element))
                        {
                            TreeViewItem subitem = new TreeViewItem { Header = new Item() { NameT = new DirectoryInfo(element).Name, Icon = new BitmapImage(new Uri(@"Icons\folder.png", UriKind.Relative)) }, Tag = element };
                            subitem.Items.Add(treeNode);
                            subitem.Expanded += new RoutedEventHandler(GetChild);
                            item.Items.Add(subitem);
                        }
                        
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Method return element of ThisPC view
        /// </summary>
        /// <param name="pcControl">Panel of logicall drives </param>
        /// <param name="homeControl">Panel of user's folders</param>
        public static void ThisPcElements(ItemsControl pcControl, ItemsControl homeControl)
        {
            
            List<Element> thisPc = new List<Element>();
            foreach (var item in GetDrives())
            {
                Element element = new Element { NameE = item, Icon = new BitmapImage(new Uri(@"Icons\drive.png", UriKind.Relative)), Tag = item };
                thisPc.Add(element);
                elementsThisPC.Add(element);
            }
            pcControl.Items.Clear();
            pcControl.ItemsSource = thisPc;
            
            List<Element> home = new List<Element>();
            List<string> acceptedFolders = new List<string>() {"Documents", "Music", "Downloads", "Desktop" ,"Pictures", "Videos"};

            foreach (var item in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)))
            {
                if (acceptedFolders.Contains(new DirectoryInfo(item).Name))
                {
                    Element element = new Element { NameE = new DirectoryInfo(item).Name, Icon = new BitmapImage(new Uri(@"Icons\folder.png", UriKind.Relative)), Tag = item };
                    home.Add(element);
                    elementsThisPC.Add(element);

                }
            }

            homeControl.Items.Clear();
            homeControl.ItemsSource = home;
        }

        /// <summary>
        /// Method to return elements path folders 
        /// </summary>
        /// <param name="pathControl"> ItemsControl </param>
        /// <param name="path">Current Paths</param>
        public static void ShowCurrentPath(ItemsControl pathControl, string path)
        {
            List<ElementOfPath> pathElements = new List<ElementOfPath>();
            string parent,  root = "ThisPC";
            if (path != null)
            {
                if (Directory.GetParent(path) != null)
                {
                    pathElements.Add(new ElementOfPath { Tag=path, NameE = new DirectoryInfo(path).Name, Icon = new BitmapImage(new Uri(@"Icons\right.png", UriKind.Relative)) });
                    while (Directory.GetParent(path) != null)
                    {
                        parent = Directory.GetParent(path).Name;
                        pathElements.Add(new ElementOfPath { Tag= Directory.GetParent(path).ToString(), NameE = parent, Icon = new BitmapImage(new Uri(@"Icons\right.png", UriKind.Relative)) });
                        path = Directory.GetParent(path).ToString();
                    }
                }
                else
                {
                    pathElements.Add(new ElementOfPath { Tag=path,  NameE = new DirectoryInfo(path).Name, Icon = new BitmapImage(new Uri(@"Icons\right.png", UriKind.Relative)) });
                }
            }
            pathElements.Add(new ElementOfPath { Tag = null, NameE = root, Icon = new BitmapImage(new Uri(@"Icons\right.png", UriKind.Relative)) });
            pathElements.Reverse();

            pathControl.Items.Clear();
            foreach (var item in pathElements)
            {   
                pathControl.Items.Add(item);
            }

           
            
        }
       
        public static bool CheckPath(string path)
        {
            if (Directory.Exists(path))
            {
                CheckElement(path);
                return true;
            }
            else
            {   
                return false;
            }
        }

        public static void GetSearchingElement(string name, string path)
        {
            searchingDone = false;
            elements.Clear();
            if(path is null)
            {
                foreach (var drive in GetDrives())
                {
                    currentPathsDirectories = Directory.GetDirectories(drive);
                    currentPathsFiles = Directory.GetFiles(drive);

                    foreach (var item in currentPathsFiles)
                    {
                        if (CheckAcessPath(item))
                        {
                            if (Equals(Path.GetFileNameWithoutExtension(item).ToLower(), name) || item.ToLower().Contains(name))
                            {
                                elements.Add(new Element { NameE = new FileInfo(item).Name, Icon = GetIconOfFile(new FileInfo(item).Extension), Tag = item });
                            }
                        }

                    }
                    foreach (var item in currentPathsDirectories)
                    {
                        if (CheckAcessPath(item))
                        {
                            if (Equals(new DirectoryInfo(item).Name.ToLower(), name) || item.ToLower().Contains(name))
                            {
                                elements.Add(new Element { NameE = new FileInfo(item).Name, Icon = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/folder.png", UriKind.Absolute)), Tag = item });
                            }
                            SearchSubDirectory(name, item);
                        }

                    }
                }
            }
            else
            {
                currentPathsFiles = Directory.GetFiles(path);
                currentPathsDirectories = Directory.GetDirectories(path);
                foreach (var item in currentPathsFiles)
                {
                    if (CheckAcessPath(item))
                    {
                        if (Equals(Path.GetFileNameWithoutExtension(item).ToLower(), name) || item.ToLower().Contains(name))
                        {
                            elements.Add(new Element { NameE = new FileInfo(item).Name, Icon = GetIconOfFile(new FileInfo(item).Extension), Tag = item });
                        }
                    }

                }
                foreach (var item in currentPathsDirectories)
                {
                    if (CheckAcessPath(item))
                    {
                        if (Equals(new DirectoryInfo(item).Name.ToLower(), name) || item.ToLower().Contains(name))
                        {
                            elements.Add(new Element { NameE = new FileInfo(item).Name, Icon = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/folder.png", UriKind.Absolute)), Tag = item });
                        }
                        SearchSubDirectory(name, item);
                    }

                }
            }
            searchingDone = true;
            
        }

        private static void SearchSubDirectory(string name, string path)
        {
            currentPathsFiles = Directory.GetFiles(path);
            currentPathsDirectories = Directory.GetDirectories(path);
                foreach (var item in currentPathsFiles)
                {
                    if(CheckAcessPath(item))
                    {
                        if (Equals(Path.GetFileNameWithoutExtension(item).ToLower(), name) || item.ToLower().Contains(name))
                        {
                            elements.Add(new Element { NameE = new FileInfo(item).Name, Icon = GetIconOfFile(new FileInfo(item).Extension), Tag = item });
                        }
                    }
                    
                }
                foreach (var item in currentPathsDirectories)
                {
                    if(CheckAcessPath(item))
                    {
                        if (Equals(new DirectoryInfo(item).Name.ToLower(), name) || item.ToLower().Contains(name))
                        {
                            elements.Add(new Element { NameE = new FileInfo(item).Name, Icon = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/folder.png", UriKind.Absolute)), Tag = item });
                        }
                        SearchSubDirectory(name, item);
                    }
                    
                }
            
        }

        private static bool CheckAcessPath(string path)
        {
            try
            {
                    FileAttributes file = File.GetAttributes(path);

                    if (file.HasFlag(FileAttributes.Directory) && file.HasFlag(FileAttributes.Hidden) is false)
                    {
                        return true;
                    }
                    else if (file.HasFlag(FileAttributes.Hidden) is false && file.HasFlag(FileAttributes.Directory) is false && file.HasFlag(FileAttributes.Temporary) is false )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (StackOverflowException)
            {
                return false;
            }
         

        }

        public static void ShowFoundedElements()
        {
            foreach (var i in elements)
                    {
                        if(File.GetAttributes(i.Tag) == FileAttributes.Directory)
                        {
                            MainWindow.elements.Add(new ElementOfDirectory { Icon = new BitmapImage(new Uri(@"Icons\folder.png",UriKind.Relative)),  Name = new DirectoryInfo(i.Tag).Name, Date = new DirectoryInfo(i.Tag).LastWriteTime, Type = "directory", Size = null, Path = i.Tag });
                        }
                        else
                        {
                            MainWindow.elements.Add(new ElementOfDirectory {Icon = GetIconOfFile(new FileInfo(i.Tag).Extension),  Name = Path.GetFileNameWithoutExtension(i.Tag), Date = new FileInfo(i.Tag).LastWriteTime, Type = new FileInfo(i.Tag).Extension, Size = (new FileInfo(i.Tag).Length / 1024).ToString() + " KB", Path = i.Tag });
                        }
                    }
        }

        public static void NewElementsOfConMen(MenuItem subMenu)
        {
            List<MenuItem> list = new List<MenuItem>();

            MenuItem subItem = new MenuItem();

            subItem.Header = "Folder";
            subItem.Click += Files.AddNewFolder;
            subItem.Icon = new Image {Source = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/folder.png", UriKind.Absolute)) };
            list.Add(subItem);
            //subMenu.ContextMenu.Items.Add(subItem);
            subMenu.ItemsSource = list;

            //subItem.Header = "Shortcut";
            //subItem.Click += Files.CreateShortCut;
            //subItem.Icon = new Image {Source = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/shortcut.png", UriKind.Absolute)) };


            //subMenu.ContextMenu.Items.Add(subItem);

            //subItem.Header = "Text Document";
            //subItem.Click += Files.AddNewFileTxt;
            //subItem.Icon = new Image {Source = new BitmapImage(new Uri(@"pack://application:,,,/FileExplorer;component/Icons/txt.png", UriKind.Absolute)) };

            //subMenu.ContextMenu.Items.Add(subItem);



        }

    }

    
}
