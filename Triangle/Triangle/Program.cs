using System;
using System.Globalization;

namespace MyApp // Note: actual namespace depends on the project name.
{
class Triangle
{
    public decimal sideA, sideB, sideC;
    public Triangle(decimal sideA = 0, decimal sideB = 0, decimal sideC = 0)
    {
        this.sideA = sideA;
        this.sideB = sideB;
        this.sideC = sideC;
    }
}

internal class Program
{
    static Triangle? ReadTriangle(string[] args)
    {
        if (args.Length != 3)
        {
            return null;
        }
        const NumberStyles style = NumberStyles.Any;
        CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
        Triangle triangle = new();

        if (Decimal.TryParse(args[0], style, culture, out triangle.sideA) &&
            Decimal.TryParse(args[1], style, culture, out triangle.sideB) &&
            Decimal.TryParse(args[2], style, culture, out triangle.sideC))
        {
            return triangle;
        }
        return null;
    }
    static bool IsEquilateral(in Triangle triangle)
    {
        if (triangle.sideA == triangle.sideB && triangle.sideB == triangle.sideC)
        {
            return true;
        }
        return false;
    }
    static bool IsLegalTriangle(in Triangle triangle)
    {
        var a = triangle.sideA;
        var b = triangle.sideB;
        var c = triangle.sideC;
        if (c - b < a && b - c < a && a - c < b)
        {
            return true;
        }
        return false;
    }
    static bool IsIsosceles(in Triangle triangle)
    {
        if (triangle.sideA == triangle.sideB || triangle.sideA == triangle.sideC || triangle.sideB == triangle.sideC)
        {
            return true;
        }
        return false;
    }
    static void Main(string[] args)
    {
        var triangle = ReadTriangle(args);
        if (triangle == null)
        {
            Console.WriteLine("неизвестная ошибка");
            return;
        }
        if (!IsLegalTriangle(triangle))
        {
            Console.WriteLine("не треугольник");
            return;
        }
        if (IsEquilateral(triangle))
        {
            Console.WriteLine("равносторонний");
            return;
        }
        if (IsIsosceles(triangle))
        {
            Console.WriteLine("равнобедренный");
            return;
        }
        Console.WriteLine("обычный");
        return;
    }
}
}
