using System;
using System.Net;

namespace StarTradersUI.Utilities;

public class HttpStatusException(HttpStatusCode code) : Exception($"HTTP request failed with status code {code}")
{
    public HttpStatusCode Code => code;
}