using System;

namespace DependencyInjector.Exceptions
{
    public class ResolutionException : Exception
    {
        public ResolutionException (string message) : base(message)
        {
        }
    }
}