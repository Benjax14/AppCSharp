using AppService.Tables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Views
{
    public class BookCountBySpeciality
    {
        public int Id { get; set; }
        public SpecialtyType Speciality { get; set; }
        public int BookCount { get; set; }
    }
}
