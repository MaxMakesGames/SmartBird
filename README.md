# SmartBird
A game where birds use a neural network to learn how to flap to survive in Unity.

This is meant to be a code reference for a basic random neural network and how to apply it to a game
I made a video explaining a bit more how the neural network works here: https://youtu.be/k-HTSCEFoZw

If you try to import this whole project, you may need to create a new object layer, set the bird prefab on that layer and disable in settings -> physics 2D in the collision matrix the collision between that layer and itself. This is to avoid the birds colliding with each other.
