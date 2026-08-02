namespace MovieDatabase.Model
{
    public class FilterCriteria
    {
        public string? TitleFilter { get; set; }
        public string? DescriptionFilter { get; set; }
        public string? DirectorFilter { get; set; }
        public string? ActorsFilter { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public bool? Favorite { get; set; }
        public List<Guid> TagIds { get; set; } = new();

        public bool IsActive =>
            !string.IsNullOrWhiteSpace(TitleFilter) ||
            !string.IsNullOrWhiteSpace(DescriptionFilter) ||
            !string.IsNullOrWhiteSpace(DirectorFilter) ||
            !string.IsNullOrWhiteSpace(ActorsFilter) ||
            YearFrom != null ||
            YearTo != null ||
            Favorite != null ||
            TagIds.Count > 0;

        public void Reset()
        {
            TitleFilter = null;
            DescriptionFilter = null;
            DirectorFilter = null;
            ActorsFilter = null;
            YearFrom = null;
            YearTo = null;
            Favorite = null;
            TagIds.Clear();
        }

        public bool Matches(Movie movie)
        {
            if (!WildcardMatcher.IsMatch(movie.Title, TitleFilter))
            {
                return false;
            }

            if (!WildcardMatcher.IsMatch(movie.Description, DescriptionFilter))
            {
                return false;
            }

            if (!WildcardMatcher.IsMatch(movie.Director, DirectorFilter))
            {
                return false;
            }

            if (!WildcardMatcher.IsMatch(movie.ActorsDisplay, ActorsFilter))
            {
                return false;
            }

            if (YearFrom != null || YearTo != null)
            {
                if (movie.Year == null)
                {
                    return false;
                }

                int year = movie.Year.Value.Year;

                if (YearFrom != null && year < YearFrom)
                {
                    return false;
                }

                if (YearTo != null && year > YearTo)
                {
                    return false;
                }
            }

            if (Favorite != null && movie.Favorite != Favorite)
            {
                return false;
            }

            if (TagIds.Count > 0 && !TagIds.Any(id => movie.Tags.Contains(id)))
            {
                return false;
            }

            return true;
        }
    }
}
