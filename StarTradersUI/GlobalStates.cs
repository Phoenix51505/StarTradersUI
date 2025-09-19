using System.Net.Http;
using System.Net.Http.Headers;
namespace StarTradersUI;

public static class GlobalStates
{
    public static readonly HttpClient client = new HttpClient();
    public static string authorization = "";
}