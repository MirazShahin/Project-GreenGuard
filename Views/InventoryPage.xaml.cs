using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class InventoryPage : ContentPage
    {
        private readonly ApiService _api;
        private List<Tree> allTrees = new();
        private List<Tree> filteredTrees = new();

        public InventoryPage()
        {
            InitializeComponent();
            _api = new ApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadInventory();
        }

        private async Task LoadInventory()
        {
            allTrees = await _api.GetAllTrees();
            filteredTrees = new List<Tree>(allTrees);
            RefreshUI();
        }

        private void RefreshUI()
        {
            InventoryCollection.ItemsSource = null;
            InventoryCollection.ItemsSource = filteredTrees;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TreeSearchBar.Text?.ToLower() ?? "";

            filteredTrees = allTrees
                .Where(t => t.Name!.ToLower().Contains(search))
                .ToList();

            RefreshUI();
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            var tree = (sender as Button)!.CommandParameter as Tree;

            string newStock = await DisplayPromptAsync(
                "Update Stock",
                $"Enter new stock for {tree.Name}:",
                "OK", "Cancel",
                initialValue: tree.Stock.ToString()
            );

            if (!int.TryParse(newStock, out int stock))
            {
                await DisplayAlert("Error", "Invalid number!", "OK");
                return;
            }

            tree.Stock = stock;

            bool success = await _api.UpdateTree(tree);

            if (!success)
            {
                await DisplayAlert("Error", "Failed to update tree!", "OK");
                return;
            }

            await DisplayAlert("Success", $"{tree.Name} updated!", "OK");
            await LoadInventory();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
