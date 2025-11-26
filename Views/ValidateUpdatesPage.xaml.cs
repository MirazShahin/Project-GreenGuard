using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class ValidateUpdatesPage : ContentPage
    {
        private readonly ApiService _api;
        private List<VolunteerUpdate> updates;

        public ValidateUpdatesPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUpdates();
        }

        private async Task LoadUpdates()
        {
            updates = await _api.GetPendingUpdates();
            UpdatesCollectionView.ItemsSource = updates;
        }

        private async void OnApproveClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is VolunteerUpdate update)
            {
                bool ok = await _api.ApproveUpdate(update.Id);
                if (ok)
                {
                    await LoadUpdates();
                    await DisplayAlert("Approved", $"{update.VolunteerName}'s update approved!", "OK");
                }
            }
        }

        private async void OnRejectClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is VolunteerUpdate update)
            {
                bool ok = await _api.RejectUpdate(update.Id);
                if (ok)
                {
                    await LoadUpdates();
                    await DisplayAlert("Rejected", $"{update.VolunteerName}'s update rejected!", "OK");
                }
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
