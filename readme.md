## NuGet

The release package is published to:

http://nuget.org/List/Packages/WP7LongListSelectorHelper

So you can fetch this using:

PM> Install-Package WP7LongListSelectorHelper

## Long List helper classes

Cirrious.LongList provides helper classes for use with the LongListSelector from the Silverlight Toolkit for Windows Phone (Mango)

The entry point for all of these helper classes are the IEnumerable<T> extension methods in LongListShaper.cs

The use of the extension methods is demonstrated in the LongListApp sample:

```
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
```

## The future....

Hopefully this project will be useful in WP7Contrib...