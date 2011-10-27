using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Cirrious.LongList;
using PhoneToolkitSample.Data;

namespace LongListApp.Model
{
    public class MainViewModel
    {
        public ShapedLongList<string, Movie> MoviesA2Z { get; set; }
        public ShapedLongList<string, Movie> MoviesA2ZSparse { get; set; }
        public ShapedLongList<string, Movie> MoviesByCategory { get; set; }
        public ShapedLongList<string, Movie> MoviesByRating { get; set; }
        public ShapedLongList<MovieDurationGroup, Movie> MoviesByRunTime { get; set; }
        public ShapedLongList<string, Movie> MoviesByStar { get; set; }
        public ShapedLongList<string, Movie> MoviesByStarWithEmpties { get; set; }

        public MainViewModel()
        {
            // build a random data set
            var movies = new List<Movie>();
            for (var i = 0; i < 50; i++)
                movies.Add(Movie.CreateRandom());

            // demonstrates automatic alphabetical listing - using # and A->Z and providing empty slots for non-populated keys
            MoviesA2Z = movies.ToFullAlphabeticalLongListShape((movie) => movie.Title);

            // demonstrates custom alphabetical listing
            MoviesA2ZSparse = movies.ToLongListShape((movie) => movie.Title.Substring(0, 1));

            // demonstrates the simplest use - movies will be grouped by category, both groups and movies will be sorted alphabetically
            MoviesByCategory = movies.ToLongListShape((movie) => movie.Category);

            // demonstrates custom keys, custom sort orders, and a default list of keys
            MoviesByRating = movies.ToLongListShape((movie) => movie.Rating, itemComparer: new MovieRunTimeComparer(), keyComparer: new MovieRatingComparer(), defaultKeys: DefaultMovieRatings);

            // demonstrates custom sort orders
            MoviesByRunTime = movies.ToLongListShape((movie) => TimeSpanToTenMinuteGroup(movie.RunTime), keyComparer: new MovieRunTimeGroupComparer(), itemComparer: new MovieRunTimeComparer(), defaultKeys: GenerateMovieRunTimeKeys());

            // demonstrates multiple keys per item
            MoviesByStar = movies.ToLongListShape<string, Movie>((movie) => new string[] { movie.Star1, movie.Star2 });

            // demonstrates multiple keys per item, plus how to add empty groups
            MoviesByStarWithEmpties = movies.ToLongListShape<string, Movie>((movie) => new string[] { movie.Star1, movie.Star2 })
                                        .AddKeys(AdditionalStars);
        }

        private static readonly string[] AdditionalStars = new string[]
                                                               {
                                                                   "Fred Flintstone",
                                                                   "Barney Rubble",
                                                                   "Wilma Flintstone",
                                                                   "Betty Rubble",
                                                               };

        private static readonly string[] DefaultMovieRatings = new string[] { "G", "PG", "PG-9", "PG-13", "R", "XXX" };

        public class MovieDurationGroup
        {
            public TimeSpan MinDurationInclusive { get; set; }
            public TimeSpan MaxDurationInclusive { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                var mdg = obj as MovieDurationGroup;
                if (mdg == null)
                    return false;

                return MinDurationInclusive == mdg.MinDurationInclusive
                       && MaxDurationInclusive == mdg.MaxDurationInclusive;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("{0:HH:mm:ss} to {1:HH:mms:ss}", MinDurationInclusive, MaxDurationInclusive);
            }
        }

        public class TenMinuteMovieDurationGroup : MovieDurationGroup
        {
            public TenMinuteMovieDurationGroup(TimeSpan minDuration)
            {
                MinDurationInclusive = minDuration;
                MaxDurationInclusive = minDuration.Add(TimeSpan.FromMinutes(10.0)).Subtract(TimeSpan.FromSeconds(1.0));
            }
        }

        public class MovieRunTimeGroupComparer : IComparer<MovieDurationGroup>
        {
            public int Compare(MovieDurationGroup x, MovieDurationGroup y)
            {
                return x.MinDurationInclusive.CompareTo(y.MinDurationInclusive);
            }
        }

        public class MovieRunTimeComparer : IComparer<Movie>
        {
            public int Compare(Movie x, Movie y)
            {
                return x.RunTime.CompareTo(y.RunTime);
            }
        }

        public class MovieRatingComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return RatingToInt(x).CompareTo(RatingToInt(y));
            }

            private static int RatingToInt(string rating)
            {
                switch (rating)
                {
                    case "G":
                        return 1;
                    case "PG-9":
                        return 2;
                    case "PG-13":
                        return 3;
                    case "PG":
                        return 4;
                    case "R":
                        return 5;
                    case "XXX":
                        return 6;
                }

                throw new ArgumentOutOfRangeException("unexpected rating was " + rating);
            }
        }

        private static MovieDurationGroup TimeSpanToTenMinuteGroup(TimeSpan input)
        {
            var start = TimeSpan.FromMinutes(10.0 * Math.Floor(input.TotalMinutes / 10.0));
            return new TenMinuteMovieDurationGroup(start);
        }

        private static IEnumerable<MovieDurationGroup> GenerateMovieRunTimeKeys()
        {
            var list = new List<MovieDurationGroup>();
            for (var timeSpan = TimeSpan.Zero; timeSpan < TimeSpan.FromHours(4.0); timeSpan = timeSpan.Add(TimeSpan.FromMinutes(10.0)))
            {
                list.Add(new TenMinuteMovieDurationGroup(timeSpan));
            }
            return list;
        }
    }
}
