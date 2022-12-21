using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShitBlazor.Data
{
    public class Tag
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public HashSet<Movie_bd> Movies { get; set; }
    }
}
