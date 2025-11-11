Cours Procedural

## Table Of Contents

<details>
<summary>Details</summary>

- [Architecture Général](#architecture)
  - [Grid](#grid)
  - [Cell](#cell)
  - [ProceduralGenerationMethod](#proceduralGenerationMethod)
- [Comment ajouter un nouvelle algorithme](#ajouterNewAlgo)
- [Simple Room Placement](#srp)
  - [Utilité](#srpU)
  - [Limites](#srpL)
- [BSP](#bsp)
  - [Utilité](#bspU)
  - [Limites](#bspL)
- [Cellular Automata](#ca)
  - [Utilité](#caU)
  - [Limites](#caL)
  - [Cell Rule Systeme](#caCRS)
  - [Configuration](#caCRSC)
- [Noise](#noise)
  - [Utilité](#noiseU)
  - [Limites](#noiseL)
  - [Terrain](#noiseT)

</details>


## Architecture Général 

Chaque Algorithme de génération de procédural de terrain hérite d'une architecture principal. <br>
Cette architecture met en place 3 scritps majeur #Grid, #Cell, #ProceduralGenerationMethod.



## Grid

Comme sont nom l'évoque ce script permet la gestion de la grille de #Cell ou vas ce retrouver le terrain final.

  - ## Methode principal

      #TryGetCellByCoordinates
          Cette méthode permet de détecter si une #Cell est contenue dans à des coordonées précise.
          Les arguments que reçoit cette methodes sont :
              1. Les coordonées de la cellule voulue en int (x, y) ou via un Vector2Int
              2. La cellule voulue via #Cell
          La methode renvoit un booléan.






























