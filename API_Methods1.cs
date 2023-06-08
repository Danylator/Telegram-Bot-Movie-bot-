using System.Net.Http.Headers;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net;
using static API_Space.API_Methods2;

namespace API_Space
{
    public class API_Methods1
    {
        public static async Task<string[]> GetMovieByNameAndGenere(string Year, string Genere)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://moviesminidatabase.p.rapidapi.com/movie/byYear/{Year}/byGen/{Genere}/"),
                Headers =
        {
            { "X-RapidAPI-Key", "82c179a39cmsh5367586e51f123fp1a5c93jsnd2ceeb91502e" },
            { "X-RapidAPI-Host", "moviesminidatabase.p.rapidapi.com" },
        },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

             
                var movieData = JsonSerializer.Deserialize<MovieData>(body);

                if (movieData != null)
                {

                    var movies = movieData.results;


                    var movieTitles = new string[movies.Count];


                    for (int i = 0; i < movies.Count; i++)
                    {
                        movieTitles[i] = movies[i].title;
                    }

                    if (movieTitles.Length == 0)
                    {
                        return new[] { $"{Year} {Genere} movie was not found" };
                    }
                    return movieTitles;
                }
                else
                {
                    Console.WriteLine("Failed to deserialize JSON response.");
                }
            }

            throw new Exception("Failed to get movie data.");
        }

        public static async Task<string[]> GetAllGeneres()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://moviesminidatabase.p.rapidapi.com/genres/"),
                Headers =
        {
            { "X-RapidAPI-Key", "82c179a39cmsh5367586e51f123fp1a5c93jsnd2ceeb91502e" },
            { "X-RapidAPI-Host", "moviesminidatabase.p.rapidapi.com" },
        },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();


                var genreData = JsonSerializer.Deserialize<GenreData>(body);

                if (genreData != null)
                {

                    var genres = genreData.results;

                    var genreNames = new string[genres.Count];


                    for (int i = 0; i < genres.Count; i++)
                    {
                        genreNames[i] = genres[i].genre;
                    }


                    return genreNames;
                }
                else
                {
                    Console.WriteLine("Failed to deserialize JSON response.");
                }
            }

            throw new Exception("Failed to get genre data.");
        }
        public static async Task<string> GetAllGeneresString()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://moviesminidatabase.p.rapidapi.com/genres/"),
                Headers =
        {
            { "X-RapidAPI-Key", "82c179a39cmsh5367586e51f123fp1a5c93jsnd2ceeb91502e" },
            { "X-RapidAPI-Host", "moviesminidatabase.p.rapidapi.com" },
        },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();


                var genreData = JsonSerializer.Deserialize<GenreData>(body);

                if (genreData != null)
                {

                    var genres = genreData.results;

                    var genreNames = "";


                    for (int i = 0; i < genres.Count; i++)
                    {
                        genreNames += genres[i].genre + "\n";
                    }

                    return genreNames;
                }
                else
                {
                    Console.WriteLine("Failed to deserialize JSON response.");
                }
            }

            throw new Exception("Failed to get genre data.");
        }

        public class GenreData
        {
            public List<Genre> results { get; set; }
        }

        public class Genre
        {
            public string genre { get; set; }
        }

        public class MovieData
        {
            public LinkData links { get; set; }
            public int count { get; set; }
            public List<Movie> results { get; set; }
        }

        public class LinkData
        {
            public string next { get; set; }
            public string previous { get; set; }
        }

        public class Movie
        {
            public string imdb_id { get; set; }
            public string title { get; set; }
        }

    }
    public class API_Methods2
    {
        public class PrimaryImage
        {
            public string Url { get; set; }
        }

        public class ResultEntry
        {
            public PrimaryImage PrimaryImage { get; set; }
        }

        public class MovieSearchResponse
        {
            public ResultEntry[] Results { get; set; }
        }



        public static async Task<string> GetImage(string MovieName)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://moviesdatabase.p.rapidapi.com/titles/search/title/{MovieName}?exact=true&titleType=movie"),
                Headers =
        {
            { "X-RapidAPI-Key", "82c179a39cmsh5367586e51f123fp1a5c93jsnd2ceeb91502e" },
            { "X-RapidAPI-Host", "moviesdatabase.p.rapidapi.com" },
        },
            };

            try
            {
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();

                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        };

                        var movieSearchResponse = JsonSerializer.Deserialize<MovieSearchResponse>(body, options);

                        if (movieSearchResponse.Results != null && movieSearchResponse.Results.Length > 0)
                        {
                            var primaryImage = movieSearchResponse.Results[0].PrimaryImage;
                            if (primaryImage != null && primaryImage.Url != null)
                            {
                                var primaryImageUrl = primaryImage.Url;
                                return primaryImageUrl;
                            }
                        }

                        return "No Image found";
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return "No Image found";
                    }
                    else
                    {
                       //something here
                    }
                }
            }
            catch (Exception ex)
            {
              Console.WriteLine($"{ex} in GetImage method");
            }

            return "Error occurred";
        }


    }
}