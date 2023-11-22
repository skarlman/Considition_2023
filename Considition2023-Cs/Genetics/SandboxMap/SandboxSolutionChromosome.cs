using GeneticSharp;
using Shared.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics
{

    internal class SandboxSolutionChromosome : ChromosomeBase
    {
        const int totalStores =
                Scoring.maxGroceryStoreLarge +
                Scoring.maxGroceryStore +
                Scoring.maxConvenience +
                Scoring.maxGasStation +
                Scoring.maxKiosk;

        public SandboxSolutionChromosome(MapData mapData, GeneralData generalData) : base(totalStores)
        {
            _mapData = mapData;
            _generalData = generalData;
            NumLocations = _mapData.Hotspots.Count();
        }

        public override IChromosome CreateNew()
        {

            var result = new SandboxSolutionChromosome(_mapData, _generalData);
            do
            {
                result.CreateGenes();
            } while (result
                        .GetGenes()
                        .Select(g =>
                                    (((int, int, int))g.Value).Item1
                               )
                        .Distinct()
                        .Count() != result.Length);

            return result;
        }
        Random rng = new Random();

        public int NumLocations { get; }
        public MapData _mapData { get; }
        public GeneralData _generalData { get; }

        public override Gene GenerateGene(int geneIndex)
        {
            int locationIndex = -1;
            int smallMachines = 0;
            int bigMachines = 0;

            do
            {
                locationIndex = rng.Next(NumLocations);
                
                smallMachines = rng.Next(3);
                bigMachines = rng.Next(3);


                // in 95% of the cases, don't mix small and big machines
                if (rng.Next(20) > 0)
                {
                    if (rng.Next(2) > 0)
                    {
                        smallMachines = 0;
                    }
                    else
                    {
                        bigMachines = 0;
                    }
                }
            } while (smallMachines == 0 && bigMachines == 0);

            return new Gene((locationIndex, smallMachines, bigMachines));
        }

        private bool IsValidHotSpotLocation(int locationIndex, MapData mapData)
        {
            return mapData.Hotspots[locationIndex].Longitude < mapData.Border.LongitudeMax
                && mapData.Hotspots[locationIndex].Longitude > mapData.Border.LongitudeMin
                && mapData.Hotspots[locationIndex].Latitude < mapData.Border.LatitudeMax
                && mapData.Hotspots[locationIndex].Latitude > mapData.Border.LatitudeMin
                ;
        }


        //public static SandboxSolutionChromosome CreateAdamChromosome(MapData mapData, GeneralData generalData)
        //{
        //    var result = new SandboxSolutionChromosome(mapData, generalData);
        //    do
        //    {
        //        for (int i = 0; i < totalStores; i++)
        //        {
        //            result.ReplaceGene(i, new Gene((1, 0)));
        //        }
        //    } while (result
        //                .GetGenes()
        //                .Select(g =>
        //                            (((int, int, int))g.Value).Item1
        //                       )
        //                .Distinct()
        //                .Count() != result.Length);

        //    return result;
        //}

        internal SubmitSolution ToSolution(MapData mapData)
        {
            SubmitSolution result = new SubmitSolution()
            {
                Locations = new Dictionary<string, PlacedLocations>(totalStores)
            };
            int ctr = 0;

            for (int i = 0; i < Scoring.maxGroceryStoreLarge; i++)
            {
                var currentGene = ((int, int, int))GetGene(ctr).Value;
                int locationIndex = currentGene.Item1;
                PlacedLocations value = new PlacedLocations
                {
                    Freestyle3100Count = currentGene.Item2,
                    Freestyle9100Count = currentGene.Item3,
                    LocationType = _generalData.LocationTypes["groceryStoreLarge"].Type,
                    Longitude = _mapData.Hotspots[locationIndex].Longitude,
                    Latitude = _mapData.Hotspots[locationIndex].Latitude
                };
                
                if (value.Longitude > mapData.Border.LongitudeMax)
                    value.Longitude = mapData.Border.LongitudeMax;

                if (value.Longitude < mapData.Border.LongitudeMin)
                    value.Longitude = mapData.Border.LongitudeMin;

                if (value.Latitude > mapData.Border.LatitudeMax)
                    value.Latitude = mapData.Border.LatitudeMax;

                if (value.Latitude < mapData.Border.LatitudeMin)
                    value.Latitude = mapData.Border.LatitudeMin;

                result.Locations.Add($"location{ctr + 1}", value);

                ctr++;
            }
            for (int i = 0; i < Scoring.maxGroceryStore; i++)
            {
                var currentGene = ((int, int, int))GetGene(ctr).Value;
                int locationIndex = currentGene.Item1;
                PlacedLocations value = new PlacedLocations
                {
                    Freestyle3100Count = currentGene.Item2,
                    Freestyle9100Count = currentGene.Item3,
                    LocationType = _generalData.LocationTypes["groceryStore"].Type,
                    Longitude = _mapData.Hotspots[locationIndex].Longitude,
                    Latitude = _mapData.Hotspots[locationIndex].Latitude
                };
                if (value.Longitude > mapData.Border.LongitudeMax)
                    value.Longitude = mapData.Border.LongitudeMax;

                if (value.Longitude < mapData.Border.LongitudeMin)
                    value.Longitude = mapData.Border.LongitudeMin;

                if (value.Latitude > mapData.Border.LatitudeMax)
                    value.Latitude = mapData.Border.LatitudeMax;

                if (value.Latitude < mapData.Border.LatitudeMin)
                    value.Latitude = mapData.Border.LatitudeMin;

                result.Locations.Add($"location{ctr + 1}", value);

                ctr++;
            }
            for (int i = 0; i < Scoring.maxConvenience; i++)
            {
                var currentGene = ((int, int, int))GetGene(ctr).Value;
                int locationIndex = currentGene.Item1;
                PlacedLocations value = new PlacedLocations
                {
                    Freestyle3100Count = currentGene.Item2,
                    Freestyle9100Count = currentGene.Item3,
                    LocationType = _generalData.LocationTypes["convenience"].Type,
                    Longitude = _mapData.Hotspots[locationIndex].Longitude,
                    Latitude = _mapData.Hotspots[locationIndex].Latitude
                };
                if (value.Longitude > mapData.Border.LongitudeMax)
                    value.Longitude = mapData.Border.LongitudeMax;

                if (value.Longitude < mapData.Border.LongitudeMin)
                    value.Longitude = mapData.Border.LongitudeMin;

                if (value.Latitude > mapData.Border.LatitudeMax)
                    value.Latitude = mapData.Border.LatitudeMax;

                if (value.Latitude < mapData.Border.LatitudeMin)
                    value.Latitude = mapData.Border.LatitudeMin;

                result.Locations.Add($"location{ctr + 1}", value);

                ctr++;
            }
            for (int i = 0; i < Scoring.maxGasStation; i++)
            {
                var currentGene = ((int, int, int))GetGene(ctr).Value;
                int locationIndex = currentGene.Item1;
                PlacedLocations value = new PlacedLocations
                {
                    Freestyle3100Count = currentGene.Item2,
                    Freestyle9100Count = currentGene.Item3,
                    LocationType = _generalData.LocationTypes["gasStation"].Type,
                    Longitude = _mapData.Hotspots[locationIndex].Longitude,
                    Latitude = _mapData.Hotspots[locationIndex].Latitude
                };
                if (value.Longitude > mapData.Border.LongitudeMax)
                    value.Longitude = mapData.Border.LongitudeMax;

                if (value.Longitude < mapData.Border.LongitudeMin)
                    value.Longitude = mapData.Border.LongitudeMin;

                if (value.Latitude > mapData.Border.LatitudeMax)
                    value.Latitude = mapData.Border.LatitudeMax;

                if (value.Latitude < mapData.Border.LatitudeMin)
                    value.Latitude = mapData.Border.LatitudeMin;

                result.Locations.Add($"location{ctr + 1}", value);

                ctr++;
            }

            for (int i = 0; i < Scoring.maxKiosk; i++)
            {
                var currentGene = ((int, int, int))GetGene(ctr).Value;
                int locationIndex = currentGene.Item1;
                PlacedLocations value = new PlacedLocations
                {
                    Freestyle3100Count = currentGene.Item2,
                    Freestyle9100Count = currentGene.Item3,
                    LocationType = _generalData.LocationTypes["kiosk"].Type,
                    Longitude = _mapData.Hotspots[locationIndex].Longitude,
                    Latitude = _mapData.Hotspots[locationIndex].Latitude
                };
                if (value.Longitude > mapData.Border.LongitudeMax)
                    value.Longitude = mapData.Border.LongitudeMax;

                if (value.Longitude < mapData.Border.LongitudeMin)
                    value.Longitude = mapData.Border.LongitudeMin;

                if (value.Latitude > mapData.Border.LatitudeMax)
                    value.Latitude = mapData.Border.LatitudeMax;

                if (value.Latitude < mapData.Border.LatitudeMin)
                    value.Latitude = mapData.Border.LatitudeMin;

                result.Locations.Add($"location{ctr + 1}", value);

                ctr++;
            }

            //Scoring.SandboxValidation(_mapData.MapName, result, _mapData);

            return result;
        }
    }
    public class Terminator : ITermination
    {
        private double _scoreToExceed;

        public Terminator(double scoreToExceed)
        {
            _scoreToExceed = scoreToExceed;
        }

        public bool HasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            return geneticAlgorithm.BestChromosome.Fitness > _scoreToExceed;
        }
    }
}
