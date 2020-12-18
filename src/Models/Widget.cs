using System;
using System.Linq;

namespace PagingExample.Models{

    public class Widget{

        public Guid Id{get;set;}
        public string Name {get;set;}
        public string PartNumber{get;set;}
        public string Description {get;set;}
        public int Quantity{get;set;}
        public decimal Price {get;set;}
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
}

}