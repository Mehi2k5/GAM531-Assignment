# Assignment 3: Rendering a 3D Cube

## Library Used
This project uses **OpenTK 4.9.4** with **.NET 8.0**.  
OpenTK provides the bindings to OpenGL needed to render 2D/3D graphics in C#.

## How the Cube Was Rendered
- Defined a cube using **8 unique 3D vertices** and indices to draw faces.
- Applied a **perspective projection matrix** to simulate 3D depth.
- Used a **view matrix** to position the camera looking at the cube.
- Implemented a simple **rotation transformation on the Y-axis** so the cube spins in place.
- Enabled **depth testing** to ensure correct face visibility when rendering.

The program creates an OpenGL context using OpenTK’s `GameWindow`, sets up the cube geometry, and renders it inside the main loop with continuous rotation.

## Example Screenshot
![Cube Screenshot](screenshot.png)

*(Replace `screenshot.png` with the actual filename of your screenshot in the repo.)*

## How to Run
1. Clone the repository:
   ```bash
   git clone git@github.com:Mehi2k5/GAM531-Assignment1.git
