using System.Text.Json;
using System.Collections.Generic;
using AppService.Tables;
using Vectork.Utilities;
using AppService;
using System.Net.WebSockets;
using AppService.Tables.Enums;

namespace AppClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("----CLIENT----");
            var client = new SocketsClient("localhost", 4000);
            client.DataReceived += OnDataReceived;
            client.Connect();


            //var requestGet = new AppService.RequestGet()
            //{
            //    PageIndex = 1,
            //    PageSize = 100,
            //};

            //var requestGetJson = JsonSerializer.Serialize(requestGet);

            //var request = new AppService.Request()
            //{
            //    EntityName = "Book",
            //    MethodName = "Get",
            //    InnerRequestJson = requestGetJson
            //};

            //var requestJsonGet = JsonSerializer.Serialize(request);


            ////////////////////////////////////////////

            //var author = new AppService.Tables.Person()
            //{
            //    Name = "J.K. Rowling",
            //    Rut = "20194234-3",
            //};

            //var editorial = new AppService.Tables.Enterprise()
            //{
            //    Name = "Bloomsbury Publishing",
            //    Rut = "200.233.123-1",
            //};

            //var book = new AppService.Tables.Book()
            //{
            //    Name = "Baldor",
            //    Type = SpecialtyType.Maths,
            //    Pages = 100,
            //    PublicationDate = new DateTime(2012, 05, 15),
            //    OnlineAvailable = true,
            //    Author = author,
            //    Editorial = editorial
            //};

            //var itemBook = JsonSerializer.Serialize(book);

            //var requestSave = new AppService.RequestSave()
            //{
            //    ItemJson = itemBook
            //};

            //var requestSaveJson = JsonSerializer.Serialize(requestSave);

            //var request = new AppService.Request()
            //{
            //    EntityName = "Book",
            //    MethodName = "Save",
            //    InnerRequestJson = requestSaveJson
            //};

            //var requestJsonSave = JsonSerializer.Serialize(request);

            ////////////////////////////////////////////////////

            var requestDelete = new AppService.RequestDelete()
            {
                Id = 14,
            };

            var requestDeleteJson = JsonSerializer.Serialize(requestDelete);

            var request = new AppService.Request()
            {
                EntityName = "Book",
                MethodName = "Delete",
                InnerRequestJson = requestDeleteJson
            };
            var requestJsonDelete = JsonSerializer.Serialize(request);

            client.Send(requestJsonDelete);

            //client.Send(requestJsonSave);


            // client.Send(requestJsonGet);


            Console.WriteLine("Press any key");
            Console.ReadLine();
        }

        private static void OnDataReceived(object sender, string e)
        {
            try
            {

                Console.Write(e);

                var request = JsonSerializer.Deserialize<AppService.Request>(e);


                //var recordsJson = response.Items["Records"];

                //var books = JsonSerializer.Deserialize<List<AppService.Tables.Book>>(recordsJson.ToString());


                //foreach (var book in books)
                //{
                //    Console.WriteLine("{0} - {1} ({2})", book.Name, book.Author.Name, book.Editorial.Name);
                //}




            }
            catch(JsonException ex)
            {
                Console.WriteLine("Error deserializing JSON: {0}", ex.Message);
            }
            

        }
    }
}