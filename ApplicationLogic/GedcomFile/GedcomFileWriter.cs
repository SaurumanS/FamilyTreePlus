using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Person = ApplicationLogic.DataTable.Person;
using Family = ApplicationLogic.DataTable.Family;

namespace ApplicationLogic.GedcomFile
{
    public class GedcomFileWriter
    {
        /// <summary>
        /// http://go.inf.ua/Gedcom.htm является эталоном для данного проекта, по всем вопросам заполнения ответ искать на этом ресурсе
        /// ID всегда передаётся в формате @I{ID}@ or @F{ID}@
        /// </summary>
        StreamWriter Writer { get; set; }
        List<string> FileData { get; set; }
        int LastPersonIndex { get; set; }
        public GedcomFileWriter(FileStream file)
        {
            Writer = new StreamWriter(file, System.Text.Encoding.Unicode);
            FileData = GedcomFileReader.ReturnFIleData(file.Name);
        }

        #region Adding
        public void AddPerson(Person newPerson)
        {
            var newGedNote = ReturnGedFormatPerson(newPerson);
            int insertIndex = GedcomFileReader.IndexOf(FileData, @"0 @F\d+@ FAM"); //Индекс первой семьи в файле
            if (insertIndex == -1) FileData.AddRange(newGedNote);
            else FileData.InsertRange(insertIndex, newGedNote);
        }
        public void AddChild(Person child)//Предполагается, что семья уже занесена в файл
        {
            AddPerson(child);
            int parentsFamily = GedcomFileReader.IndexOf(FileData, $"0 {child.Parents} FAM");
            if (parentsFamily == -1) throw new ArgumentException("Каким-то образом семья родителей не была занесена в файл");
            //Если есть жена, значит в семье 2 родителя, добавляем через 3 строчки, иначе если один родитель через 2
            if (FileData[parentsFamily + 2].Contains("WIFE"))
            {
                FileData.Insert(parentsFamily + 3, $"1 CHIL {child.Id}");
            }
            else FileData.Insert(parentsFamily + 2, $"1 CHIL {child.Id}");
        }
        public void AddFamily(Family newFamily)//Если в семье один родитель, то в зависимости от пола ставим null, также передается была ли у них свадьба (ID - @I+ID@), на данный момент не предполагается, что при создании новой семьи она будет иметь детей сразу
        {
            var newGedFamily = ReturnGedFormatFamily(newFamily);
            FileData.AddRange(newGedFamily);
        }
        #endregion

        #region Changing

        public void ChangeInfoAboutPerson(Person changedPerson)// ID - "@I{ID}@" (если изменения связаны с семьями, то добавление записи о семье в файл должно происходить вместе с вызовом этого метода)
        {
            int personIndexStart = GedcomFileReader.IndexOf(FileData, changedPerson.Id);//Индекс, где написан номер человека (самая верхняя строчка)
            int personIndexEnd = GedcomFileReader.IndexOf(FileData, @"^0",personIndexStart+1);//Индекс, который больше на 1 индеса последней записи о человеке
            var changedGedPerson = ReturnGedFormatPerson(changedPerson);
            if (personIndexEnd == -1)//Если -1, то был достигнут конец файла без обнаружения совпадений, следовательно это последняя запись в файле
            {
                FileData.RemoveRange(personIndexStart, FileData.Count - personIndexStart);
                FileData.AddRange(changedGedPerson);
            }
            else
            {
                FileData.RemoveRange(personIndexStart, personIndexEnd - personIndexStart + 1);
                FileData.InsertRange(personIndexStart, changedGedPerson);
            }
        }
        public void ChangeInfoAboutFamily(Family changedFamily)
        {
            var changedGedFamily = ReturnGedFormatFamily(changedFamily);
            int personIndexStart = GedcomFileReader.IndexOf(FileData, changedFamily.ID);//Индекс, где написан номер человека (самая верхняя строчка)
            int personIndexEnd = GedcomFileReader.IndexOf(FileData, @"^0", personIndexStart + 1);//Индекс, который больше на 1 индеса последней записи о человеке
            if (personIndexEnd == -1)//Если -1, то был достигнут конец файла без обнаружения совпадений, следовательно это последняя запись в файле
            {
                FileData.RemoveRange(personIndexStart, FileData.Count - personIndexStart);
                FileData.AddRange(changedGedFamily);
            }
            else
            {
                FileData.RemoveRange(personIndexStart, personIndexEnd - personIndexStart + 1);
                FileData.InsertRange(personIndexStart, changedGedFamily);
            }
        }

        #endregion

        #region Deleting

        //Предполагается, что при удалении персоны все записи в семьях, где он присутствует будут удалены посредством метода ChangeInfoAboutFamily, где он просто будет не упомянут
        //Также предполагается, что 
        public void DeletePerson(Person person)
        {
            if (person.PersonFamilies.Count > 0)
                throw new ArgumentException("К сожалению из-за лени человека, писавшего эту программу, вы сначала должны сделать так, чтобы ни в одной семье в файле не было упоминания этого человека. I'm sorry, really");
            int personStartIndex = GedcomFileReader.IndexOf(FileData, person.Id);
            int personEndIndex = GedcomFileReader.IndexOf(FileData, @"^0",personStartIndex+1);
            if (personEndIndex == -1) FileData.RemoveRange(personStartIndex, FileData.Count - personStartIndex);
            else FileData.RemoveRange(personStartIndex, personEndIndex - personStartIndex + 1);
        }
        public void DeleteFamily(Family family)
        {
            if (family.ChildrenID.Count > 0)
                throw new ArgumentException("В удаляемой семье не может быть детей");
            int personStartIndex = GedcomFileReader.IndexOf(FileData, family.ID);
            int personEndIndex = GedcomFileReader.IndexOf(FileData, @"^0", personStartIndex + 1);
            if (personEndIndex == -1) FileData.RemoveRange(personStartIndex, FileData.Count - personStartIndex);
            else FileData.RemoveRange(personStartIndex, personEndIndex - personStartIndex + 1);
        }
        #endregion
        public List<string> ReturnGedFormatPerson(Person person)//Возвращает строки, которые будут описывать нового человека в файле (Поля добавляются в любом случае, даже если они будут пустые, для простоты их поиска и замены информации)
        {
            List<string> newPersonDescription = new List<string>();
            newPersonDescription.Add($"0 {person.Id} INDI");
            newPersonDescription.Add($"1 {GedcomAttributes.PhotoPath} {person.PathPhoto}");
            string current = $"1 NAME {person.FirstName} {person.Patronymic} /{person.SecondName}/";
            newPersonDescription.Add(current);
            newPersonDescription.Add($"1 SEX {person.Gender}");

            newPersonDescription.Add("1 BIRT");
            newPersonDescription.Add($"2 DATE {person.DateOfBirth}");
            newPersonDescription.Add($"2 PLAC {person.PlaceOfBirth}");

            newPersonDescription.Add($"1 FAMC {person.Parents}");
            int familyIndex = GedcomFileReader.IndexOf(FileData, $"FAM {person.Parents}");


            newPersonDescription.Add("1 DEAT");
            newPersonDescription.Add($"2 DATE {person.DateOfDeath}");
            newPersonDescription.Add($"2 PLAC {person.PlaceOfDeath}");

            int familiesCount = person.PersonFamilies.Count;
            for (int counter = 0; counter < familiesCount; counter++)
                newPersonDescription.Add($"1 FAMS {person.PersonFamilies[counter]}");
            newPersonDescription.Add($"1 NOTE {person.Description}");

            return newPersonDescription;
        }
        //public List<string> ReturnGedFormatPerson(Person person)//Возвращает строки, которые будут описывать нового человека в файле (Здесь поля добавляются, если они не пустые)
        //{
        //    List<string> newPersonDescription = new List<string>();
        //    newPersonDescription.Add($"0 {person.Id} INDI");
        //    if (!string.IsNullOrEmpty(person.PathPhoto)) newPersonDescription.Add($"1 {GedcomAttributes.PhotoPath} {person.PathPhoto}");
        //    string current = $"1 NAME {person.FirstName} {person.Patronymic} /{person.SecondName}/";
        //    newPersonDescription.Add(current);
        //    if (!string.IsNullOrEmpty(person.Gender)) newPersonDescription.Add($"1 SEX {person.Gender}");
        //    if (!string.IsNullOrEmpty(person.DateOfBirth))
        //    {
        //        newPersonDescription.Add("1 BIRT");
        //        newPersonDescription.Add($"2 DATE {person.DateOfBirth}");
        //        if (!string.IsNullOrEmpty(person.PlaceOfBirth)) newPersonDescription.Add($"2 PLAC {person.PlaceOfBirth}");
        //    }
        //    if (!string.IsNullOrEmpty(person.Parents))
        //    {
        //        newPersonDescription.Add($"1 FAMC {person.Parents}");
        //        int familyIndex = GedcomFileReader.IndexOf(FileData,$"FAM {person.Parents}");
        //    }
        //    if (!string.IsNullOrEmpty(person.DateOfDeath))
        //    {
        //        newPersonDescription.Add("1 DEAT");
        //        newPersonDescription.Add($"2 DATE {person.DateOfDeath}");
        //        if (!string.IsNullOrEmpty(person.PlaceOfDeath)) newPersonDescription.Add($"2 PLAC {person.PlaceOfDeath}");
        //    }
        //    int familiesCount = person.PersonFamilies.Count;
        //    for (int counter = 0; counter < familiesCount; counter++)
        //        newPersonDescription.Add($"1 FAMS {person.PersonFamilies[counter]}");
        //    if (!string.IsNullOrEmpty(person.Description)) newPersonDescription.Add($"1 NOTE {person.Description}");

        //    return newPersonDescription;
        //}
        public List<string> ReturnGedFormatFamily(Family family)
        {
            bool husbandIsEmptyOrNull = string.IsNullOrEmpty(family.HusbandID) ? true : false;
            bool wifeIsEmptyOrNull = string.IsNullOrEmpty(family.WifeID) ? true : false;
            if (husbandIsEmptyOrNull && wifeIsEmptyOrNull) throw new ArgumentException("Должен присутствовать хотя бы один родитель");
            List<string> newFamilyDescription = new List<string>();
            newFamilyDescription.Add($"0 {family.ID} FAM");
            if (!husbandIsEmptyOrNull)
            {
                newFamilyDescription.Add($"1 HUSB {family.HusbandID}");
            }
            if (!wifeIsEmptyOrNull)
            {
                newFamilyDescription.Add($"1 WIFE {family.WifeID}");
            }
            for (int counter = 0; counter < family.ChildrenID.Count; counter++)
                newFamilyDescription.Add($"1 CHIL {family.ChildrenID[counter]}");
            if (family.TheyWasMarried) newFamilyDescription.Add($"1 MARR");
            return newFamilyDescription;
        }
    }
}
