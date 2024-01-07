using FluentValidation;
using Users.Api.DataTransferObjects;

namespace Users.Api.Utilities.FluentValidation
{
    public class UserDtoForValidation : AbstractValidator<UserDtoForInsertion>
    {
        public UserDtoForValidation()
        {
            RuleFor(r => r.FullName).NotEmpty().WithMessage("Full Name can not be empty.");
            RuleFor(r => r.FullName).MaximumLength(3).WithMessage("Full Name must be greater than 3 letter");
        }
    }
}
