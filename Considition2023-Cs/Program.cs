using Considition2023_Cs;
using Considition2023_Cs.Game;
using Considition2023_Cs.Genetics;
using Considition2023_Cs.Genetics.NormalMap;
using GeneticSharp;
using System.Text;
using System.Text.Json.Serialization;

const string apikey = "3c5671b2-caaa-4644-b453-b788e5d83549";


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
Console.WriteLine($"10: {MapNames.GSandbox}");
Console.WriteLine($"11: {MapNames.SSandbox}");

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
    "8" => MapNames.Linkoping,
    "9" => MapNames.Berlin,
    "10" => MapNames.GSandbox,
    "11" => MapNames.SSandbox,
    _ => null
};

if (mapName is null)
{
    Console.WriteLine("Invalid map selected");
    return;
}


bool isHardcore = Scoring.SandBoxMaps.Contains(mapName.ToLower());

HttpClient client = new();
Api api = new(client);
MapData mapData = await api.GetMapDataAsync(mapName, apikey);
GeneralData generalData = await api.GetGeneralDataAsync();


// ----  My solution -----

DateTime starttime = DateTime.UtcNow;


SubmitSolution solution = isHardcore
                            ? RunSandboxMapFitness(mapData, generalData)
                            : RunNormalMapFitness(mapData, generalData);


Console.WriteLine($"Found in {(DateTime.UtcNow - starttime).TotalSeconds}s.");
GameData score = Scoring.CalculateScore(mapData.MapName, solution, mapData, generalData);
Console.WriteLine($"GameScore: {score.GameScore.Total}");

Scoring.SandboxValidation(mapName, solution, mapData);

GameData serverScore = await api.SumbitAsync(mapName, solution, apikey);
Console.WriteLine($"GameId: {serverScore.Id}");
Console.WriteLine($"GameScore: {serverScore.GameScore.Total}");

if (score.GameScore.Total != serverScore.GameScore.Total)
{
    Console.WriteLine("!!!! LOCAL SCORE DIFFERS FROM SERVER !!!!!");
}

Console.ReadLine();
return;



// --- Original sample code ----
SubmitSolution solution_orig = new()
{
    Locations = new()
};


if (isHardcore)
{
    var hotspot = mapData.Hotspots[0];
    var hotspot2 = mapData.Hotspots[1];

    solution_orig.Locations.Add("location1", new PlacedLocations()
    {
        Freestyle9100Count = 2,
        Freestyle3100Count = 1,
        //LocationType = generalData.LocationTypes["kiosk"].Type,
        LocationType = generalData.LocationTypes["groceryStoreLarge"].Type,
        Longitude = hotspot.Longitude,
        Latitude = hotspot.Latitude
    });
    solution_orig.Locations.Add("location2", new PlacedLocations()
    {
        Freestyle9100Count = 0,
        Freestyle3100Count = 1,
        LocationType = generalData.LocationTypes["groceryStore"].Type,
        Longitude = hotspot2.Longitude,
        Latitude = hotspot2.Latitude
    });
}
else
{
    foreach (KeyValuePair<string, StoreLocation> locationKeyPair in mapData.locations)
    {
        StoreLocation location = locationKeyPair.Value;
        //string name = locationKeyPair.Key;
        var salesVolume = location.SalesVolume;
        if (salesVolume > 100)
        {
            solution_orig.Locations[location.LocationName] = new PlacedLocations()
            {
                Freestyle3100Count = 0,
                Freestyle9100Count = 1
            };
        }
    }
}

if (isHardcore)
{
    var hardcoreValidation = Scoring.SandboxValidation(mapName, solution_orig, mapData);
    if (hardcoreValidation is not null)
    {
        throw new Exception("Hardcore validation failed");
    }
}

GameData score_orig = Scoring.CalculateScore(mapName, solution_orig, mapData, generalData);
Console.WriteLine($"GameScore: {score_orig.GameScore.Total}");
GameData prodScore = await api.SumbitAsync(mapName, solution_orig, apikey);
Console.WriteLine($"GameId: {prodScore.Id}");
Console.WriteLine($"GameScore: {prodScore.GameScore.Total}");
Console.ReadLine();

static SubmitSolution RunNormalMapFitness(MapData mapData, GeneralData generalData)
{
    var populationMinSize = 100;
    var populationMaxSize = 150;
    var iterations = 10;

    return FitnessRunner.RunEvolution(mapData, generalData, populationMinSize, populationMaxSize, iterations);
}
static SubmitSolution RunSandboxMapFitness(MapData mapData, GeneralData generalData)
{
    var populationMinSize = 100;
    var populationMaxSize = 150;
    var iterations = 10;

    return SandboxFitnessRunner.RunEvolution(mapData, generalData, populationMinSize, populationMaxSize, iterations);
}

