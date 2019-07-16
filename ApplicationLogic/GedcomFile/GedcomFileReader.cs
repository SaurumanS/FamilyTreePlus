using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationLogic.GedcomFile
{
    public class GedcomFileReader
    {

        internal static List<string> ReturnFIleData(string path)
        {
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path, System.Text.Encoding.Unicode);
                string currentString;
                List<string> fileData = new List<string>();
                while (!string.IsNullOrEmpty(currentString = reader.ReadLine()))
                {
                    fileData.Add(currentString);
                }
                return fileData;
            }
            else throw new IOException("Файла по данному пути не существует");
        }

        
        #region ReturnId
        //Производится поиск полей, которые содержат информацию об id, которые будут иметь последний добавленный человек или семья
        //Числа записываются в свойства ниже
        //Предполагается, что обращение к свойствам будут происходить при непосредственном добавлении человека или семьи
        //Данные числа могут только инкрементироваться, то есть при удалении человека или семьи они не декрементируются
        //Предполагается, что номера будут перезаписаны в файл при его непосредственном сохренении, а не срузу же при добавлении
        private static StreamReader currentFile;
        private static string currentPath { get; set; } = "";
        public static int currentPersonId { get; private set; }
        public static int currentFamilyId { get; private set; }
        internal static void findPersonAndFamilyId(string path)
        {
            if (path == currentPath) throw new ArgumentException("Попытка повторно передать один и тот же файл");
            if (!File.Exists(path)) throw new IOException("Файла по данному пути не существует");
            currentPath = path;
            currentFile = new StreamReader(path, System.Text.Encoding.Unicode);
            while (true)
            {
                string currentString = currentFile.ReadLine();
                if (currentString.Contains(GedcomAttributes.PersonId))
                {
                    Regex regex = new Regex(@"\@\I\d+\@");
                    string numberStr = regex.Match(currentString).Value;
                    currentPersonId = int.Parse(numberStr.Trim('@', 'I'));
                }
                else if (currentString.Contains(GedcomAttributes.FamilyID))
                {
                    Regex regex = new Regex(@"\@\F\d+\@");
                    string numberStr = regex.Match(currentString).Value;
                    currentFamilyId = int.Parse(numberStr.Trim('@', 'F'));
                }
            }
        }
        internal static string ReturnNewPersonNumber()
        {
            if (string.IsNullOrEmpty(currentPath)) throw new Exception("Ошибка: Файл не был добавлен в методе findPersonAndFamilyId");

            string result = $"@I{currentPersonId}@" +
                $"";
            currentPersonId++;
            return result;
        }
        internal static string ReturnNewFamilyNumber()
        {
            if (string.IsNullOrEmpty(currentPath)) throw new Exception("Ошибка: Файл не был добавлен в методе findPersonAndFamilyId");

            string result = $"@I{currentFamilyId}@";
            currentFamilyId++;
            return result;
        }
        #endregion

        #region Finding

        internal static int IndexOf(List<string> file,string pattern) //Поиск первого вхождения входной строки
        {
            int counter = 0;
            int result = -1;
            Regex regex = new Regex(pattern);
            while(counter<file.Count)
            {
                if (regex.IsMatch(file[counter]))
                {
                    result=counter;
                    break;
                }
                counter++;
            }
            return result;
        }
        internal static int IndexOf(List<string> file, string pattern, int startWith) //Поиск первого вхождения входной строки
        {
            if (startWith >= file.Count) throw new ArgumentException("Начальный индекс больше длины списка");
            int counter = startWith;
            int result = -1;
            Regex regex = new Regex(pattern);
            while (counter < file.Count)
            {
                if (regex.IsMatch(file[counter]))
                {
                    result = counter;
                    break;
                }
                counter++;
            }
            return result;
        }

        #endregion
    }
}
