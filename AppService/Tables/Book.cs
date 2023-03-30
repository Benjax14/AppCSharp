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
        public string? NameBook { get; set; } //Field
        public SpecialtyType? TypeBook { get; set; }
        public int? PagesCount { get; set; }
        public DateTime? PublicationDate { get; set; }
        public bool? OnlineAvailable { get; set; }
        public Person AuthorBook { get; set; }
        public Enterprise Editorial { get; set; }

        public Book()
        {
        }
        // Constructor
        public Book(string name, SpecialtyType subject, int pages, DateTime date, bool available, Person author, Enterprise editorial)
        {
            NameBook = name;
            TypeBook = subject;
            PagesCount = pages;
            PublicationDate = date;
            OnlineAvailable = available;
            AuthorBook = author;
            Editorial = editorial;
        }
    }
}
