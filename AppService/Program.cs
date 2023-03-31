using AppService.DataLayer;
using AppService.Tables;
using AppService.Tables.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppService
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var dbContext = new AppDbContext();
            //PoblateDatabase(dbContext);

            var request = new RequestGet()
            {
                OrderDirection = OrderDirection.Descending,
            };

            var response = DataLayer.Book.Get(request,dbContext);

            foreach (var book in (List<Tables.Book>)response.Items["Records"])
            {
                Console.WriteLine("{0} - {1} ({2})", book.Name, book.Author.Name, book.Editorial.Name);
            }

            Console.ReadLine();
        }

        public static void PoblateDatabase(AppDbContext dBContext)
        {
            dBContext.Person.Add(new Person("Roberto Maturana", "20.444.222-3"));
            dBContext.Enterprise.Add(new Enterprise("Salo", "44.231.222-1"));

            dBContext.SaveChanges();

            dBContext.Book.Add(new Tables.Book() { Name = "Baldor", Type = SpecialtyType.Maths, Pages = 100, PublicationDate = new DateTime(2012, 05, 15), OnlineAvailable= true, Author = dBContext.Person.Find(1), Editorial = dBContext.Enterprise.Find(1) });
            dBContext.Book.Add(new Tables.Book() { Name = "The Hitchhiker's Guide to the Galaxy", Type = SpecialtyType.Literature, Pages = 208, PublicationDate = new DateTime(1979, 10, 12), OnlineAvailable = false, Author = dBContext.Person.Find(1), Editorial = dBContext.Enterprise.Find(1) });
            dBContext.Book.Add(new Tables.Book() { Name = "Principles of Physics", Type = SpecialtyType.Physics, Pages = 450, PublicationDate = new DateTime(2005, 10, 01), OnlineAvailable = true, Author = dBContext.Person.Find(1), Editorial = dBContext.Enterprise.Find(1) });

            dBContext.SaveChanges();
        }
    }
}