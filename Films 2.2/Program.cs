using Films_2._2;
using Microsoft.EntityFrameworkCore;

ApplicationContext db = new ApplicationContext();

void WriteActor(Actor a)
{
    string movies = "";
    foreach (Movie_bd m in a.Movies)
    {
        movies += m.Name + "; ";
    }
    string answer = $"NAME: {a.Name} \nMOVIES: {movies}";
    Console.WriteLine(answer);
}

void WriteDirector(Director d)
{
    string movies = "";
    foreach (Movie_bd m in d.Movies)
    {
        movies += m.Name + "; ";
    }
    string answer = $"NAME: {d.Name} \nMOVIES: {movies}";
    Console.WriteLine(answer);
}

void WriteTag(Tag t)
{
    string movies = "";
    foreach (Movie_bd m in t.Movies)
    {
        movies += m.Name + "; ";
    }
    string answer = $"NAME: {t.Name} \nMOVIES: {movies}";
    Console.WriteLine(answer);
}

void WriteMovie(Movie_bd m)
{
    string actors = "";
    foreach (Actor a in m.Actors)
    {
        actors += a.Name + "; ";
    }

    string directors = "";
    foreach (Director d in m.Directors)
    {
        directors += d.Name + "; ";
    }

    string tags = "";
    foreach (Tag t in m.Tags)
    {
        tags += t.Name + "; ";
    }

    string answer = $"NAME: {m.Name} \nRATING: {m.Rating} \n" +
        $"ACTORS: {actors} \nDIRECTORS: {directors} \nTAGS: {tags}";
    Console.WriteLine(answer);
}

while (true)
{
    Console.WriteLine("COMMANDS: Person, Movie, Tag");
    string input = Console.ReadLine();
    string name;

    switch (input)
    {
        case "Person":
            Console.WriteLine("ENTRY PERSON'S NAME");
            name = Console.ReadLine();

            List<Actor> actors = db.Actors.Include(a => a.Movies).Where(a => a.Name == name).ToList();
            if (actors.Count == 0)
            {
                List<Director> directors = db.Directors.Include(d => d.Movies).Where(d => d.Name == name).ToList(); 

                if (directors.Count == 0)
                {
                    Console.WriteLine("THE PERSON DOESN'T EXIST");
                }
                else
                {
                    WriteDirector(directors[0]);
                }
            }
            else
            {
                WriteActor(actors[0]);
            }

            break;

        case "Movie":
            Console.WriteLine("ENTRY MOVIE'S NAME");
            name = Console.ReadLine();

            List<Movie_bd> movies = db.Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.Tags)
                .Where(m => m.Name == name).ToList();
            if (movies.Count == 0)
            {
                Console.WriteLine("THE MOVIE DOESN'T EXIST");
            }
            else
            {
                WriteMovie(movies[0]);
            }

            break;

        case "Tag":
            Console.WriteLine("ENTRY TAG'S NAME");
            name = Console.ReadLine();

            List<Tag> tags = db.Tags.Include(t => t.Movies).Where(t => t.Name == name).ToList();
            if (tags.Count == 0)
            {
                Console.WriteLine("THE TAG DOESN'T EXIST");
            }
            else
            {
                WriteTag(tags[0]);
            }

            break;
    }

}
