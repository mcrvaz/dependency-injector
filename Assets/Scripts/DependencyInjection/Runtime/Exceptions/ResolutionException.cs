using System;

namespace DependencyInjector
{
    public class ResolutionException : Exception
    {
        public ResolutionException (string message) : base(message)
        {
        }
    }
}