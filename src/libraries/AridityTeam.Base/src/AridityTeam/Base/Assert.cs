using System;
using System.Collections.Generic;
using System.Linq;
using AridityTeam.Base.Internal;

namespace AridityTeam.Base;

/// <summary>
/// Assert class.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Checks if the expected type is equal to the actual.
    /// </summary>
    /// <param name="expected">Expected type</param>
    /// <param name="actual">Actual type</param>
    /// <typeparam name="T">Any type bro.</typeparam>
    /// <exception cref="AssertionErrorException">Throws when the expected type is not equal to the actual type.</exception>
    public static void AreEqual<T>(T expected, T actual)
    {
        if (!Equals(expected, actual))
        {
            throw new AssertionErrorException($"Expected: {expected}, but was: {actual}");
        }
    }
    
    /// <summary>
    /// Same as the AreEqual function, but it's the complete opposite.
    /// </summary>
    /// <param name="expected">Expected type</param>
    /// <param name="actual">Actual type</param>
    /// <typeparam name="T">Any type bro.</typeparam>
    /// <exception cref="AssertionErrorException">Throws when of course, the expected type is actually EQUAL to the actual type.</exception>
    public static void AreNotEqual<T>(T expected, T actual)
    {
        if (Equals(expected, actual))
        {
            throw new AssertionErrorException($"Expected: {expected} not equal to {actual}");
        }
    }

    /// <summary>
    /// Checks if the string contains the... You've already got what it means.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="message"></param>
    /// <exception cref="AssertionErrorException"></exception>
    public static void Contains(string expected, string actual, string? message = null)
    {
        if (!actual.Contains(expected)) 
            throw new AssertionErrorException(message ?? $"Assertion failed: expected for {expected} to be inside of the collection");
    }
    
    /// <summary>
    /// The opposite of the Contains function.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="message"></param>
    /// <exception cref="AssertionErrorException"></exception>
    public static void DoesNotContain(string expected, string actual, string? message = null)
    {
        if (actual.Contains(expected)) 
            throw new AssertionErrorException(message ?? $"Assertion failed: expected for {expected} to be inside of the collection");
    }
    
    /// <summary>
    /// Checks if the enumerable type contains the... Again, you already know what this means.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="collection"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="AssertionErrorException"></exception>
    public static void Contains<T>(T? expected, IEnumerable<T?> collection, string? message = null)
    {
        if (!collection.Contains(expected)) 
            throw new AssertionErrorException(message ?? $"Assertion failed: expected for {expected} to be inside of the collection");
    }
    
    /// <summary>
    /// Opposite of the Contains function.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="collection"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="AssertionErrorException"></exception>
    public static void DoesNotContain<T>(T expected, IEnumerable<T> collection, string? message = null)
    {
        if (collection.Contains(expected)) 
            throw new AssertionErrorException(message ?? $"Assertion failed: expected value ({expected}) is currently inside of the collection");
    }

    public static void DoesThrow<T>(T? expected, Action action, string? message = null)
    {
        if (expected == null)
            throw new AssertionErrorException(message ?? "Assertion failed: expected value is null");

        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (ex.GetType() != expected.GetType())
                throw new AssertionErrorException(message ?? $"Assertion failed: expected value ({expected}) is not equal to {ex.GetType()}: {ex.Message}");
        }
    }

    public static void IsTrue(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw new AssertionErrorException(message ?? "Assertion failed: expected true, but was false.");
        }
    }

    public static void IsFalse(bool condition, string? message = null)
    {
        if (condition)
        {
            throw new AssertionErrorException(message ?? "Assertion failed: expected false, but was true.");
        }
    }

    public static void IsNull(object? obj, string? message = null)
    {
        if (obj != null)
        {
            throw new AssertionErrorException(message ?? "Assertion failed: expected null, but was not null.");
        }
    }

    public static void IsNotNull(object? obj, string? message = null)
    {
        if (obj == null)
        {
            throw new AssertionErrorException(message ?? "Assertion failed: expected not null, but was null.");
        }
    }
}