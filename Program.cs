using MathLibrary;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;


class Program
{
    static void Main()
    {
        // Vector operations
        var v1 = new Vector3D(1, 2, 3);
        var v2 = new Vector3D(4, 5, 6);

        Console.WriteLine("Vector Addition: " + (v1 + v2));
        Console.WriteLine("Vector Subtraction: " + (v1 - v2));
        Console.WriteLine("Dot Product: " + Vector3D.Dot(v1, v2));
        Console.WriteLine("Cross Product: " + Vector3D.Cross(v1, v2));

        // Matrix operations
        var scale = Matrix4x4.Scale(2, 2, 2);
        var rotate = Matrix4x4.RotateZ((float)Math.PI / 4); // 45 degrees
        var transform = Matrix4x4.Multiply(scale, rotate);

        var v = new Vector3D(1, 0, 0);
        var result = transform.Transform(v);

        Console.WriteLine("Original Vector: " + v);
        Console.WriteLine("Scaled + Rotated Vector: " + result);
    }
}

class Game : GameWindow
{
    private int _vertexArrayObject;
    private int _vertexBufferObject;
    private int _shaderProgram;

    // Rectangle made of two triangles
    private readonly float[] _vertices =
    {
        // X     Y     Z
        -0.5f, -0.5f, 0.0f, // bottom left
         0.5f, -0.5f, 0.0f, // bottom right
         0.5f,  0.5f, 0.0f, // top right

        -0.5f, -0.5f, 0.0f, // bottom left
         0.5f,  0.5f, 0.0f, // top right
        -0.5f,  0.5f, 0.0f  // top left
    };

    // Simple shaders
    private readonly string _vertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }
    ";

    private readonly string _fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 0.8, 0.2, 1.0); // green color
        }
    ";
    
    public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Compile vertex shader
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, _vertexShaderSource);
        GL.CompileShader(vertexShader);

        // Compile fragment shader
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, _fragmentShaderSource);
        GL.CompileShader(fragmentShader);

        // Link shaders into a program
        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);

        // Delete shaders (no longer needed once linked)
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // Generate and bind VAO
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        // Generate and bind VBO
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        // Set vertex attribute pointer
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Set background color
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteProgram(_shaderProgram);
    }
}
