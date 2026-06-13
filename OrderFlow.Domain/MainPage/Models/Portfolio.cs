using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.MainPage.Models
{
    public class Portfolio
    {
        public Link? ProjectName { get; set; }
        public string? TechnologyName { get; set; }
        public Metodology? Metodology { get; set; }
    }
}
