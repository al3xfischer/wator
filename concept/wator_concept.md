---
title: Wa-Tor
author: Alexander Fischer & Paul-Noel Ablöscher
date: June 2021
header-includes:
    \hypersetup{colorlinks=false, allbordercolors={0 0 0}, pdfborderstyle={/S/U/W 1}}
    \usepackage{float}
    \floatplacement{figure}{H}
---

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

# MPI Concept

# Multithreading Concept