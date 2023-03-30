using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppService.Tables
{
    public class Book
    {
        public int Id {  get; set; }
        public string Name { get; set; } //Field
        public SpecialtyType Type { get; set; }
        public int Pages { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool OnlineAvailable { get; set; }
        public Person Author { get; set; }
        public int AuthorId { get; set; }
        public Enterprise Editorial { get; set; }
        public int EditorialId { get; set; }

        public Book()
        {
        }
        // Constructor
        public Book(string name, SpecialtyType subject, int pages, DateTime date, bool available, Person author, Enterprise editorial)
        {
            Name = name;
            Type = subject;
            Pages = pages;
            PublicationDate = date;
            OnlineAvailable = available;
            Author = author;
            Editorial = editorial;
        }
    }
}
