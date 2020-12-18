using System.Collections.Generic;
using PagingExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace PagingExample.Pages.Components.Pagination
{
    public class PaginationViewComponent: ViewComponent
    {
        public PaginationViewComponent() { }
        public IViewComponentResult Invoke(IPaginatedList values, Dictionary<string,string> routeData = null)
        {
            return View("Default", new PaginationViewModel(){List = values, RouteData = routeData});
        }
    }

    public class PaginationViewModel
    {
        public IPaginatedList List { get; set; }
        public Dictionary<string, string> RouteData { get; set; }
    }
}