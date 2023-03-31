using AppService.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppService.DataLayer
{
    public class Book
    {
        public static Response GetBook(RequestGet request, AppDbContext dbContext)
        {
            var books = dbContext.Book.Include(b => b.Author).Include(b => b.Editorial).ToList();

            var response = new Response();

            response.Items.Add("Records", books);

            return response;
        }

        public static Response SaveBook(RequestSave request, AppDbContext dbContext)
        {
            try
            {
                var item = JsonSerializer.Deserialize<Tables.Book>(request.ItemJson);
                if(item.Id == 0)
                {
                   dbContext.Book.Add(item);
                }
                else
                {
                    dbContext.Update(item);
                }

                dbContext.SaveChanges();

                return new Response { Items = { { "Result", "OK" } } };
            }
            catch (Exception ex)
            {
                return new Response { Items = { { "NotOK", ex.Message } } };
            }
        }

        public static Response DeleteBook(RequestDelete request, AppDbContext dbContext)
        {
            try
            {
                var item = dbContext.Book.FirstOrDefault(b => b.Id == request.Id);

                dbContext.Book.Remove(item);
                dbContext.SaveChanges();

                return new Response { Items = { { "Result", "OK" } } };

            }catch(Exception ex)
            {
                return new Response { Items = { { "NotOK", ex.Message } } };
            }
        }
    }
}
