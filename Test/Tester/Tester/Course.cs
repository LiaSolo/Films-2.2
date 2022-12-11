using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Films_2._2
{
    internal class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Students { get; set; } = new List<User>();
    }
}
