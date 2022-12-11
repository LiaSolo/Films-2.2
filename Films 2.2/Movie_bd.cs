using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Films_2._2
{
    internal class Movie_bd
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public HashSet<Actor> Actors { get; set; }
        public HashSet<Tag> Tags { get; set; }
        
        public HashSet<Director> Directors { get; set; }
        public double Rating { get; set; }

    }
}
