using FluentValidation;

namespace GraphUserManagement.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserDto.DisplayName)
            .NotEmpty()
            .MaximumLength(256)
            .WithMessage("Display name is required and must not exceed 256 characters.");

        RuleFor(x => x.UserDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .WithMessage("A valid email address is required and must not exceed 256 characters.");

        RuleFor(x => x.UserDto.GivenName)
            .NotEmpty()
            .MaximumLength(64)
            .WithMessage("Given name is required and must not exceed 64 characters.");

        RuleFor(x => x.UserDto.Surname)
            .NotEmpty()
            .MaximumLength(64)
            .WithMessage("Surname is required and must not exceed 64 characters.");

        RuleFor(x => x.UserDto.UserPrincipalName)
            .NotEmpty()
            .MaximumLength(113)
            .WithMessage("User principal name is required and must not exceed 113 characters.");

        RuleFor(x => x.UserDto.JobTitle)
            .MaximumLength(128)
            .When(x => !string.IsNullOrEmpty(x.UserDto.JobTitle))
            .WithMessage("Job title must not exceed 128 characters.");

        RuleFor(x => x.UserDto.Department)
            .MaximumLength(64)
            .When(x => !string.IsNullOrEmpty(x.UserDto.Department))
            .WithMessage("Department must not exceed 64 characters.");

        RuleFor(x => x.UserDto.MobilePhone)
            .MaximumLength(15)
            .When(x => !string.IsNullOrEmpty(x.UserDto.MobilePhone))
            .WithMessage("Mobile phone must not exceed 15 characters.");
    }
}