using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class ExternalRegistrationPage : ContentPage
    {
        private readonly ApiService _api;

        public ExternalRegistrationPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        private void OnRoleChanged(object sender, EventArgs e)
        {
            string role = RolePicker.SelectedItem?.ToString();

            if (role == "NGO")
            {
                OrganizationEntry.IsEnabled = true;
                OrganizationEntry.Opacity = 1;
            }
            else
            {
                OrganizationEntry.IsEnabled = false;
                OrganizationEntry.Opacity = 0.5;
                OrganizationEntry.Text = string.Empty;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text?.Trim();
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();
            string role = RolePicker.SelectedItem?.ToString();
            string org = OrganizationEntry.Text?.Trim();

            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(role))
            {
                await DisplayAlert("Error", "Please fill all fields.", "OK");
                return;
            }

            var newUser = new ExternalUser
            {
                FullName = name,
                Email = email,
                Password = password,
                Role = role,
                OrganizationName = role == "NGO" ? org : null
            };

            bool success = await _api.RegisterExternalUser(newUser);

            if (success)
            {
                await DisplayAlert("Success", "External user registered!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Registration failed.", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
