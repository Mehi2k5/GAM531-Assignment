using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace OpenTK_FPSCamera
{
    public class Game : GameWindow
    {
        private readonly float[] _vertices =
        {
            // positions         // normals (not used here) // texcoords
            -0.5f, -0.5f, -0.5f,  0,0,-1,  0,0,
             0.5f, -0.5f, -0.5f,  0,0,-1,  1,0,
             0.5f,  0.5f, -0.5f,  0,0,-1,  1,1,
             0.5f,  0.5f, -0.5f,  0,0,-1,  1,1,
            -0.5f,  0.5f, -0.5f,  0,0,-1,  0,1,
            -0.5f, -0.5f, -0.5f,  0,0,-1,  0,0,

            -0.5f, -0.5f,  0.5f,  0,0,1,   0,0,
             0.5f, -0.5f,  0.5f,  0,0,1,   1,0,
             0.5f,  0.5f,  0.5f,  0,0,1,   1,1,
             0.5f,  0.5f,  0.5f,  0,0,1,   1,1,
            -0.5f,  0.5f,  0.5f,  0,0,1,   0,1,
            -0.5f, -0.5f,  0.5f,  0,0,1,   0,0,

            -0.5f,  0.5f,  0.5f, -1,0,0,   1,0,
            -0.5f,  0.5f, -0.5f, -1,0,0,   1,1,
            -0.5f, -0.5f, -0.5f, -1,0,0,   0,1,
            -0.5f, -0.5f, -0.5f, -1,0,0,   0,1,
            -0.5f, -0.5f,  0.5f, -1,0,0,   0,0,
            -0.5f,  0.5f,  0.5f, -1,0,0,   1,0,

             0.5f,  0.5f,  0.5f,  1,0,0,   1,0,
             0.5f,  0.5f, -0.5f,  1,0,0,   1,1,
             0.5f, -0.5f, -0.5f,  1,0,0,   0,1,
             0.5f, -0.5f, -0.5f,  1,0,0,   0,1,
             0.5f, -0.5f,  0.5f,  1,0,0,   0,0,
             0.5f,  0.5f,  0.5f,  1,0,0,   1,0,

            -0.5f, -0.5f, -0.5f,  0,-1,0,  0,1,
             0.5f, -0.5f, -0.5f,  0,-1,0,  1,1,
             0.5f, -0.5f,  0.5f,  0,-1,0,  1,0,
             0.5f, -0.5f,  0.5f,  0,-1,0,  1,0,
            -0.5f, -0.5f,  0.5f,  0,-1,0,  0,0,
            -0.5f, -0.5f, -0.5f,  0,-1,0,  0,1,

            -0.5f,  0.5f, -0.5f,  0,1,0,   0,1,
             0.5f,  0.5f, -0.5f,  0,1,0,   1,1,
             0.5f,  0.5f,  0.5f,  0,1,0,   1,0,
             0.5f,  0.5f,  0.5f,  0,1,0,   1,0,
            -0.5f,  0.5f,  0.5f,  0,1,0,   0,0,
            -0.5f,  0.5f, -0.5f,  0,1,0,   0,1
        };

        private int _vbo;
        private int _vao;
        private Shader _shader;
        private Camera _camera;

        private bool _firstMove = true;
        private Vector2 _lastPos;
        public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
        {
            CursorState = CursorState.Grabbed; // hides and locks cursor
        }



        protected override void OnLoad()
        {
            GL.Enable(EnableCap.DepthTest);

            // setup VAO/VBO
            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            int stride = 8 * sizeof(float);
            // positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            // normals (not used, but skip)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            // texcoords
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));

            // shader
            _shader = new Shader(vertexShaderSource, fragmentShaderSource);
            _shader.Use();

            // camera
            _camera = new Camera(new Vector3(0, 0, 3), Size.X / (float)Size.Y);

            base.OnLoad();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) // skip if the window is not focused
                return;

            var input = KeyboardState;
            float delta = (float)args.Time;

            const float speedMultiplier = 1.0f;
            // movement
            if (input.IsKeyDown(Keys.W))
                _camera.Position += _camera.Front * _camera.Speed * delta * speedMultiplier;
            if (input.IsKeyDown(Keys.S))
                _camera.Position -= _camera.Front * _camera.Speed * delta * speedMultiplier;
            if (input.IsKeyDown(Keys.A))
                _camera.Position -= _camera.Right * _camera.Speed * delta * speedMultiplier;
            if (input.IsKeyDown(Keys.D))
                _camera.Position += _camera.Right * _camera.Speed * delta * speedMultiplier;
            if (input.IsKeyDown(Keys.Space))
                _camera.Position += _camera.Up * _camera.Speed * delta * speedMultiplier;
            if (input.IsKeyDown(Keys.LeftShift))
                _camera.Position -= _camera.Up * _camera.Speed * delta * speedMultiplier;

            // ESC to release cursor & exit
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (_firstMove)
            {
                _lastPos = new Vector2(e.X, e.Y);
                _firstMove = false;
            }

            var deltaX = e.DeltaX;
            var deltaY = e.DeltaY;

            _camera.Yaw += deltaX * _camera.Sensitivity;
            _camera.Pitch -= deltaY * _camera.Sensitivity; // inverted Y

            _camera.UpdateCameraVectors();
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.Fov -= e.OffsetY; // wheel up -> positive
            _camera.Fov = MathHelper.Clamp(_camera.Fov, 30f, 90f);
            base.OnMouseWheel(e);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.1f, 0.12f, 0.15f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Projection and View from camera
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shader.SetMatrix4("view", _camera.GetViewMatrix());

            // model matrix: demonstrate multiple cubes / grid
            for (int x = -2; x <= 2; x += 2)
            {
                for (int z = -2; z <= 2; z += 2)
                {
                    var model = Matrix4.CreateTranslation(x, 0.0f, z);
                    _shader.SetMatrix4("model", model);

                    GL.BindVertexArray(_vao);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            _shader?.Dispose();
            base.OnUnload();
        }

        #region Shaders (inline)
        private const string vertexShaderSource = @"
#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTex;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoord;

void main()
{
    FragPos = vec3(model * vec4(aPos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    TexCoord = aTex;
    gl_Position = projection * view * vec4(FragPos, 1.0);
}
";

        private const string fragmentShaderSource = @"
#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

void main()
{
    // very simple color based on normal for visibility
    vec3 color = normalize(Normal) * 0.5 + vec3(0.5);
    FragColor = vec4(color, 1.0);
}
";
        #endregion
    }
}
    