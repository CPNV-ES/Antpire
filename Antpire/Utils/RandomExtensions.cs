namespace Antpire.Utils; 

public static class RandomExtensions {
    public static double NextDouble(this Random random, double minValue, double maxValue) =>
        random.NextDouble() * (maxValue - minValue) + minValue;
}
