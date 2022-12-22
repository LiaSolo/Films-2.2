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
        public string Rating { get; set; }

        public string[]? Top10 { get; set; }

        public static double Relevance(Movie_bd movie1, Movie_bd movie2)
        {
            HashSet<Actor> actors1 = movie1.Actors;
            HashSet<Actor> actors2 = movie2.Actors;

            HashSet<Director> directors1 = movie1.Directors;
            HashSet<Director> directors2 = movie2.Directors;

            HashSet<Tag> tags1 = movie1.Tags;
            HashSet<Tag> tags2 = movie2.Tags;

            string rating1 = movie1.Rating;
            string rating2 = movie2.Rating;

            double r1 = Convert.ToDouble(rating1.Replace('.', ','));
            double r2 = Convert.ToDouble(rating2.Replace('.', ','));


            int a_intesect = (actors1.Intersect(actors2)).Count();
            int d_intersect = (directors1.Intersect(directors2)).Count();
            int t_intersect = (tags1.Intersect(tags2)).Count();


            int a_count = actors1.Count + actors2.Count - a_intesect;
            int t_count = tags1.Count + tags2.Count - t_intersect;
            int d_count = directors1.Count + directors2.Count - d_intersect;

            double relevance = (a_intesect / a_count + t_intersect / t_count + d_intersect / d_count + (r2 + r1) / 20) / 4;
            return relevance;

        }
    }

}
