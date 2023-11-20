using Newtonsoft.Json;
using System.Text.Json;

namespace Shared.Game;

public class Api
{
    private readonly HttpClient _httpClient;

    public Api(HttpClient httpClient, bool useLocalProxy=false)
    {
        _httpClient = httpClient;
        if (useLocalProxy)
        {
            httpClient.BaseAddress = new Uri("https://localhost:7263/");

        }
        else
        {
            httpClient.BaseAddress = new Uri("https://api.considition.com/");
        }
    }

    public async Task<MapData> GetMapDataAsync(string mapName, string apiKey)
    {
        if (!Directory.Exists("datacache"))
            Directory.CreateDirectory("datacache");

        string cacheFileName = $"datacache\\cached-MapData-{mapName}.json";

        try
        {
            string responseText;
            if (!File.Exists(cacheFileName))
            {
                HttpRequestMessage request = new();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri($"/api/game/getmapdata?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative);
                request.Headers.Add("x-api-key", apiKey);
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText(cacheFileName, responseText);
            }
            else
            {
                responseText = File.ReadAllText(cacheFileName);
            }

            return JsonConvert.DeserializeObject<MapData>(responseText);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"GETMAP {mapName} EXCEPTION! {ex.Message}, retrying...");
            string responseText;
            if (!File.Exists(cacheFileName))
            {
                HttpRequestMessage request = new();
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri($"/api/game/getmapdata?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative);
                request.Headers.Add("x-api-key", apiKey);
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText(cacheFileName, responseText);
            }
            else
            {
                responseText = File.ReadAllText(cacheFileName);
            }

            return JsonConvert.DeserializeObject<MapData>(responseText);
        }
    }

    public async Task<GeneralData> GetGeneralDataAsync()
    {
        if (!Directory.Exists("datacache"))
            Directory.CreateDirectory("datacache");

        string cacheFileName = $"datacache\\cached-GeneralData.json";

        try
        {
            string responseText;
            if (!File.Exists(cacheFileName))
            {
                var response = await _httpClient.GetAsync("/api/game/getgeneralgamedata");
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText(cacheFileName, responseText);
            }
            else
            {
                responseText = File.ReadAllText(cacheFileName);
            }

            return JsonConvert.DeserializeObject<GeneralData>(responseText);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"GETGENERALDATA EXCEPTION! {ex.Message}, retrying...");

            string responseText;
            if (!File.Exists(cacheFileName))
            {
                var response = await _httpClient.GetAsync("/api/game/getgeneralgamedata");
                response.EnsureSuccessStatusCode();
                responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText(cacheFileName, responseText);
            }
            else
            {
                responseText = File.ReadAllText(cacheFileName);
            }

            return JsonConvert.DeserializeObject<GeneralData>(responseText);
            
        }
    }

    public async Task<GameData> GetGameAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/game/getgamedata{id}");
        response.EnsureSuccessStatusCode();
        string responseText = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GameData>(responseText);
    }

    public async Task<GameData> SumbitAsync(string mapName, SubmitSolution solution, string apiKey, double? score = null)
    {
        if (score.HasValue)
        {
            await Console.Out.WriteLineAsync($" Score [{score.Value}] for solution on map [{mapName}]");
        }

        try
        {
            HttpRequestMessage request = new();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri($"/api/Game/submitSolution?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative);
            request.Headers.Add("x-api-key", apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(solution), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.Send(request);
            string responseText = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GameData>(responseText);
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"SUBMIT EXCEPTION! {ex.Message}, retrying...");
            HttpRequestMessage request = new();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri($"/api/Game/submitSolution?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative);
            request.Headers.Add("x-api-key", apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(solution), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.Send(request);
            string responseText = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GameData>(responseText);
        }
    }
}
