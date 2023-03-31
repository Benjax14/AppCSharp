using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.DataLayer
{
    public class Book
    {
        public static Response GetBook(RequestGet request)
        {
            var dbContext = new AppDbContext();
            var books = dbContext.Book.Include(b => b.Author).Include(b => b.Editorial).ToList();

            var response = new Response
            {
                Books = books,
            };

            return response;
        }
    }
}
