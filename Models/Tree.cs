namespace GreenGuard.Models
{
    public class Tree
    {
        public string? Id { get; set; }
        public string? Name { get; set; }     // ✔ Correct
        public string? Description { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
    }
}
