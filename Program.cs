using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Drawing;
using System.Numerics;

namespace CubeDemo
{
    public class CubeWindow : GameWindow
    {
        private readonly float[] _vertices =
        {
            // Positions (x,y,z)      Colors (r,g,b)
            -0.5f, -0.5f, -0.5f,     1f, 0f, 0f,
             0.5f, -0.5f, -0.5f,     0f, 1f, 0f,
             0.5f,  0.5f, -0.5f,     0f, 0f, 1f,
            -0.5f,  0.5f, -0.5f,     1f, 1f, 0f,
            -0.5f, -0.5f,  0.5f,     1f, 0f, 1f,
             0.5f, -0.5f,  0.5f,     0f, 1f, 1f,
             0.5f,  0.5f,  0.5f,     1f, 1f, 1f,
            -0.5f,  0.5f,  0.5f,     0f, 0f, 0f
        };

        private readonly uint[] _indices =
        {
            0, 1, 2, 2, 3, 0, // back
            4, 5, 6, 6, 7, 4, // front
            0, 4, 7, 7, 3, 0, // left
            1, 5, 6, 6, 2, 1, // right
            3, 2, 6, 6, 7, 3, // top
            0, 1, 5, 5, 4, 0  // bottom
        };

        private int _vao, _vbo, _ebo, _shader;
        private float _time;

        public CubeWindow(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // color attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Compile shaders
            string vertexShaderSrc = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec3 aColor;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                out vec3 ourColor;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPos, 1.0);
                    ourColor = aColor;
                }";

            string fragmentShaderSrc = @"
                #version 330 core
                out vec4 FragColor;
                in vec3 ourColor;
                void main()
                {
                    FragColor = vec4(ourColor, 1.0);
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSrc);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0) throw new Exception(GL.GetShaderInfoLog(vertexShader));

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSrc);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0) throw new Exception(GL.GetShaderInfoLog(fragmentShader));

            _shader = GL.CreateProgram();
            GL.AttachShader(_shader, vertexShader);
            GL.AttachShader(_shader, fragmentShader);
            GL.LinkProgram(_shader);
            GL.GetProgram(_shader, GetProgramParameterName.LinkStatus, out success);
            if (success == 0) throw new Exception(GL.GetProgramInfoLog(_shader));

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(_shader);

            _time += (float)args.Time;

            // Model: rotation
            Matrix4 model = Matrix4.CreateRotationY(_time);

            // View: move back
            Matrix4 view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            // Projection: perspective
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f),
                Size.X / (float)Size.Y, 0.1f, 100f);

            int modelLoc = GL.GetUniformLocation(_shader, "model");
            int viewLoc = GL.GetUniformLocation(_shader, "view");
            int projLoc = GL.GetUniformLocation(_shader, "projection");

            GL.UniformMatrix4(modelLoc, false, ref model);
            GL.UniformMatrix4(viewLoc, false, ref view);
            GL.UniformMatrix4(projLoc, false, ref projection);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteProgram(_shader);
        }
    }

    class Program
    {
        static void Main()
        {
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "3D Cube with OpenTK"
            };

            using var window = new CubeWindow(gws, nws);
            window.Run();
        }
    }
}