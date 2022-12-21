using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitBlazor.Data
{
    internal class Movie2
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Top10 Top { get; set; }
    }
}
