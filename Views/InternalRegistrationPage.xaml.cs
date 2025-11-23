using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class InternalRegistrationPage : ContentPage
    {
        private readonly ApiService _api;

        public InternalRegistrationPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string? name = NameEntry.Text?.Trim();
            string? email = EmailEntry.Text?.Trim();
            string? password = PasswordEntry.Text?.Trim();
            string? role = RolePicker.SelectedItem?.ToString();
            string? zone = ZoneEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                await DisplayAlert("Error", "Please fill all fields.", "OK");
                return;
            }

            InternalUser user = new InternalUser
            {
                FullName = name,
                Email = email,
                Password = password,
                Role = role,
                Zone = zone
            };

            bool ok = await _api.RegisterInternalUser(user);

            if (ok)
            {
                await DisplayAlert("Success", "Registration successful!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Cannot connect to server.", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
