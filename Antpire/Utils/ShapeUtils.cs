namespace Antpire.Utils; 

internal class ShapeUtils {
    /// <summary>
    /// Generates a random polyon containing <c>verticesCount</c> vertices restrained into a circle of radius <c>radius</c>
    /// </summary>
    /// <param name="verticesCount"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Vector2[] GenerateConvexPolygon(int verticesCount, float radius) {
        var r = new Random();
        var vertices = Enumerable.Repeat(0, verticesCount)
            .Select(i => r.NextDouble()*Math.PI*2)
            .OrderBy(i => i)
            .Select(x => new Vector2((float)Math.Sin(x)*radius, (float)Math.Cos(x)*radius))
            .ToArray();

        var minX = vertices.Min(x => x.X);
        var minY = vertices.Min(x => x.Y);
        
        // Move to positive quadrant
        var x = vertices.Select(x => new Vector2(x.X - minX, x.Y - minY)).ToArray();
        return x;
    }

    /// <summary>
    /// Generates points for a stack of <c>linesCount</c> lines restrained into a circle of radius <c>radius</c>
    /// </summary>
    /// <param name="linesCount"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Vector2[] GenerateLineStack(int linesCount, float radius) {
        var r = new Random();
        return Enumerable.Repeat(0, linesCount)
            .Select(i => r.NextDouble() * Math.PI * 2)
            .Select(x => new Vector2((float)Math.Sin(x) * radius, (float)Math.Cos(x) * radius))
            .ToArray();
    }

    /// <summary>
    /// Returns a random <c>Vector2</c> in a circle of the specified <c>radius</c>
    /// </summary>
    /// <param name="radius">The radius of the circle to pick a point in</param>
    /// <returns></returns>
    public static Vector2 GetRandomPointInCircle(float radius) {
        var rand = new Random();
        var a = rand.NextDouble() * 2 * Math.PI;
        var r = radius * Math.Sqrt(rand.NextDouble());

        return new Vector2((float)(r * Math.Cos(a)), (float)(r * Math.Sin(a)));
    }
  
    /// <summary>
    /// Returns an approximated bezier spline from a set of points
    /// </summary>
    /// <param name="controlPoints"></param>
    /// <param name="outputSegmentCount"></param>
    /// <remarks>From: https://stackoverflow.com/a/13948059/7619126</remarks>
    /// <returns></returns>
    public static Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount) {
        Vector2[] points = new Vector2[outputSegmentCount + 1];
        for (int i = 0; i <= outputSegmentCount; i++) {
            float t = (float)i / outputSegmentCount;
            points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
        }
        return points;
    }

    private static Vector2 GetBezierPoint(float t, Vector2[] controlPoints, int index, int count) {
        if (count == 1)
            return controlPoints[index];
        var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
        var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
        return new Vector2((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
    } 
}