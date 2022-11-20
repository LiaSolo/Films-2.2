



Dictionary<string, string> code_movie = new Dictionary<string, string>();                                // код фильма - название фильма
Dictionary<string, HashSet<string>> movieCode_directors = new Dictionary<string, HashSet<string>>(); // код фильма - имена режиссёров 
Dictionary<string, HashSet<string>> movieCode_actors = new Dictionary<string, HashSet<string>>();    // код фильма - имена актёров
Dictionary<string, List<string>> personCode_movies = new Dictionary<string, List<string>>();              // код человека - названия фильмов с ним
Dictionary<string, string> personCode_name = new Dictionary<string, string>();                          // код человека - имя человека
Dictionary<string, string> movielens_imdb = new Dictionary<string, string>();                              // код movielens - код imdb

Dictionary<string, HashSet<string>> imdb_tags = new Dictionary<string, HashSet<string>>();                 // код фильма imdb - названия тегов
Dictionary<string, string> movieCode_rating = new Dictionary<string, string>();                               // код фильма - его рейтинг
Dictionary<string, string> code_tag = new Dictionary<string, string>();                                  // код тега - название тега
Dictionary<string, HashSet<string>> tag_movies = new Dictionary<string, HashSet<string>>();           // код тега - названия фильмов с этим тегом

Dictionary<string, Films_2._2.Movie> name_movie = new Dictionary<string, Films_2._2.Movie>();                        // название фильма - объект класса фильм
Dictionary<string, HashSet<Films_2._2.Movie>> actors_directors_names_movies = new Dictionary<string, HashSet<Films_2._2.Movie>>();
Dictionary<string, HashSet<Films_2._2.Movie>> tags_movies = new Dictionary<string, HashSet<Films_2._2.Movie>>();

#region

Task rating = Task.Factory.StartNew(() =>
   Read_Ratings_IMDB());


Task links = Task.Factory.StartNew(() =>
   Read_links_IMDB_MovieLens());


Task tag_codes = Task.Factory.StartNew(() =>
   Read_TagCodes_MovieLens());


Task movie_codes = Task.Factory.StartNew(() =>
   Read_MovieCodes_IMDB());


Task a_d_names = movie_codes.ContinueWith(t =>
  Read_ActorsDirectorsNames_IMDB());
Task a_d_codes = a_d_names.ContinueWith(t =>
  Read_ActorsDirectorsCodes_IMDB());


Task[] prev_for_tag_scores = new Task[] { links, movie_codes, tag_codes };
Task tag_scores = Task.WhenAll(prev_for_tag_scores).ContinueWith(t =>
  Read_TagScores_MovieLens());


Task[] predecessors = new Task[] { a_d_codes, rating, tag_scores };
Task dict_name_movie = Task.WhenAll(predecessors).ContinueWith(t
    => Make_name_movie_dict());

//Task dict_actors_directors_movies = dict_name_movie.ContinueWith(t =>
//  Make_actors_directors_names_movies_dict());

//Task dict_tags_movies = dict_name_movie.ContinueWith(t =>
//  Make_tags_movies_dict());


//dict_name_movie.Wait();

Task write_ans = dict_name_movie.ContinueWith(t =>
{
    foreach (var m in name_movie)
    {
        Console.WriteLine(m.Key);
        Films_2._2.Movie.PrintMovie(m.Value);
    }

});

write_ans.Wait();
#endregion


//Read_links_IMDB_MovieLens();
//Read_ActorsDirectorsNames_IMDB();
//Read_MovieCodes_IMDB();
//Read_TagCodes_MovieLens();

// Последовательности:

// Read_Ratings_IMDB
//                                  /Read_ActorsDirectorsNames_IMDB -- Read_ActorsDirectorsCodes_IMDB
// Read_MovieCodes_IMDB ----------<
// Read_TagCodes_MovieLens---\     \
//                            >Read_TagScores_MovieLens
// Read_links_IMDB_MovieLens /


Dictionary<string, Films_2._2.Movie> Make_name_movie_dict()
{
    foreach (var m in code_movie)
    {
        string id = m.Key;
        string name = m.Value;

        if (movieCode_rating.ContainsKey(id) && movieCode_actors.ContainsKey(id)
            && movieCode_directors.ContainsKey(id) && imdb_tags.ContainsKey(id))
        {
            string rating = movieCode_rating[id];

            HashSet<string> actors = movieCode_actors[id];

            HashSet<string> directors = movieCode_directors[id];

            HashSet<string> tags = imdb_tags[id];

            if (!name_movie.ContainsKey(name))
            {
                name_movie.Add(name, new Films_2._2.Movie(name, actors, directors, tags, rating));
            }
        }

    }
    return name_movie;
}

Dictionary<string, HashSet<Films_2._2.Movie>> Make_actors_directors_names_movies_dict()
{
    foreach (var p in personCode_name)
    {
        string id = p.Key;
        string name = p.Value;

        HashSet<Films_2._2.Movie> movies = new HashSet<Films_2._2.Movie>();
        foreach (string m_n in personCode_movies[id])
        {
            movies.Add(name_movie[m_n]);
        }

        actors_directors_names_movies.Add(name, movies);
    }


    return actors_directors_names_movies;
}

Dictionary<string, HashSet<Films_2._2.Movie>> Make_tags_movies_dict()
{
    foreach (var t in code_tag)
    {
        string id = t.Key;
        string name = t.Value;

        HashSet<Films_2._2.Movie> movies = new HashSet<Films_2._2.Movie>();
        foreach (string m_n in tag_movies[id])
        {
            movies.Add(name_movie[m_n]);
        }

        tags_movies.Add(name, movies);
    }

    return tags_movies;
}


//для каждого фильма получила наборы актёров и режиссёров (код - имена)
void Read_ActorsDirectorsCodes_IMDB()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\ActorsDirectorsCodes_IMDB.tsv"))
    {
        Console.WriteLine("ActorsDirectorsCodes_IMDB");

        string line = sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] row = line.Split('\t');
            string id_movie = row[0];
            string id_person = row[2];
            string job = row[3];

            if (code_movie.ContainsKey(id_movie) && personCode_name.ContainsKey(id_person))
            {
                if (job == "director")
                {
                    if (!movieCode_directors.ContainsKey(id_movie))
                    {
                        movieCode_directors.Add(id_movie, new HashSet<string>());
                    }
                    movieCode_directors[id_movie].Add(personCode_name[id_person]);
                }
                else if (row[3] == "actor")
                {
                    if (!movieCode_actors.ContainsKey(id_movie))
                    {
                        movieCode_actors.Add(id_movie, new HashSet<string>());
                    }
                    movieCode_actors[id_movie].Add(personCode_name[id_person]);
                }
            }
            //Console.WriteLine(line);
        }
    }
}


//для каждого актёра и режиссёра получила наборы фильмов с участием (код - название)
//для каждого актёра и режиссёра  сопоставила код и имя
void Read_ActorsDirectorsNames_IMDB()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\ActorsDirectorsNames_IMDB.txt"))
    {
        Console.WriteLine("ActorsDirectorsNames_IMDB");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            string[] row = line.Split('\t');

            string id = row[0];
            string name = row[1];

            List<string> job = new List<string>();
            job.AddRange(row[4].Split(','));

            List<string> moviesCodes = new List<string>();
            moviesCodes.AddRange(row[5].Split(','));

            List<string> moviesNames = new List<string>();

            foreach (string id_m in moviesCodes)
            {
                if (code_movie.ContainsKey(id_m))
                {
                    moviesNames.Add(code_movie[id_m]);
                }
            }

            if (job.Contains("actor") || job.Contains("actress") || job.Contains("director"))
            {
                personCode_movies.Add(id, moviesNames);
                personCode_name.Add(id, name);
            }

            //Console.WriteLine(line);
        }
    }
}


//для каждого фильма сопоставила код imdb и код movielens
void Read_links_IMDB_MovieLens()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\links_IMDB_MovieLens.csv"))
    {
        Console.WriteLine("links_IMDB_MovieLens");
        string line = sr.ReadLine();
        //Console.WriteLine(line);

        while ((line = sr.ReadLine()) != null)
        {
            string[] row = line.Split(',');
            string movielens = row[0];
            string imdb = row[1];

            movielens_imdb.Add(movielens, "tt" + imdb);
            //Console.WriteLine(line);
        }
    }
}


//для каждого фильма сопоставила код IMDB и название
void Read_MovieCodes_IMDB()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\MovieCodes_IMDB.tsv"))
    {
        Console.WriteLine("MovieCodes_IMDB");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {

            string[] row = line.Split('\t');
            string lang = row[3];
            string id = row[0];
            string name = row[2];

            if (lang == "US" || lang == "RU")
            {
                if (!code_movie.ContainsKey(row[0]))
                {
                    code_movie.Add(id, name);
                    //Console.WriteLine(line);
                }
            }
        }
    }
}


//для каждого фильма нашла рейтинг (код - рейтинг)
void Read_Ratings_IMDB()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\Ratings_IMDB.tsv"))
    {
        Console.WriteLine("Ratings_IMDB");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            string[] row = line.Split('\t');
            string id = row[0];
            string rate = row[1];

            movieCode_rating.Add(id, rate);
            //Console.WriteLine(line);
        }
    }
}


//для каждого тега сопоставила код и сам тег
void Read_TagCodes_MovieLens()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\TagCodes_MovieLens.csv"))
    {
        Console.WriteLine("TagCodes_MovieLens");
        string line = sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            string[] row = line.Split(',');
            code_tag.Add(row[0], row[1]);
            //Console.WriteLine(line);
        }
    }
}


//для каждого тега нашла коды imdb фильмов с ним (код - коды)
//для каждого фильма сопоставила код imdb и теги (код - коды)
void Read_TagScores_MovieLens()
{
    using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\TagScores_MovieLens.csv"))
    {
        Console.WriteLine("TagScores_MovieLens");
        string line = sr.ReadLine();
        //Console.WriteLine(line);

        while ((line = sr.ReadLine()) != null)
        {
            //Console.WriteLine(line);
            string[] row = line.Split(',');
            string id_movielens = row[0];
            string id_imdb = movielens_imdb[id_movielens];
            string tag_id = row[1];
            string level = row[2];

            if (code_movie.ContainsKey(id_imdb))
            {
                if (!tag_movies.ContainsKey(tag_id))
                {
                    tag_movies.Add(tag_id, new HashSet<string>());
                }

                if (!imdb_tags.ContainsKey(id_imdb))
                {
                    imdb_tags.Add(id_imdb, new HashSet<string>());
                }
                string[] split = level.Split('.');
                double relevance = Convert.ToDouble(split[0] + ',' + split[1]);
                if (relevance > 0.5)
                {
                    imdb_tags[id_imdb].Add(code_tag[tag_id]);
                    tag_movies[tag_id].Add(code_movie[id_imdb]);
                }
            }
        }
    }
}

