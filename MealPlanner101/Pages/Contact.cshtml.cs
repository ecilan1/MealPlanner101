using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner101.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; } = "Page model in C#";

        public void OnGet()
        {
            Message += $"Server time is: {DateTime.Now}";
        }
    }
}
