using GreenGuard.Models;
using GreenGuard.Services;
using System.Collections.ObjectModel;

namespace GreenGuard.Views
{
    public partial class VolunteerManagementPage : ContentPage
    {
        private readonly ApiService _api;
        private List<InternalUser> _volunteers = new();

        public VolunteerManagementPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadVolunteers();
        }

        private async Task LoadVolunteers()
        {
            _volunteers = await _api.GetVolunteers();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string search = VolunteerSearchBar.Text?.ToLower() ?? "";

            var filtered = _volunteers
                .Where(v =>
                    (v.FullName?.ToLower().Contains(search) ?? false) ||
                    (v.Email?.ToLower().Contains(search) ?? false) ||
                    (v.VolunteerId?.ToLower().Contains(search) ?? false)
                )
                .GroupBy(v => v.Zone)
                .Select(g => new VolunteerGroup(g.Key, g.ToList()))
                .ToList();

            GroupedVolunteerCollectionView.ItemsSource = filtered;
        }

        // ADD VOLUNTEER
        private async void OnAddVolunteerClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Add Volunteer", "Enter full name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string email = await DisplayPromptAsync("Add Volunteer", "Enter email:");
            if (string.IsNullOrWhiteSpace(email)) return;

            string password = await DisplayPromptAsync("Add Volunteer", "Enter password:");
            if (string.IsNullOrWhiteSpace(password)) return;

            string zone = await DisplayPromptAsync("Add Volunteer", "Enter zone:");
            if (string.IsNullOrWhiteSpace(zone)) return;

            var v = new InternalUser
            {
                FullName = name,
                Email = email,
                Password = password,
                Zone = zone,
                Role = "Volunteer"
            };

            bool ok = await _api.AddVolunteer(v);

            if (ok)
            {
                await LoadVolunteers();
                await DisplayAlert("Success", "Volunteer added.", "OK");
            }
        }

        // EDIT
        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is InternalUser v)
            {
                string newName = await DisplayPromptAsync("Edit", "New Name:", initialValue: v.FullName);
                if (!string.IsNullOrWhiteSpace(newName)) v.FullName = newName;

                string newZone = await DisplayPromptAsync("Edit", "New Zone:", initialValue: v.Zone);
                if (!string.IsNullOrWhiteSpace(newZone)) v.Zone = newZone;

                bool ok = await _api.UpdateVolunteer(v);
                if (ok)
                {
                    await LoadVolunteers();
                    await DisplayAlert("Updated", "Volunteer updated.", "OK");
                }
            }
        }

        // DELETE
        private async void OnRemoveClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is InternalUser v)
            {
                bool confirm = await DisplayAlert("Confirm", $"Delete {v.FullName}?", "Yes", "No");
                if (!confirm) return;

                bool ok = await _api.DeleteVolunteer(v.Id);

                if (ok)
                {
                    await LoadVolunteers();
                    await DisplayAlert("Removed", "Volunteer removed.", "OK");
                }
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }

    public class VolunteerGroup : ObservableCollection<InternalUser>
    {
        public string Key { get; }
        public VolunteerGroup(string key, IEnumerable<InternalUser> items) : base(items)
        {
            Key = key;
        }
    }
}
