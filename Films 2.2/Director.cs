using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Films_2._2
{
    internal class Director
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public HashSet<Movie_bd> Movies { get; set; }
    }
}
