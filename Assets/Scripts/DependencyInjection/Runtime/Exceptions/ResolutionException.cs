using System;

namespace DependencyInjectionFramework
{
    public class ResolutionException : Exception
    {
        public ResolutionException (string message) : base(message)
        {
        }
    }
}
