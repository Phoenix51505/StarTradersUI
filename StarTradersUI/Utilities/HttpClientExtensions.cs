using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using StarTradersUI.Api;

namespace StarTradersUI.Utilities;

public static class HttpClientExtensions
{
    public static async Task<T[]> GetAllPaginatedData<T>(this HttpClient client, string url, string? cacheKey = null,
        string? authToken = null)
    {
        
        if (cacheKey != null && await GlobalStates.GlobalDataCache.TryGet<T[]>(cacheKey) is { } cachedResults)
        {
            return cachedResults;
        }

        T[]? results = null;
        var nextStartIndex = 0;
        int total;
        var page = 1;
        do
        {
            var subUrl = $"{url}?page={page}&limit=20";
            var json = await client.GetJsonAsync<PagedResults<T>>(subUrl, authToken);
            total = json!.Meta.Total;
            results ??= new T[total];
            var resultsSpan = results.AsSpan();
            json.Data.CopyTo(resultsSpan[nextStartIndex..]);
            nextStartIndex += 20;
            page += 1;
        } while (nextStartIndex < total);

        results ??= [];
        if (cacheKey != null) await GlobalStates.GlobalDataCache.Set(cacheKey, results);
        return results;
    }

    public static async Task RateLimitedRequest(this HttpClient client, Func<HttpRequestMessage> message,
        Func<HttpResponseMessage, Task> onResponse)
    {
        while (true)
        {
            var response = await client.SendAsync(message());
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP Request Failed {response}");
            }

            await onResponse(response);
            return;
        }

        ;
    }

    public static async Task<T> RateLimitedRequest<T>(this HttpClient client, Func<HttpRequestMessage> message,
        Func<HttpResponseMessage, Task<T>> onResponse)
    {
        while (true)
        {
            var response = await client.SendAsync(message());
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP Request Failed {response}");
            }

            return await onResponse(response);
        }

        ;
    }

    public static async Task RateLimitedRequest(this HttpClient client, Func<HttpRequestMessage> message,
        Action<HttpResponseMessage> onResponse)
    {
        while (true)
        {
            var response = await client.SendAsync(message());
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP Request Failed {response}");
            }

            onResponse(response);
            return;
        }

        ;
    }


    public static async Task<T> RateLimitedRequest<T>(this HttpClient client, Func<HttpRequestMessage> message,
        Func<HttpResponseMessage, T> onResponse)
    {
        while (true)
        {
            var response = await client.SendAsync(message());
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP Request Failed {response}");
            }

            return onResponse(response);
        }

        ;
    }

    public static async Task<T?> GetJsonAsync<T>(this HttpClient client, string url, string? authToken = null)
        where T : class
    {
        return await client.RateLimitedRequest(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (authToken != null) request.Headers.Add("Authorization", $"Bearer {authToken}");
            return request;
        }, async response =>
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonUtils.FromJson<T>(stream);
        });
    }
}