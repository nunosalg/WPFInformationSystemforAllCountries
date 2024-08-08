namespace WPFProjetoFinalPaises.Models
{
    public class Currency
    {
        public string Name { get; set; } = "n/d";

        public string Symbol { get; set; } = "n/d";

        public override string ToString()
        {
            return $"{Name} ({Symbol})";
        }
    }
}
