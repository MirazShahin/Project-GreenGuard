namespace GreenGuard.Views
{
    public partial class VolunteerDashboard : ContentPage
    {
        private string assignedLeader = "Leader A";

        public VolunteerDashboard()
        {
            InitializeComponent();
        }

        private async void OnPlantationUpdateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlantationUpdatePage());
        }

        private async void OnRequirementUpdateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TreeRequirementUpdatePage());
        }

        private async void OnMessageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewMessagePage("Volunteer", assignedLeader));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
