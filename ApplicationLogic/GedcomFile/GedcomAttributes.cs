using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.GedcomFile
{
    static class GedcomAttributes//Содержит атрибуты, используемые в файле
    {
        internal const string PersonId = "_PesonID"; //Атрибут индекса нового человека(последнего кого добавят в файл)
        internal const string FamilyID = "_FamilyID"; //Также как и _PesonID , только для семей.
        internal const string Family = "_Family"; //Атрибут названия семьи
        internal const string PhotoPath = "_PhotoPath";
    }
}
