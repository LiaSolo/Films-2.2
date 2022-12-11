using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Films_2._2
{
    public class Test
    {
        static void Main(string[] args)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // создание и добавление моделей
                User tom = new User { Name = "Tom" };
                User alice = new User { Name = "Alice" };
                User bob = new User { Name = "Bob" };
                db.Students.AddRange(tom, alice, bob);

                Course algorithms = new Course { Name = "Алгоритмы" };
                Course basics = new Course { Name = "Основы программирования" };
                db.Courses.AddRange(algorithms, basics);

                // добавляем к студентам курсы
                tom.Courses.Add(algorithms);
                tom.Courses.Add(basics);
                algorithms.Students.Add(alice);
                basics.Students.Add(bob);

                db.SaveChanges();
            }

            using (ApplicationContext db = new ApplicationContext())
            {
                var courses = db.Courses.Include(c => c.Students).ToList();
                // выводим все курсы
                foreach (var c in courses)
                {
                    Console.WriteLine($"Course: {c.Name}");
                    // выводим всех студентов для данного кура
                    foreach (User s in c.Students)
                        Console.WriteLine($"Name: {s.Name}");
                    Console.WriteLine("-------------------");
                }
            }
        }

    }
}
