namespace OurRecipes.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public Unit Unit { get; set; }
        public decimal Amount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}
