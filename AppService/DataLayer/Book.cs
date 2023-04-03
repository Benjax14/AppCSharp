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
            var query = dbContext.Book.Include(b => b.Author).Include(b => b.Editorial).AsQueryable();

            query = ApplySorting(query, request);
            query = ApplyFilters(query, request);
            query = ApplyPagination(query, request);

            var books = query.ToList();

            int totalRecords = books.Count;
            int totalPages = (int)Math.Ceiling((decimal)(totalRecords / request.PageSize));

            var response = new Response();

            response.Items.Add("Records", books);

            var summary = new
            {
                TotalPages = books.Sum(b => b.Pages)
            };

            response.Items.Add("Summary", summary);


            return response;
        }

        private static IQueryable<Tables.Book> ApplySorting(IQueryable<Tables.Book> query, RequestGet request)
        {
            if(request.OrderProperty == null)
            {
                request.OrderProperty = nameof(Tables.Book.Name);
            }

            switch(request.OrderProperty) { 
            
                case nameof(Tables.Book.Id):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Id);
                    else
                        query = query.OrderByDescending(o => o.Id);
                    break;
                case nameof(Tables.Book.Name):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Name);
                    else
                        query = query.OrderByDescending(o => o.Name);
                    break;
                case nameof(Tables.Book.Type):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Type);
                    else
                        query = query.OrderByDescending(o => o.Type);
                    break;
                case nameof(Tables.Book.PublicationDate):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.PublicationDate);
                    else
                        query = query.OrderByDescending(b => b.PublicationDate);
                    break;
                case nameof(Tables.Book.OnlineAvailable):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.OnlineAvailable);
                    else
                        query = query.OrderByDescending(b => b.OnlineAvailable);
                    break;
            }
        
            return query;
        }

        private static IQueryable<Tables.Book> ApplyPagination(IQueryable<Tables.Book> books, RequestGet request)
        {
            int pageIndex = request.PageIndex ?? 1;
            int pageSize = request.PageSize ?? 1;

            return books.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        private static IQueryable<Tables.Book> ApplyFilters(IQueryable<Tables.Book> books, RequestGet request)
        {
            if (request.Filters == null) return books;

            foreach (var filter in request.Filters)
            {
                switch (filter.PropertyName) {

                    case nameof(Tables.Book.Name):
                        if(filter.Comparer == Comparer.Equal)
                            books = books.Where(o => o.Name == filter.Value);
                        else
                            books = books.Where(o => o.Name.Contains(filter.Value));
                        break;
                    case nameof(Tables.Book.PublicationDate):

                        var date = DateTime.Parse(filter.Value);

                        switch (filter.Comparer)
                        {
                            case Comparer.Equal:
                                books = books.Where(o => o.PublicationDate == date);
                                break;
                            case Comparer.NotEqual:
                                books = books.Where(o => o.PublicationDate != date);
                                break;
                            case Comparer.Greater:
                                books = books.Where(o => o.PublicationDate > date);
                                break;  
                            case Comparer.GreaterOrEqual:
                                books = books.Where(o => o.PublicationDate >= date);
                                break;
                            case Comparer.Lower:
                                books = books.Where(o => o.PublicationDate < date);
                                break;
                            case Comparer.LowerOrEqual:
                                books = books.Where(o => o.PublicationDate <= date);
                                break;
                            default:
                                break;
                        }
                        break;
                    case nameof(Tables.Book.OnlineAvailable):

                        if (filter.Equals(true))
                            books = books.Where(o => o.OnlineAvailable == true);
                        else
                            books = books.Where(o => o.OnlineAvailable == false);

                        break;
                    case nameof(Tables.Book.Type):

                        if (filter.Comparer == Comparer.Equal)
                        {
                            Type typeFilter = Type.GetType(filter.Value);
                            if (typeFilter != null && typeFilter.IsEnum) 
                                books = books.Where(o => o.Type.GetType() == typeFilter);
                            else
                                return books;
                        }

                        break;
                    case nameof(Tables.Book.Pages):

                        var page = int.Parse(filter.Value);

                        switch (filter.Comparer)
                        {

                            case Comparer.Equal:
                                books = books.Where(o => o.Pages == page);
                                break;
                            case Comparer.NotEqual:
                                books = books.Where(o => o.Pages != page);
                                break;
                            case Comparer.Greater:
                                books = books.Where(o => o.Pages > page);
                                break;
                            case Comparer.GreaterOrEqual:
                                books = books.Where(o => o.Pages >= page);
                                break;
                            case Comparer.Lower:
                                books = books.Where(o => o.Pages < page);
                                break;
                            case Comparer.LowerOrEqual:
                                books = books.Where(o => o.Pages <= page);
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
