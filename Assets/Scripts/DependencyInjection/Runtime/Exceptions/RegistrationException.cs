using System;

namespace DependencyInjector.Exceptions
{
    public class RegistrationException : Exception
    {
        public RegistrationException (string message) : base(message)
        {
        }
    }
}