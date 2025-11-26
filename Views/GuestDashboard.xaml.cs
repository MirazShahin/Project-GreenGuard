using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class GuestDashboard : ContentPage
    {
        private readonly ApiService _api;

        public GuestDashboard()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTrees();
        }

        private async Task LoadTrees()
        {
            try
            {
                var trees = await _api.GetTrees();
                TreeCollectionView.ItemsSource = trees;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load trees:\n{ex.Message}", "OK");
            }
        }

        private async void OnBuyTreeClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Tree tree)
            {
                string guestId = "Guest";

                await Navigation.PushAsync(new BuyTreePage(tree, 1, guestId));
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Do you want to logout?", "Yes", "No");
            if (confirm)
                await Navigation.PopToRootAsync();
        }
    }
}
