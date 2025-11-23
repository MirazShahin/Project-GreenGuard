namespace GreenGuard.Views
{
    public partial class ZoneManagementPage : ContentPage
    {
        private static List<ZoneInfo> zones = new();
        private static List<PermissionRequest> requests = new();
        private List<ZoneInfo> filteredZones = new();

        public ZoneManagementPage()
        {
            InitializeComponent();

            if (!zones.Any())
            {
                zones = new List<ZoneInfo>
                {
                    new ZoneInfo { ZoneName="Zone A", LeaderName="Leader A", ZoneType="Govt", PermissionStatus="Approved" },
                    new ZoneInfo { ZoneName="Zone B", LeaderName="Leader B", ZoneType="Owner", Owner="Mr. Rahman", PermissionStatus="Pending" },
                    new ZoneInfo { ZoneName="Zone C", LeaderName="Leader C", ZoneType="Public", PermissionStatus="Not Needed" }
                };
            }

            ApplyFilters();
        }

        private void RefreshUI()
        {
            ZoneCollectionView.ItemsSource = null;
            ZoneCollectionView.ItemsSource = filteredZones;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string searchText = ZoneSearchBar.Text?.ToLower() ?? "";
            string selectedFilter = ZoneTypeFilterPicker.SelectedItem?.ToString() ?? "All";

            filteredZones = zones
                .Where(z => (string.IsNullOrEmpty(searchText) || z.ZoneName.ToLower().Contains(searchText))
                            && (selectedFilter == "All" || z.ZoneType == selectedFilter))
                .ToList();

            RefreshUI();
        }

        private async void OnAddZoneClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Add Zone", "Enter zone name:");
            if (string.IsNullOrEmpty(name)) return;

            string type = await DisplayActionSheet("Select Zone Type", "Cancel", null, "Govt", "Owner", "Public");
            if (type == "Cancel" || string.IsNullOrEmpty(type)) return;

            string owner = "";
            if (type == "Owner")
            {
                owner = await DisplayPromptAsync("Owner", "Enter owner name:");
            }

            zones.Add(new ZoneInfo
            {
                ZoneName = name,
                ZoneType = type,
                Owner = owner,
                PermissionStatus = type == "Public" ? "Not Needed" : "Pending"
            });

            ApplyFilters();
        }

        private async void OnEditZoneClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is ZoneInfo zone)
            {
                string newName = await DisplayPromptAsync("Edit Zone", "Update zone name:", initialValue: zone.ZoneName);
                if (!string.IsNullOrEmpty(newName))
                    zone.ZoneName = newName;

                string newType = await DisplayActionSheet("Update Zone Type", "Cancel", null, "Govt", "Owner", "Public");
                if (newType != "Cancel" && !string.IsNullOrEmpty(newType))
                {
                    zone.ZoneType = newType;

                    if (newType == "Owner")
                    {
                        zone.Owner = await DisplayPromptAsync("Owner", "Enter owner name:", initialValue: zone.Owner);
                    }
                    else
                    {
                        zone.Owner = "";
                    }
                }

                ApplyFilters();
            }
        }

        private async void OnDeleteZoneClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is ZoneInfo zone)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete {zone.ZoneName}?", "Yes", "No");
                if (confirm)
                {
                    zones.Remove(zone);
                    ApplyFilters();
                }
            }
        }
        private async void OnChangeLeaderClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is ZoneInfo zone)
            {
                if (zone.ZoneType == "Govt")
                {
                    await DisplayAlert("Permission Required", $"{zone.ZoneName} is Govt Controlled. Govt approval required!", "OK");
                    return;
                }
                else if (zone.ZoneType == "Owner")
                {
                    await DisplayAlert("Permission Required", $"{zone.ZoneName} belongs to {zone.Owner}. Owner permission required!", "OK");
                    return;
                }

                string newLeader = await DisplayPromptAsync("Change Leader", $"Enter new leader for {zone.ZoneName}:", "OK", "Cancel", initialValue: zone.LeaderName);
                if (!string.IsNullOrEmpty(newLeader))
                {
                    zone.LeaderName = newLeader;
                    await DisplayAlert("Success", $"Leader for {zone.ZoneName} updated to {newLeader}.", "OK");
                }

                ApplyFilters();
            }
        }
        private async void OnRequestPermissionClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is ZoneInfo zone)
            {
                string permissionFrom = zone.ZoneType == "Govt" ? "Government" : zone.Owner;

                if (zone.ZoneType == "Public")
                {
                    await DisplayAlert("Not Needed", "This is a public zone. No permission required.", "OK");
                    return;
                }

                requests.Add(new PermissionRequest
                {
                    ZoneName = zone.ZoneName,
                    Action = "Leader Change Request",
                    PermissionFrom = permissionFrom,
                    RequestedBy = "Admin",
                    Status = "Pending"
                });

                zone.PermissionStatus = "Pending";
                await DisplayAlert("Permission Requested", $"Request sent to {permissionFrom} for {zone.ZoneName}.", "OK");

                ApplyFilters();
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        public static List<PermissionRequest> GetRequests() => requests;

        public static List<string> GetZones()
        {
            return zones.Select(z => z.ZoneName).ToList();
        }

        public class ZoneInfo
        {
            public string ZoneName { get; set; }
            public string LeaderName { get; set; }
            public string ZoneType { get; set; } 
            public string Owner { get; set; }
            public string PermissionStatus { get; set; } 

            public string LeaderNameDisplay => $"Leader: {(string.IsNullOrEmpty(LeaderName) ? "None" : LeaderName)}";
            public string ZoneTypeDisplay => $"Type: {ZoneType}" + (ZoneType == "Owner" ? $" ({Owner})" : "");
        }

        public class PermissionRequest
        {
            public string ZoneName { get; set; }
            public string Action { get; set; }
            public string PermissionFrom { get; set; }
            public string RequestedBy { get; set; }
            public string Status { get; set; } 
        }
    }
}
