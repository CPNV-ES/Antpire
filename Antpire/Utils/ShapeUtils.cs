namespace Antpire.Utils {
    internal class ShapeUtils {
        /// <summary>
        /// Generates a random polyon containing <c>verticesCount</c> vertices restrained into a circle of radius <c>radius</c>
        /// </summary>
        /// <param name="verticesCount"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vector2[] GenerateConvexPolygon(int verticesCount, float radius) {
            var r = new Random();
            return Enumerable.Repeat(0, verticesCount)
                .Select(i => r.NextDouble()*Math.PI*2)
                .OrderBy(i => i)
                .Select(x => new Vector2((float)Math.Sin(x)*radius, (float)Math.Cos(x)*radius))
                .ToArray();
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
    }
}
