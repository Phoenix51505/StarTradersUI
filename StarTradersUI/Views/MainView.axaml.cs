using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace StarTradersUI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

    }
    private void SwitchEvent(object? sender, RoutedEventArgs e)
    {
        SignLabel.IsVisible = !SignLabel.IsVisible;
        SignBox.IsVisible = !SignBox.IsVisible;
        SignButton.IsVisible = !SignButton.IsVisible;
        SignSwitch.IsVisible = !SignSwitch.IsVisible;
        RegisterLabel.IsVisible = !RegisterLabel.IsVisible;
        RegisterAccountBox.IsVisible = !RegisterAccountBox.IsVisible;
        RegisterSymbolBox.IsVisible = !RegisterSymbolBox.IsVisible;
        RegisterComboBox.IsVisible = !RegisterComboBox.IsVisible;
        RegisterButton.IsVisible = !RegisterButton.IsVisible;
        RegisterSwitch.IsVisible = !RegisterSwitch.IsVisible;
        if (SignLabel.IsVisible == true)
        {
            Entry.Height = 156;
            EntryBorder.Height = 176;
        }
        else
        {
            Entry.Height = 228;
            EntryBorder.Height = 248;
        }
    }
}
