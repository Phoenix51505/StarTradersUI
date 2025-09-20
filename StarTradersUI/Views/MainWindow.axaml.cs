using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using StarTradersUI;
namespace StarTradersUI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await GlobalStates.InitializeGlobalState();
        
    }
}