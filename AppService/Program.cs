using AppService.DataLayer;
using AppService.Tables;
using AppService.Tables.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static AppService.DataLayer.Book;
using Vectork.Utilities;
using System.Text.Json;

namespace AppService
{
    class Program
    {
        public static SocketsServer _Server;
        static void Main(string[] args)
        {
            Console.WriteLine("------SERVER------");

            _Server = new SocketsServer("localhost",4000);
            _Server.DataReceived += OnDataReceived;

            _Server.Connect();

            Console.WriteLine("Press any key");
            Console.ReadLine();

        }

        private static void OnDataReceived(object sender, string e)
        {
            var requestGet = JsonSerializer.Deserialize<RequestGet>(e);

            var dbContext = new AppDbContext();
            var response = DataLayer.Book.Get(requestGet, dbContext);

            var responseJson = JsonSerializer.Serialize(response);

            _Server.Send(responseJson);
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