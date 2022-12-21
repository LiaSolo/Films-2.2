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

            if (movies.Count == 0)
            {
                Console.WriteLine("THE MOVIE DOESN'T EXIST");
                return null;
            }
            else
            {
                return movies[0];
            }
        }
        public List<Movie_bd> GetMoviesFromTag(string inputValue)
        {
            return db.Tags.Include(t => t.Movies).Where(t => t.Name.ToLower() == inputValue.ToLower()).First().Movies.ToList();
        }
        public List<Movie_bd> GetMoviesFromActor(string inputValue)
        {
            return db.Actors.Include(a => a.Movies).Where(a => a.Name.ToLower() == inputValue.ToLower()).First().Movies.ToList();
        }
        public List<Movie_bd> GetMoviesFromDirector(string inputValue)
        {
            var x = db.Directors.Include(d => d.Movies).Where(d => d.Name.ToLower() == inputValue.ToLower()).ToList();
            return x.FirstOrDefault()?.Movies?.ToList();
        }
    }
}
