using Films_2._2;
using Microsoft.EntityFrameworkCore;

// Код был неправильный, но суть отражает
//                           До рефакторинга  // Parallel + AsSpan 
//links_IMDB_MovieLens:      00:00:00.4315613 // 00:00:00.0826156  
//TagCodes_MovieLens:        00:00:00.0020740 // 00:00:00.0123031  
//Ratings_IMDB:              00:00:04.0039178 // 00:00:01.0276114  
//MovieCodes_IMDB:           00:00:08.8379080 // 00:00:15.1513265  
//TagScores_MovieLens:       00:01:06.2264605 // 00:03:32.8199461  
//ActorsDirectorsNames_IMDB: 00:01:20.6478404 // 00:04:54.0480671  
//ActorsDirectorsCodes_IMDB: 00:01:08.4717690 // 00:09:35.3564674  


//links_IMDB_MovieLens: 00:00:01.0497923
//TagCodes_MovieLens: 00:00:00.0021146
//Ratings_IMDB: 00:00:04.6487216
//MovieCodes_IMDB: 00:00:17.3084494
//TagScores_MovieLens: 00:01:20.7440463
//ActorsDirectorsNames_IMDB: 00:01:38.5665250
//ActorsDirectorsCodes_IMDB: 00:01:07.5001354
//Make_name_movie_dict: 00:00:01.4159479
//Make_tags_movies_dict: 00:00:01.5898943
//Make_actors_directors_names_movies_dict: 00:00:25.0546039
//END: 00:03:35.6380214


//links_IMDB_MovieLens: 00:00:00.3760003
//TagCodes_MovieLens: 00:00:00.0024475
//Ratings_IMDB: 00:00:05.8479639
//MovieCodes_IMDB: 00:00:13.1520870
//TagScores_MovieLens: 00:01:40.5136919
//ActorsDirectorsNames_IMDB: 00:01:54.2586392
//ActorsDirectorsCodes_IMDB: 00:01:26.6601016
//Make_name_movie_dict: 00:00:01.5660076
//Make_tags_movies_dict: 00:00:00.8737967
//Make_actors_directors_names_movies_dict: 00:00:21.1857709
//END: 00:04:01.1339075


//links_IMDB_MovieLens: 00:00:00.6432169
//TagCodes_MovieLens: 00:00:00.0025308
//Ratings_IMDB: 00:00:05.8299031
//MovieCodes_IMDB: 00:00:14.5645626
//TagScores_MovieLens: 00:01:27.4829294
//ActorsDirectorsNames_IMDB: 00:01:34.7174115
//ActorsDirectorsCodes_IMDB: 00:01:02.2334247
//Make_name_movie_dict: 00:00:01.6230089
//Make_tags_movies_dict: 00:00:00.8270069
//Make_actors_directors_names_movies_dict: 00:00:12.8743267
//END: 00:03:11.3143937



Dictionary<string, Movie_bd> code_movie = new Dictionary<string, Movie_bd>();                                // код фильма - класс фильма
Dictionary<string, Movie2> code_movie2 = new Dictionary<string, Movie2>();

Dictionary<string, HashSet<string>> movie_directors = new Dictionary<string, HashSet<string>>(); // код фильма - класс режиссёров 
Dictionary<string, HashSet<string>> movie_actors = new Dictionary<string, HashSet<string>>();    // код фильма - класс актёров



Dictionary<string, List<string>> person_moviesCodes = new Dictionary<string, List<string>>();              // код человека - коды фильмов с ним
//Dictionary<string, HashSet<Movie_bd>> person_movies = new Dictionary<string, HashSet<Movie_bd>>();              // код человека - класс фильмов с ним



Dictionary<string, Actor> code_actor = new Dictionary<string, Actor>();                          // код режиссёра - класс режиссёра
Dictionary<string, Director> code_director = new Dictionary<string, Director>();                     // код актёра - класс актёра
Dictionary<string, Tag> code_tag = new Dictionary<string, Tag>();                                  // код тега - класс тега
Dictionary<string, HashSet<Movie_bd>> tag_movies = new Dictionary<string, HashSet<Movie_bd>>();           // код тега - класс фильмов с этим тегом
//Dictionary<string, HashSet<Tag>> imdb_tags = new Dictionary<string, HashSet<Tag>>();                 // код фильма imdb - класс тегов

Dictionary<string, string> movielens_imdb = new Dictionary<string, string>();                              // код movielens - код imdb
//Dictionary<string, string> movie_rating = new Dictionary<string, string>();                               // код фильма - его рейтинг

Dictionary<string, List<string>> code_actor_movies = new Dictionary<string, List<string>>();
Dictionary<string, List<string>> code_director_movies = new Dictionary<string, List<string>>();


//Dictionary<string, Movie> name_movie = new Dictionary<string, Movie>();                        // название фильма - объект класса фильм
//Dictionary<string, HashSet<Movie>> personName_movies = new Dictionary<string, HashSet<Movie>>();
//Dictionary<string, HashSet<Movie>> tagName_movies = new Dictionary<string, HashSet<Movie>>();

HashSet<string> a_check = new HashSet<string>();
HashSet<string> d_check = new HashSet<string>();
HashSet<string> m_check = new HashSet<string>();
HashSet<string> t_check = new HashSet<string>();


ApplicationContext db2 = new ApplicationContext();

int IdFilm = 1;
int IdTag = 1;
int IdActor = 1;
int IdDirector = 1;

//System.Diagnostics.Stopwatch sw_ = new System.Diagnostics.Stopwatch();
//sw_.Start();


#region

Task read_check = Task.Factory.StartNew(() =>
ReadCheck());

Task movie_codes = read_check.ContinueWith( t =>
   Read_MovieCodes_IMDB());

Task rating = movie_codes.ContinueWith(t =>
  Read_Ratings_IMDB());


Task links = Task.Factory.StartNew(() =>
   Read_links_IMDB_MovieLens());


Task tag_codes = read_check.ContinueWith(t =>
  Read_TagCodes_MovieLens());



Task a_d_names = read_check.ContinueWith(t =>
  Read_ActorsDirectorsNames_IMDB());
Task a_d_codes = Task.WhenAll(new Task[] { a_d_names, movie_codes }).ContinueWith(t =>
  Read_ActorsDirectorsCodes_IMDB());


Task tag_scores = Task.WhenAll(new Task[] { links, movie_codes, tag_codes }).ContinueWith(t =>
  Read_TagScores_MovieLens());

//Task make_bd = Task.WhenAll(new Task[] { rating, tag_scores, a_d_codes }).ContinueWith(t => Make_bd());

Task end = Task.WhenAll(new Task[] { a_d_codes, tag_scores }).ContinueWith(t =>
{
    Make_top();
    Console.WriteLine("Database created!");
});

end.Wait();

#endregion

//sw_.Stop();
//Console.WriteLine($"END: {sw_.Elapsed}");


//Read_links_IMDB_MovieLens();
//Read_ActorsDirectorsNames_IMDB();
//Read_MovieCodes_IMDB();
//Read_TagCodes_MovieLens();
//Read_TagScores_MovieLens();
//Read_ActorsDirectorsCodes_IMDB();
//Read_Ratings_IMDB();


// Последовательности:

// Read_Ratings_IMDB
// Read_ActorsDirectorsNames_IMDB -- Read_ActorsDirectorsCodes_IMDB
// Read_MovieCodes_IMDB ___________/
// Read_TagCodes_MovieLens____     \
// Read_links_IMDB_MovieLens--\Read_TagScores_MovieLens


void ReadCheck()
{
    using (StreamReader s = new StreamReader(@"C:\Users\HP\Desktop\pre\actors.txt"))
    {
        string line;
        for (int i = 0; i < 20000; i++)
        {
            line = s.ReadLine();
            a_check.Add(line);
        }

        Console.WriteLine(a_check.Count);
    }

    using (StreamReader s = new StreamReader(@"C:\Users\HP\Desktop\pre\directors.txt"))
    {
        string line;
        for (int i = 0; i < 5000; i++)
        {
            line = s.ReadLine();
            d_check.Add(line);
        }

        Console.WriteLine(d_check.Count);
    }

    using (StreamReader s = new StreamReader(@"C:\Users\HP\Desktop\pre\movies.txt"))
    {
        string line;
        for (int i = 0; i < 20000; i++)
        {
            line = s.ReadLine();
            m_check.Add(line);
        }

        Console.WriteLine(m_check.Count);
    }

    using (StreamReader s = new StreamReader(@"C:\Users\HP\Desktop\pre\tags.txt"))
    {
        string line;
        for (int i = 0; i < 2000; i++)
        {
            line = s.ReadLine();
            t_check.Add(line);
        }

        Console.WriteLine(t_check.Count);
    }
}


void Make_top()
{
    int flag = 0;
    foreach (var m in code_movie)
    {
        string code = m.Key;
        Movie_bd movie = m.Value;
        HashSet<Movie_bd> vs = new HashSet<Movie_bd>();

        string[] ans = new string[10];

        Dictionary<string, double> top = new Dictionary<string, double>();

        foreach (Actor a in movie.Actors)
        {
            foreach (Movie_bd ma in a.Movies)
            {
                vs.Add(ma);
            }
        }

        foreach (Director d in movie.Directors)
        {
            foreach (Movie_bd md in d.Movies)
            {
                vs.Add(md);


                //Console.WriteLine($"{m.Name} && {mad.Name}: {rel}");
                //Console.WriteLine($"{m.Name} && {mad.Name}: {rel}");
            }
        }

        foreach (Tag t in movie.Tags)
        {
            foreach (Movie_bd mt in t.Movies)
            {
                vs.Add(mt);
            }
        }

        vs.Remove(movie);

        foreach (Movie_bd mov in vs)
        {
            double rel = Movie_bd.Relevance(movie, mov);

            if (!top.ContainsKey(mov.Name))
            {
                top.Add(mov.Name, rel);
            }
           
        }

        int count = 0;
        var sorted_dic = top.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        foreach (var n in sorted_dic)
        {
            ans[count] = n.Key;
            count++;
            //Console.WriteLine($"{n.Key}: {n.Value}");
            if (count == 10) break;
        }

        List<Movie_bd> movies = db2.Movies
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.Tags)
                .Where(m => m.Name == movie.Name).ToList();

        movies[0].Top10 = ans;

        db2.Movies.Update(movies[0]);
        flag++;


        if (flag > 110)
        {
            lock (db2)
            {
                db2.SaveChanges();
            }
            flag = 0;
        }

    }

    lock(db2)
    {
        db2.SaveChanges();
    }

}


//для каждого фильма получила наборы актёров и режиссёров (код - имена)
void Read_ActorsDirectorsCodes_IMDB()
{
    //tt0000001 (index 1)  2 (index 2)  nm0005690 (index 3) director {index 4) \N  \N

    //Console.WriteLine("ActorsDirectorsCodes_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\ActorsDirectorsCodes_IMDB.tsv"))
    {
        //Console.WriteLine("ActorsDirectorsCodes_IMDB");

        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            var line_span = line.AsSpan();

            int index = line_span.IndexOf('\t'); // 1
            string id_movie = line_span.Slice(0, index).ToString();

            if (!code_movie.ContainsKey(id_movie))
            {
                continue;
            }

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 2

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 3

            string id_person = line_span.Slice(0, index).ToString();

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 4

            string job = line_span.Slice(0, index).ToString();


            if (job == "director" && code_director.ContainsKey(id_person))
            {
                code_movie[id_movie].Directors.Add(code_director[id_person]);
                code_director[id_person].Movies.Add(code_movie[id_movie]);
            }
            else if ((job == "actor" || job == "actress") && code_actor.ContainsKey(id_person))
            {
                code_movie[id_movie].Actors.Add(code_actor[id_person]);
                code_actor[id_person].Movies.Add(code_movie[id_movie]);
            }
            
            
            //Console.WriteLine(line);
        }
    }
    #endregion
    sw.Stop();
    Console.WriteLine($"ActorsDirectorsCodes_IMDB: {sw.Elapsed}");
}


//для каждого актёра и режиссёра получила наборы фильмов с участием (код - коды)
//для каждого актёра и режиссёра  сопоставила код и имя
void Read_ActorsDirectorsNames_IMDB()
{
    // nm0000002 (index 1)	Lauren Bacall (index 2)	1924	(index 3) 2014	(index 4) actress,soundtrack 	(index 5)   tt0117057,tt0037382,tt0038355,tt0071877

    //Console.WriteLine("ActorsDirectorsNames_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\ActorsDirectorsNames_IMDB.txt"))
    {
        //Console.WriteLine("ActorsDirectorsNames_IMDB");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            var line_span = line.AsSpan();

            int index = line_span.IndexOf('\t'); // 1
            string id = line_span.Slice(0, index).ToString();

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 2

            string name = line_span.Slice(0, index).ToString();

            if (a_check.Contains(id))
            {
                Actor a = new Actor {Name = name, Movies = new HashSet<Movie_bd>() };
                IdActor++;
                code_actor.Add(id, a);
            }

            if (d_check.Contains(id))
            {
                Director d = new Director { Id = IdDirector, Name = name, Movies = new HashSet<Movie_bd>() };
                IdDirector++;
                code_director.Add(id, d);
            }
        }

        #endregion

        sw.Stop();
        Console.WriteLine($"ActorsDirectorsNames_IMDB: {sw.Elapsed}");
    }

}

//для каждого фильма сопоставила код imdb и код movielens
void Read_links_IMDB_MovieLens()
{
    // 1,0114709,862

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\links_IMDB_MovieLens.csv"))
    {
        //Console.WriteLine("links_IMDB_MovieLens");
        string line = sr.ReadLine();
        //Console.WriteLine(line);

        while ((line = sr.ReadLine()) != null)
        {
            var line_span = line.AsSpan();
            int index = line_span.IndexOf(','); // 1
            string movielens = line_span.Slice(0, index).ToString();

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf(','); // 2

            string imdb = line_span.Slice(0, index).ToString();

            movielens_imdb.Add(movielens, "tt" + imdb);
            //Console.WriteLine(line);
        }
    }

    #endregion

    sw.Stop();
    Console.WriteLine($"links_IMDB_MovieLens: {sw.Elapsed}");
}

//для каждого фильма сопоставила код IMDB и название
void Read_MovieCodes_IMDB()
{
    // tt0000001 (index 1)	1 (index 2)	Carmencita - spanyol tánc (index 3)	HU (index 4)	\N	imdbDisplay	\N	0

    //Console.WriteLine("MovieCodes_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    int flag = 0;
    #region


    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\MovieCodes_IMDB.tsv"))
    {
        //Console.WriteLine("MovieCodes_IMDB");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {

            var line_span = line.AsSpan();

            int index = line_span.IndexOf('\t'); // 1
            string id = line_span.Slice(0, index).ToString();

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 2

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 3
            string name = line_span.Slice(0, index).ToString();

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 4
            string reg = line_span.Slice(0, index).ToString();

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 5
            string lang = line_span.Slice(0, index).ToString();

            if (m_check.Contains(id) && !code_movie.ContainsKey(id) && 
                (reg == "US" || reg == "RU" || reg == "GB" || lang == "en" || lang == "ru"))
            {
                Movie_bd m = new Movie_bd
                {
                    Id = IdFilm,
                    Name = name,
                    Actors = new HashSet<Actor>(),
                    Directors = new HashSet<Director>(),
                    Tags = new HashSet<Tag>(),
                    Rating = "-1",
                    Top10 = new string[10]
                };

                IdFilm++;
                code_movie.Add(id, m);

                flag++;

                if (flag == 110)
                {
                    lock (db2)
                    {
                        db2.SaveChanges();
                    }
                    flag = 0;                   
                }
            }
        }

        #endregion

        sw.Stop();
        Console.WriteLine($"MovieCodes_IMDB: {sw.Elapsed}");
    }

    lock (db2)
    {
        db2.SaveChanges();
    }
}

//для каждого фильма нашла рейтинг (код - рейтинг)
void Read_Ratings_IMDB()
{
    // tt0000001 (index 1)	5.8	(index 2) 1435

    //Console.WriteLine("Ratings_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region

    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\Ratings_IMDB.tsv"))
    {
        //Console.WriteLine("Ratings_IMDB");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            var line_span = line.AsSpan();
            int index = line_span.IndexOf('\t'); // 1
            string id = line_span.Slice(0, index).ToString();

            if (!code_movie.ContainsKey(id))
            {
                continue;
            }

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 2
            string rate = line_span.Slice(0, index).ToString();

            code_movie[id].Rating = rate;


            //Console.WriteLine(line);
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"Ratings_IMDB: {sw.Elapsed}");
}


//для каждого тега сопоставила код и сам тег
void Read_TagCodes_MovieLens()
{

    // 1,007

    //Console.WriteLine("TagCodes_MovieLens");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\TagCodes_MovieLens.csv"))
    {
        //Console.WriteLine("TagCodes_MovieLens");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            var line_span = line.AsSpan();
            int index = line_span.IndexOf(',');

            string id = line_span.Slice(0, index).ToString();
            string name_tag = line_span.Slice(index + 1).ToString();

            if (!t_check.Contains(id))
            {
                continue;
            }

            Tag tag = new Tag { Id = IdTag, Name = name_tag, Movies = new HashSet<Movie_bd>() };
            IdTag++;
            code_tag.Add(id, tag);
            //Console.WriteLine(line);
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"TagCodes_MovieLens: {sw.Elapsed}");
}


//для каждого тега нашла коды imdb фильмов с ним (код - коды)
//для каждого фильма сопоставила код imdb и теги (код - коды)
void Read_TagScores_MovieLens()
{
    // 1,1,0.029000000000000026

    //Console.WriteLine("TagScores_MovieLens");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();
    #region
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\TagScores_MovieLens.csv"))
    {
        //Console.WriteLine("TagScores_MovieLens");
        string line = sr.ReadLine();
        //Console.WriteLine(line);

        while ((line = sr.ReadLine()) != null)
        {
            //Console.WriteLine(line);
            var line_span = line.AsSpan();

            int index = line_span.IndexOf(','); // 1
            string id_movielens = line_span.Slice(0, index).ToString();
            string id_imdb = movielens_imdb[id_movielens];

            if (!code_movie.ContainsKey(id_imdb))
            {
                continue;
            }

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf(','); // 2


            string tag_id = line_span.Slice(0, index).ToString();
            string level = line_span.Slice(index + 1).ToString();

            if (!code_tag.ContainsKey(tag_id))
            {
                continue;
            }

            level = level.Replace('.', ',');
            double relevance = Convert.ToDouble(level);

            if (relevance > 0.5)
            {
                code_movie[id_imdb].Tags.Add(code_tag[tag_id]);
                code_tag[tag_id].Movies.Add(code_movie[id_imdb]);

            }
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"TagScores_MovieLens: {sw.Elapsed}");

}


