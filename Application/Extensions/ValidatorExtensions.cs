using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var options = ruleBuilder
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");
            return options;
        }
    }
}
