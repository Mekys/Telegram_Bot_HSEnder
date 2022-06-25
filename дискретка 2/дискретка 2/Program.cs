using System;

namespace дискретка_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int x;
            int y;
            int buff;
            Console.WriteLine("введите x");
            x = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите y");
            y = Convert.ToInt32(Console.ReadLine());
            int max = Math.Max(x, y);
            int min= Math.Min(x, y);
            Console.WriteLine($"Сначала решим вспомогательное уравнение:{x}*x+{y}*y=НОД({x},{y})");
            while (max%min!=0)
            {
                Console.WriteLine($"{max},{min}     {max}={min}*{max / min}+{max % min}    {max % min}={max}-{min}*{max / min}");
                max = max%min;
                buff = Math.Max(max, min);
                min = Math.Min(max, min);
                max = buff;
            }
            Console.WriteLine($"{max},{min}     {max}={min}*{max / min}+{max % min}    НОД({x},{y})={min}");
            Console.WriteLine($"Вспомогательное уравнение:{x}*x+{y}*y={min}");
        }
    }
}
