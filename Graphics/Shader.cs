using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Huy_Hoang_Midterm_Game.Graphics
{
    public class Shader : IDisposable
    {
        public int Handle { get; private set; }

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragmentPath);

            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, vertexSource);
            GL.CompileShader(vert);
            CheckCompileErrors(vert, "VERTEX");

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, fragmentSource);
            GL.CompileShader(frag);
            CheckCompileErrors(frag, "FRAGMENT");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vert);
            GL.AttachShader(Handle, frag);
            GL.LinkProgram(Handle);
            CheckLinkErrors(Handle);

            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
        }

        void CheckCompileErrors(int shader, string type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new Exception($"ERROR::{type}_SHADER_COMPILATION_ERROR\n{info}");
            }
        }

        void CheckLinkErrors(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new Exception($"ERROR::PROGRAM_LINKING_ERROR\n{info}");
            }
        }

        public void Use() => GL.UseProgram(Handle);

        public int GetAttribLocation(string name) => GL.GetAttribLocation(Handle, name);

        public void SetInt(string name, int value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        public void SetFloat(string name, float value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
        public void SetVec3(string name, OpenTK.Mathematics.Vector3 v) => GL.Uniform3(GL.GetUniformLocation(Handle, name), v);
        public void SetBool(string name, bool value) => GL.Uniform1(GL.GetUniformLocation(Handle, name), value ? 1 : 0);
        public void SetMatrix4(string name, OpenTK.Mathematics.Matrix4 mat) => GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), false, ref mat);

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
