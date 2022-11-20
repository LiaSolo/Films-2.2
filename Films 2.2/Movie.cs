using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Films_2._2
{
    internal class Movie
    {
        public HashSet<string> Actors { get; set; }
        public HashSet<string> Tags { get; set; }
        public string Name { get; set; }
        public HashSet<string> Directors { get; set; }
        public string Rating { get; set; }

        public Movie(string name, HashSet<string> actors, HashSet<string> directors, HashSet<string> tags, string rating)
        {
            Name = name;
            Actors = actors;
            Directors = directors;
            Tags = tags;
            Rating = rating;
        }

        public static void PrintMovie(Movie movie)
        {
            string str_actors = "{ ";
            foreach (string a in movie.Actors)
            {
                str_actors += $" {a};";
            }
            str_actors += "}";

            string str_directors = "{ ";
            foreach (string d in movie.Directors)
            {
                str_directors += $" {d};";
            }
            str_directors += "}";

            string str_tags = "{ ";
            foreach (string t in movie.Tags)
            {
                str_tags += $" {t};";
            }
            str_tags += "}";


            string ans = $"Название: {movie.Name}.\n Рейтинг: {movie.Rating}.\n Актёры: {str_actors}.\n Режиссёры: {str_directors}.\n Тэги {str_tags}.";
            Console.WriteLine(ans);
        }
    }
}
