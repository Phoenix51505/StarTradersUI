using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using StarTradersUI.Api.Information;
using StarTradersUI.Utilities;

namespace StarTradersUI;

public static class GlobalStates
{
    public static readonly HttpClient client = new HttpClient();
    public static string authorization = "";

    public static QuadTree<SystemInformation> SystemTree;
    
    public static ServerInformation GlobalServerInformation;
    public static DataCache GlobalDataCache;
    public static bool IsInialized = false;
    private static event Action? OnInitializationComplete;

    public static async Task InitializeGlobalState()
    {
        GlobalServerInformation = (await client.GetJsonAsync<ServerInformation>("https://api.spacetraders.io/v2/"))!;
        GlobalDataCache = new DataCache("./Cache", GlobalServerInformation.ResetDate);
        
        var systemData = await client.GetAllPaginatedData<Api.SystemInfo.System>("https://api.spacetraders.io/v2/systems","systems");
        var minX = systemData.Min(x => x.X);
        var minY = systemData.Min(x => x.Y);
        var maxX = systemData.Max(x => x.X);
        var maxY = systemData.Max(x => x.Y);
        SystemTree = new QuadTree<SystemInformation>(minX - 5, maxX + 5, minY - 5, maxY + 5, 6);
        foreach (var system in systemData)
        {
            SystemTree.Add(new SystemInformation(system));
        }

        IsInialized = true;
        OnInitializationComplete?.Invoke();
    }

    public static void DoWhenInitialized(Action todo)
    {
        if (IsInialized)
        {
            todo();
        }
        else
        {
            OnInitializationComplete += todo;
        }
    }
}