namespace WPFProjetoFinalPaises.Models
{
    public class Country
    {
        #region Properties

        public CountryName Name { get; set; }

        public bool? Independent { get; set; }

        public string Cca3 { get; set; } = "n/d";

        public List<string> Capital { get; set; } = new List<string>();

        public string Region { get; set; } = "n/d";

        public string Subregion { get; set; } = "n/d";

        public Dictionary<string, Currency> Currencies { get; set; } = new Dictionary<string, Currency>();

        public Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>();

        public double? Area { get; set; }

        public int? Population { get; set; }

        public Dictionary<string, double> Gini { get; set; } = new Dictionary<string, double>();

        public List<string> Borders { get; set; } = new List<string>();

        public Flag Flags { get; set; }

        #endregion

        #region Read-only properties
        public string BordersDisplay => Borders != null && Borders.Count != 0 ? string.Join(", ", Borders) : "None";

        public string IndependentDisplay => Independent.HasValue ? (Independent.Value ? "Yes" : "No") : "n/d";

        public string CapitalsDisplay => Capital != null && Capital.Count != 0 ? string.Join(", ", Capital) : "n/d";

        public string AreaDisplay => Area.HasValue ? $"{Area.Value} Km²" : "n/d";

        public string PopulationDisplay => Population.HasValue ? $"{Population.Value} inhabitants" : "n/d";

        public string GiniDisplay => Gini != null && Gini.Count != 0 ? string.Join(", ", Gini.Select(p => $"{p.Key} - {p.Value}%")) : "n/d";

        public string CurrenciesDisplay => Currencies != null && Currencies.Count != 0 ? string.Join(", ", Currencies.Values) : "n/d";

        public string LanguagesDisplay => Languages != null && Languages.Count != 0 ? string.Join(", ", Languages.Values) : "n/d";

        #endregion
    }
}
