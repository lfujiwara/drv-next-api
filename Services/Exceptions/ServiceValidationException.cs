using System;

namespace drv_next_api.Services.Exceptions
{
    public class ServiceValidationException : Exception
    {
        public FluentValidation.Results.ValidationResult result;

        public ServiceValidationException(FluentValidation.Results.ValidationResult _result)
        {
            result = _result;
        }
    }
}