namespace OurRecipes.Models
{
    public class Unit
    {
        public int Id { get; set; }    
        public Language Language { get; set; }
        public bool MetricSystem { get; set; }
        public string Singular { get; set; }
        public string Plural { get; set; }
        public string AbbreviationSingle { get; set; }
        public string AbbreviationPlural { get; set; }
    }
}
