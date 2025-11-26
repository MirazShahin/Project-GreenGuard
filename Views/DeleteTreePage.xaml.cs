using GreenGuard.Models;
using GreenGuard.Services;

namespace GreenGuard.Views;

public partial class DeleteTreePage : ContentPage
{
    private readonly ApiService _api;
    private List<Tree> _trees = new();  
    private Tree? _selected;

    public DeleteTreePage()
    {
        InitializeComponent();
        _api = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTrees();
    }

    private async Task LoadTrees()
    {
        try
        {
            _selected = null;

            _trees = await _api.GetTrees();

            if (_trees == null || _trees.Count == 0)
            {
                TreePicker.ItemsSource = new List<string>();
                await DisplayAlert("Info", "No trees available to delete.", "OK");
                return;
            }

            TreePicker.ItemsSource = _trees
                .Select(t => t.Name ?? "Unnamed Tree")
                .ToList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load trees.\n" + ex.Message, "OK");
            TreePicker.ItemsSource = new List<string>();
        }
    }

    private void OnTreeSelected(object sender, EventArgs e)
    {
        if (_trees == null || _trees.Count == 0)
            return;

        if (TreePicker.SelectedIndex < 0 || TreePicker.SelectedIndex >= _trees.Count)
            return;

        _selected = _trees[TreePicker.SelectedIndex];
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selected == null)
        {
            await DisplayAlert("Error", "Please select a tree first!", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(_selected.Id))
        {
            await DisplayAlert("Error", "Selected tree has no valid ID.", "OK");
            return;
        }

        bool confirm = await DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete '{_selected.Name ?? "this tree"}'?",
            "Yes",
            "No"
        );

        if (!confirm) return;

        bool success = await _api.DeleteTree(_selected.Id);

        if (success)
        {
            await DisplayAlert("Success", "Tree deleted successfully!", "OK");
             
            _trees.Remove(_selected);
            _selected = null;

            if (_trees.Count == 0)
            {
                TreePicker.ItemsSource = new List<string>();
            }
            else
            {
                TreePicker.ItemsSource = _trees
                    .Select(t => t.Name ?? "Unnamed Tree")
                    .ToList();
                TreePicker.SelectedIndex = -1;
            } 
        }
        else
        {
            await DisplayAlert("Error", "Failed to delete tree!", "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
