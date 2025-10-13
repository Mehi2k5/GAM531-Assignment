using System;
using OpenTK.Mathematics;


namespace OpenTK_FPSCamera
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Front = -Vector3.UnitZ; // forward
        public Vector3 Up = Vector3.UnitY;
        public Vector3 Right = Vector3.UnitX;
        public Vector3 WorldUp = Vector3.UnitY;

        public float Yaw = -90f;   // facing -Z initially
        public float Pitch = 0f;
        public float Speed = 3.5f; // units per second
        public float Sensitivity = 0.1f; // mouse sensitivity
        public float Fov = 60f;    // degrees

        public float AspectRatio;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
            UpdateCameraVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), AspectRatio, 0.1f, 100f);
        }

        public void UpdateCameraVectors()
        {
            // Convert degrees to radians for trig
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            Vector3 front;
            front.X = MathF.Cos(pitchRad) * MathF.Cos(yawRad);
            front.Y = MathF.Sin(pitchRad);
            front.Z = MathF.Cos(pitchRad) * MathF.Sin(yawRad);
            Front = Vector3.Normalize(front);

            // Recalculate Right and Up
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));

            // Clamp pitch to prevent flipping
            Pitch = MathHelper.Clamp(Pitch, -89f, 89f);
        }
    }
}
