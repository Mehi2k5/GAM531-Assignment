using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TextureMappingOpenTK
{
    public class Shader : IDisposable
    {
        public int Handle { get; private set; }

        public Shader(string vertPath, string fragPath)
        {
            string vertSource = File.ReadAllText(vertPath);
            string fragSource = File.ReadAllText(fragPath);

            int vertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertShader, vertSource);
            GL.CompileShader(vertShader);
            GL.GetShader(vertShader, ShaderParameter.CompileStatus, out int statusV);
            if (statusV == 0)
            {
                string info = GL.GetShaderInfoLog(vertShader);
                throw new Exception($"Vertex shader compilation failed: {info}");
            }

            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, fragSource);
            GL.CompileShader(fragShader);
            GL.GetShader(fragShader, ShaderParameter.CompileStatus, out int statusF);
            if (statusF == 0)
            {
                string info = GL.GetShaderInfoLog(fragShader);
                throw new Exception($"Fragment shader compilation failed: {info}");
            }

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertShader);
            GL.AttachShader(Handle, fragShader);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int statusL);
            if (statusL == 0)
            {
                string info = GL.GetProgramInfoLog(Handle);
                throw new Exception($"Shader linking failed: {info}");
            }

            GL.DetachShader(Handle, vertShader);
            GL.DetachShader(Handle, fragShader);
            GL.DeleteShader(vertShader);
            GL.DeleteShader(fragShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetInt(string name, int value)
        {
            int loc = GL.GetUniformLocation(Handle, name);
            if (loc == -1) throw new Exception($"Uniform '{name}' not found on shader.");
            GL.Uniform1(loc, value);
        }

        public void SetMatrix4(string name, Matrix4 mat)
        {
            int loc = GL.GetUniformLocation(Handle, name);
            if (loc == -1) throw new Exception($"Uniform '{name}' not found on shader.");
            GL.UniformMatrix4(loc, false, ref mat);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
