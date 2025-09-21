using System;
using System.Net;
using System.Net.Http;

namespace StarTradersUI.Utilities;

public class HttpStatusException(HttpResponseMessage response) : Exception($"HTTP request failed with status code {response.StatusCode}")
{
    public HttpResponseMessage Response = response;
    public HttpStatusCode Code => Response.StatusCode;
}