namespace GreenGuard.Views
{
    public partial class NGODashboard : ContentPage
    {
        public NGODashboard()
        {
            InitializeComponent();
        }

        private async void OnCreateRequestClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NGOCreateRequestPage());
        }

        private async void OnTrackRequestsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NGORequestStatusPage());
        }

        private async void OnPurchaseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PersonalTreePurchasePage()); 
        }
        private async void OnMessagesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NGOMessagesPage());
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (confirm)
                await Navigation.PopToRootAsync();
        }
    }
}
