using UnityEngine;

public static class Vector2Extension {
    public static Vector2 Rotate(this Vector2 v, float degrees) => Quaternion.Euler(0,0,degrees) * v;
}
