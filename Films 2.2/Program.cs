

//                           До рефакторинга  // Parallel + AsSpan // AsSpan (Parallel в Rating)
//links_IMDB_MovieLens:      00:00:00.4315613 // 00:00:00.0826156  // 00:00:00.0800730
//TagCodes_MovieLens:        00:00:00.0020740 // 00:00:00.0123031  // 00:00:00.0017357
//Ratings_IMDB:              00:00:04.0039178 // 00:00:01.0276114  // 00:00:01.2489364
//MovieCodes_IMDB:           00:00:08.8379080 // 00:00:15.1513265  // 00:00:04.7125392
//TagScores_MovieLens:       00:01:06.2264605 // 00:03:32.8199461  // 00:00:18.9300531
//ActorsDirectorsNames_IMDB: 00:01:20.6478404 // 00:04:54.0480671  // 00:00:33.9521709
//ActorsDirectorsCodes_IMDB: 00:01:08.4717690 // 00:09:35.3564674  // 00:00:28.1936335





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


dict_name_movie.Wait();

//Task write_ans = dict_name_movie.ContinueWith(t =>
//{
//    foreach (var m in name_movie)
//    {
//        Console.WriteLine(m.Key);
//        Films_2._2.Movie.PrintMovie(m.Value);
//    }

//});

//write_ans.Wait();
#endregion


//Read_links_IMDB_MovieLens();
//Read_ActorsDirectorsNames_IMDB();
//Read_MovieCodes_IMDB();
//Read_TagCodes_MovieLens();
//Read_TagScores_MovieLens();
//Read_ActorsDirectorsCodes_IMDB();
//Read_Ratings_IMDB();

Console.WriteLine("OK");

// Последовательности:

// Read_Ratings_IMDB
//                                  /Read_ActorsDirectorsNames_IMDB -- Read_ActorsDirectorsCodes_IMDB
// Read_MovieCodes_IMDB ----------<
// Read_TagCodes_MovieLens---\     \
//                            >Read_TagScores_MovieLens
// Read_links_IMDB_MovieLens /


Dictionary<string, Films_2._2.Movie> Make_name_movie_dict()
{
    Console.WriteLine("Make_name_movie_dict");

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
    //tt0000001 (index)  2 (index2)  nm0005690 (index) director {index2) \N  \N

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

            int index = line_span.IndexOf('\t');
            string id_movie = line_span.Slice(0, index).ToString();

            int index2 = line_span.Slice(index + 1).IndexOf('\t');
            index = line_span.Slice(index2 + 1).IndexOf('\t');
            string id_person = line_span.Slice(index2, index).ToString();

            index2 = line_span.Slice(index + 1).IndexOf('\t');
            string job = line_span.Slice(index, index2).ToString();

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
                else if (job == "actor")
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
    #endregion

    sw.Stop();
    Console.WriteLine($"ActorsDirectorsCodes_IMDB: {sw.Elapsed}");
}


//для каждого актёра и режиссёра получила наборы фильмов с участием (код - название)
//для каждого актёра и режиссёра  сопоставила код и имя
void Read_ActorsDirectorsNames_IMDB()
{
    // nm0000002 (index)	Lauren Bacall (index2)	1924	(index) 2014	(index2) actress,soundtrack 	(index)   tt0117057,tt0037382,tt0038355,tt0071877

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

            int index = line_span.IndexOf('\t');
            string id = line_span.Slice(0, index).ToString();

            int index2 = line_span.Slice(index + 1).IndexOf('\t');
            string name = line_span.Slice(index, index2).ToString();

            index = line_span.Slice(index2 + 1).IndexOf('\t');
            index2 = line_span.Slice(index + 1).IndexOf('\t');

            index = line_span.Slice(index2 + 1).IndexOf('\t');
            List<string> jobs = new List<string>();
            jobs.AddRange(line_span.Slice(index2, index).ToString().Split(','));

            List<string> moviesCodes = new List<string>();
            moviesCodes.AddRange(line_span.Slice(index + 1).ToString().Split(','));

            List<string> moviesNames = new List<string>();

            foreach (string id_m in moviesCodes)
            {
                if (code_movie.ContainsKey(id_m))
                {
                    moviesNames.Add(code_movie[id_m]);
                }
            }

            if (jobs.Contains("actor") || jobs.Contains("actress") || jobs.Contains("director"))
            {
                personCode_movies.Add(id, moviesNames);
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
            int index = line_span.IndexOf(',');
            string movielens = line_span.Slice(0, index).ToString();

            int index2 = line_span.Slice(index + 1).IndexOf(',');
            string imdb = line_span.Slice(index + 1, index2).ToString();

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
    // tt0000001 (index)	1 (index2)	Carmencita - spanyol tánc (index)	HU (index2)	\N	imdbDisplay	\N	0

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
            int index = line_span.IndexOf('\t');
            string id = line_span.Slice(0, index).ToString();

            int index2 = line_span.Slice(index + 1).IndexOf('\t');
            index = line_span.Slice(index2 + 1).IndexOf('\t');
            string name = line_span.Slice(index2, index).ToString();

            index2 = line_span.Slice(index + 1).IndexOf('\t');
            string lang = line_span.Slice(index, index2).ToString();

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
    // tt0000001 (index)	5.8	1435

    //Console.WriteLine("Ratings_IMDB");

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    var all_lines = File.ReadAllLines
        (@"C:\Users\HP\Desktop\Фильмы\ml-latest\Ratings_IMDB.tsv").Skip(1);
    Parallel.ForEach(all_lines, line =>
    {
        var line_span = line.AsSpan();
        int index = line_span.IndexOf('\t');
        string id = line_span.Slice(0, index).ToString();

        int index2 = line_span.Slice(index + 1).IndexOf('\t');
        string rate = line_span.Slice(index, index2).ToString();
    });

    #region

    //using (StreamReader sr = new StreamReader(@"C:\Users\HP\Desktop\Фильмы\ml-latest\Ratings_IMDB.tsv"))
    //{
    //    //Console.WriteLine("Ratings_IMDB");
    //    string line = sr.ReadLine();

    //    while ((line = sr.ReadLine()) != null)
    //    {
    //        string[] row = line.Split('\t');
    //        string id = row[0];
    //        string rate = row[1];

    //        movieCode_rating.Add(id, rate);
    //        //Console.WriteLine(line);
    //    }
    //}
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
            int index = line_span.IndexOf(',');
            string id_movielens = line_span.Slice(0, index).ToString();

            int index2 = line_span.Slice(index + 1).IndexOf(',');
            string id_imdb = movielens_imdb[id_movielens];
            string tag_id = line_span.Slice(index, index2).ToString();
            string level = line_span.Slice(index2).ToString();

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
    #endregion

    sw.Stop();
    Console.WriteLine($"TagScores_MovieLens: {sw.Elapsed}");
}

