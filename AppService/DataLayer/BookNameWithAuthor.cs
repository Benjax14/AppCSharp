﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.DataLayer
{
    public class BookNameWithAuthor
    {
        public static Response Get(RequestGet request, AppDbContext dbContext)
        {
            var query = dbContext.Book.AsQueryable();

            query = ApplySorting(query, request);
            query = ApplyFilters(query, request);
            query = ApplyPagination(query, request);

            var records = query.Select(o => new Views.BookNameWithAuthor()
            {
                BookName = o.Name,
                AuthorName = o.Author.Name
            }).ToList();

            var response = new Response();

            response.Items.Add("Records",records);
            return response;

        }


        private static IQueryable<Tables.Book> ApplySorting(IQueryable<Tables.Book> query, RequestGet request)
        {
            if (request.OrderProperty == null)
            {
                request.OrderProperty = nameof(Views.BookNameWithAuthor.BookName);
            }

            switch (request.OrderProperty)
            {

                case nameof(Views.BookNameWithAuthor.Id):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Id);
                    else
                        query = query.OrderByDescending(o => o.Id);
                    break;
                case nameof(Views.BookNameWithAuthor.BookName):
                    if (request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Name);
                    else
                        query = query.OrderByDescending(o => o.Name);
                    break;
                case nameof(Views.BookNameWithAuthor.AuthorName):
                    if(request.OrderDirection == OrderDirection.Ascending)
                        query = query.OrderBy(b => b.Author.Name);
                    else
                        query = query.OrderByDescending(b => b.Author.Name);
                    break;
            }

            return query;
        }

        private static IQueryable<Tables.Book> ApplyPagination(IQueryable<Tables.Book> books, RequestGet request)
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
            return books.Skip(startIndex).Take(pageSize.Value);

        }

        private static IQueryable<Tables.Book> ApplyFilters(IQueryable<Tables.Book> query, RequestGet request)
        {
            if (request.Filters == null) return query;

            foreach (var filter in request.Filters)
            {
                switch (filter.PropertyName)
                {

                    case nameof(Views.BookNameWithAuthor.BookName):
                        if (filter.Comparer == Comparer.Equal)
                            query = query.Where(o => o.Name == filter.Value);
                        else
                            query = query.Where(o => o.Name.Contains(filter.Value));
                        break;
                    case nameof(Views.BookNameWithAuthor.AuthorName):
                        if (filter.Comparer == Comparer.Equal)
                            query = query.Where(o => o.Author.Name == filter.Value);
                        else
                            query = query.Where(o => o.Author.Name.Contains(filter.Value));
                        break;
                }
            }
            return query;
        }
    }
}
