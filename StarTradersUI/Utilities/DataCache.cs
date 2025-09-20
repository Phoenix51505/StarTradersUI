using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using StarTradersUI.Api;

namespace StarTradersUI.Utilities;

public class DataCache
{
    private string _folder;

    public DataCache(string folder, DateTime lastUpdate)
    {
        _folder = folder;

        if (Directory.Exists(folder) && File.Exists(folder + "/last_updated.txt"))
        {
            var lastUpdatedText = File.ReadAllText(folder + "/last_updated.txt");
            if (lastUpdatedText != lastUpdate.ToBinary().ToString())
            {
                Directory.Delete(folder, true);
            }
        }

        if (Directory.Exists(folder)) return;

        Directory.CreateDirectory(folder);
        File.WriteAllText(folder + "/last_updated.txt", lastUpdate.ToBinary().ToString());
    }

    public async Task<T?> TryGet<T>(string key, CancellationToken ct = default) where T : class
    {
        if (!File.Exists(_folder + "/" + key + ".json")) return null;

        using var f = new StreamReader(_folder + "/" + key + ".json");
        var result = await JsonUtils.FromJson<T>(f.BaseStream, ct);
        return result;
    }

    public async Task Set<T>(string key, T value, CancellationToken ct=default) where T : class
    {
        if (!File.Exists(_folder + "/" + key + ".json"))
        {
            var directory = Path.GetDirectoryName(_folder + "/" + key + ".json");
            Directory.CreateDirectory(directory!);
            await using var f = File.Create(_folder + "/" + key + ".json");
            await JsonUtils.ToJson(value, f, ct);
            await f.FlushAsync(ct);
        }
        else
        {
            await using var f = File.Open(_folder + "/" + key + ".json", FileMode.Open);
            await JsonUtils.ToJson(value, f, ct);
            await f.FlushAsync(ct);
        }
    }
}