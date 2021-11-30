using System;
using System.Linq;

namespace FileManager
{
    public class QueryParse
    {
        // сокращение строки пути для больших путей
        public string ToShortPath(string path)
        {            
            string[] pathStr = path.Split('\\');            // сплит по символу '\'
            if (pathStr.Length > 3)
            {
                return @$"{pathStr[0]}\...\{pathStr[pathStr.Length - 2]}\{pathStr[pathStr.Length - 1]}";
            }
            else return path;
        }

        // возвращает имя файла или папки
        public string fileOrFolderName(string path)
        {            
            string[] pathStr = path.Split('\\');
            return pathStr[pathStr.Length - 1];
        }

        // возвращает расширение файла
        public string ToExtension(string path, out string fileName)
        {            
            fileName = path.Remove(path.LastIndexOf('.')); 
            return path.Remove(0, path.LastIndexOf('.'));            
        }

        // возвращает команду
        public string ToCommand(string commandString)
        {
            string[] commandStr = commandString.Split(' ');
            return commandStr[0];
        }

        // возвращает путь
        public string ToPath(string commandString, out string pathTo)
        {
            string[] commandStr = commandString.Split(' ');
            if (commandStr.Length > 1)         // проверка для одиночных команд, без пути
            {
                string pathAndPath = commandString.Substring(commandStr[0].Length + 1);
                string path1 = "";
                string path2 = "";
                if (commandStr.Contains("->"))
                {    
                    for (int i = 0; i < pathAndPath.Length; i++)
                    {
                        if ((pathAndPath[i + 1] != '-') && (pathAndPath[i + 2] != '>'))
                        {
                            path1 += pathAndPath[i];
                        }
                        else break;
                    }
                    for (int i = path1.Length + 4; i < pathAndPath.Length; i++)
                    {
                        path2 += pathAndPath[i];
                    }
                    pathTo = path2;
                    return path1;
                }
                else 
                {                    
                    for (int i = 0; i < pathAndPath.Length; i++)
                    {
                        path1 += pathAndPath[i];
                    }
                    pathTo = "";
                    return path1;
                }                
            }
            else
            {
                pathTo = "";
                return "";
            }
        }
    }
}