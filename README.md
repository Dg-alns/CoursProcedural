Cours Procedural

## Table Of Contents

<details>
<summary>Details</summary>

- [Architecture Général](#architecture-général)
  - [Grid](#grid)
  - [Cell](#cell)
  - [Procedural Generation Method](#proceduralGenerationMethod)
- [Comment ajouter un nouvelle algorithme](#ajouterNewAlgo)
- [Simple Room Placement](#srp)
- [BSP](#bsp)
- [Cellular Automata](#ca)
  - [Cell Rule Systeme](#caCRS)
- [Noise](#noise)
  - [Terrain](#noiseT)

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
          </ul>
    <br>


## Comment ajouter un nouvelle algorithme

Pour créer et ajouter un nouvelle algorithme, il suffit de suivre ce chemin.<br>
<ul>
  <li>Créer un nouveau scripts Monobehavior.<br>
  <li>Remplacer Monobehavior par *ProceduralGenerationMethod*.<br>
  <li>Ajouter le méthode **ApplyGeneration** en override.<br>
  <li>Créer votre algorithme<br>
</ul>
  <br>


## Simple Room Placement

Cette algorithme permet de crée des rooms aléatoirement (en fonction d'une seed) sur la grille. <br>

Des couloirs sont placer par la suite en respectant une règle.<br>

Chaque room ne contient que **un** couloir d'entrer et **un** couloir de sortie.<br>

On commence par relier la room la plus à gauche et en haut de la grille à celle la plus proche.<br>

Et ainsi de suite la room relier devient la source et relier la room la plus proche...

<br><br>

Les avantages de cette algorithme mais qui en fais aussi une limite est sa complexiter minime.
Les rooms ne pouvant pas être interconecter on retouve vite une shémas de cercle.

  <br>


## BSP

L'algorithme BSP ou Binary Space Partition, est un algorithme basée sur le découpage de la grille en deux sur plusieure génération.<br>

La grille de base ou aussi appeler **root** vas alors se sindre en 2 et crée un enfant A et un enfant B, c'est enfant sont appeler **node**.<br>

Ces enfant vont respectivement de cindre en 2<br>

Les enfant fineaux, appeler **leafs** vont correspondre au espace ou des rooms vont pourvoire être crées.<br>

Ces rooms vont alors être relier en fraterie via leur **node** et on remonte les node ainsi de suite pour relier tous les rooms.

<br><br>

L'avantage principale de cette algorithme est présent notament sur la création des couloire liant les rooms entre elle. On perd l'aspet de cercle present dans l'algorithme Simple Room Placement.<br>
Cette algorithme vas être généralement favoriser pour la création de donjon.


  <br>


## Cellular Automata

Le Cellular Automata est un algorithme vivant. Il va génerer un bruit blanc dit **noise** (génération purement aléatoire) de *Cell*.<br>

L'algorithme vas gérer l'état de des *Cell* en fonction des *Cell* qui l'entoure.<br>

Une *Cell* vas avoir au maximum 8 *Cell* voisine ayant chacune un état propre.<br>

Le *Cell Rules* vas alors récupérer les états des *Cell* voisine et gérer en fontion des **Rule** associer au type de la cellule que l'on inspect.

<br><br>

L'avantage de cette algorithme est que l'on crée des cellule qui s'adapte à un environement vivant et qui s'adapte à c'est alentour.<br>
Cette algorithme est donc priser lors de la création de terrain de taille restraint.<br>

La **taille** est la limite majeur à cette algorithme au vu du à la gestion de toute les cellules en fonction de leur voisine.


  <br>


## Noise

Le noise est un algorithme qui vas générer une **noise** (valeur aléatoire entre -1 et 1) dans la grille.<br>

On va donc générer des *Tile* en fonction de la valeur associer au coordonées de la *Cell*.<br>

Exemple 
<ul>
  <li>(-1, -0.5): Eau</li>
  <li>(-0.51, 0.5): Herbe</li>
  <li>(0.51, 1): Pierre</li>
</ul>
<br>

Le **noise** utiliser est un **fractal noise**, c'est une addition de plusieur **noise** permetant d'ajouter du detail<br>

Ce **noise** est soumis à de multiple contrainte dons les plus importante sont:<br>
<ul>
  <li>Amplitude: De base à 1 si l'on augmente l'amplitude on vas alors augmente des les extremes (plus accesibles)</li>
  <li>Frequence: De base à 0.03 si l'on augmente la fréquence on vas alors détailler le **noise** (principe d'un zoom)</li>
  <br>
  <li>Lacunariter: De base à 2 si l'on augmente la fréquence vas augmenter pour chaque octave. On vas donc accorder d'avantage d'influence au dernière octave sur l'aspect final. La lacunariter augmente les petits détails</li>
  <li>Persistance: De base à 0.5 elle permet de donner ou non de l'importance au octave. La persistance affecte ou non la modification de ces détailles lors des génération</li>
</ul>

<br><br>

L'avantage de cette algorithme est que l'on peut générer de très grande grille assez rapidement.<br>
Mais aussi on peut générer des meshs avec cette algorithme.



<img src="Documentation/Images/PNG_Zenject-colour (1).png?raw=true" alt="Zenject" width="900px" height="234px"/>










