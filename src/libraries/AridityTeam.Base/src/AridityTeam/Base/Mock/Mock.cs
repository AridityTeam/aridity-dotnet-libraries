using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AridityTeam.Base.Mock;

public class Mock<T> : DispatchProxy where T : class
{
    private readonly Dictionary<string, List<object>> _returns = new();
    private readonly Dictionary<string, int> _callCounts = new();

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) return null;
        var methodName = targetMethod.Name;

        // Record the call
        if (!_callCounts.TryAdd(methodName, 1))
        {
            _callCounts[methodName]++;
        }

        // Return the predefined value if set
        if (_returns.TryGetValue(methodName, out var returnValues) && returnValues.Count > 0)
        {
            return returnValues[0]; // Return the first value for simplicity
        }

        throw new Exception($"No return value set for method {methodName}.");
    }

    public void Setup(Expression<Func<T, object>> expression)
    {
        var methodName = GetMethodName(expression);
        if (!_returns.ContainsKey(methodName))
        {
            _returns[methodName] = new List<object>();
        }
    }

    public void Returns(Expression<Func<T, object>> expression, object returnValue)
    {
        var methodName = GetMethodName(expression);
        if (_returns.TryGetValue(methodName, out var @return))
        {
            @return.Add(returnValue);
        }
    }

    public void Verify(Expression<Func<T, object>> expression, int expectedCallCount)
    {
        var methodName = GetMethodName(expression);
        if (_callCounts.TryGetValue(methodName, out var actualCallCount))
        {
            if (actualCallCount != expectedCallCount)
            {
                throw new Exception($"Method {methodName} was called {actualCallCount} times, but expected {expectedCallCount}.");
            }
        }
        else
        {
            throw new Exception($"Method {methodName} was never called.");
        }
    }

    public static Mock<T> Create()
    {
        object proxy = Create<T, Mock<T>>();
        return (Mock<T>)proxy;
    }

    private string GetMethodName(Expression expression)
    {
        if (expression is LambdaExpression { Body: MethodCallExpression methodCall })
        {
            return methodCall.Method.Name;
        }
        throw new ArgumentException("Invalid expression");
    }
}