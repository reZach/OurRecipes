namespace OurRecipes.Models.API
{
    public class MeasurementAPI
    {
        public int Id { get; set; }
        public int UnitId { get; set; }
        public decimal Amount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}
