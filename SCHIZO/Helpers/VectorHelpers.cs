using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Helpers;

public static class VectorHelpers
{
    public static Vector3 Reciprocal(this Vector3 vec3)
        => new(1f / vec3.x, 1f / vec3.y, 1f / vec3.z);
    public static Vector3 FixAngles(this Vector3 vec3)
    {
        return vec3 with
        {
            x = WrapCircle(vec3.x),
            y = WrapCircle(vec3.y),
            z = WrapCircle(vec3.z),
        };
    }
    public static Vector2 FixAngles(this Vector2 vec2)
    {
        return vec2 with
        {
            x = WrapCircle(vec2.x),
            y = WrapCircle(vec2.y),
        };
    }

    private static float WrapCircle(float t)
    {
        t %= 360;
        return t > 180 ? t - 360 : t < -180 ? t + 360 : t;
    }

    public readonly struct Box3DPoints(Vector3 min, Vector3 max)
    {
        public readonly Vector3 BottomLeft = min;
        public readonly Vector3 BottomRight = max with { y = min.y }; // top right but bottom
        public readonly Vector3 TopLeft = min with { y = max.y }; // bottom left but top
        public readonly Vector3 TopRight = max;

        public void Deconstruct(out Vector3 BottomLeft, out Vector3 BottomRight, out Vector3 TopLeft, out Vector3 TopRight)
            => (BottomLeft, BottomRight, TopLeft, TopRight) = (this.BottomLeft, this.BottomRight, this.TopLeft, this.TopRight);
        public IEnumerable<(Vector3, Vector3)> Edges()
            => [(BottomLeft, BottomRight), (BottomRight, TopRight), (TopRight, TopLeft), (TopLeft, BottomLeft)];
    }

    public static Box3DPoints BoxPoints(this Vector3 min, Vector3 max)
        => new(min, max);
    public static IEnumerable<(Vector3, Vector3)> BoxEdges(this Vector3 min, Vector3 max)
        => BoxPoints(min, max).Edges();

    public readonly struct Cube3DPoints(Vector3 min, Vector3 max)
    {
        public readonly Vector3 BottomLeftBack = min;
        public readonly Vector3 BottomRightBack = min with { x = max.x };   // bottom left back but right
        public readonly Vector3 BottomLeftFront = min with { z = max.z };
        public readonly Vector3 BottomRightFront = max with { y = min.y };  // top right front but bottom
        public readonly Vector3 TopLeftBack = min with { y = max.y };
        public readonly Vector3 TopRightBack = max with { z = min.z };      // etc etc
        public readonly Vector3 TopLeftFront = max with { x = min.x };
        public readonly Vector3 TopRightFront = max;

        public IEnumerable<(Vector3, Vector3)> Edges()
        {
            // bottom face
            yield return (BottomLeftBack, BottomRightBack);
            yield return (BottomRightBack, BottomRightFront);
            yield return (BottomRightFront, BottomLeftFront);
            yield return (BottomLeftFront, BottomLeftBack);

            // top face
            yield return (TopLeftBack, TopRightBack);
            yield return (TopRightBack, TopRightFront);
            yield return (TopRightFront, TopLeftFront);
            yield return (TopLeftFront, TopLeftBack);

            // sides
            yield return (BottomLeftBack, TopLeftBack);
            yield return (BottomRightBack, TopRightBack);
            yield return (BottomRightFront, TopRightFront);
            yield return (BottomLeftFront, TopLeftFront);
        }
    }

    public static Cube3DPoints CubePoints(this Vector3 min, Vector3 max)
        => new(min, max);
    public static IEnumerable<(Vector3, Vector3)> CubeEdges(this Vector3 min, Vector3 max)
        => CubePoints(min, max).Edges();
}
