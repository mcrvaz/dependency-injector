using System;

namespace DependencyInjector.Exceptions
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException (string message) : base(message)
        {
        }
    }
}