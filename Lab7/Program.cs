// Lab 7
// Student name: Ella Gladders
// Student number: 78754249

using System;
using System.Collections.Generic;

Console.WriteLine("CPEN223 Lab 7");

//Testing: Write some test cases to test well all methods you are to implement
//         This is to demonstrates what test cases you have considered
// Map: square each value.
// Expected: 1, 4, 9
Console.WriteLine(string.Join(", ",
    EmgAnalyzer.Map(new List<double> { 1, 2, 3 }, x => x * x)));

// Map: empty input.
// Expected: an empty line
Console.WriteLine(string.Join(", ",
    EmgAnalyzer.Map(new List<double>(), x => x * 2)));

// Filter: keep positive values.
// Expected: 3, 5
Console.WriteLine(string.Join(", ",
    EmgAnalyzer.Filter(new List<double> { -2, 0, 3, 5 }, x => x > 0)));

// Reduce: add all values.
// Expected: 10
Console.WriteLine(EmgAnalyzer.Reduce(
    new List<double> { 1, 2, 3, 4 }, 0,
    (sum, x) => sum + x));

// Reduce: empty input returns the initial value.
// Expected: 7
Console.WriteLine(EmgAnalyzer.Reduce(
    new List<double>(), 7.0,
    (sum, x) => sum + x));

// CleanAndCalibrate: discard unusable values, then calibrate.
// Expected: 2, 4
Console.WriteLine(string.Join(", ",
    EmgAnalyzer.CleanAndCalibrate(
        new List<double> { 0.0, 2.0, double.NaN, 3.0, 100.0 },
        1.0, 5.0, 1.0, 2.0)));

// RootMeanSquare.
// Expected: approximately 3.5355339
Console.WriteLine(EmgAnalyzer.RootMeanSquare(
    new List<double> { 3.0, 4.0 }));

// FractionAboveThreshold: two of four samples meet the threshold.
// Expected: 0.5
Console.WriteLine(EmgAnalyzer.FractionAboveThreshold(
    new List<double> { -5, 1, 6, 2 }, 4));
//end Testing code

//Do not change the program skeleton: keep the class name, method names,
//parameters, and return types exactly as given.
//You may add private helper methods.

public static class EmgAnalyzer
{
    public static List<double> Map(
        IReadOnlyList<double> values,
        Func<double, double> transform)
    {
        // reject null arguments
        if (values is null || transform is null)
        {
            throw new ArgumentNullException("values and transform cannot be null");
        }

        // store transformed vals in a new list 
        List<double> result = new();

        // visit each input value in its original order
        foreach (double x in values)
        {
            result.Add(transform(x));
        }
        return result;
    }

    public static List<double> Filter(
        IReadOnlyList<double> values,
        Func<double, bool> keep)
    {
        if (values is null || keep is null)
        {
            throw new ArgumentNullException("values and keep cannot be null");
        }

        // store matching vals in a new list 
        List<double> result = new();

        // check each value in its original order 
        foreach (double x in values)
        {
            if (keep(x))
            {
                result.Add(x);
            }
        }
        return result;
    }

    public static double Reduce(
        IReadOnlyList<double> values,
        double initial,
        Func<double, double, double> combine)
    {
        if (values is null || combine is null)
        {
            throw new ArgumentNullException("values and combine cannot be null");
        }
        // start accumulator at initial value
        double result = initial;

        // visit each value in its original order
        foreach (double x in values)
        {
            result = combine(result, x);
        }
        return result;
    }

    public static List<double> CleanAndCalibrate(
        IReadOnlyList<double> rawSamples,
        double minimum,
        double maximum,
        double offset,
        double gain)
    {
        // reject a null input 
        if (rawSamples is null)
        {
            throw new ArgumentNullException("rawSamples cannot be null");
        }

        // all calibration params and range limits must be finite 
        if (!double.IsFinite(minimum) || !double.IsFinite(maximum) ||
            !double.IsFinite(offset) || !double.IsFinite(gain))
        {
            throw new ArgumentException("All calibration parameters and range limits must be finite");
        }

        // range must be valid and gain must be positive 
        if (minimum > maximum || gain <= 0)
        {
            throw new ArgumentException("Range must be valid and gain must be positive");
        }

        // keep finite samples within range, including both boundaries 
        List<double> usable = Filter(rawSamples, x => double.IsFinite(x) && x >= minimum && x <= maximum);

        // calibrate each usable sample
        return Map (usable, x => (x - offset) * gain);
    }

    public static double RootMeanSquare(
        IReadOnlyList<double> samples)
    {
       // reject a null or empty input list
    if (samples is null || samples.Count == 0)
    {
        throw new ArgumentException("samples cannot be null or empty");
    }

    // check that each sample is finite, then square it
    List<double> squares = Map(samples, x =>
    {
        if (!double.IsFinite(x))
        {
            throw new ArgumentException("All samples must be finite");
        }

        return x * x;
    });

    // add all the squared values
    double sum = Reduce(squares, 0.0, (acc, x) => acc + x);

    // return the square root of the mean of the squared values
    return Math.Sqrt(sum / samples.Count);
    }

    public static double FractionAboveThreshold(
        IReadOnlyList<double> samples,
        double threshold)
    {
        // reject null
    if (samples is null || samples.Count == 0)
    {
        throw new ArgumentException("samples cannot be null or empty");
    }

    // the threshold must be finite and non-negative
    if (!double.IsFinite(threshold) || threshold < 0)
    {
        throw new ArgumentException("threshold must be finite and non-negative");
    }

    // validate each sample and count those meeting the threshold
    double count = Reduce(samples, 0.0, (acc, x) =>
    {
        if (!double.IsFinite(x))
        {
            throw new ArgumentException("All samples must be finite");
        }

        // count both positive and negative magnitudes, including equality.
        return Math.Abs(x) >= threshold ? acc + 1 : acc;
    });

    // divide the matching count by the total number of samples.
    return count / samples.Count;
    }
}
