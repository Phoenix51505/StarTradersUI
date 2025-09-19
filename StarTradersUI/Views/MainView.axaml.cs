using Avalonia.Controls;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace StarTradersUI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Dispatcher.UIThread.InvokeAsync(GlobalStates.InitializeGlobalState);
    }
}
