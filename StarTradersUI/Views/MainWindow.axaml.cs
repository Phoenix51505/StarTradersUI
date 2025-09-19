using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StarTradersUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.spacetraders.io/v2/factions?limit=20"),
            };
        using var response = await GlobalStates.client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var factionList = await response.Content.ReadAsStringAsync();
        }
    }
}