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
            //ApplicationContext db = new ApplicationContext();
            ApplicationContext2 db2 = new ApplicationContext2();

            db2.Database.EnsureDeleted();
            db2.Database.EnsureCreated();

            //User tom = new User { Name = "Tom", Age = 12 };
            //User alice = new User { Name = "Alice", Age = 10 };
            //User bob = new User { Name = "Bob", Age = 13 };
            //User monika = new User { Name = "Monika", Age = 15 };
            //db.Students.AddRange(tom, alice, bob, monika);


            //Course algorithms = new Course { Name = "Алгоритмы" };
            //Course basics = new Course { Name = "Основы программирования" };
            //db.Courses.AddRange(algorithms, basics);

            //tom.Courses.Add(algorithms);
            //tom.Courses.Add(basics);
            //algorithms.Students.Add(alice);
            //basics.Students.Add(bob);
            //algorithms.Students.Add(monika);

            ////tom1.Friends.Add(alice2);
            ////tom1.Friends.Add(bob2);
            ////bob1.Friends.Add(alice2);

            //db.SaveChanges();


            User1 tom1 = new User1 { Name = "Tom"};
            User1 alice1 = new User1 { Name = "Alice" };
            User1 bob1 = new User1 { Name = "Bob"};
            User1 monika1 = new User1 { Name = "Monika"};
            db2.Students1.AddRange(tom1, alice1, bob1, monika1);

            //var T = db.Students
            //.Where(m => m.Name == "Tom").ToList();

            //var A = db.Students
            //.Where(m => m.Name == "Alice").ToList();

            //var B = db.Students
            //.Where(m => m.Name == "Bob").ToList();


            //User Tom = T[0];
            //User Alice = A[0];
            //User Bob = B[0];

            List<string> tf = new List<string>();
            tf.Add(alice1.Name);
            tf.Add(bob1.Name);

            SetFriends sf = new SetFriends { Friends = tf };
            db2.Friends.Add(sf);
            tom1.Friends = sf;

            db2.SaveChanges();


            //using (ApplicationContext db = new ApplicationContext())
            //{


            //    //Tom.Friends.Add(Alice);
            //    //Tom.Friends.Add(Bob);
            //    //Bob.Friends.Add(Alice);

            //    foreach(var f in Tom.Friends)
            //    {
            //        Console.WriteLine(f.Name);
            //    }

            //    //db.SaveChanges();

            //}
        }

    }
}
