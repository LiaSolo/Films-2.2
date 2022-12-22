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
            List<Tag> tags = db.Tags
                .Include(t => t.Movies)
                .Where(t => t.Name == tag).ToList();

            if (tags.Count == 0) return null;
            else
            {
                List<Movie_bd> movies = db.Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.Tags)
                .Where(m => m.Tags.Contains(tags[0])).ToList();

                return movies;
            }
        }
        public List<Movie_bd> GetMoviesFromPerson(string name)
        {
            List<Actor> actors = db.Actors
              .Include(a => a.Movies)
              .Where(a => a.Name == name).ToList();

            List<Director> directors = db.Directors
              .Include(d => d.Movies)
              .Where(d => d.Name == name).ToList();

            if (actors.Count + directors.Count == 0) return null;
            else
            {
                List<Movie_bd> movies = new List<Movie_bd>();
                if (actors.Count != 0)
                {
                    movies.AddRange(db.Movies
                    .Include(m => m.Actors)
                    .Include(m => m.Directors)
                    .Include(m => m.Tags)
                    .Where(m => m.Actors.Contains(actors[0])));
                }

                if (directors.Count != 0)
                {
                    movies.AddRange(db.Movies
                    .Include(m => m.Actors)
                    .Include(m => m.Directors)
                    .Include(m => m.Tags)
                    .Where(m => m.Directors.Contains(directors[0])).ToList());
                }
                
                return movies;
            }
        }

        ApplicationContext2 db2;
        public ApplicationService(ApplicationContext2 context2)
        {
            db2 = context2;   
        }

        public List<Movie_bd> GetTop10(string movie_name)
        {
            List<Movie_bd> movie = db.Movies
            .Include(m => m.Actors)
            .Include(m => m.Directors)
            .Include(m => m.Tags)
            .Where(m => m.Name == movie_name).ToList();

            List<Movie_bd> top = new List<Movie_bd>();
            if (movie.Count == 0) return null;
            else
            {
                List<Top10> top_list = (from m in db2.Movies where m.Name == movie_name select m.Top).ToList();

                for (int j = 0; j < 10; j++)
                {
                    top.Add(GetMovie(top_list[0].Top[j]));
                }

                return top;
            }
        }
    }
}
