using AppService.Tables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AppService.DataLayer
{
    public class BookCountBySpeciality 
    {

        public static Response Get(RequestGet request, AppDbContext dbContext)
        {
            var books = dbContext.Book.ToList();

            var group = books.GroupBy(x => x.Type).Select(g => new Views.BookCountBySpeciality
            {
                Speciality = g.Key,
                BookCount = g.Count()

            }).ToList();

            group = ApplySorting(group, request);
            group = ApplyFilters(group, request);
            group = ApplyPagination(group, request);

            //Para usar variables que no referencia una atributo de una tabla, es mejor ocupar un select en donde creo la instancia (objeto) y lo listo


            var response = new Response();

            response.Items.Add("Records", group);

            return response;


        }

        private static List<Views.BookCountBySpeciality> ApplySorting(List<Views.BookCountBySpeciality> query, RequestGet request)
        {
            if (request.OrderProperty == null)
            {
                request.OrderProperty = nameof(Views.BookCountBySpeciality.Id);
            }

            switch (request.OrderProperty)
            {

                case nameof(Views.BookCountBySpeciality.Id):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Id).ToList();
                    else
                        query = query.OrderByDescending(o => o.Id).ToList();
                    break;
                case nameof(Views.BookCountBySpeciality.Speciality):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Speciality).ToList();
                    else
                        query = query.OrderByDescending(o => o.Speciality).ToList();
                    break;
                case nameof(Views.BookCountBySpeciality.BookCount):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.BookCount).ToList();
                    else
                        query = query.OrderByDescending(o => o.BookCount).ToList();
                    break;
            }

            return query;
        }

        private static List<Views.BookCountBySpeciality> ApplyPagination(List<Views.BookCountBySpeciality> books, RequestGet request)
        {
            if (books == null)
            {
                return books;
            }

            int? pageIndex = request.PageIndex;
            int? pageSize = request.PageSize;

            if (pageIndex == null || pageSize == null)
            {
                return books;
            }

            int startIndex = (pageIndex.Value - 1) * pageSize.Value;
            return books.Skip(startIndex).Take(pageSize.Value).ToList();
        }

        private static List<Views.BookCountBySpeciality> ApplyFilters(List<Views.BookCountBySpeciality> books, RequestGet request)
        {
            if (request.Filters == null) return books;

            foreach (var filter in request.Filters)
            {
                switch (filter.PropertyName)
                {

                    case nameof(Views.BookCountBySpeciality.Speciality):

                        if (filter.PropertyName == nameof(Views.BookCountBySpeciality.Speciality) && filter.Comparer == Comparer.Equal)
                        {
                            var enumValue = filter.Value;


                            switch (enumValue)
                            {
                                case "Maths":
                                    books = books.Where(o => o.Speciality == SpecialtyType.Maths).ToList();
                                    break;
                                case "Literature":
                                    books = books.Where(o => o.Speciality == SpecialtyType.Literature).ToList();
                                    break;
                                case "Chemistry":
                                    books = books.Where(o => o.Speciality == SpecialtyType.Chemistry).ToList();
                                    break;
                                case "Physics":
                                    books = books.Where(o => o.Speciality == SpecialtyType.Physics).ToList();
                                    break;
                            }
                        }

                        break;
                    case nameof(Views.BookCountBySpeciality.BookCount):

                        var value = int.Parse(filter.Value);

                        switch (filter.Comparer)
                        {

                            case Comparer.Equal:
                                books = books.Where(o => o.BookCount == value).ToList();
                                break;
                            case Comparer.NotEqual:
                                books = books.Where(o => o.BookCount != value).ToList();
                                break;
                            case Comparer.Greater:
                                books = books.Where(o => o.BookCount > value).ToList();
                                break;
                            case Comparer.GreaterOrEqual:
                                books = books.Where(o => o.BookCount >= value).ToList();
                                break;
                            case Comparer.Lower:
                                books = books.Where(o => o.BookCount < value).ToList();
                                break;
                            case Comparer.LowerOrEqual:
                                books = books.Where(o => o.BookCount <= value).ToList();
                                break;

                        }
                        break;

                }
            }
            return books;
        }

    }

}
