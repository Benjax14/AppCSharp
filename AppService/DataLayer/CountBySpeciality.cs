using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AppService.DataLayer
{
    public class CountBySpeciality 
    {

        public static Response Get(RequestGet request, AppDbContext dbContext)
        {
            var books = dbContext.Book.ToList();

            books = ApplySorting(books, request);
            books = ApplyFilters(books, request);
            books = ApplyPagination(books, request);

            //Para usar variables que no referencia una atributo de una tabla, es mejor ocupar un select en donde creo la instancia (objeto) y lo listo

            var group = books.GroupBy(x => x.Type).Select(g => new Views.CountBySpeciality
            {
                Speciality = g.Key,
                CountSpeciality = g.Count()

            }).ToList();

            var response = new Response();

            response.Items.Add("Records", group);

            return response;


        }

        private static List<Tables.Book> ApplySorting(List<Tables.Book> query, RequestGet request)
        {
            if (request.OrderProperty == null)
            {
                request.OrderProperty = nameof(Views.CountBySpeciality.Id);
            }

            switch (request.OrderProperty)
            {

                case nameof(Views.CountBySpeciality.Id):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Id).ToList();
                    else
                        query = query.OrderByDescending(o => o.Id).ToList();
                    break;
                case nameof(Views.CountBySpeciality.Speciality):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Type).ToList();
                    else
                        query = query.OrderByDescending(o => o.Type).ToList();
                    break;
                case nameof(Views.CountBySpeciality.CountSpeciality):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.PublicationDate).ToList();
                    else
                        query = query.OrderByDescending(b => b.PublicationDate).ToList();
                    break;
            }

            return query;
        }

        private static List<Tables.Book> ApplyPagination(List<Tables.Book> books, RequestGet request)
        {
            int pageIndex = request.PageIndex ?? 1;
            int pageSize = request.PageSize ?? 1;

            return books.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        private static List<Tables.Book> ApplyFilters(List<Tables.Book> books, RequestGet request)
        {
            if (request.Filters == null) return books;

            foreach (var filter in request.Filters)
            {
                switch (filter.PropertyName)
                {

                    case nameof(Views.CountBySpeciality.Speciality):

                        if (filter.Comparer == Comparer.Equal)
                        {
                            Type typeFilter = Type.GetType(filter.Value);
                            if (typeFilter != null && typeFilter.IsEnum)
                                books = books.Where(o => o.Type.GetType() == typeFilter).ToList();
                            else
                                return books;
                        }

                        break;

                }
            }
            return books;
        }

        public static Response Delete(RequestDelete request, AppDbContext dbContext)
        {
            try
            {
                var item = dbContext.Book.FirstOrDefault(b => b.Id == request.Id);

                dbContext.Book.Remove(item);
                dbContext.SaveChanges();

                return new Response { Items = { { "Result", "OK" } } };

            }
            catch (Exception ex)
            {
                return new Response { Items = { { "NotOK", ex.Message } } };
            }
        }

    }

}
