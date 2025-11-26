namespace GreenGuard.Models
{
    public class VolunteerUpdate
    {
        public string? Id { get; set; }

        public string VolunteerId { get; set; } = "";
        public string VolunteerName { get; set; } = "";
        public string Zone { get; set; } = "";
        public string UpdateText { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string Status { get; set; } = "Pending";   

        public DateTime Date { get; set; } 
        public string DisplayDate => Date.ToString("MMM dd, yyyy — hh:mm tt");

        public string StatusColor =>
            Status switch
            {
                "Approved" => "LightGreen",
                "Rejected" => "Red",
                _ => "Orange"
            };
    }
}
