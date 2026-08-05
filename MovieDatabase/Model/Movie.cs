using System.Text.Json.Serialization;

namespace MovieDatabase.Model
{
    public partial class Movie : ObservableObject
    {
        public Guid Id { get; set; }

        [ObservableProperty]
        public partial string? Title { get; set; }

        [ObservableProperty]
        public partial string? Description { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasDirector))]
        public partial string? Director { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ActorsDisplay))]
        [NotifyPropertyChangedFor(nameof(HasActors))]
        public partial string[]? Actors { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CountryYearDisplay))]
        [NotifyPropertyChangedFor(nameof(HasCountryYear))]
        public partial DateOnly? Year { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CountryYearDisplay))]
        [NotifyPropertyChangedFor(nameof(HasCountryYear))]
        public partial string? Country { get; set; }

        [ObservableProperty]
        public partial bool Favorite { get; set; }

        [JsonIgnore]
        [ObservableProperty]
        public partial bool IsMarked { get; set; }

        public List<Guid> Tags { get; set; } = new List<Guid>();

        [ObservableProperty]
        public partial string? DisplayId { get; set; }

        [JsonIgnore]
        public string ActorsDisplay => Actors is null ? string.Empty : string.Join(", ", Actors);

        [JsonIgnore]
        public string CountryYearDisplay
        {
            get
            {
                string? year = Year?.Year.ToString();

                if (string.IsNullOrEmpty(Country))
                {
                    return year ?? string.Empty;
                }

                return year is null ? Country : $"{Country} {year}";
            }
        }

        [JsonIgnore]
        public bool HasCountryYear => !string.IsNullOrEmpty(CountryYearDisplay);

        [JsonIgnore]
        public bool HasDirector => !string.IsNullOrEmpty(Director);

        [JsonIgnore]
        public bool HasActors => Actors is not null && Actors.Length > 0;

        [JsonIgnore]
        public List<TagBadge> TagBadges { get; set; } = new List<TagBadge>();

        public Movie()
        {
            Id = Guid.NewGuid();
        }

        public Movie(string title, DateOnly? year, string? displayId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Year = year;
            DisplayId = displayId;
        }
    }

    [JsonSerializable(typeof(List<Movie>))]
    internal sealed partial class MovieContext : JsonSerializerContext
    {

    }
}
