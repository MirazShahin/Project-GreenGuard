namespace GreenGuard.Views
{
    public partial class LeaderDashboard : ContentPage
    {
        public LeaderDashboard()
        {
            InitializeComponent();
        }

        private async void OnValidateClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Validate", "Volunteer updates validation flow.", "OK");
        }

        private async void OnPerformanceClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LeaderboardPage());
        }

        private async void OnMessagesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MessagesPage("Leader"));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
