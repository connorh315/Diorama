using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Diorama
{
    public class Histogram<T>
    {
        private readonly FieldInfo[] _fields =
            typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     .Where(f =>
                         !f.IsStatic &&
                         (f.FieldType.IsValueType || f.FieldType.IsEnum))
                     .ToArray();

        public Dictionary<string, Dictionary<object?, int>> Data { get; } = new();

        public int Count { get; private set; }

        public void Add(T obj)
        {
            Count++;

            foreach (var field in _fields)
            {
                object? value = field.GetValue(obj);

                if (!Data.TryGetValue(field.Name, out var histogram))
                {
                    histogram = new Dictionary<object?, int>();
                    Data[field.Name] = histogram;
                }

                if (value == null)
                    continue;

                histogram.TryGetValue(value, out int current);
                histogram[value] = current + 1;
            }
        }

        public void Save(string path)
        {
            using var writer = new StreamWriter(path);

            foreach (var field in Data.OrderBy(x => x.Key))
            {
                writer.WriteLine(field.Key);

                foreach (var value in field.Value.OrderByDescending(x => x.Value).Take(6))
                {
                    double percent = value.Value * 100.0 / Count;

                    writer.WriteLine(
                        $"    {value.Key ?? "<null>",-30} {value.Value,5} ({percent:F1}%)");
                }

                writer.WriteLine();
            }
        }

        public void Print()
        {
            foreach (var field in Data.OrderBy(x => x.Key))
            {
                Console.WriteLine(field.Key);

                foreach (var value in field.Value.OrderByDescending(x => x.Value).Take(6))
                {
                    double percent = value.Value * 100.0 / Count;

                    Console.WriteLine(
                        $"    {value.Key ?? "<null>",-30} {value.Value,5} ({percent:F1}%)");
                }

                Console.WriteLine();
            }
        }
    }
}
