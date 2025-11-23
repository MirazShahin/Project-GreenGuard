using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views
{
    public partial class UpdateTreePage : ContentPage
    {
        private readonly ApiService _api;
        private List<Tree> _trees;
        private Tree _selectedTree;

        public UpdateTreePage()
        {
            InitializeComponent();
            _api = new ApiService();
            LoadTrees();
        }

        private async void LoadTrees()
        {
            _trees = await _api.GetTrees();
            TreePicker.ItemsSource = _trees.Select(t => t.Name).ToList();
        }

        private void OnTreeSelected(object sender, EventArgs e)
        {
            if (TreePicker.SelectedIndex == -1) return;

            _selectedTree = _trees[TreePicker.SelectedIndex];

            TreeNameEntry.Text = (string)_selectedTree.Name;
            TreeDescriptionEditor.Text = _selectedTree.Description;
            TreePriceEntry.Text = _selectedTree.Price.ToString();
            TreeStockEntry.Text = _selectedTree.Stock.ToString();
        }

        private async void OnUpdateTreeClicked(object sender, EventArgs e)
        {
            if (_selectedTree == null)
            {
                await DisplayAlert("Error", "Please select a tree first!", "OK");
                return;
            }

            // Validate inputs
            if (string.IsNullOrWhiteSpace(TreeNameEntry.Text) ||
                string.IsNullOrWhiteSpace(TreeDescriptionEditor.Text) ||
                !int.TryParse(TreePriceEntry.Text, out int price) ||
                !int.TryParse(TreeStockEntry.Text, out int stock))
            {
                await DisplayAlert("Error", "Please fill all fields correctly!", "OK");
                return;
            }

            // Update tree object
            _selectedTree.Name = TreeNameEntry.Text.Trim();
            _selectedTree.Description = TreeDescriptionEditor.Text.Trim();
            _selectedTree.Price = price;
            _selectedTree.Stock = stock;

            bool success = await _api.UpdateTree(_selectedTree);

            if (success)
            {
                await DisplayAlert("Success", "Tree updated successfully!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update tree.", "OK");
            }
        }


        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
