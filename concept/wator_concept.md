---
title: Wa-Tor
author: Alexander Fischer & Paul-Noel Ablöscher
date: June 2021
header-includes:
    \hypersetup{colorlinks=false, allbordercolors={0 0 0}, pdfborderstyle={/S/U/W 1}}
    \usepackage{float}
    \floatplacement{figure}{H}
---

\pagebreak

# Wa-Tor Simulation
Wa-Tor is a predators and prey population simulation.
The planet on which those species live is shaped like a doughnut or torus.
It's normally realized as a two dimensional grid.
Each cell on that grid can contain a fish, shark or just water.
Time passes in chronons.
All animals move during each chronon and if they trespass the grid borders they reappear on the opposite border.
Movement can take place in four directions up, down, left and right.
Sharks must eat fish regularly to survive otherwise they starve.
Fish eat plankton which is assumed to be infinitely available.
Both species breed if certain conditions are met.

## Rules
### Fish
Move to a randomly chosen empty field.
If no field is available they do not move.

Once it survived a certain amount of chronons a new fish is bred to an adjacent cell.

### Sharks
Moves to a surrounding cell that contains a fish.
If no prey is found then a shark moves to an empty field otherwise it does not move.

Each chronon a shark loses energy and dies upon reaching zero.
Sharks gain a fixed amount of energy when eating fish.
Once it survived a certain amount of chronons a new shark is bred to an adjacent cell.
Energy is distributed equally between the current and the new shark.

# General Concept
A two dimensional is used to represent the planet Wa-Tor.
The array stores integer values to represent population.

| Condition | Represented Content |
| --------- | ------------------- |
| cell < 0  | Shark               |
| cell > 0  | Fish                |
| cell = 0  | Water               |

To model energy drain and aging the integer values are increased by one during each chronon.
If a shark's energy increases its energy values moves towards zero (due to being negative) which means it starves.
If a fish's value increases it simply means that it aged by one chronon.

## Why integers?
Prior to using integer values objects were used to represent population.
That led to excessive memory usage.
A 10,000 x 10,000 field filled with animal objects used approximately 4GB of memory.
Switching to integers reduced the memory usage by a factor of ten.

## Parallelization
Geometric decomposition is used to process the field in parallel.
The basic concept of that pattern is to split the field into subfields which are processed in parallel.
It cannot be naively parallelized because the decisions of animals in subfield borders can affect other subfields.
We decided to use a simple solution in which we divide the field by rows.
By using this approach only two borders per subfield must be synchronized.

![Geometric decomposition](./images/geometric_decomposition.drawio.png)

# MPI Concept

The master process assigns each worker process including itself a subfield.
The first step is to calculate the local inner field.
Then the local version of the last two rows are sent to the next worker
which uses the information to calculate its upper border and sends back the result.
The last two rows are sent because animals on the border can move backwards to the inner field.

![MPI Parallelization](./images/mpi.drawio.png)

## Communication
The send and receive methods block code execution until the transmission is completed.
It was a challenge to implement the communication so that no deadlocks are produced.
That is because it is not easy to properly debug multi-process programs.
Another problem that occurred during implementation was that efficient data-structures such as "Memory2D" do not feature the "Serializable" attribute.
In consequence we were not able to use it as MPI is not able to de/serialize it. 

# Multithreading Concept

At the program start the field is split into sections and each thread is responsible for one section.
Similar to the MPI concept each inner sub section is processed in parallel.
Then the borders are calculated within their own thread.

![Multithreading Parallelization](./images/multithreading.drawio.png)

## Coordination
We had less challenge in the multithreading solution.
That is due to the missing communication overhead, no deadlocks and easier debugging.
Provided methods of the "System.Threading.Tasks.Parallel" namespace such as "Parallel.For(...)" kept the implementation simple.

# Performance Analysis
## System Specs
| Hardware | Name              |
| -------- | ----------------- |
| CPU      | AMD Ryzen 7 2700X |
| RAM      | 16 GB             |

\pagebreak
## Performance Data
### MPI
| Processing Units | Average in ms | 20 Iterations in ms |
| ---------------- | ------------- | ------------------- |
| 1                | 6,980         | 139,596             |
| 2                | 40,108        | 812,160             |
| 4                | 34,671        | 656,420             |
| 8                | 16,700        | 334,400             |
| 16               | 38,373        | 787,460             |
| 32               | 62,770        | 1,245,400           |

### Multithreading
| Processing Units | Average in ms | 20 Iterations in ms |
| ---------------- | ------------- | ------------------- |
| 1                | 8,631         | 172,620             |
| 2                | 4,768         | 96,361              |
| 4                | 3,200         | 64,012              |
| 8                | 2,675         | 53,616              |
| 16               | 2,654         | 53,090              |
| 32               | 2,655         | 53,118              |

## Interpretation
As displayed in the performance data above using MPI is significantly slower than multithreading.
That may be due to the communication overhead MPI introduces.
Each subfield is distributed over the network and finally gathered and merged by the master process.
It can also be noticed that the MPI performance peaked when using 8 processes and decreased when using more processes.

The multiprocessing approach performed better than the MPI one.
Its performance increased upon reaching 8 threads and then stagnated.
But the performance did no decrease while using more than 8 threads.
It can be hypothesized that the multithreading approach performed better because no network communication is necessary.
In addition multithreading runs in a single process and therefore the memory is shared among all threads.

Performance data also showed a peak in both approaches when reaching 8 processing units.
It could be argued that this number of processing units is optimal as the system CPU features 8 cores.

# Appendix

![Class diagram core project](./images/ClassDiagram_Core.png)

![Class diagram mpi project](./images/ClassDiagram_MPI.png)

![Sequence diagram MPI](./images/mpi_sequence.drawio.png)

![Sequence diagram Multithreading](./images/multithread_sequence.drawio.png)