using UnityEngine;

public class MysticUtil
{
    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    public static Vector3 DampVector(Vector3 a, Vector3 b, float lambda, float dt)
    {
        return new Vector3(
            Damp(a.x, b.x, lambda, dt),
            Damp(a.y, b.y, lambda, dt),
            Damp(a.z, b.z, lambda, dt)
        );
    }

    public static Bounds GetCameraWorldBounds(Camera camera)
    {
        Vector2 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0,0));
        Vector2 topRight = camera.ViewportToWorldPoint(new Vector3(1,1));
        Vector2 size = topRight - bottomLeft;
        return new Bounds( size * 0.5f + bottomLeft, size);
    }
}