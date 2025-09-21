using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Threading;
using StarTradersUI.Api.FactionInfo;
using StarTradersUI.Api.Information;
using StarTradersUI.Utilities;

namespace StarTradersUI;

public static class GlobalStates
{
    public static readonly HttpClient client = new HttpClient();
    public static string? authorization = null;

    public static QuadTree<SystemInformation> SystemTree;

    public static ServerInformation GlobalServerInformation;
    public static DataCache GlobalDataCache;
    public static bool IsInitialized = false;
    public static Faction[] Factions;

    private static event Action? OnInitializationComplete;
    private static event Action? OnServerReset;

    public static Dictionary<string, string> CachedTokens = [];

    public static string? CachedAccountToken
    {
        get => File.Exists("account_token.txt") ? File.ReadAllText("account_token.txt") : null;
        set
        {
            if (value == null)
            {
                if (File.Exists("account_token.txt")) File.Delete("account_token.txt");
            }
            else
            {
                File.WriteAllText("account_token.txt", value);
            }
        }
    }

    public static async Task InitializeGlobalState(Action<string, int, int> progressBarCallBack)
    {
        progressBarCallBack("Reading global server information...", 0, 1);
        GlobalServerInformation = (await client.GetJsonAsync<ServerInformation>("https://api.spacetraders.io/v2/"))!;
        GlobalDataCache = new DataCache("./Cache", GlobalServerInformation.ResetDate);
        // We just want to add this to the server code
        _ = Dispatcher.UIThread.InvokeAsync(async () =>
        {
            while (DateTime.Now < GlobalServerInformation.ServerResets.Next)
            {
                await Task.Delay(1000);
            }

            OnServerReset?.Invoke();
        });

        var systemData = await client.GetAllPaginatedData<Api.SystemInfo.System>(
            "https://api.spacetraders.io/v2/systems", "systems",
            progressBarCallback: (currentPage, totalPages) =>
            {
                progressBarCallBack($"Reading item {currentPage}/{totalPages} of system data...", currentPage,
                    totalPages);
            });
        var minX = systemData.Min(x => x.X);
        var minY = systemData.Min(x => x.Y);
        var maxX = systemData.Max(x => x.X);
        var maxY = systemData.Max(x => x.Y);
        SystemTree = new QuadTree<SystemInformation>(minX - 5, maxX + 5, minY - 5, maxY + 5, 6);
        foreach (var system in systemData)
        {
            SystemTree.Add(new SystemInformation(system));
        }

        Factions = await client.GetAllPaginatedData<Faction>("https://api.spacetraders.io/v2/factions", "factions",
            progressBarCallback:
            (currentPage, totalPages) =>
            {
                progressBarCallBack($"Reading item {currentPage}/{totalPages} of factions...", currentPage, totalPages);
            });

        CachedTokens = await GlobalDataCache.TryGet<Dictionary<string, string>>("auth_tokens") ?? [];

        IsInitialized = true;
        OnInitializationComplete?.Invoke();
    }

    public static void DoWhenInitialized(Action todo)
    {
        if (IsInitialized)
        {
            todo();
        }
        else
        {
            OnInitializationComplete += todo;
        }
    }

    public static void DoOnServerReset(Action todo)
    {
        if (DateTime.Now >= GlobalServerInformation.ServerResets.Next)
        {
            todo();
        }
        else
        {
            OnServerReset += todo;
        }
    }

    public static async Task AddAgent(string bearerToken, string shipName)
    {
        if (CachedTokens.TryGetValue(shipName, out var token))
        {
            if (token == bearerToken) return;
        }
        CachedTokens[shipName] = bearerToken;
        await GlobalDataCache.Set("auth_tokens", CachedTokens);
    }
}