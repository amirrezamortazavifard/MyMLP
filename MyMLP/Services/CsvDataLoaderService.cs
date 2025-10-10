using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MyMLP.Exceptions;
using MyMLP.Models;

namespace MyMLP.Services
{
    public class CsvDataLoaderService : IDataLoaderService
    {
        public List<DataPoint> LoadData(string filePath, int inputSize, int outputSize)
        {
            var dataPoints = new List<DataPoint>();
            var lines = File.ReadAllLines(filePath);

            if (!lines.Any())
            {
                return dataPoints;
            }

            var firstLineValues = lines.First().Split(',').Length;
            var expectedColumnCount = inputSize + outputSize;

            if (firstLineValues != expectedColumnCount)
            {
                throw new DataFormatException(
                    $"Column count mismatch in the data file. The neural network expects {expectedColumnCount} columns ({inputSize} inputs + {outputSize} outputs), but the CSV file has {firstLineValues} columns.");
            }

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var values = line.Split(',')
                                 .Select(v => double.Parse(v.Trim(), CultureInfo.InvariantCulture))
                                 .ToList();

                var dataPoint = new DataPoint
                {
                    Features = values.Take(inputSize).ToList(),
                    ExpectedOutput = values.Skip(inputSize).Take(outputSize).ToList()
                };

                dataPoints.Add(dataPoint);
            }

            return dataPoints;
        }
    }
}