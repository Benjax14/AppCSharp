using AppService.DataLayer;
using AppService.Tables;
using AppService.Tables.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static AppService.DataLayer.Book;
using Vectork.Utilities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var request = JsonSerializer.Deserialize<Request>(e);
                Response response = null;
                var dbContext = new AppDbContext();


                //switch (request.MethodName)
                //{
                //    case "Get":
                //        var requestGet = JsonSerializer.Deserialize<RequestGet>(request.InnerRequestJson);

                //        if (request.EntityName == "Book")
                //        {
                //            response = AppService.DataLayer.Book.Get(requestGet, dbContext);
                //        }else if(request.EntityName == "BookCountBySpeciality")
                //        {
                //            response = AppService.DataLayer.BookCountBySpeciality.Get(requestGet, dbContext);
                //        }else if (request.EntityName == "BookNameWithAuthor")
                //        {
                //            response = AppService.DataLayer.BookNameWithAuthor.Get(requestGet, dbContext);
                //        }
                //        break;
                //    case "Save":
                //        var requestSave = JsonSerializer.Deserialize<RequestSave>(request.InnerRequestJson);
                //        response = AppService.DataLayer.Book.Save(requestSave, dbContext);
                //        break;
                //    case "Delete":
                //        var requestDelete = JsonSerializer.Deserialize<RequestDelete>(request.InnerRequestJson);
                //        response = AppService.DataLayer.Book.Delete(requestDelete, dbContext);
                //        break;
                //}

                var typeName = $"AppService.DataLayer.{request.EntityName}";

                var type = Type.GetType(typeName);

                if (type != null)
                {
                    var method = type.GetMethod(request.MethodName);

                    if (method != null)
                    {
                        var requestObj = JsonSerializer.Deserialize(request.InnerRequestJson, method.GetParameters()[0].ParameterType);

                        response = (Response)method.Invoke(null, new object[] { requestObj, dbContext });
                    }
                }


                var responseJson = JsonSerializer.Serialize(response);

                _Server.Send(responseJson);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

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