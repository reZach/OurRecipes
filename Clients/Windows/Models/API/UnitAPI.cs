namespace OurRecipes.Models.API
{
    public class UnitAPI
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public bool MetricSystem { get; set; }
        public string Singular { get; set; }
        public string Plural { get; set; }
        public string AbbreviationSingle { get; set; }
        public string AbbreviationPlural { get; set; }
    }
}
