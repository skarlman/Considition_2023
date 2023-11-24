# Genetic algorithm for the Considition 2023 competition
## Defending the "Best worst algorithm prize" from 2022

### Competition:
[See competition here for rules](https://www.considition.com/)

The solution contains 3 projects. 
* Considition2023-Cs  - The game logic
* Shared - Pocos and config
* Solution submitter - Rest proxy to handle retries and allow fast submissions for the game regardless of the server (big part of the success for 2022)

## Algorithm
There are two variations of the game - Normal maps and Sandbox maps

### Normal maps
A gene consists of two numbers, how many of each type of refill stations. It is initialized randomly between 0-2, and in 95% of the cases one is set to 0 to not mix stations (hypothesis, it will very rarely be optimal to mix)
A Chromosome consists of a gene for every location in the map.

Then the genetic algorithm evolves by Elite selection, Crossovers and mutations. The given Scoring functions in the starting code is used for fitness evaluation. Run until happy and submit the solution every 30 seconds.

### Sandbox maps
Now we have 4 variables - two refill stations, a location and a location type. There is however a constraint on how many location types you can use.

The assumption is that the optimal solution will contain the maximum allowed location types placed on the map. The chromosome length is therefore now the total number of location types.

I.e. First 5 genenes are "Large gorcery stores", next 20 are "Grocery stores", next 20 "Convenience stores" etc.

The gene initializes (and mutates) with a random location from the HotSpots (moved into the bounds of the map if needed) and 0-2 of the refill types (as in the normal map). Then the whole chromosome is checked that there is no other gene with the same location, otherwise it will pick another random location.

### Closing remarks
The beauty of the algorithm is that since no analysis is needed - it was very simple to adapt to the new conditions when the sandbox maps where introduced. By definition it just optimizes on the constraints given by the rules (scoring function).

 It performed quite good for the little effort put into it; but ended up quite bad in place 50/67 (but #5 on the Sandbox map). 

 Don't know if it got stuck in a local maxima or just needed more CPU power/time (it is not optimized for performance)