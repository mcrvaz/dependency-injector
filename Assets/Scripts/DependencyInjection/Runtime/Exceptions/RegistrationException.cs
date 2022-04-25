using System;

namespace DependencyInjector
{
    public class RegistrationException : Exception
    {
        public RegistrationException (string message) : base(message)
        {
        }
    }
}