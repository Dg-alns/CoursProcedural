Cours Procedural

## Table Of Contents

<details>
<summary>Details</summary>

- [Architecture Général](#architecture-général)
  - [Grid](#grid)
  - [Cell](#cell)
  - [Procedural Generation Method](#proceduralGenerationMethod)
  - [Unity Inspector](#noise)
- [Comment ajouter un nouvelle algorithme](#ajouterNewAlgo)
- [Simple Room Placement](#srp)
  - [Unity Inspector](#noise)
- [BSP](#bsp)
  - [Unity Inspector](#noise)
- [Cellular Automata](#ca)
  - [Unity Inspector](#noise)
  - [Cell Rule Systeme](#caCRS)
  - [Configuration](#caCRSC)
- [Noise](#noise)
  - [Terrain](#noiseT)
- [Unity Inspector](#noise)

</details>


## Architecture Général 

Chaque Algorithme de génération de procédural de terrain hérite d'une architecture principal. <br>
Cette architecture met en place 3 scritps majeur *Grid*, *Cell*, *ProceduralGenerationMethod*.<br>



## Grid

Comme sont nom l'évoque ce script permet la gestion de la grille de *Cell* ou vas ce retrouver le terrain final.<br>

  - ## Methode principal

      *TryGetCellByCoordinates*<br>
          <ul>
            Cette méthode permet de détecter si une *Cell* est contenue dans à des coordonées précise.<br>
            Les arguments que reçoit cette methodes sont :<br>
          <ul>
              <li> Les coordonées de la cellule voulue en int (x, y) ou via un Vector2Int.
              <li> La cellule voulue via *Cell*.
          </ul>
          </ul>
    <br>
    La methode renvoit un booléan.<br>


## Cell

*Cell*, est le script permetant l'accès au information contenue dans une cellule.<br>

  - ## Getter principal

      *ContainObject*<br>
          <ul>
            Ce getter revoit un booléan permettant de savoir si la cellule contient une object.<br>
          </ul>
    <br>
    
      *GridObject*<br>
          <ul>
            Ce getter revoit un *GridObject* permettant d'accéder au information de l'object notament son nom via **Template** conteneu dans *GridObject*.<br>
          </ul>
    <br>

    
## Procedural Generation Method

**Procedural Generation Method** est un scripteble object.<br>
Il va contenir des information comme le nombre maximum de step, *Grid* et aussi le nom des tile (object) à placer dans *Cell*.


  - ## Methode principal

      *ApplyGeneration*<br>
          <ul>
            Cette méthode abstract est à définir **obligatoirement** dans les scipts enfant de *Procedural Generation Method*.<br>
            Elle permet d'effectuer la génération du terrain d'un algoriytme.<br>
          </ul>
    <br>

      *CanPlaceRoom*<br>
          <ul>
            Cette méthode permet de détecter si une room peut être placer ou non.<br>
            Les arguments que reçoit cette methodes sont :<br>
          <ul>
              <li> La room à placer via un Vector2Int.
              <li> L'espace voulue autour de la room (pour éviter de coller 2 room) via un int.
          </ul>
          </ul>
    <br>
    La methode renvoit un booléan.    
    <br>

      *AddTileToCell*<br>
          <ul>
            Cette méthode va ajouter une tile (object) à une *Cell*.<br>
            Les arguments que reçoit cette methodes sont :<br>
          <ul>
              <li> La cellule qui va contenir la tile via *Cell*.
              <li> Le non de la tile via un string.
              <li> La posibiliter de remplacer la tile existante via un bool.
          </ul>
          ```csharp
            protected void AddTileToCell(Cell cell, string tileName, bool overrideExistingObjects)
          {
              var tileTemplate = ScriptableObjectDatabase.GetScriptableObject<GridObjectTemplate>(tileName);
              GridGenerator.AddGridObjectToCell(cell, tileTemplate, overrideExistingObjects);
          }
          ```
          </ul>
    <br>


## Procedural Generation Method



























