using ShitBlazor.Data;
using Microsoft.EntityFrameworkCore;

namespace ShitBlazor.Services
{
    public class ApplicationService
    {
        ApplicationContext db;
        public ApplicationService(ApplicationContext context)
        {
            db = context;
        }

        public Movie_bd GetMovie(string name)
        {
            List<Movie_bd> movies = db.Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.Tags)
                .Where(m => m.Name == name).ToList();

            if (movies.Count == 0) return null;
            else return movies[0];
        }
        public List<Movie_bd> GetMoviesFromTag(string tag)
        {
            List<Movie_bd> movies = db.Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.Tags)
                .Where(m => m.Top10 != null && m.Tags
                .Any(p => p.Name == tag)).ToList();

            return movies;
        }
        public List<Movie_bd> GetMoviesFromPerson(string name)
        {
            List<Movie_bd> movies = new List<Movie_bd>();

            movies.AddRange(db.Movies
                    .Include(m => m.Actors)
                    .Include(m => m.Directors)
                    .Include(m => m.Tags)
                    .Where(m => m.Top10 != null && m.Actors
                    .Any(p => p.Name == name)));

            movies.AddRange(db.Movies
                    .Include(m => m.Actors)
                    .Include(m => m.Directors)
                    .Include(m => m.Tags)
                    .Where(m => m.Top10 != null && m.Directors
                    .Any(p => p.Name == name)));

            return movies;
        }

        //ApplicationContext2 db2;
        //public ApplicationService(ApplicationContext2 context2)
        //{
        //    db2 = context2;   
        //}

        //public List<Movie_bd> GetTop10(string movie_name)
        //{
        //    List<Movie_bd> movie = db.Movies
        //    .Include(m => m.Actors)
        //    .Include(m => m.Directors)
        //    .Include(m => m.Tags)
        //    .Where(m => m.Name == movie_name).ToList();

        //    List<Movie_bd> top = new List<Movie_bd>();
        //    if (movie.Count == 0) return null;
        //    else
        //    {

        //        foreach(string mt in movie[0].Top10)
        //        {
        //            top.Add(GetMovie(mt));
        //        }

        //        return top;
        //    }
        //}
    }
}
