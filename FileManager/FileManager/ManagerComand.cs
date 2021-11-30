using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileManager
{
    public class ManagerCommand
    {        
        private string _command = "";
        private string _path = "";
        private string _pathTo = "";
        private List<string> _printList = new List<string>();
        private QueryParse _queryParse = new QueryParse();
        private Paging _paging = new Paging();
        private List<string> _treeList = new List<string>();

        private void PrintList(List<string> list)
        {
            foreach (string el in list)
                Console.WriteLine(el);
        }

        public void Command(string commandString)
        {
            _command = _queryParse.ToCommand(commandString);               // парсим строку на команду
            _path = _queryParse.ToPath(commandString, out _pathTo);         // и путь Form -> To
            
            switch (_command)
            {
                case "!help":
                    help();
                    break;
                case "cd": Cd(_path);                                       // перемещение по папкам (+)
                    break;
                case "copyFile":                                            // копирование файла
                    copyFile(_path, _pathTo);
                    break;
                case "copyFFF":                                             // копирование файлов из папки
                    copyFilesFromFolderToFolder(_path, _pathTo);
                    break;
                case "delFolder":                                           // удаление папки
                    deleteFolder(_path);                                          
                    break;
                case "delFile":                                             // удаление файла
                    deleteFile(_path);                                          
                    break;
                case "tree":                                                // дерево на 2 уровня вверх
                    Tree(Directory.GetCurrentDirectory());
                    _paging.PrintPageDialog(_treeList);
                    break;
                case "fi":                                                  // информация о файле
                    Console.WriteLine(fi(_path));
                    break;
                case "di":                                                  // информация о папке
                    Console.WriteLine(di(_path));
                    break;
                case "ls":                                                  // список папок и файлов в текущей папке (+)
                case "list":                    
                        List(Directory.GetCurrentDirectory());
                        _paging.PrintPageDialog(_printList);
                        break;
                case "!clear": Console.Clear();                             // команда очистки консоли                   
                    break;
                default: Console.WriteLine("Неверный формат команды");
                    break;
            }
        }

        // копирование файлов из каталога в новый каталог
        private void copyFilesFromFolderToFolder(string pathFrom, string pathTo)
        {
            try
            {
                Directory.CreateDirectory(pathTo);
                if (Directory.Exists(pathFrom))
                {
                    string[] files = Directory.GetFiles(pathFrom);
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(pathTo, fileName);
                        File.Copy(file, destFile, true);
                    }
                }
                else
                {
                    Console.WriteLine("По указанному пути не существует папки!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // копирование файла
        private void copyFile(string pathFrom, string pathTo)
        {
            try
            {
                if (File.Exists(pathFrom))
                {
                    FileInfo file = new FileInfo(pathFrom);
                    File.Copy(Path.Combine(pathFrom), Path.Combine(pathTo), true);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // удаление папки
        private void deleteFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        File.Delete(file);
                    }
                    foreach (string directory in Directory.GetDirectories(path))
                    {
                        deleteFolder(directory);
                    }
                    Directory.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // удаление файла
        private void deleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // вызов просмотра дерева
        private void Tree(string path)
        {
            try
            {
                List<string> directories = Directory.GetDirectories(path).ToList();
                List<string> files = Directory.GetFiles(path).ToList();
                foreach (string name in directories)
                {
                    //Console.WriteLine(name);
                    _treeList.Add(name);
                    Tree(name, 1);
                }
                foreach (string name in files)
                {
                    //Console.WriteLine(name);
                    _treeList.Add(name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // рекурсия на 2 папки
        private void Tree(string path, int depth)
        {
            if (depth <= 2)
            {
                List<string> directories = Directory.GetDirectories(path).ToList();
                List<string> files = Directory.GetFiles(path).ToList();
                foreach (string name in directories)
                {
                    //Console.WriteLine(name + "\t");
                    _treeList.Add(name);
                    Tree(name, depth + 1);
                }
                foreach (string name in files)
                {
                    //Console.WriteLine(name);
                    _treeList.Add(name);
                }
            }
        }

        // ls
        private void List(string path)
        {
            DirectoryInfo dirList = new DirectoryInfo(path);
            DirectoryInfo[] foldersList = dirList.GetDirectories();
            FileInfo[] filesList = dirList.GetFiles();
            try
            {               
                if (foldersList.Length > 0) _printList.Add("__________Папки:_________");
                foreach (DirectoryInfo folder in foldersList)
                {
                    var message = folder.Name;
                    _printList.Add(message);                    
                }
                if (filesList.Length > 0) _printList.Add("__________Файлы:_________");
                foreach (FileInfo file in filesList)
                {
                    var message = file.Name;
                    _printList.Add(message);
                }
           }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        // перемещение по файловой системе (cd <Путь>)
        private void Cd(string path)
        {            
            Directory.SetCurrentDirectory(path);
            var message = _queryParse.ToShortPath(Directory.GetCurrentDirectory());  // с подрезкой типа C:\...\folder\folder1
            Console.WriteLine(message);
            string dllPath = Assembly.GetExecutingAssembly().Location;
            string txtPath = new FileInfo(dllPath).DirectoryName + "\\backup.txt";
            File.WriteAllText(txtPath, path);   
        }

        // file info
        private string fi(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            string str = $"Информация о файле:\nИмя: {fileInfo.Name} | Расширение: {fileInfo.Extension} | Размер файла: {fileInfo.Length} байт | " +
                $"Создан: {fileInfo.CreationTime} | \nИзменен: {fileInfo.LastWriteTime} | Атрибуты: {fileInfo.Attributes}" +
                $"\nПолный путь: {fileInfo.FullName}";
            return str;
        }
        // dir info
        private string di(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            string str = $"Информация о папке:\nИмя: {dirInfo.Name} | Атрибуты: {dirInfo.Attributes} | " +
                $"\nСоздана: {dirInfo.CreationTime} | Изенена: {dirInfo.CreationTime}";
            return str;
        }

        private void help()
        {
            Console.WriteLine("-------------------------------------------- Основные команды ---------------------------------------------");            
            Console.WriteLine("cd <путь> \t\t\t - перемещение по файловой системе\t");
            Console.WriteLine("ls или list \t\t\t - вывод списка файлов и папок в текущей директории постраничный");
            Console.WriteLine("delFile <путь> \t\t\t - удаление указанного файла");
            Console.WriteLine("delFolder <путь> \t\t - удаление указанной папки");
            Console.WriteLine("copyFile <путь> -> <путь>\t - копирование файла с заменой");
            Console.WriteLine("copyFFF <путь> -> <путь>\t - копирование файлов из папки в другую папку");
            Console.WriteLine("tree \t\t\t\t - просмотр файловой системы из текущей директори постраничный");
            Console.WriteLine("fi <путь> \t\t\t - вывод информации о файле. Путь относительный или полный");
            Console.WriteLine("di <путь> \t\t\t - вывод информации о папке. Путь относительный или полный");
            Console.WriteLine("!clear \t\t\t\t - очистка консоли");
            Console.WriteLine("!help \t\t\t\t - вызов описания команд");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------");
        }
    }
}
