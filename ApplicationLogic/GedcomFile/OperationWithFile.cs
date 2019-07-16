using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.GedcomFile
{
    class OperationWithFile //Различнык операции над файлом (создание, открытие и тд.)
    {
        public static GedcomFileWriter CreateGedFile(string path, string family)//Создаёт файл
        {
            if (File.Exists(path)) throw new IOException("Файл с таким именем уже существует");
            using (FileStream newFile = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                AddHeadInNewFile(newFile, family);
            }
            return OpenFile(path);
        }
        private static void AddHeadInNewFile(FileStream newFile, string family)//Записывает технический заголовок в файл
        {
            string head = $"0 HEAD\n1 SOUR Reunion\n2 VERS V8.0\n2 VERS V8.0\n1 GEDC \n2 VERS 5.5\n1 CHAR UNICODE\n1_Family {family}\n" +
                $"1 _PesonID @I0@\n1_FamilyID @F0@";
            using (StreamWriter writer = new StreamWriter(newFile, System.Text.Encoding.Unicode))
            {
                writer.WriteLine(head);
            }
        }
        public static GedcomFileWriter OpenFile(string path)
        {
            if (!File.Exists(path)) throw new IOException("Файла по данному пути не существует");
            GedcomFileReader.findPersonAndFamilyId(path);
            FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write);
            GedcomFileWriter openedFile = new GedcomFileWriter(fileStream);
            return openedFile;
        }
    }
}
