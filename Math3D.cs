using System;

namespace MathLibrary
{
    public struct Vector3D
    {
        public float X, Y, Z;

        public Vector3D(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        // Addition
        public static Vector3D operator +(Vector3D a, Vector3D b) =>
            new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        // Subtraction
        public static Vector3D operator -(Vector3D a, Vector3D b) =>
            new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        // Dot product
        public static float Dot(Vector3D a, Vector3D b) =>
            a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        // Cross product
        public static Vector3D Cross(Vector3D a, Vector3D b) =>
            new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );

        public override string ToString() => $"({X}, {Y}, {Z})";
    }

    public class Matrix4x4
    {
        public float[,] M = new float[4, 4];

        // Identity matrix
        public static Matrix4x4 Identity()
        {
            var mat = new Matrix4x4();
            for (int i = 0; i < 4; i++) mat.M[i, i] = 1;
            return mat;
        }

        // Scaling matrix
        public static Matrix4x4 Scale(float sx, float sy, float sz)
        {
            var mat = Identity();
            mat.M[0, 0] = sx;
            mat.M[1, 1] = sy;
            mat.M[2, 2] = sz;
            return mat;
        }

        // Rotation around Z axis
        public static Matrix4x4 RotateZ(float radians)
        {
            var mat = Identity();
            float c = (float)Math.Cos(radians);
            float s = (float)Math.Sin(radians);
            mat.M[0, 0] = c; mat.M[0, 1] = -s;
            mat.M[1, 0] = s; mat.M[1, 1] = c;
            return mat;
        }

        // Matrix multiplication
        public static Matrix4x4 Multiply(Matrix4x4 a, Matrix4x4 b)
        {
            var result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        result.M[i, j] += a.M[i, k] * b.M[k, j];
            return result;
        }

        // Apply matrix to vector
        public Vector3D Transform(Vector3D v)
        {
            float x = v.X * M[0, 0] + v.Y * M[0, 1] + v.Z * M[0, 2] + M[0, 3];
            float y = v.X * M[1, 0] + v.Y * M[1, 1] + v.Z * M[1, 2] + M[1, 3];
            float z = v.X * M[2, 0] + v.Y * M[2, 1] + v.Z * M[2, 2] + M[2, 3];
            return new Vector3D(x, y, z);
        }
    }
}
