using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace TextureMappingOpenTK
{
    public class Game : GameWindow, IDisposable     
    {
        private int _vbo;
        private int _vao;
        private int _ebo;

        private Shader _shader;
        private Texture _texture;

        private float _angle = 0f;

        public Game()
            : base(GameWindowSettings.Default, new NativeWindowSettings
            {
                Size = new Vector2i(800, 600),
                Title = "OpenTK Texture Mapping"
            })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Load shader
            _shader = new Shader("Shaders/vertex.glsl", "Shaders/fragment.glsl");

            // Setup cube geometry
            SetupCube();

            // Load texture
            _texture = new Texture("Textures/Texture.jpg");

            _shader.Use();
            _shader.SetInt("texture0", 0);
        }

        private void SetupCube()
        {
            float[] vertices = {
                // positions         // texture coords
                -0.5f, -0.5f,  0.5f,  0f, 0f,
                 0.5f, -0.5f,  0.5f,  1f, 0f,
                 0.5f,  0.5f,  0.5f,  1f, 1f,
                -0.5f,  0.5f,  0.5f,  0f, 1f,
                -0.5f, -0.5f, -0.5f,  1f, 0f,
                 0.5f, -0.5f, -0.5f,  0f, 0f,
                 0.5f,  0.5f, -0.5f,  0f, 1f,
                -0.5f,  0.5f, -0.5f,  1f, 1f,
            };

            uint[] indices = {
                0,1,2, 2,3,0,
                1,5,6, 6,2,1,
                5,4,7, 7,6,5,
                4,0,3, 3,7,4,
                4,5,1, 1,0,4,
                3,2,6, 6,7,3
            };

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // texture coords attribute
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _angle += 30f * (float)args.Time;  // degrees per second

            Matrix4 model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_angle));
            Matrix4 view = Matrix4.CreateTranslation(0, 0, -3f);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                Size.X / (float)Size.Y,
                0.1f, 100f);

            _shader.Use();
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            _texture.Use(TextureUnit.Texture0);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);

            _shader.Dispose();
            _texture.Dispose(); //random comment
        }
    }
}
