using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Threading;
using StarTradersUI.Api;
using StarTradersUI.Api.Agents;
using StarTradersUI.Api.Information;
using StarTradersUI.Utilities;

namespace StarTradersUI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await GlobalStates.InitializeGlobalState((ᛏᛖᛣᛇᛏ, ᛣᛠᚱᛂᚾᛏ, ᛏᚩᛏᛚ) =>
            {
                MainProgressBar.ProgressTextFormat = ᛏᛖᛣᛇᛏ;
                MainProgressBar.Value = ᛣᛠᚱᛂᚾᛏ;
                MainProgressBar.Maximum = ᛏᚩᛏᛚ;
            });
            foreach (var kvp in GlobalStates.CachedTokens.OrderBy(kvp => kvp.Key))
            {
                StoredToken.Items.Add(new ComboBoxItem { Content = kvp.Key });
            }

            foreach (var faction in GlobalStates.Factions.OrderBy(fact => fact.Name))
            {
                RegisterComboBox.Items.Add(new ComboBoxItem { Content = $"{faction.Name}" });
            }

            RegisterAccountBox.Text = GlobalStates.CachedAccountToken;
            LoadingUI.IsVisible = false;
            LogInUI.IsVisible = true;
            MainScreen.IsVisible = false;
        });
    }

    private void SwitchEvent(object? sender, RoutedEventArgs e)
    {
        SignLabel.IsVisible = !SignLabel.IsVisible;
        SignBoxWithPassword.IsVisible = !SignBoxWithPassword.IsVisible;
        SignButton.IsVisible = !SignButton.IsVisible;
        SignSwitch.IsVisible = !SignSwitch.IsVisible;
        RegisterLabel.IsVisible = !RegisterLabel.IsVisible;
        RegisterBoxWithPassword.IsVisible = !RegisterBoxWithPassword.IsVisible;
        RegisterSymbolBox.IsVisible = !RegisterSymbolBox.IsVisible;
        RegisterComboBox.IsVisible = !RegisterComboBox.IsVisible;
        RegisterButton.IsVisible = !RegisterButton.IsVisible;
        RegisterSwitch.IsVisible = !RegisterSwitch.IsVisible;
        StoredToken.IsVisible = !StoredToken.IsVisible;
        if (SignLabel.IsVisible)
        {
            Entry.Height = 181;
            EntryBorder.Height = 201;
        }
        else
        {
            Entry.Height = 228;
            EntryBorder.Height = 248;
        }
    }

    private void RevealAgentToken_OnClick(object? sender, RoutedEventArgs e)
    {
        SignBox.RevealPassword = !SignBox.RevealPassword;
    }

    private void StoredToken_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (StoredToken is { SelectedItem: ComboBoxItem { Content: string key } })
        {
            SignBox.Text = GlobalStates.CachedTokens[key];
        }
    }

    private void SignButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(SignInAsAgent);
        e.Handled = true;
    }

    private async Task SignInAsAgent()
    {
        try
        {
            var agentDetails =
                await GlobalStates.client.GetJsonAsync<ResponseWithData<Agent>>("https://api.spacetraders.io/v2/my/agent", SignBox.Text);
            GlobalStates.authorization = SignBox.Text;
            await GlobalStates.AddAgent(SignBox.Text!, agentDetails!.Data.Symbol);
            Console.WriteLine($"Logged in as agent: {agentDetails.Data.Symbol}");
            LogInUI.IsVisible = false;
            MainScreen.IsVisible = true;
        }
        catch (HttpStatusException exception)
        {
            Console.WriteLine($"Error logging in as agent: {exception.Message}");
            // TODO: A "something went wrong" message here
        }
    }

    private void RevealAccountToken_OnClick(object? sender, RoutedEventArgs e)
    {
        RegisterAccountBox.RevealPassword = !RegisterAccountBox.RevealPassword;
    }

    private void RegisterButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(CreateNewAgent);
        e.Handled = true;
    }

    private async Task CreateNewAgent()
    {
        try
        {
            if (RegisterComboBox is not { SelectedItem: ComboBoxItem { Content: string factionName } })
            {
                Console.WriteLine("You must select a faction!");
                return;
            }

            var request = new AgentRegisterRequest
            {
                Faction = GlobalStates.Factions.First(x => x.Name == factionName).Symbol,
                Symbol = RegisterSymbolBox.Text!
            };

            var response =
                await GlobalStates.client.PostJsonAsync<ResponseWithData<AgentRegisterResponse>, AgentRegisterRequest>(
                    "https://api.spacetraders.io/v2/register", request, RegisterAccountBox.Text);
            await GlobalStates.AddAgent(response!.Data.Token, response.Data.Agent.Symbol);
            GlobalStates.authorization = response.Data.Token;
            GlobalStates.CachedAccountToken = RegisterAccountBox.Text;
            Console.WriteLine($"Registered agent: {response.Data.Agent.Symbol}");
            LogInUI.IsVisible = false;
            MainScreen.IsVisible = true;
        }
        catch (HttpStatusException exception)
        {
            Console.WriteLine($"Error registering agent: {exception.Message}");
            Console.WriteLine(await exception.Response.Content.ReadAsStringAsync());
        }
    }
}