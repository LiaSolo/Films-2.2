using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Films_2._2
{
    public  class BD_worker
    {
        //Название: Morning.
        //Рейтинг: 6.7.
        //Актёры: { Henry, Milla }.
        //Режиссёры: {  Smith;}.
        //Тэги {  romantic; war; }.

        //Название: Hollywood.
        //Рейтинг: 6.7.
        //Актёры: { Milla, Debbie }.
        //Режиссёры: {  Smith; Krause; }.
        //Тэги {  party; romantic; }.

        //          ACTORS
        // Milla - { New Year; Morning }
        // Henry - { The Clansman; Morning }
        // Debbie - { Hollywood }

        //          DIRECTORS
        // Smith - { Morning; Hollywood }
        // Krause - { Hollywood }

        //          TAGS
        // war - { Morning }
        // party - {Hollywood }
        // romantic - { Hollywood; Morning }

        public static void go()
        {
            string[] actors = { "Milla", "Henry", "Debbie" };
            string[] directors = { "Smith", "Krause" };
            string[] tags = { "war", "romantic", "party" };
            string[] movies = { "Morning", "Hollywood" };

            Dictionary<string, Actor> Adict = new Dictionary<string, Actor>();
            foreach (string a in actors)
            {
                Actor actor = new Actor { Name = a };
                Adict.Add(a, actor);
            }

            Dictionary<string, Movie_bd> Mdict = new Dictionary<string, Movie_bd>();
            foreach (string m in movies)
            {
                Movie_bd movie_bd = new Movie_bd { Name = m };
                Mdict.Add(m, movie_bd);
            }

            
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                foreach (string a in actors)
                {
                    Actor actor = new Actor { Name = a };
                    db.Add(actor);
                    Adict.Add(a, actor);
                }

                //foreach (var m in dict)
                //{
                //    Movie_bd movie_bd = new Movie_bd { Name = m.Key };
                //    db.Add(movie_bd);
                //    foreach (var a in m.Value.Actors)
                //    {
                //        movie_bd.Actors.Add(Adict[a]);
                //    }
                //}

                db.SaveChanges();

                //foreach (string d in directors)
                //{
                //    Director director = new Director { Name = d };
                //    db.Add(director);
                //}

                //foreach (string t in tags)
                //{
                //    Tag tag = new Tag { Name = t };
                //    db.Add(tag);
                //}
            }      
        }


        //public static void bd_create_movie(string movie_name)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        db.Database.EnsureDeleted();
        //        db.Database.EnsureCreated();

        //        Movie_bd movie_bd = new Movie_bd { Name = movie_name };
        //        db.Add(movie_bd);

        //        db.SaveChanges();
        //    }

        //    //using (ApplicationContext db = new ApplicationContext())
        //    //{
        //    //    var movies = db.Movies.Include(m => m.Actors).ToList();

        //    //    foreach (var m in movies)
        //    //    {
        //    //        Console.WriteLine($"Movie: {m.Name}");

        //    //        foreach (var a in m.Actors)
        //    //            Console.WriteLine($"Actor: {a.Name}");
        //    //        Console.WriteLine("-------------------");
        //    //    }
        //    //}
        //}

        //public static void bd_create_actor(string  actor_name)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        Actor actor = new Actor { Name = actor_name };
        //        db.Add(actor);

        //        db.SaveChanges();
        //    }
        //}

        //public static void bd_create_director(string director_name)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        Director director = new Director { Name = director_name };
        //        db.Add(director);

        //        db.SaveChanges();
        //    }
        //}

        //public static void bd_create_tag(string tag_name)
        //{
        //    using (ApplicationContext db = new ApplicationContext())
        //    {
        //        Tag tag = new Tag { Name = tag_name };
        //        db.Add(tag);

        //        db.SaveChanges();
        //    }
        //}
    }
}
