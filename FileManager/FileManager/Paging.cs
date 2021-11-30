using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FileManager
{
    public class Paging
    {
        private QueryParse commandParse = new QueryParse();

        private int _pageSize = 20;

        private void getPagesSize()
        {
            string dllPath = Assembly.GetExecutingAssembly().Location;
            string path = new FileInfo(dllPath).DirectoryName + "\\config.txt";
            string text = File.ReadAllText(path);
            string[] str = text.Split(' ');
            _pageSize = Convert.ToInt32(str[1]);
        }

        // печать страницы
        public void PrintPageDialog(List<string> list)
        {
            // по дефолту печатаем первую страницу
            int number = 1;

            // получаем размер страницы
            getPagesSize();

            PrintPage(list, number, _pageSize);
            
            // дальше режим диалога
            if (_pageSize < list.Count)
            {                
                while (true)
                {
                    try
                    {
                        string UpOrDown = Console.ReadLine();
                        if ((UpOrDown == "+") && (number <= (int)Math.Ceiling((double)(list.Count / _pageSize))))
                        {                            
                            number++;
                            PrintPage(list, number, _pageSize);
                        }
                        else
                        if ((UpOrDown == "-") && (number > 1))
                        {                            
                            number--;
                            PrintPage(list, number, _pageSize);
                        }
                        if (UpOrDown == "!exit")
                        {
                            Console.Clear();
                            Console.WriteLine("Вы вышли из режима просмотра содержимого папки.\nДоступны основные команды");
                            Console.WriteLine(commandParse.ToShortPath(Directory.GetCurrentDirectory()));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        // метод печати страницы
        private void PrintPage(List<string> list, int number, int pageSize)
        {            
            int beginIndex = number * pageSize - pageSize;
            int endIndex = beginIndex + pageSize;
            int pagesSum = (int)Math.Ceiling((double)(list.Count / pageSize)) + 1;            
            if ((beginIndex <= list.Count) && (endIndex < list.Count) && (beginIndex >= 0))
            {
                Console.Clear();
                Console.WriteLine($"Содержимое папки [{commandParse.ToShortPath(Directory.GetCurrentDirectory())}]:\n\n");
                for (int i = beginIndex; i < endIndex; i++)
                {                    
                    Console.WriteLine(list[i]);
                }
                Console.WriteLine($"\n_______Страница {number} из {pagesSum}_______");                
            }
            else if ((beginIndex <= list.Count) && (endIndex >= list.Count))
            {
                Console.Clear();
                Console.WriteLine($"Содержимое папки [{commandParse.ToShortPath(Directory.GetCurrentDirectory())}]:\n\n");
                for (int i = beginIndex; i < list.Count; i++)
                {                    
                    Console.WriteLine(list[i]);
                }
                Console.WriteLine($"\n_______Страница {number} из {pagesSum}_______");
            }
            if (pagesSum > 1)
            {
                Console.WriteLine("\nДля прокрутки страниц используйте символы + или - ");
                Console.WriteLine("Для выхода из режима просмотра введите !exit");
            }
        }
    }
}