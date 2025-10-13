using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Text;

namespace OpenTK_FPSCamera
{
    public class Shader : IDisposable
    {
        public readonly int Handle;

        public Shader(string vertexSource, string fragmentSource)
        {
            var vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSource);
            GL.CompileShader(vertex);
            GL.GetShader(vertex, ShaderParameter.CompileStatus, out int vsStatus);
            if (vsStatus == 0)
                throw new Exception($"Vertex shader compilation failed: {GL.GetShaderInfoLog(vertex)}");

            var fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSource);
            GL.CompileShader(fragment);
            GL.GetShader(fragment, ShaderParameter.CompileStatus, out int fsStatus);
            if (fsStatus == 0)
                throw new Exception($"Fragment shader compilation failed: {GL.GetShaderInfoLog(fragment)}");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertex);
            GL.AttachShader(Handle, fragment);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
                throw new Exception($"Program linking failed: {GL.GetProgramInfoLog(Handle)}");

            GL.DetachShader(Handle, vertex);
            GL.DetachShader(Handle, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
