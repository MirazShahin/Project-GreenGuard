using GreenGuard.Services;
using GreenGuard.Models;

namespace GreenGuard.Views
{
    public partial class AdminDashboard : ContentPage
    {
        private readonly ApiService _api;

        public AdminDashboard()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            try
            {
                TotalSoldLabel.Text = "Loading...";
                TopTreeLabel.Text = "Loading...";

                var data = await _api.GetAdminDashboard();   // API CALL

                if (data == null)
                {
                    TotalSoldLabel.Text = "0";
                    TopTreeLabel.Text = "None";
                    return;
                }

                TotalSoldLabel.Text = data.TotalTreesSold.ToString();

                TopTreeLabel.Text = string.IsNullOrWhiteSpace(data.TopTreeName)
                    ? "None"
                    : data.TopTreeName;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load analytics.\n" + ex.Message, "OK");

                TotalSoldLabel.Text = "0";
                TopTreeLabel.Text = "Error";
            }
        }

        private async void OnAddTreeClicked(object sender, EventArgs e)
        {
            await SafeNavigateAsync(new AddTreePage());
        }

        private async void OnDeleteTreeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeleteTreePage());
        }

        private async void OnNGORequestsClicked(object sender, EventArgs e)
        {
            await SafeNavigateAsync(new AdminNGORequestsPage());
        }


        private async void OnViewLeadersClicked(object sender, EventArgs e)
        {
            await SafeNavigateAsync(new LeaderManagementPage());
        }

        private async void OnViewVolunteersClicked(object sender, EventArgs e)
        {
            await SafeNavigateAsync(new VolunteerManagementPage());
        }

        private async void OnZoneWiseClicked(object sender, EventArgs e)
        {
            await SafeNavigateAsync(new ZoneManagementPage());
        }

        private async void OnMessagesClicked(object sender, EventArgs e)
        {
            await SafeNavigateAsync(new MessagesPage("Admin"));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout",
                                              "Are you sure you want to logout?",
                                              "Yes", "No");

            if (confirm)
                await Navigation.PopToRootAsync();
        }

        private async Task SafeNavigateAsync(Page page)
        {
            try
            {
                if (Navigation != null)
                    await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error",
                                   "Unable to open the page.\n" + ex.Message,
                                   "OK");
            }
        }
    }
}
