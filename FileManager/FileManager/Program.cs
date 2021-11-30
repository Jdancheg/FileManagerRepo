using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FileManager
{
    class Program
    {
        static string _path = "";
        static void Main(string[] args)
        {
            ManagerCommand directoryCommand = new ManagerCommand();
            bool isFirstCOmmand = true;
            bool isFileLoaded = false;
            bool isLucky = true;
            // основной цикл

            while (true)
            {
                // вывод справки при запуске приложения
                if (isFirstCOmmand == true)
                {
                    directoryCommand.Command("!help");
                    isFirstCOmmand = false;
                }

                if (isFileLoaded == false)
                {
                    try
                    {
                        string dllPath = Assembly.GetExecutingAssembly().Location;
                        _path = new FileInfo(dllPath).DirectoryName + "\\backup.txt";
                        if (File.Exists(_path))
                        {
                            Console.WriteLine("Попытка возобновить предыдущий сеанс...");                            
                            isFileLoaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Невозможно найти сохраненную директорию");
                        isFileLoaded = true;
                        isLucky = false;
                    }
                    finally
                    {
                        if (isLucky == true)
                        {
                            Console.WriteLine("Предыдущий сеанс успешно возобновлен!");
                            directoryCommand.Command("cd " + File.ReadAllText(_path));                            
                        }
                    }                    
                } 
                
                directoryCommand.Command(Console.ReadLine());
            }
        }

        
    }
}
