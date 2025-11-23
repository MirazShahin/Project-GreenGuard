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
                TopTreeLabel.Text = data.TopTreeName;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load analytics\n" + ex.Message, "OK");
                TotalSoldLabel.Text = "0";
                TopTreeLabel.Text = "Error";
            }
        }


        // ------ Existing button handlers ------
        private async void OnAddTreeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTreePage());
        }
        private async void OnUpdateTreeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateTreePage());
        }
        private async void OnInventoryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InventoryPage());
        }
        private async void OnPermissionRequestsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PermissionRequestsPage());
        }
        private async void OnNGORequestsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdminNGORequestsPage());
        }
        private async void OnProjectRequestsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdminProjectApprovalPage());
        }
        private async void OnViewLeadersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LeaderManagementPage());
        }
        private async void OnViewVolunteersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new VolunteerManagementPage());
        }
        private async void OnZoneWiseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ZoneManagementPage());
        }
        private async void OnSalesReportClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SalesSummaryPage());
        }
        private async void OnTreeAnalysisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TreeAnalysisPage());
        }
        private async void OnMessagesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MessagesPage("Admin"));
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (confirm)
                await Navigation.PopToRootAsync();
        }
    }
}
