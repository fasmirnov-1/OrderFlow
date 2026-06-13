using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFlow.Domain.MainPage.Models
{
    public class Feedback
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Text { get; set; }
        public Raiting Raiting { get; set; }
    }
}
