using GreenGuard.Models;
using GreenGuard.Services;
using System.Collections.ObjectModel;

namespace GreenGuard.Views
{
    public partial class LeaderManagementPage : ContentPage
    {
        private readonly ApiService _api;
        private List<InternalUser> _leaders = new();

        public LeaderManagementPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadLeaders();
        }

        private async Task LoadLeaders()
        {
            _leaders = await _api.GetLeaders();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string search = LeaderSearchBar.Text?.ToLower() ?? "";

            var filtered = _leaders
                .Where(l =>
                    (l.FullName?.ToLower().Contains(search) ?? false) ||
                    (l.Email?.ToLower().Contains(search) ?? false) ||
                    (l.LeaderId?.ToLower().Contains(search) ?? false)
                )
                .GroupBy(l => l.Zone)
                .Select(g => new LeaderGroup(g.Key, g.ToList()))
                .ToList();

            GroupedLeaderCollectionView.ItemsSource = filtered;
        }

        // ADD LEADER
        private async void OnAddLeaderClicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Add Leader", "Enter full name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            string email = await DisplayPromptAsync("Add Leader", "Enter email:");
            if (string.IsNullOrWhiteSpace(email)) return;

            string password = await DisplayPromptAsync("Add Leader", "Enter password:");
            if (string.IsNullOrWhiteSpace(password)) return;

            string zone = await DisplayPromptAsync("Add Leader", "Enter zone:");
            if (string.IsNullOrWhiteSpace(zone)) return;

            var leader = new InternalUser
            {
                FullName = name,
                Email = email,
                Password = password,
                Zone = zone,
                Role = "Leader"
            };

            bool ok = await _api.AddLeader(leader);

            if (ok)
            {
                await LoadLeaders();
                await DisplayAlert("Success", "Leader added.", "OK");
            }
        }

        // EDIT
        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is InternalUser leader)
            {
                string newName = await DisplayPromptAsync("Edit", "Update name:", initialValue: leader.FullName);
                if (!string.IsNullOrWhiteSpace(newName)) leader.FullName = newName;

                string newZone = await DisplayPromptAsync("Edit", "Update zone:", initialValue: leader.Zone);
                if (!string.IsNullOrWhiteSpace(newZone)) leader.Zone = newZone;

                bool ok = await _api.UpdateLeader(leader);
                if (ok)
                {
                    await LoadLeaders();
                    await DisplayAlert("Updated", "Leader updated.", "OK");
                }
            }
        }

        // DELETE
        private async void OnRemoveClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is InternalUser leader)
            {
                bool confirm = await DisplayAlert("Delete", $"Delete {leader.FullName}?", "Yes", "No");
                if (!confirm) return;

                bool ok = await _api.DeleteLeader(leader.Id);

                if (ok)
                {
                    await LoadLeaders();
                    await DisplayAlert("Removed", "Leader removed.", "OK");
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

    public class LeaderGroup : ObservableCollection<InternalUser>
    {
        public string Key { get; }
        public LeaderGroup(string key, IEnumerable<InternalUser> items) : base(items)
        {
            Key = key;
        }
    }
}
