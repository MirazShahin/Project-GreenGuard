using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class BuyTreePage : ContentPage
    {
        private readonly ApiService _api;
        private readonly Tree _tree;
        private readonly int _quantity;
        private readonly string _userId;

        public BuyTreePage(Tree tree, int quantity, string userId)
        {
            InitializeComponent();
            _api = new ApiService();

            _tree = tree;
            _quantity = quantity;
            _userId = userId;

            LoadSummary();
        }

        private void LoadSummary()
        {
            TreeNameLabel.Text = $"Tree: {_tree.Name}";
            TreeDescriptionLabel.Text = _tree.Description;
            TreePriceLabel.Text = $"Price per Tree: ৳{_tree.Price}";
            TreeQuantityLabel.Text = $"Quantity: {_quantity}";

            int total = _tree.Price * _quantity;
            TotalPriceLabel.Text = $"Total Price: ৳{total}";
        }

        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            bool ok = await _api.PurchaseTree(_tree.Id, _userId, _quantity);

            if (ok)
            {
                await DisplayAlert("Success", "🌱 Tree purchase successful!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Not enough stock or purchase failed.", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
