using System.Text.Json;
using Vectork.Utilities;

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

            var requestGet = new AppService.RequestGet()
            {
                PageIndex = 1,
                PageSize = 100,
            };

            var requestGetJson = JsonSerializer.Serialize(requestGet);

            client.Send(requestGetJson);


            Console.WriteLine("Press any key");
            Console.ReadLine();
        }

        private static void OnDataReceived(object? sender, string e)
        {
            var response = JsonSerializer.Deserialize<AppService.Response>(e);
            var recordsJson = response.Items["Records"];
            var books = JsonSerializer.Deserialize<List<AppService.Tables.Book>>(recordsJson.ToString());


            foreach (var book in (List<AppService.Tables.Book>)books)
            {
                Console.WriteLine("{0} - {1} ({2})", book.Name, book.Author.Name, book.Editorial.Name);
            }

        }
    }
}