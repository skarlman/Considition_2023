using Considition2023_Cs;
using System.Text.Json.Serialization;

string apikey = "";


if (string.IsNullOrWhiteSpace(apikey))
{
    Console.WriteLine("Configure apiKey");
    return;
}

Console.WriteLine($"1: {MapNames.Stockholm}");
Console.WriteLine($"2: {MapNames.Goteborg}");
Console.WriteLine($"3: {MapNames.Malmo}");
Console.WriteLine($"4: {MapNames.Uppsala}");
Console.WriteLine($"5: {MapNames.Vasteras}");
Console.WriteLine($"6: {MapNames.Orebro}");
Console.WriteLine($"7: {MapNames.London}");
Console.WriteLine($"8: {MapNames.Linkoping}");
Console.WriteLine($"9: {MapNames.Berlin}");

Console.Write("Select the map you wish to play: ");
string option = Console.ReadLine();

var mapName = option switch
{
    "1" => MapNames.Stockholm,
    "2" => MapNames.Goteborg,
    "3" => MapNames.Malmo,
    "4" => MapNames.Uppsala,
    "5" => MapNames.Vasteras,
    "6" => MapNames.Orebro,
    "7" => MapNames.London,
    _ => null
};

if (mapName is null)
{
    Console.WriteLine("Invalid map selected");
    return;
}

HttpClient client = new();
Api api = new(client);
MapData mapData = await api.GetMapDataAsync(mapName, apikey);
GeneralData generalData = await api.GetGeneralDataAsync();
SubmitSolution solution = new() 
{
    Locations = new()
};
foreach (KeyValuePair<string, StoreLocation> locationKeyPair in mapData.locations)
{
    StoreLocation location = locationKeyPair.Value;
    //string name = locationKeyPair.Key;
    var salesVolume = location.SalesVolume;
    if (salesVolume > 100)
    {
        solution.Locations[location.LocationName] = new PlacedLocations() 
        { 
            Freestyle3100Count = 0, 
            Freestyle9100Count = 1
        };
    }
}

GameData score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
Console.WriteLine(score.GameScore.Total);
//GameData prodScore = await api.SumbitAsync(mapName, solution, apikey);
//Console.WriteLine(prodScore.GameScore.Total);
Console.ReadLine();