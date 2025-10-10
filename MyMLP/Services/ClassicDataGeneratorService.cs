using MyMLP.Models;
using System;
using System.Collections.Generic;

namespace MyMLP.Services
{
    public class ClassicDataGeneratorService : IDataGeneratorService
    {
        public List<DataPoint> GenerateData(DatasetType type, int sampleCount, double noise)
        {
            return type switch
            {
                DatasetType.Circle => GenerateCircleData(sampleCount, noise),
                DatasetType.XOR => GenerateXorData(sampleCount, noise),
                DatasetType.Spiral => GenerateSpiralData(sampleCount, noise),
                DatasetType.TwoMoons => GenerateTwoMoonsData(sampleCount, noise),
                DatasetType.Gaussian => GenerateGaussianData(sampleCount, noise),
                DatasetType.FourCorners => GenerateFourCornersData(sampleCount, noise),
                DatasetType.CenterRing => GenerateCenterRingData(sampleCount, noise),
                DatasetType.SineWave => GenerateSineWaveData(sampleCount, noise),
                _ => new List<DataPoint>(),
            };
        }

        private List<DataPoint> GenerateCircleData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            double radius = 5;

            for (int i = 0; i < sampleCount / 2; i++)
            {
                dataPoints.Add(CreatePointInAnnulus(radius / 2, 0, 1, 0, noise, random));
                dataPoints.Add(CreatePointInAnnulus(radius, radius / 2, 0, 1, noise, random));
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateXorData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            int samplesPerQuadrant = sampleCount / 4;

            for (int i = 0; i < samplesPerQuadrant; i++)
            {
                dataPoints.Add(CreatePointInQuadrant(5, 5, 1, 0, noise, random));
                dataPoints.Add(CreatePointInQuadrant(-5, -5, 1, 0, noise, random));
                dataPoints.Add(CreatePointInQuadrant(-5, 5, 0, 1, noise, random));
                dataPoints.Add(CreatePointInQuadrant(5, -5, 0, 1, noise, random));
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateSpiralData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            int pointsPerClass = sampleCount / 2;

            for (int i = 0; i < pointsPerClass; i++)
            {
                double r = i / (double)pointsPerClass * 5;
                double t = 1.75 * i / (double)pointsPerClass * 2 * Math.PI;

                double x1 = r * Math.Sin(t) + (random.NextDouble() - 0.5) * noise;
                double y1 = r * Math.Cos(t) + (random.NextDouble() - 0.5) * noise;
                dataPoints.Add(new DataPoint { Features = new List<double> { x1, y1 }, ExpectedOutput = new List<double> { 1, 0 } });

                double x2 = -r * Math.Sin(t) + (random.NextDouble() - 0.5) * noise;
                double y2 = -r * Math.Cos(t) + (random.NextDouble() - 0.5) * noise;
                dataPoints.Add(new DataPoint { Features = new List<double> { x2, y2 }, ExpectedOutput = new List<double> { 0, 1 } });
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateTwoMoonsData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            int pointsPerClass = sampleCount / 2;
            double radius = 5;

            for (int i = 0; i < pointsPerClass; i++)
            {
                double angle = (i / (double)pointsPerClass) * Math.PI;
                double noiseX = (random.NextDouble() - 0.5) * noise * 2;
                double noiseY = (random.NextDouble() - 0.5) * noise * 2;

                double x1 = radius * Math.Cos(angle) + noiseX;
                double y1 = radius * Math.Sin(angle) + noiseY;
                dataPoints.Add(new DataPoint { Features = new List<double> { x1, y1 }, ExpectedOutput = new List<double> { 1, 0 } });

                double x2 = radius * Math.Cos(angle + Math.PI) + 1 + noiseX;
                double y2 = radius * Math.Sin(angle + Math.PI) - 0.5 + noiseY;
                dataPoints.Add(new DataPoint { Features = new List<double> { x2, y2 }, ExpectedOutput = new List<double> { 0, 1 } });
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateGaussianData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            int pointsPerClass = sampleCount / 2;

            for (int i = 0; i < pointsPerClass; i++)
            {
                dataPoints.Add(CreateGaussianPoint(2, 2, 1 + noise, 1, 0, random));
                dataPoints.Add(CreateGaussianPoint(4, 4, 1 + noise, 0, 1, random));
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateFourCornersData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            int samplesPerQuadrant = sampleCount / 4;
            for (int i = 0; i < samplesPerQuadrant; i++)
            {
                dataPoints.Add(CreatePointInQuadrant(5, 5, 1, 0, noise, random));
                dataPoints.Add(CreatePointInQuadrant(-5, -5, 1, 0, noise, random));
                dataPoints.Add(CreatePointInQuadrant(-5, 5, 0, 1, noise, random));
                dataPoints.Add(CreatePointInQuadrant(5, -5, 0, 1, noise, random));
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateCenterRingData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();
            double radius = 5;

            for (int i = 0; i < sampleCount / 2; i++)
            {
                dataPoints.Add(CreatePointInAnnulus(radius / 2.5, 0, 1, 0, noise, random));
                dataPoints.Add(CreatePointInAnnulus(radius, radius / 1.5, 0, 1, noise, random));
            }
            return dataPoints;
        }

        private List<DataPoint> GenerateSineWaveData(int sampleCount, double noise)
        {
            var dataPoints = new List<DataPoint>();
            var random = new Random();

            for (int i = 0; i < sampleCount; i++)
            {
                double x = (random.NextDouble() - 0.5) * 10;
                double y = Math.Sin(x) + (random.NextDouble() - 0.5) * (noise + 0.5);
                var expected = y > Math.Sin(x) ? new List<double> { 0, 1 } : new List<double> { 1, 0 };
                dataPoints.Add(new DataPoint { Features = new List<double> { x, y }, ExpectedOutput = expected });
            }
            return dataPoints;
        }

        private DataPoint CreatePointInAnnulus(double maxRadius, double minRadius, double out1, double out2, double noise, Random random)
        {
            double r = random.NextDouble() * (maxRadius - minRadius) + minRadius;
            double angle = random.NextDouble() * 2 * Math.PI;
            double x = r * Math.Sin(angle) + (random.NextDouble() - 0.5) * noise * 2;
            double y = r * Math.Cos(angle) + (random.NextDouble() - 0.5) * noise * 2;
            return new DataPoint { Features = new List<double> { x, y }, ExpectedOutput = new List<double> { out1, out2 } };
        }

        private DataPoint CreatePointInQuadrant(double baseX, double baseY, double out1, double out2, double noise, Random random)
        {
            double x = baseX + (random.NextDouble() - 0.5) * noise * 5;
            double y = baseY + (random.NextDouble() - 0.5) * noise * 5;
            return new DataPoint { Features = new List<double> { x, y }, ExpectedOutput = new List<double> { out1, out2 } };
        }

        private DataPoint CreateGaussianPoint(double meanX, double meanY, double stdDev, double out1, double out2, Random random)
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double x = meanX + stdDev * randStdNormal;
            double y = meanY + stdDev * randStdNormal;
            return new DataPoint { Features = new List<double> { x, y }, ExpectedOutput = new List<double> { out1, out2 } };
        }
    }
}