using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class ZoneManagementPage : ContentPage
    {
        private readonly ApiService _api;
        private List<Zone> zones = new();
        private List<Zone> filteredZones = new();

        public ZoneManagementPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadZones();
        }

        private async Task LoadZones()
        {
            zones = await _api.GetZones();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string search = ZoneSearchBar.Text?.ToLower() ?? "";
            string filter = ZoneTypeFilterPicker.SelectedItem?.ToString() ?? "All";

            filteredZones = zones
                .Where(z =>
                    (string.IsNullOrWhiteSpace(search) || z.ZoneName.ToLower().Contains(search))
                    && (filter == "All" || z.ZoneType == filter)
                )
                .ToList();

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

        private async void OnAddZoneClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Add Zone", "Enter zone name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string type = await DisplayActionSheet("Select Zone Type", "Cancel", null, "Govt", "Owner", "Public");
            if (type == "Cancel") return;

            string owner = type == "Owner"
                ? await DisplayPromptAsync("Owner Name", "Enter owner name:")
                : "";

            Zone newZone = new()
            {
                ZoneName = name,
                ZoneType = type,
                Owner = owner,
                PermissionStatus = type == "Public" ? "Not Needed" : "Pending"
            };

            bool ok = await _api.AddZone(newZone);
            if (ok)
            {
                await LoadZones();
                await DisplayAlert("Success", "Zone added.", "OK");
            }
        }

        private async void OnEditZoneClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Zone zone)
            {
                string newName = await DisplayPromptAsync("Edit Zone", "Update Name", initialValue: zone.ZoneName);
                if (!string.IsNullOrEmpty(newName))
                    zone.ZoneName = newName;

                string newType = await DisplayActionSheet("Zone Type", "Cancel", null, "Govt", "Owner", "Public");
                if (newType != "Cancel")
                {
                    zone.ZoneType = newType;

                    if (newType == "Owner")
                        zone.Owner = await DisplayPromptAsync("Owner", "Enter owner name:", initialValue: zone.Owner);
                    else
                        zone.Owner = "";
                }

                bool ok = await _api.UpdateZone(zone);
                if (ok)
                {
                    await LoadZones();
                    await DisplayAlert("Updated", "Zone updated.", "OK");
                }
            }
        }

        private async void OnDeleteZoneClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Zone zone)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete {zone.ZoneName}?", "Yes", "No");
                if (!confirm) return;

                bool ok = await _api.DeleteZone(zone.Id);
                if (ok)
                {
                    await LoadZones();
                    await DisplayAlert("Deleted", "Zone removed.", "OK");
                }
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        internal static List<PermissionRequest>? GetRequests()
        {
            throw new NotImplementedException();
        }

        internal class PermissionRequest
        {
            public string ZoneName { get; internal set; }
            public string Action { get; internal set; }
            public string PermissionFrom { get; internal set; }
        }
    }
}
