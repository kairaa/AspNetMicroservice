using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        //this will collect all the validators (updateordercommandvalidator etc.)
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //if has any validator class
            if(_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                //validates all the context
                //runs the all the validation rule in validator class
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                //holds all the validation errors
                var failures = validationResults.SelectMany(a => a.Errors).Where(f => f != null).ToList();
            
                //if there is failure, throw error
                if(failures.Count > 0)
                {
                    throw new Ordering.Application.Exceptions.ValidationException(failures);
                }
            }
            //continue to request pipeline
            return await next();
        }
    }
}
