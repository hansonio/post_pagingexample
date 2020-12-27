using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PagingExample.Data;
using PagingExample.Models;
using Faker;
using PagingExample.Configuration;

namespace PagingExample.Pages
{
    public class WidgetsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public WidgetsModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Sort = "name";
        }

        public PaginatedList<Widget> Widgets{get;set;}

        [BindProperty(SupportsGet = true, Name = "p")]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public SortDirection Direction { get; set; }

         public Dictionary<string, string> LinkData =>
            new()
            {
                {"filter", Filter},
                {"p", Widgets.CurrentPage.ToString()},
                {"sort", Sort},
                {"direction", Direction.ToString()}
            };

        public async Task<IActionResult> OnGetAsync()
        {
            Widgets = await _dbContext.Widgets
                                .Filter(Filter)
                                .OrderBy(Sort, Direction)
                                .ToPagedList(PageIndex, AppConfig.PageSize);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(){

            for(int i = 0; i < 100; i++){
                var widget = new Widget(){
                    Id = Guid.NewGuid(),
                    Name = string.Join(" ",Faker.Lorem.Words( (i % 2) + 1)),
                    Description = Faker.Lorem.Sentence(),
                    PartNumber = Faker.Identification.UsPassportNumber(),
                    Quantity = Faker.RandomNumber.Next(0, 10000),
                    Price = decimal.ToDouble(Faker.Finance.Coupon())
                };

                await _dbContext.Widgets.AddAsync(widget);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToPage("./Widgets");
        }

        public SortDirection GetNextSortDirection(string name, SortDirection defaultOrder){
            if(Sort?.ToLower() != name?.ToLower()){
                return defaultOrder;
            }


            return Direction == SortDirection.Asc? SortDirection.Desc: SortDirection.Asc;
        }
    }
}
