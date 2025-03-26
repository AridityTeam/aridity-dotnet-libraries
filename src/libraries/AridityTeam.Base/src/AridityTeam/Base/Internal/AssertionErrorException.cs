using System;
using System.Linq.Expressions;

namespace AridityTeam.Base.Internal;

internal class AssertionErrorException : Exception
{
    public AssertionErrorException(string methodName, Expression expression) : base($"{methodName}({expression})")
    {
    }

    public AssertionErrorException(string msg) : base(msg)
    {
    }

    public AssertionErrorException(string msg, Exception innerException) : base(msg, innerException)
    {
        
    }
}