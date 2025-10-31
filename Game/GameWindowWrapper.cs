using Huy_Hoang_Midterm_Game.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.IO;

namespace Game
{
    public class GameWindowWrapper : GameWindow
    {
        private Shader _shader = null!;
        private Mesh _cubeMesh = null!;
        private Mesh _planeMesh = null!;
        private Texture _crateTex = null!;
        private Texture _floorTex = null!;
        private Texture _gemTex = null!;
        private Camera _camera = null!;

        private Vector3 _lightPos = new Vector3(0f, 2f, 2f);
        private bool _lightEnabled = true;

        private Vector3 _lampPos = new Vector3(0f, 1.2f, 2f);
        private Vector3 _gemPos = new Vector3(-1.5f, 0.5f, -1.5f);
        private bool _gemCollected = false;

        private float _lastX, _lastY;
        private bool _firstMove = true;
        private double _time;

        public GameWindowWrapper(int width, int height, string title)
            : base(GameWindowSettings.Default,
                   new NativeWindowSettings() { Size = new Vector2i(width, height), Title = title })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            try
            {
                // Load shaders
                string vertexPath = Path.Combine(AppContext.BaseDirectory, "Shaders", "vertex.glsl");
                string fragmentPath = Path.Combine(AppContext.BaseDirectory, "Shaders", "fragment.glsl");
                if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
                    throw new FileNotFoundException("Shader files not found. Make sure they are in the output directory.");

                _shader = new Shader(vertexPath, fragmentPath);

                // Create meshes
                _cubeMesh = CreateCube();
                _planeMesh = CreatePlane();

                // Load textures
                string cratePath = Path.Combine(AppContext.BaseDirectory, "Assets", "crate.jpg");
                string floorPath = Path.Combine(AppContext.BaseDirectory, "Assets", "floor.jpg");
                string gemPath = Path.Combine(AppContext.BaseDirectory, "Assets", "gem.jpg");

                if (!File.Exists(cratePath) || !File.Exists(floorPath) || !File.Exists(gemPath))
                    throw new FileNotFoundException("One or more texture files are missing.");

                _crateTex = new Texture(cratePath);
                _floorTex = new Texture(floorPath);
                _gemTex = new Texture(gemPath);

                _camera = new Camera(new Vector3(0f, 1.2f, 5f));

                CursorState = CursorState.Grabbed;

                // Initialize mouse position
                var state = MouseState;
                _lastX = state.X;
                _lastY = state.Y;

                Console.WriteLine("Game loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during OnLoad: " + ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1); // Close gracefully instead of crashing silently
            }
        }


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            _time += args.Time;

            if (KeyboardState.IsKeyDown(Keys.Escape))
                CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;

            float speed = 3.5f;
            float delta = (float)args.Time;
            if (KeyboardState.IsKeyDown(Keys.W)) _camera.Position += _camera.Front * speed * delta;
            if (KeyboardState.IsKeyDown(Keys.S)) _camera.Position -= _camera.Front * speed * delta;
            if (KeyboardState.IsKeyDown(Keys.A)) _camera.Position -= _camera.Right * speed * delta;
            if (KeyboardState.IsKeyDown(Keys.D)) _camera.Position += _camera.Right * speed * delta;

            if (KeyboardState.IsKeyPressed(Keys.E))
            {
                if ((_camera.Position - _lampPos).Length <= 2.0f)
                    _lightEnabled = !_lightEnabled;
            }

            if (KeyboardState.IsKeyPressed(Keys.F))
            {
                if (!_gemCollected && (_camera.Position - _gemPos).Length <= 1.5f)
                {
                    _gemCollected = true;
                    Console.WriteLine("💎 Gem collected!");
                }
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (CursorState != CursorState.Grabbed) return;

            if (_firstMove)
            {
                _lastX = e.X;
                _lastY = e.Y;
                _firstMove = false;
            }

            float deltaX = e.X - _lastX;
            float deltaY = e.Y - _lastY;
            _lastX = e.X;
            _lastY = e.Y;

            _camera.ProcessMouseMovement(deltaX, deltaY);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0.12f, 0.15f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(_camera.Fov),
                Size.X / (float)Size.Y,
                0.1f,
                100f);

            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetVec3("viewPos", _camera.Position);

            _shader.SetInt("material.diffuse", 0);
            _shader.SetVec3("material.specular", new Vector3(0.5f));
            _shader.SetFloat("material.shininess", 32f);

            _shader.SetVec3("light.position", _lightPos);
            _shader.SetVec3("light.ambient", new Vector3(0.1f));
            _shader.SetVec3("light.diffuse", new Vector3(0.8f));
            _shader.SetVec3("light.specular", new Vector3(1.0f));
            _shader.SetBool("light.enabled", _lightEnabled);

            // Floor
            _shader.SetMatrix4("model", Matrix4.CreateScale(10f, 1f, 10f));
            _floorTex.Use(TextureUnit.Texture0);
            _planeMesh.Render();

            // Crate
            _shader.SetMatrix4("model", Matrix4.CreateScale(1f) * Matrix4.CreateTranslation(0f, 0.5f, 0f));
            _crateTex.Use(TextureUnit.Texture0);
            _cubeMesh.Render();

            // Lamp
            _shader.SetMatrix4("model", Matrix4.CreateScale(0.4f) * Matrix4.CreateTranslation(_lampPos));
            _crateTex.Use(TextureUnit.Texture0);
            _cubeMesh.Render();

            // Gem
            if (!_gemCollected)
            {
                _shader.SetMatrix4("model", Matrix4.CreateScale(0.3f) * Matrix4.CreateTranslation(_gemPos));
                _gemTex.Use(TextureUnit.Texture0);
                _cubeMesh.Render();
            }

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _shader?.Dispose();
            _cubeMesh?.Dispose();
            _planeMesh?.Dispose();
            _crateTex?.Dispose();
            _floorTex?.Dispose();
            _gemTex?.Dispose();
        }

        private Mesh CreateCube()
        {
            float[] vertices = {
                // positions        // normals         // texcoords
                -0.5f,-0.5f,0.5f, 0f,0f,1f, 0f,0f,
                 0.5f,-0.5f,0.5f, 0f,0f,1f, 1f,0f,
                 0.5f, 0.5f,0.5f, 0f,0f,1f, 1f,1f,
                -0.5f, 0.5f,0.5f, 0f,0f,1f, 0f,1f,
                -0.5f,-0.5f,-0.5f,0f,0f,-1f, 1f,0f,
                 0.5f,-0.5f,-0.5f,0f,0f,-1f, 0f,0f,
                 0.5f, 0.5f,-0.5f,0f,0f,-1f, 0f,1f,
                -0.5f, 0.5f,-0.5f,0f,0f,-1f, 1f,1f,
                -0.5f,-0.5f,-0.5f,-1f,0f,0f, 0f,0f,
                -0.5f,-0.5f,0.5f,-1f,0f,0f, 1f,0f,
                -0.5f, 0.5f,0.5f,-1f,0f,0f, 1f,1f,
                -0.5f, 0.5f,-0.5f,-1f,0f,0f, 0f,1f,
                 0.5f,-0.5f,-0.5f,1f,0f,0f, 1f,0f,
                 0.5f,-0.5f,0.5f,1f,0f,0f, 0f,0f,
                 0.5f, 0.5f,0.5f,1f,0f,0f, 0f,1f,
                 0.5f, 0.5f,-0.5f,1f,0f,0f, 1f,1f,
                -0.5f, 0.5f,0.5f,0f,1f,0f,0f,0f,
                 0.5f, 0.5f,0.5f,0f,1f,0f,1f,0f,
                 0.5f, 0.5f,-0.5f,0f,1f,0f,1f,1f,
                -0.5f, 0.5f,-0.5f,0f,1f,0f,0f,1f,
                -0.5f,-0.5f,0.5f,0f,-1f,0f,1f,1f,
                 0.5f,-0.5f,0.5f,0f,-1f,0f,0f,1f,
                 0.5f,-0.5f,-0.5f,0f,-1f,0f,0f,0f,
                -0.5f,-0.5f,-0.5f,0f,-1f,0f,1f,0f
            };

            uint[] indices = {
                0,1,2,2,3,0,4,5,6,6,7,4,8,9,10,10,11,8,
                12,13,14,14,15,12,16,17,18,18,19,16,20,21,22,22,23,20
            };

            return new Mesh(vertices, indices);
        }

        private Mesh CreatePlane()
        {
            float[] vertices = {
                -0.5f,0f,-0.5f,0f,1f,0f,0f,0f,
                 0.5f,0f,-0.5f,0f,1f,0f,1f,0f,
                 0.5f,0f,0.5f,0f,1f,0f,1f,1f,
                -0.5f,0f,0.5f,0f,1f,0f,0f,1f
            };
            uint[] indices = { 0, 1, 2, 2, 3, 0 };
            return new Mesh(vertices, indices);
        }
    }
}
