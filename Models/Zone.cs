namespace GreenGuard.Models
{
    public class Zone
    {
        public string? Id { get; set; }
        public string ZoneName { get; set; } = "";
        public string LeaderName { get; set; } = "";
        public string ZoneType { get; set; } = "";      
        public string Owner { get; set; } = "";
        public string PermissionStatus { get; set; } = "";

        public bool ShowOwner => ZoneType == "Owner";
    }
}
