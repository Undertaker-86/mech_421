using System;
using System.Collections.Generic;
using System.Linq;
using DistanceMonitor.Models;

namespace DistanceMonitor.Services;

public static class CalibrationService
{
    public static double[] FitPolynomial(IEnumerable<CalibrationPoint> points, int order)
    {
        if (order < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(order), "Polynomial order must be at least 1.");
        }

        var data = points.ToList();
        if (data.Count <= order)
        {
            throw new InvalidOperationException($"Need at least {order + 1} calibration points for a polynomial of order {order}.");
        }

        int n = order + 1;
        var matrix = new double[n, n];
        var vector = new double[n];

        foreach (var point in data)
        {
            double x = point.RawValue;
            double y = point.MeasuredDistance;

            for (int row = 0; row < n; row++)
            {
                double xPowRow = Math.Pow(x, row);
                vector[row] += xPowRow * y;

                for (int col = 0; col < n; col++)
                {
                    matrix[row, col] += xPowRow * Math.Pow(x, col);
                }
            }
        }

        return SolveGaussian(matrix, vector);
    }

    public static double EvaluatePolynomial(IReadOnlyList<double> coefficients, double x)
    {
        double result = 0;
        double xPower = 1;
        for (int i = 0; i < coefficients.Count; i++)
        {
            result += coefficients[i] * xPower;
            xPower *= x;
        }

        return result;
    }

    private static double[] SolveGaussian(double[,] matrix, double[] vector)
    {
        int n = vector.Length;
        var augmented = new double[n, n + 1];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmented[i, j] = matrix[i, j];
            }

            augmented[i, n] = vector[i];
        }

        for (int pivot = 0; pivot < n; pivot++)
        {
            int maxRow = pivot;
            for (int row = pivot + 1; row < n; row++)
            {
                if (Math.Abs(augmented[row, pivot]) > Math.Abs(augmented[maxRow, pivot]))
                {
                    maxRow = row;
                }
            }

            if (Math.Abs(augmented[maxRow, pivot]) < 1e-12)
            {
                throw new InvalidOperationException("Calibration matrix is singular; try reducing polynomial order or using different data points.");
            }

            if (maxRow != pivot)
            {
                SwapRows(augmented, pivot, maxRow);
            }

            double pivotValue = augmented[pivot, pivot];
            for (int col = pivot; col <= n; col++)
            {
                augmented[pivot, col] /= pivotValue;
            }

            for (int row = 0; row < n; row++)
            {
                if (row == pivot)
                {
                    continue;
                }

                double factor = augmented[row, pivot];
                for (int col = pivot; col <= n; col++)
                {
                    augmented[row, col] -= factor * augmented[pivot, col];
                }
            }
        }

        var solution = new double[n];
        for (int i = 0; i < n; i++)
        {
            solution[i] = augmented[i, n];
        }

        return solution;
    }

    private static void SwapRows(double[,] matrix, int rowA, int rowB)
    {
        int columns = matrix.GetLength(1);
        for (int col = 0; col < columns; col++)
        {
            (matrix[rowA, col], matrix[rowB, col]) = (matrix[rowB, col], matrix[rowA, col]);
        }
    }
}
