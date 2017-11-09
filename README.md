# Voronoi selection in 3D

This is a Voronoi selection implementation for 3D scenarios. It greatly improves the user experience for complex data visualization and allows users to easily select specific nodes and interact with them. Voronoi is a well-known method for 3D tessellation and for 2D graph selection. See the following samples to understand the power of Voronoi:

http://bl.ocks.org/nbremer/d5ef6c58f85aba2da48b (no Voronoi selection)
![image](https://user-images.githubusercontent.com/10086264/32611020-725bdb16-c532-11e7-900e-0486de44b2f5.png)

http://bl.ocks.org/nbremer/61cd485e399b6a71d5fb2b1072fbc6c1 (with Voronoi selection)
![image](https://user-images.githubusercontent.com/10086264/32611029-781f0ad2-c532-11e7-8d80-0dc9586d9d76.png)


# How it works

Voronoi takes an array of 2D coordinates as input and generates areas around each of these points while making sure they do not intersect with each other. This creates great hit targets around points and allows for a margin of error when selecting in a 3D environment. 

# Required Software

  - **Unity 5.6.4f1** or higher


# Getting started
  - Open the project in Unity 
  - Hit Play and hold right click to pan around the scene. Focused nodes will turn blue.
  - Hold shift and left click to drag the selected node
  - Be amazed! 
  - Change number of nodes generated and spwan radius in the VoronoiScript under Graph 

# Applications
  - **Complex Medical Data** - This implementation is used by Weill Cornell Medicine to interact with complex Cancer Drug Networks. The application is called HoloGraph and can be downloaded from the Microsoft Store. For more information https://elementolab.github.io/holograph/ 
