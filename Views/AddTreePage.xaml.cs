using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class AddTreePage : ContentPage
    {
        private readonly ApiService _api;

        public AddTreePage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        private async void OnSaveTreeClicked(object sender, EventArgs e)
        {
            string? name = TreeNameEntry.Text?.Trim();
            string? description = TreeDescriptionEditor.Text?.Trim();

            if (!int.TryParse(TreePriceEntry.Text, out int price) ||
                !int.TryParse(TreeStockEntry.Text, out int stock))
            {
                await DisplayAlert("Error", "Price/Stock must be numbers.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            {
                await DisplayAlert("Error", "Please fill all fields!", "OK");
                return;
            }

            var tree = new Tree
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock
            };

            bool success = await _api.AddTree(tree);

            if (!success)
            {
                await DisplayAlert("Error", "Failed to add tree!", "OK");
                return;
            }

            await DisplayAlert("Success", $"Tree '{name}' added successfully!", "OK");
            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
