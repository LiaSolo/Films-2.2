using Films_2._2;

//links_IMDB_MovieLens: 00:00:00.4163476
//TagCodes_MovieLens: 00:00:00.0089323
//MovieCodes_IMDB: 00:00:32.4184226
//Ratings_IMDB: 00:00:07.7732367
//ActorsDirectorsNames_IMDB: 00:01:21.1104432
//TagScores_MovieLens: 00:01:15.3871955
//ActorsDirectorsCodes_IMDB: 00:01:18.0094345
//code_movie: 1245161
//Make_name_movie_dict: 00:00:00.5521773
//name_movie: 11583
//Make_tags_movies_dict: 00:00:00.4740495
//Make_actors_directors_names_movies_dict: 00:00:08.0807774
//END: 00:02:48.2860912



Dictionary<string, Movie> code_movie = new Dictionary<string, Movie>();                                // код фильма - класс фильма
Dictionary<string, string> code_tag = new Dictionary<string, string>();                                  // код тега - название тега
Dictionary<string, string> movielens_imdb = new Dictionary<string, string>();                              // код movielens - код imdb


Dictionary<string, HashSet<string>> personCode_movies = new Dictionary<string, HashSet<string>>();              // код человека - названия фильмов с ним
Dictionary<string, string> personCode_name = new Dictionary<string, string>();                          // код человека - имя человека
Dictionary<string, HashSet<string>> tag_movies_names = new Dictionary<string, HashSet<string>>();           // код тега - названия фильмов с этим тегом


Dictionary<string, Movie> name_movie = new Dictionary<string, Movie>();                        // название фильма - объект класса фильм
Dictionary<string, HashSet<Movie>> actors_directors_names_movies = new Dictionary<string, HashSet<Movie>>(); // имя человека - объекты класса фильм
Dictionary<string, HashSet<Movie>> tags_movies = new Dictionary<string, HashSet<Movie>>(); // // название тега - объекты класса фильм

HashSet<string> actors = new HashSet<string>();
HashSet<string> directors = new HashSet<string>();
HashSet<string> tags = new HashSet<string>();
HashSet<string> movies = new HashSet<string>();

System.Diagnostics.Stopwatch sw_ = new System.Diagnostics.Stopwatch();
sw_.Start(); //конец в UI


#region
Task a_d_names = Task.Factory.StartNew(() =>
  Read_ActorsDirectorsNames_IMDB());

Task movie_codes = Task.Factory.StartNew(() =>
   Read_MovieCodes_IMDB());

Task a_d_codes = Task.WhenAll(new Task[] { a_d_names, movie_codes }).ContinueWith(t =>
  Read_ActorsDirectorsCodes_IMDB());

Task rating = movie_codes.ContinueWith(t =>
   Read_Ratings_IMDB());

Task links = Task.Factory.StartNew(() =>
   Read_links_IMDB_MovieLens());

Task tag_codes = Task.Factory.StartNew(() =>
   Read_TagCodes_MovieLens());

Task tag_scores = Task.WhenAll(new Task[] { links, movie_codes, tag_codes }).ContinueWith(t =>
  Read_TagScores_MovieLens());



Task precalc = Task.WhenAll(new Task[] { a_d_codes, rating, tag_scores }).ContinueWith(t
    => Make_precalc());

precalc.Wait();

//Task dict_actors_directors_movies = dict_name_movie.ContinueWith(t =>
//  Make_actors_directors_names_movies_dict());

//Task dict_tags_movies = dict_name_movie.ContinueWith(t =>
//  Make_tags_movies_dict());



//Task ui = Task.WhenAll(new Task[] { dict_actors_directors_movies, dict_tags_movies }).ContinueWith(t
//    => UI());

//ui.Wait();



#endregion


// Последовательности:

// Read_Ratings_IMDB
// Read_ActorsDirectorsNames_IMDB -- Read_ActorsDirectorsCodes_IMDB
// Read_MovieCodes_IMDB -----------/
// Read_TagCodes_MovieLens----\    \
// Read_links_IMDB_MovieLens --Read_TagScores_MovieLens
    

void Make_precalc()
{
    Console.WriteLine($"code_movie: {code_movie.Count}");
    

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    foreach (var m in code_movie)
    {
        string id = m.Key;
        Movie movie = m.Value;
        string name = movie.Name;

        if (name_movie.ContainsKey(name))
        {
            continue;
        }

        if (movie.Rating != "-1" && movie.Actors.Count > 0
            && movie.Directors.Count > 0 && movie.Tags.Count > 0)
        {
            using (StreamWriter writer = new StreamWriter("movies.txt", true))
            {
                writer.WriteLine(id);
            }

            foreach (string a in movie.Actors)
            {
                actors.Add(a);
            }

            foreach (string d in movie.Directors)
            {
                directors.Add(d);
            }

            foreach (string t in movie.Tags)
            {
                tags.Add(t);
            }
        }

    }

    using (StreamWriter writer = new StreamWriter("actors.txt"))
    {
        foreach (string a in actors)
        {
            writer.WriteLine(a);
        }
    }

    using (StreamWriter writer = new StreamWriter("directors.txt"))
    {
        foreach (string a in directors)
        {
            writer.WriteLine(a);
        }
    }

    using (StreamWriter writer = new StreamWriter("tags.txt"))
    {
        foreach (string a in tags)
        {
            writer.WriteLine(a);
        }
    }

    sw.Stop();
    Console.WriteLine($"Make_name_movie_dict: {sw.Elapsed}");
    //Console.WriteLine($"name_movie: {name_movie.Count}");
}

#region
//void Make_actors_directors_names_movies_dict()
//{
//    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//    sw.Start();

//    foreach (var p in personCode_movies)
//    {
//        string id = p.Key;
//        string name = personCode_name[id];

//        if (actors_directors_names_movies.ContainsKey(name))
//        {
//            continue;
//        }

//        HashSet<Movie> movies = new HashSet<Movie>();
//        foreach (string m_n in p.Value)
//        {
//            if (name_movie.ContainsKey(m_n))
//            {
//                movies.Add(name_movie[m_n]);
//            }
//        }

//        if (movies.Count > 0)
//        {
//            actors_directors_names_movies.Add(name, movies);
//        }

//    }

//    sw.Stop();
//    Console.WriteLine($"Make_actors_directors_names_movies_dict: {sw.Elapsed}");
//}

//void Make_tags_movies_dict()
//{
//    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//    sw.Start();

//    foreach (var t in tag_movies_names)
//    {
//        string id = t.Key;
//        string name = code_tag[id];

//        if (tags_movies.ContainsKey(name))
//        {
//            continue;
//        }

//        HashSet<Movie> movies = new HashSet<Movie>();
//        foreach (string m_n in t.Value)
//        {
//            if (name_movie.ContainsKey(m_n))
//            {
//                movies.Add(name_movie[m_n]);
//            }              
//        }

//        if (movies.Count > 0)
//        {
//            tags_movies.Add(name, movies);
//        }
//    }

//    sw.Stop();
//    Console.WriteLine($"Make_tags_movies_dict: {sw.Elapsed}");
//}

#endregion


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

            if (personCode_name.ContainsKey(id_person))
            {
                if (!personCode_movies.ContainsKey(id_person) && (job == "director" || job == "actor" || job == "actress"))
                {
                    personCode_movies.Add(id_person, new HashSet<string>());
                }

                if (job == "director")
                {
                    code_movie[id_movie].Directors.Add(id_person);
                    //personCode_movies[id_person].Add(code_movie[id_movie].Name);
                }
                else if (job == "actor" || job == "actress")
                {
                    code_movie[id_movie].Actors.Add(id_person);
                    //personCode_movies[id_person].Add(code_movie[id_movie].Name);
                }
            }
            //Console.WriteLine(line);
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"ActorsDirectorsCodes_IMDB: {sw.Elapsed}");
}

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

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 3

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 4

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 5

            personCode_name.Add(id, name);

            //Console.WriteLine(line);
        }
    }

    #endregion

    sw.Stop();
    Console.WriteLine($"ActorsDirectorsNames_IMDB: {sw.Elapsed}");
}

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

void Read_MovieCodes_IMDB()
{
    // tt0000001 (index 1)	1 (index 2)	Carmencita - spanyol tánc (index 3)	HU (index 4)	\N (index 5)	imdbDisplay	\N	0

    //Console.WriteLine("MovieCodes_IMDB");

    //HashSet<string> region = new HashSet<string>();
    //HashSet<string> language = new HashSet<string>();

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    sw.Start();

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

            //language.Add(lang); //"" ""
            //region.Add(reg);

            if (reg == "US" || reg == "RU" || reg == "GB" || lang == "en" || lang == "ru")
            {
                if (!code_movie.ContainsKey(id))
                {
                    Movie movie = new Movie(name);
                    code_movie.Add(id, movie);
                    //Console.WriteLine(line);
                }
            }
        }
    }

    #endregion


    sw.Stop();
    Console.WriteLine($"MovieCodes_IMDB: {sw.Elapsed}");
}

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

            line_span = line_span.Slice(index + 1).ToString();
            index = line_span.IndexOf('\t'); // 2
            string rate = line_span.Slice(0, index).ToString();
            
            if (code_movie.ContainsKey(id))
            {
                code_movie[id].Rating = rate;
            }
            //Console.WriteLine(line);
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"Ratings_IMDB: {sw.Elapsed}");
}

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
            string tag = line_span.Slice(index + 1).ToString();
            code_tag.Add(id, tag);
            //Console.WriteLine(line);
        }
    }
    #endregion

    sw.Stop();
    Console.WriteLine($"TagCodes_MovieLens: {sw.Elapsed}");
}

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
                if (!tag_movies_names.ContainsKey(tag_id))
                {
                    tag_movies_names.Add(tag_id, new HashSet<string>());
                }

                level = level.Replace('.', ',');
                double relevance = Convert.ToDouble(level);
                if (relevance > 0.5)
                {
                    code_movie[id_imdb].Tags.Add(tag_id);
                    //tag_movies_names[tag_id].Add(code_movie[id_imdb].Name);
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
                    foreach (Movie m in actors_directors_names_movies[name])
                    {

                        Console.WriteLine(m.Name);
                    }
                }
                else
                {
                    Console.WriteLine("THE PERSON DOES'T EXIST");
                }

                break;
            case "Movie":
                Console.WriteLine("ENTRY MOVIE'S NAME");
                name = Console.ReadLine();

                if (name_movie.ContainsKey(name))
                {
                    Movie.PrintMovie(name_movie[name]);
                }
                else
                {
                    Console.WriteLine("THE MOVIE DOES'T EXIST");
                }

                break;
            case "Tag":
                Console.WriteLine("ENTRY TAG'S NAME");
                name = Console.ReadLine();

                if (tags_movies.ContainsKey(name))
                {
                    foreach (Movie m in tags_movies[name])
                    {
                        Console.WriteLine(m.Name);
                    }
                }
                else
                {
                    Console.WriteLine("THE TAG DOES'T EXIST");
                }

                break;
        }

    }
}