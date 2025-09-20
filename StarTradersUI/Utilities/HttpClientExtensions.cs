using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using StarTradersUI.Api;

namespace StarTradersUI.Utilities;

public static class HttpClientExtensions
{
    public static async Task<T[]> GetAllPaginatedData<T>(this HttpClient client, string url, string? cacheKey = null,
        string? authToken = null, Action<int, int>? progressBarCallback = null,
        CancellationToken cancellationToken = default)
    {
        if (cacheKey != null && await GlobalStates.GlobalDataCache.TryGet<T[]>(cacheKey, cancellationToken) is
                { } cachedResults)
        {
            progressBarCallback?.Invoke(1, 1);
            return cachedResults;
        }

        T[]? results = null;
        var nextStartIndex = 0;
        int total;
        var page = 1;
        progressBarCallback?.Invoke(0, 1);
        do
        {
            var subUrl = $"{url}?page={page}&limit=20";
            var json = await client.GetJsonAsync<PagedResults<T>>(subUrl, authToken, ct: cancellationToken);
            total = json!.Meta.Total;
            results ??= new T[total];
            var resultsSpan = results.AsSpan();
            json.Data.CopyTo(resultsSpan[nextStartIndex..]);
            nextStartIndex += 20;
            page += 1;
            progressBarCallback?.Invoke(nextStartIndex, total);
        } while (nextStartIndex < total);

        results ??= [];
        if (cacheKey != null) await GlobalStates.GlobalDataCache.Set(cacheKey, results, cancellationToken);
        return results;
    }

    public static async Task RateLimitedRequest(this HttpClient client, Func<HttpRequestMessage> message,
        Func<HttpResponseMessage, Task> onResponse, CancellationToken ct = default)
    {
        while (true)
        {
            var response = await client.SendAsync(message(), ct);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpStatusException(response.StatusCode);
            }

            await onResponse(response);
            return;
        }

        ;
    }

    public static async Task<T> RateLimitedRequest<T>(this HttpClient client, Func<HttpRequestMessage> message,
        Func<HttpResponseMessage, Task<T>> onResponse, CancellationToken ct = default)
    {
        while (true)
        {
            var response = await client.SendAsync(message(), ct);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpStatusException(response.StatusCode);
            }

            return await onResponse(response);
        }

        ;
    }

    public static async Task RateLimitedRequest(this HttpClient client, Func<HttpRequestMessage> message,
        Action<HttpResponseMessage> onResponse, CancellationToken ct = default)
    {
        while (true)
        {
            var response = await client.SendAsync(message(), ct);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpStatusException(response.StatusCode);
            }

            onResponse(response);
            return;
        }

        ;
    }


    public static async Task<T> RateLimitedRequest<T>(this HttpClient client, Func<HttpRequestMessage> message,
        Func<HttpResponseMessage, T> onResponse, CancellationToken ct = default)
    {
        while (true)
        {
            var response = await client.SendAsync(message(), ct);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpStatusException(response.StatusCode);
            }

            return onResponse(response);
        }
    }

    public static async Task<T?> GetJsonAsync<T>(this HttpClient client, string url, string? authToken = null,
        CancellationToken ct = default)
        where T : class
    {
        return await client.RateLimitedRequest(() =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (authToken != null) request.Headers.Add("Authorization", $"Bearer {authToken}");
            return request;
        }, async response =>
        {
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            return await JsonUtils.FromJson<T>(stream, ct);
        }, ct: ct);
    }
}