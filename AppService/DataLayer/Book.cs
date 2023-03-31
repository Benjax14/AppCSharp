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
        public static Response Get(RequestGet request, AppDbContext dbContext)
        {
            var books = dbContext.Book.Include(b => b.Author).Include(b => b.Editorial).ToList();

            books = ApplySorting(books, request);
            books = ApplyPagination(books, request);
            books = ApplyFilters(books, request);

            var response = new Response();

            response.Items.Add("Records", books);

            return response;
        }

        private static List<Tables.Book> ApplySorting(List<Tables.Book> books, RequestGet request)
        {
            if(request.OrderProperty == null)
            {
                request.OrderProperty = nameof(Tables.Book.Name);
            }

            switch(request.OrderProperty) { 
            
                case nameof(Tables.Book.Id):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        books = books.OrderBy(b => b.Id).ToList();
                    else
                        books = books.OrderByDescending(o => o.Id).ToList();
                    break;
                case nameof(Tables.Book.Name):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        books = books.OrderBy(b => b.Name).ToList();
                    else
                        books = books.OrderByDescending(o => o.Name).ToList();
                    break;
                case nameof(Tables.Book.Type):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        books = books.OrderBy(b => b.Type).ToList();
                    else
                        books = books.OrderByDescending(o => o.Type).ToList();
                    break;
                case nameof(Tables.Book.PublicationDate):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        books = books.OrderBy(b => b.PublicationDate).ToList();
                    else
                        books = books.OrderByDescending(b => b.PublicationDate).ToList();
                    break;
                case nameof(Tables.Book.OnlineAvailable):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        books = books.OrderBy(b => b.OnlineAvailable).ToList();
                    else
                        books = books.OrderByDescending(b => b.OnlineAvailable).ToList();
                    break;
            }
        
            return books;
        }

        private static List<Tables.Book> ApplyPagination(List<Tables.Book> books, RequestGet request)
        {
            int pageIndex = (int)request.PageIndex;
            int pageSize = (int)request.PageSize;

            return books.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        private static List<Tables.Book> ApplyFilters(List<Tables.Book> books, RequestGet request)
        {
            if (request.Filters == null) return books;

            foreach (var filter in request.Filters)
            {
                switch (filter.PropertyName) {

                    case nameof(Tables.Book.Name):
                        if(filter.Comparer == Comparer.Equal)
                            books = books.Where(o => o.Name == filter.Value).ToList();
                        else
                            books = books.Where(o => o.Name.Contains(filter.Value)).ToList();
                        break;
                    case nameof(Tables.Book.PublicationDate):

                        var date = DateTime.Parse(filter.Value);

                        switch (filter.Comparer)
                        {
                            case Comparer.Equal:
                                books = books.Where(o => o.PublicationDate == date).ToList();
                                break;
                            case Comparer.NotEqual:
                                books = books.Where(o => o.PublicationDate != date).ToList();
                                break;
                            case Comparer.Greater:
                                books = books.Where(o => o.PublicationDate > date).ToList();
                                break;
                            case Comparer.GreaterOrEqual:
                                break;
                            case Comparer.Lower:
                                break;
                            case Comparer.LowerOrEqual:
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            return books;
        }

        public static Response Save(RequestSave request, AppDbContext dbContext)
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

        public static Response Delete(RequestDelete request, AppDbContext dbContext)
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
