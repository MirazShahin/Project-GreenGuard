namespace GreenGuard.Models
{
    public class PlantationUpdate
    {
        public string? Id { get; set; }
        public string VolunteerId { get; set; }
        public string Zone { get; set; }
        public string TreeSpecies { get; set; }
        public int TreesPlanted { get; set; }
        public DateTime Date { get; set; }
         
        public string? ProofFileBase64 { get; set; }
        public string? ProofFileName { get; set; }
        public string? ProofFileType { get; set; }
    }
}
