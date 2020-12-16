using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PagingExample.Configuration;

namespace PagingExample.Models
{
    public interface IPaginatedList:IList
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        int TotalItems { get;  }
        bool HasPrevious { get; }
        bool HasNext { get; }
    }

    public class PaginatedList<T> : List<T>, IPaginatedList where T:class
    {
        public PaginatedList(){}
        
        public PaginatedList(List<T> items, int count, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int currentPage, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip( (currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            
            return new PaginatedList<T>(items, count, currentPage, pageSize);
        }
    }

    public static class PaginationExtensions
    {
        public static async Task<PaginatedList<T>> ToPagedList<T>(this IQueryable<T> source, int page = 1, int pageSize = AppConfig.PageSize) where T:class
        {
            var count = await source.CountAsync();
            var items = await source.Skip( (page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            return new PaginatedList<T>(items, count, page, pageSize);
        }

        public static async Task<PaginatedList<TResult>> ToPagedList<T, TResult>(this IQueryable<T> source, 
            Func<T, TResult> selectFunc, 
            int page = 1, 
            int pageSize = AppConfig.PageSize)where TResult:class where T:class
        {
            var count = await source.CountAsync();
            var items = await source.Skip( (page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<TResult>(items.Select(selectFunc).ToList(), count, page, pageSize);
        }
    }
}