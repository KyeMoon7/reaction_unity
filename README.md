# reaction_unity

Un jeu de réaction rapide où il faut cliquer quand un cercle apparait à l'écran. Scène principale : `Blinker`.

- La méthode `Trigger()` du script `GameManger.cs` démarre le jeu et permette de jouer (avec un bouton d'arcade par exemple)
- La méthode `GetUIElementVisible()` du script `GameManger.cs` retourne un `bool` pour savoir si un élément est visible à l'écran (pour synchroniser avec la dEL d'un bouton d'arcade par exemple).
