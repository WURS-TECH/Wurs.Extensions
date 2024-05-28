namespace Wurs.Extensions.ServiceCollection.Exceptions;

using System;

// Custom exception class
public class RegisterOptionException : Exception
{
    // Default constructor
    public RegisterOptionException()
    {
    }

    // Constructor with a message
    public RegisterOptionException(string message)
        : base(message)
    {
    }

    // Constructor with a message and an inner exception
    public RegisterOptionException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Constructor for serialization (used in deserialization)
    protected RegisterOptionException(System.Runtime.Serialization.SerializationInfo info,
                                System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
}

