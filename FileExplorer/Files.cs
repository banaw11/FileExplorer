using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExplorer
{
    
    public class Files
    {
        public static string copiedElement;
        public static string placePasteElement= Paths.CurrentPath;
        public static bool flagCopyCut = true;
        public static bool flag = false;
        public static void AddNewFolder(ListView view)
        {
            MainWindow.elements.Insert(0, new ElementOfDirectory { Name = new TextBox { Text = "New Folder" }, Date = DateTime.Now, Icon = new BitmapImage(new Uri(@"Icons\folder.png", UriKind.Relative)), Type="directory", Value=true });
            
            view.Items.Clear();
            foreach (var item in MainWindow.elements)
            {
                view.Items.Add(item);
            }
            view.SelectedItem = view.Items[0];
            MainWindow.flag = true;

        }
        public static void AddNewFileTxt(ListView view)
        {
            MainWindow.elements.Insert(0, new ElementOfDirectory { Name = new TextBox { Text = "New Txt Doxument" }, Date = DateTime.Now, Icon = new BitmapImage(new Uri(@"Icons\txt.png", UriKind.Relative)), Type = ".txt", Value = true });

            view.Items.Clear();
            foreach (var item in MainWindow.elements)
            {
                view.Items.Add(item);
            }
            view.SelectedItem = view.Items[0];
            MainWindow.flag = true;

        }
        public static void CreateShortCut(ListView view)
        {

        }

        public static void RenameItem(ListView view, string name)
        {
            ElementOfDirectory item = view.SelectedItem as ElementOfDirectory;
            string oldName = item.Name.Text;
            //string type = item.Type;

            item.Name.Text = name;
            if (MainWindow.flag)
            {
                if (SaveChange(name, item.Type) is false) MessageBox.Show("Error");
            }
            else
            {
                if(item.Type == "directory")
                {
                    try
                    {
                        Directory.Move(item.Path, Paths.CurrentPath + @"\" + name);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    try
                    {
                        File.Move(item.Path, Paths.CurrentPath + @"\" + name + item.Type);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
           

            
            
        }

        private static bool SaveChange(string name, string type)
        {
            if (type == "directory")
             {
                    if (Directory.Exists(Paths.CurrentPath + @"\" + name))
                    {
                        MessageBox.Show("Directory already exist");
                        return false;
                    }
                    else
                    {
                        Directory.CreateDirectory(Paths.CurrentPath + @"\" + name);
                        return true;
                    }

                }
                else
                {
                    if (File.Exists(Paths.CurrentPath + @"\" + name + type))
                    {
                        MessageBox.Show("File already exist");
                        return false;
                    }
                    else
                    {
                        File.Create(Paths.CurrentPath + @"\" + name + type);
                        return true;
                    }

                }
         }

        public static void DeleteItem(ListView list)
        {
            ElementOfDirectory element = list.SelectedItem as ElementOfDirectory;
            if(element.Type == "directory")
            {
                try
                {
                    Directory.Delete(element.Path, true);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                try
                {
                    File.Delete(element.Path);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }    
            
        public static void Paste(string path)
        {
            if(copiedElement != null)
            {
                if(File.GetAttributes(copiedElement)== FileAttributes.Directory)
                {
                    if (flagCopyCut)
                    {
                        try
                        {
                            string newPath = path + @"\" + new DirectoryInfo(copiedElement).Name;
                            Directory.CreateDirectory(newPath);
                            PasteDirectory(copiedElement, newPath);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    else
                    {
                        Directory.Move(copiedElement, Paths.CurrentPath + @"\" + new DirectoryInfo(copiedElement).Name);
                    }
                    
                }
                else
                {
                    if (flagCopyCut)
                    {
                        try
                        {
                            File.Copy(copiedElement, path + @"\" + Path.GetFileName(copiedElement), true);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                        
                    }
                    else
                    {
                        try
                        {
                            File.Move(copiedElement, path + @"\" + Path.GetFileName(copiedElement));
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                }
            }
        }

        private static void PasteDirectory(string path, string newPath)
        {
            string[] files= Directory.GetFiles(path);
            string[] directories = Directory.GetDirectories(path);
            foreach (var item in files)
            {
                if (Paths.CheckAcessPath(item))
                {
                    File.Copy(item, newPath+@"\"+Path.GetFileName(item), true);
                }

            }
            foreach (var item in directories)
            {
                if (Paths.CheckAcessPath(item))
                {
                    Directory.CreateDirectory(newPath + @"\" + new DirectoryInfo(item).Name);
                    PasteDirectory(item, newPath + @"\" + new DirectoryInfo(item).Name);
                }

            }

        }

    }
}
