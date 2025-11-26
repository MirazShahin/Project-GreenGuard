using GreenGuard.Models;
using GreenGuard.Services;
using Microsoft.Maui.Storage;

namespace GreenGuard.Views
{
    public partial class PlantationUpdatePage : ContentPage
    {
        private readonly ApiService _api;

        private string? proofBase64;
        private string? proofFileName;
        private string? proofFileType;

        public PlantationUpdatePage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        // ===================== UPLOAD PROOF =====================
        private async void OnUploadProofClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Proof (Image or Video)",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.Android, new[] { "image/*", "video/*" } },
                            { DevicePlatform.WinUI, new[] { ".jpg", ".png", ".mp4", ".mov" } }
                        })
                });

                if (result == null)
                    return;

                proofFileName = result.FileName;
                proofFileType = result.ContentType;

                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                proofBase64 = Convert.ToBase64String(ms.ToArray());

                // Preview only if it is an image
                if (result.ContentType.StartsWith("image"))
                {
                    ProofImage.IsVisible = true;
                    ProofImage.Source = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));
                }
                else
                {
                    ProofImage.IsVisible = false;
                }

                await DisplayAlert("Success", "Proof uploaded successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to upload file: {ex.Message}", "OK");
            }
        }

        // ===================== SUBMIT FORM =====================
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ZoneEntry.Text) ||
                string.IsNullOrWhiteSpace(SpeciesEntry.Text) ||
                string.IsNullOrWhiteSpace(NumberEntry.Text))
            {
                await DisplayAlert("Error", "Please fill all fields.", "OK");
                return;
            }

            if (!int.TryParse(NumberEntry.Text, out int trees))
            {
                await DisplayAlert("Error", "Enter a valid number.", "OK");
                return;
            }

            var update = new PlantationUpdate
            {
                VolunteerId = Preferences.Get("UserId", "Unknown"),
                Zone = ZoneEntry.Text,
                TreeSpecies = SpeciesEntry.Text,
                TreesPlanted = trees,
                Date = DatePickerControl.Date,
                ProofFileBase64 = proofBase64,
                ProofFileName = proofFileName,
                ProofFileType = proofFileType
            };

            bool ok = await _api.SubmitPlantationUpdate(update);

            if (ok)
            {
                await DisplayAlert("Success", "Plantation update submitted!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to submit update.", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
