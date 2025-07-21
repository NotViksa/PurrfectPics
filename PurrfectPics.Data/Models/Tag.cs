namespace PurrfectPics.Data.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<CatImage> CatImages { get; set; } = new List<CatImage>();
    }
}