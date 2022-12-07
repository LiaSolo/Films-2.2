

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


System.Diagnostics.Stopwatch sw_ = new System.Diagnostics.Stopwatch();
sw_.Start();


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


Task tag_scores = Task.WhenAll(new Task[] { links, movie_codes, tag_codes }).ContinueWith(t =>
  Read_TagScores_MovieLens());


Task dict_name_movie = Task.WhenAll(new Task[] { a_d_codes, rating, tag_scores }).ContinueWith(t
    => Make_name_movie_dict());

Task dict_actors_directors_movies = dict_name_movie.ContinueWith(t =>
  Make_actors_directors_names_movies_dict());

Task dict_tags_movies = dict_name_movie.ContinueWith(t =>
  Make_tags_movies_dict());



Task ui = Task.WhenAll(new Task[] { dict_actors_directors_movies, dict_tags_movies, dict_name_movie }).ContinueWith(t
    => UI());

ui.Wait();



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
// Read_MovieCodes_IMDB -----------/
// Read_TagCodes_MovieLens---\     \
//                            >Read_TagScores_MovieLens
// Read_links_IMDB_MovieLens /


void Make_name_movie_dict()
{
    //Console.WriteLine($"code_movie: {code_movie.Count}");
    //Console.WriteLine($"movieCode_directors: {movieCode_directors.Count}");
    //Console.WriteLine($"movieCode_actors: {movieCode_actors.Count}");
    //Console.WriteLine($"movieCode_rating: {movieCode_rating.Count}");
    //Console.WriteLine($"movieCode_directors: {imdb_tags.Count}");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    foreach (var m in code_movie)
    {
        string id = m.Key;
        string name = m.Value;

        if (name_movie.ContainsKey(name))
        {
            continue;
        }

        if (movieCode_rating.ContainsKey(id) && movieCode_actors.ContainsKey(id)
            && movieCode_directors.ContainsKey(id) && imdb_tags.ContainsKey(id))
        {
            string rating = movieCode_rating[id];

            HashSet<string> actors = movieCode_actors[id];

            HashSet<string> directors = movieCode_directors[id];

            HashSet<string> tags = imdb_tags[id];

            name_movie.Add(name, new Films_2._2.Movie(name, actors, directors, tags, rating));
        }

    }

    sw.Stop();
    Console.WriteLine($"Make_name_movie_dict: {sw.Elapsed}");
}

void Make_actors_directors_names_movies_dict()
{
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    foreach (var p in personCode_movies)
    {
        string id = p.Key;
        string name = personCode_name[id];

        if (actors_directors_names_movies.ContainsKey(name))
        {
            continue;
        }

        HashSet<Films_2._2.Movie> movies = new HashSet<Films_2._2.Movie>();
        foreach (string m_n in p.Value)
        {
            if (name_movie.ContainsKey(m_n))
            {
                movies.Add(name_movie[m_n]);
            }
        }
        actors_directors_names_movies.Add(name, movies);
    }

    sw.Stop();
    Console.WriteLine($"Make_actors_directors_names_movies_dict: {sw.Elapsed}");
}

void Make_tags_movies_dict()
{
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    foreach (var t in tag_movies)
    {
        string id = t.Key;
        string name = code_tag[id];

        if (tags_movies.ContainsKey(name))
        {
            continue;
        }

        HashSet<Films_2._2.Movie> movies = new HashSet<Films_2._2.Movie>();
        foreach (string m_n in t.Value)
        {
            if (name_movie.ContainsKey(m_n))
            {
                movies.Add(name_movie[m_n]);
            }              
        }
        tags_movies.Add(name, movies);
    }

    sw.Stop();
    Console.WriteLine($"Make_tags_movies_dict: {sw.Elapsed}");
}


//для каждого фильма получила наборы актёров и режиссёров (код - имена)
void Read_ActorsDirectorsCodes_IMDB()
{
    //tt0000001 (index 1)  2 (index 2)  nm0005690 (index 3) director {index 4) \N  \N

    //Console.WriteLine("ActorsDirectorsCodes_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\ActorsDirectorsCodes_IMDB.tsv").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    //Console.WriteLine(line);
    //    var line_span = line.AsSpan();

    //    int index = line_span.IndexOf('\t');
    //    string id_movie = line_span.Slice(0, index).ToString();

    //    int index2 = line_span.Slice(index + 1).IndexOf('\t');
    //    index = line_span.Slice(index2 + 1).IndexOf('\t');
    //    string id_person = line_span.Slice(index2, index).ToString();

    //    index2 = line_span.Slice(index + 1).IndexOf('\t');
    //    string job = line_span.Slice(index, index2).ToString();


    //    if (code_movie.ContainsKey(id_movie) && personCode_name.ContainsKey(id_person))
    //    {
    //        if (job == "director")
    //        {
    //            if (!movieCode_directors.ContainsKey(id_movie))
    //            {
    //                movieCode_directors.Add(id_movie, new HashSet<string>());
    //            }
    //            movieCode_directors[id_movie].Add(personCode_name[id_person]);
    //        }
    //        else if (job == "actor")
    //        {
    //            if (!movieCode_actors.ContainsKey(id_movie))
    //            {
    //                movieCode_actors.Add(id_movie, new HashSet<string>());
    //            }
    //            movieCode_actors[id_movie].Add(personCode_name[id_person]);
    //        }
    //    }
    //});
    #endregion

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

            if (code_movie.ContainsKey(id_movie) && personCode_name.ContainsKey(id_person))
            {
                if (job == "director")
                {
                    if (!movieCode_directors.ContainsKey(id_movie))
                    {
                        movieCode_directors.Add(id_movie, new HashSet<string>());
                    }

                    if (personCode_movies[id_person].Contains(id_movie))
                    {
                        movieCode_directors[id_movie].Add(personCode_name[id_person]);
                        personCode_movies[id_person].Remove(id_movie);
                        personCode_movies[id_person].Add(code_movie[id_movie]);
                    }
                    
                }
                else if (job == "actor" || job == "actress")
                {
                    if (!movieCode_actors.ContainsKey(id_movie))
                    {
                        movieCode_actors.Add(id_movie, new HashSet<string>());
                    }

                    if (personCode_movies[id_person].Contains(id_movie))
                    {
                        movieCode_actors[id_movie].Add(personCode_name[id_person]);
                        personCode_movies[id_person].Remove(id_movie);
                        personCode_movies[id_person].Add(code_movie[id_movie]);
                    }
                    
                }
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
    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\ActorsDirectorsNames_IMDB.txt").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    //Console.WriteLine(line);
    //    var line_span = line.AsSpan();

    //    int index = line_span.IndexOf('\t');
    //    string id = line_span.Slice(0, index).ToString();

    //    int index2 = line_span.Slice(index + 1).IndexOf('\t');
    //    string name = line_span.Slice(index, index2).ToString();

    //    index = line_span.Slice(index2 + 1).IndexOf('\t');
    //    index2 = line_span.Slice(index + 1).IndexOf('\t');

    //    index = line_span.Slice(index2 + 1).IndexOf('\t');
    //    List<string> jobs = new List<string>();
    //    jobs.AddRange(line_span.Slice(index2, index).ToString().Split(','));

    //    List<string> moviesCodes = new List<string>();
    //    moviesCodes.AddRange(line_span.Slice(index + 1).ToString().Split(','));

    //    List<string> moviesNames = new List<string>();

    //    foreach (string id_m in moviesCodes)
    //    {
    //        if (code_movie.ContainsKey(id_m))
    //        {
    //            moviesNames.Add(code_movie[id_m]);
    //        }
    //    }

    //    if (jobs.Contains("actor") || jobs.Contains("actress") || jobs.Contains("director"))
    //    {
    //        personCode_movies.Add(id, moviesNames);
    //        personCode_name.Add(id, name);
    //    }

    //});
    #endregion

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

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 3

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 4

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 5

            List<string> jobs = new List<string>();
            jobs.AddRange(line_span.Slice(0, index).ToString().Split(','));

            List<string> moviesCodes = new List<string>();
            moviesCodes.AddRange(line_span.Slice(index + 1).ToString().Split(','));

            //List<string> moviesNames = new List<string>();

            //foreach (string id_m in moviesCodes)
            //{
            //    if (code_movie.ContainsKey(id_m))
            //    {
            //        moviesNames.Add(code_movie[id_m]);
            //    }
            //}

            if (jobs.Contains("actor") || jobs.Contains("actress") || jobs.Contains("director"))
            {
                personCode_movies.Add(id, moviesCodes);
                personCode_name.Add(id, name);
            }

            //Console.WriteLine(line);
        }
    }

    #endregion

    sw.Stop();
    Console.WriteLine($"ActorsDirectorsNames_IMDB: {sw.Elapsed}");
}


//для каждого фильма сопоставила код imdb и код movielens
void Read_links_IMDB_MovieLens()
{
    // 1,0114709,862

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    //Console.WriteLine("links_IMDB_MovieLens");

    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\links_IMDB_MovieLens.csv").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    var line_span = line.AsSpan();
    //    int index = line_span.IndexOf(',');
    //    string movielens = line_span.Slice(0, index).ToString();

    //    int index2 = line_span.Slice(index + 1).IndexOf(',');
    //    string imdb = line_span.Slice(index + 1, index2).ToString();

    //    movielens_imdb.Add(movielens, "tt" + imdb);
    //});
    #endregion

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

    #region
    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\MovieCodes_IMDB.tsv").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    //Console.WriteLine(line);

    //    var line_span = line.AsSpan();
    //    int index = line_span.IndexOf('\t');
    //    string id = line_span.Slice(0, index).ToString();

    //    int index2 = line_span.Slice(index + 1).IndexOf('\t');
    //    index = line_span.Slice(index2 + 1).IndexOf('\t');
    //    string name = line_span.Slice(index2, index).ToString();

    //    index2 = line_span.Slice(index + 1).IndexOf('\t');
    //    string lang = line_span.Slice(index, index2).ToString();

    //    if (lang == "US" || lang == "RU")
    //    {
    //        if (!code_movie.ContainsKey(id))
    //        {
    //            code_movie.Add(id, name);
    //            //Console.WriteLine(line);
    //        }
    //    }
    //});
    #endregion

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
            string lang = line_span.Slice(0, index).ToString();

            if (lang == "US" || lang == "RU")
            {
                if (!code_movie.ContainsKey(id))
                {
                    code_movie.Add(id, name);
                    //Console.WriteLine(line);
                }
            }
        }
    }

    #endregion


    sw.Stop();
    Console.WriteLine($"MovieCodes_IMDB: {sw.Elapsed}");
}


//для каждого фильма нашла рейтинг (код - рейтинг)
void Read_Ratings_IMDB()
{
    // tt0000001 (index 1)	5.8	(index 2) 1435

    //Console.WriteLine("Ratings_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    #region
    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\Ratings_IMDB.tsv").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    var line_span = line.AsSpan();
    //    int index = line_span.IndexOf('\t'); // 1
    //    string id = line_span.Slice(0, index).ToString();

    //    line_span = line_span.Slice(index + 1).ToString();
    //    index = line_span.IndexOf('\t'); // 2
    //    string rate = line_span.Slice(0, index).ToString();
    //});
    #endregion

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

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 2
            string rate = line_span.Slice(0, index).ToString();

            movieCode_rating.Add(id, rate);
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
    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\TagCodes_MovieLens.csv").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    var line_span = line.AsSpan();
    //    int index = line_span.IndexOf(',');

    //    string id = line_span.Slice(0, index).ToString();
    //    string tag = line_span.Slice(index + 1).ToString();

    //    code_tag.Add(id, tag);
    //});
    #endregion

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
            string tag = line_span.Slice(index + 1).ToString();
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
    //var all_lines = File.ReadAllLines
    //    (@"C:\Users\HP\Desktop\Фильмы\ml-latest\TagScores_MovieLens.csv").Skip(1);
    //Parallel.ForEach(all_lines, line =>
    //{
    //    var line_span = line.AsSpan();
    //    int index = line_span.IndexOf(',');
    //    string id_movielens = line_span.Slice(0, index).ToString();

    //    int index2 = line_span.Slice(index + 1).IndexOf(',');
    //    string id_imdb = movielens_imdb[id_movielens];
    //    string tag_id = line_span.Slice(index, index2).ToString();
    //    string level = line_span.Slice(index2).ToString();

    //    if (code_movie.ContainsKey(id_imdb))
    //    {
    //        if (!tag_movies.ContainsKey(tag_id))
    //        {
    //            tag_movies.Add(tag_id, new HashSet<string>());
    //        }

    //        if (!imdb_tags.ContainsKey(id_imdb))
    //        {
    //            imdb_tags.Add(id_imdb, new HashSet<string>());
    //        }

    //        level = level.Replace(".", ",");
    //        double relevance = Convert.ToDouble(level);

    //        if (relevance > 0.5)
    //        {
    //            imdb_tags[id_imdb].Add(code_tag[tag_id]);
    //            tag_movies[tag_id].Add(code_movie[id_imdb]);
    //        }
    //    }
    //});
    #endregion

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

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf(','); // 2

            string id_imdb = movielens_imdb[id_movielens];
            string tag_id = line_span.Slice(0, index).ToString();
            string level = line_span.Slice(index + 1).ToString();

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
                level = level.Replace('.', ',');
                double relevance = Convert.ToDouble(level);
                if (relevance > 0.5)
                {
                    imdb_tags[id_imdb].Add(code_tag[tag_id]);
                    tag_movies[tag_id].Add(code_movie[id_imdb]);
                }
            }
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"TagScores_MovieLens: {sw.Elapsed}");

}

void UI()
{
    sw_.Stop();
    Console.WriteLine($"END: {sw_.Elapsed}");

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

                if (actors_directors_names_movies.ContainsKey(name))
                {
                    foreach (var m in actors_directors_names_movies[name])
                    {
                        Films_2._2.Movie.PrintMovie(m);
                    }
                }
                break;
            case "Movie":
                Console.WriteLine("ENTRY MOVIE'S NAME");
                name = Console.ReadLine();

                if (name_movie.ContainsKey(name))
                {
                    Films_2._2.Movie.PrintMovie(name_movie[name]);
                }
                break;
            case "Tag":
                Console.WriteLine("ENTRY TAG'S NAME");
                name = Console.ReadLine();

                if (tags_movies.ContainsKey(name))
                {
                    foreach (var m in tags_movies[name])
                    {
                        Films_2._2.Movie.PrintMovie(m);
                    }
                }
                break;
        }

    }
}