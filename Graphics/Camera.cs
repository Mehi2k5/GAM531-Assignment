using OpenTK.Mathematics;

namespace Huy_Hoang_Midterm_Game.Graphics
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 Front = -Vector3.UnitZ;
        public Vector3 Up = Vector3.UnitY;
        public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, Up));
        public float Yaw = -90f;
        public float Pitch = 0f;

        public float Fov = 60f;

        public Camera(Vector3 startPos)
        {
            Position = startPos;
            UpdateVectors();
        }

        public Matrix4 GetViewMatrix()
            => Matrix4.LookAt(Position, Position + Front, Up);

        public void ProcessMouseMovement(float deltaX, float deltaY, float sensitivity = 0.15f)
        {
            Yaw += deltaX * sensitivity;
            Pitch -= deltaY * sensitivity;

            if (Pitch > 89f) Pitch = 89f;
            if (Pitch < -89f) Pitch = -89f;

            UpdateVectors();
        }

        void UpdateVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            Front = Vector3.Normalize(front);
        }
    }
}
