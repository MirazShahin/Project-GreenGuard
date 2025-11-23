using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _api;

        public LoginPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        // ---------------- INTERNAL LOGIN ----------------
        private async void OnInternalLoginClicked(object sender, EventArgs e)
        {
            try
            {
                string? role = InternalRolePicker.SelectedItem?.ToString();
                string? email = InternalEmailEntry.Text?.Trim();
                string? password = InternalPasswordEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(role) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Error", "Please fill all fields.", "OK");
                    return;
                }

                var user = await _api.LoginInternal(email, password);

                if (user == null)
                {
                    await DisplayAlert("Error", "Invalid email or password.", "OK");
                    return;
                }

                if (user.Role != role)
                {
                    await DisplayAlert("Error",
                        $"Incorrect role selected. Your actual role is {user.Role}.", "OK");
                    return;
                }

                await DisplayAlert("Success", $"Welcome {user.FullName}!", "OK");

                // Navigate safely
                await NavigateToInternal(role);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Login failed.\n{ex.Message}", "OK");
            }
        }


        private async Task NavigateToInternal(string role)
        {
            switch (role)
            {
                case "Admin":
                    await Navigation.PushAsync(new AdminDashboard());
                    break;

                case "Leader":
                    await Navigation.PushAsync(new LeaderDashboard());
                    break;

                case "Volunteer":
                    await Navigation.PushAsync(new VolunteerDashboard());
                    break;
            }
        }


        // ---------------- EXTERNAL LOGIN ----------------
        private async void OnExternalLoginClicked(object sender, EventArgs e)
        {
            try
            {
                string? role = ExternalRolePicker.SelectedItem?.ToString();
                string? email = ExternalEmailEntry.Text?.Trim();
                string? password = ExternalPasswordEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(role) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Error", "Please fill all fields.", "OK");
                    return;
                }

                var user = await _api.LoginExternal(email, password);

                if (user == null)
                {
                    await DisplayAlert("Error", "Invalid credentials.", "OK");
                    return;
                }

                if (user.Role != role)
                {
                    await DisplayAlert("Error",
                        $"Incorrect role selected. Your actual role is {user.Role}.", "OK");
                    return;
                }

                await DisplayAlert("Success", $"Welcome {user.FullName}!", "OK");

                await NavigateToExternal(role);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"External login failed.\n{ex.Message}", "OK");
            }
        }


        private async Task NavigateToExternal(string role)
        {
            switch (role)
            {
                case "NGO":
                    await Navigation.PushAsync(new NGODashboard());
                    break;

                case "Personal User":
                    await Navigation.PushAsync(new PersonalDashboard());
                    break;
            }
        }


        // ---------------- Guest + Signup ----------------

        private async void OnGuestClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GuestDashboard());
        }

        private async void OnSignupTapped(object sender, TappedEventArgs e)
        {
            bool isInternal = await DisplayAlert(
                "Register As",
                "Choose one",
                "Internal",
                "External");

            if (isInternal)
                await Navigation.PushAsync(new InternalRegistrationPage());
            else
                await Navigation.PushAsync(new ExternalRegistrationPage());
        }
    }
}
