using System;
using System.Linq;
using System.Linq.Expressions;
using PagingExample.Data;

namespace PagingExample.Models{

    public class Widget{

        public Guid Id{get;set;}
        public string Name {get;set;}
        public string PartNumber{get;set;}
        public string Description {get;set;}
        public int Quantity{get;set;}
        public double Price {get;set;}
    }

public static class WidgetQueryExtensions{

    public static IQueryable<Widget> Filter(this IQueryable<Widget> query, string filter){
        if(string.IsNullOrWhiteSpace(filter)){
            return query;
        }

        filter = filter.ToLower();

        return query.Where(x =>
            x.Name.ToLower().Contains(filter)
            || x.PartNumber.ToLower().Contains(filter)
            || x.Description.ToLower().Contains(filter)
        );
    }

    public static IQueryable<Widget> OrderBy(this IQueryable<Widget> query, string name, SortDirection direction = SortDirection.Asc){

        Expression<Func<Widget, object>> exp = name?.ToLower() switch
        {
            "part" => x => x.PartNumber,
            "description" => x => x.Description,
            "quantity" => x => x.Quantity,
            "price" => x => x.Price,
            "quantityprice" => x => x.Quantity + x.Price,
            _ => x => x.Name
        };

        return direction == SortDirection.Asc ? query.OrderBy(exp) : query.OrderByDescending(exp);
    }
  }

}