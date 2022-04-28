using System;

namespace DependencyInjectionFramework
{
    public class RegistrationException : Exception
    {
        public RegistrationException (string message) : base(message)
        {
        }
    }
}
