using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Contract.Dto.Auth;
using Core.Entities;

namespace Application.Validation;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    private readonly UserManager<User> _userManager;
    public CreateUserDtoValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(async (email, cancellation) =>
            {
                var existing = await userManager.Users
                    .Where(u => u.Email == email)
                    .AnyAsync(cancellation);
                return !existing;
            })
            .WithMessage("This email is already registered and confirmed.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MustAsync(async (username, cancellation) =>
            {
                var existing = await userManager.Users
                    .Where(u => u.UserName == username)
                    .AnyAsync(cancellation);
                return !existing;
            })
            .WithMessage("This username is already taken.");

        RuleFor(x => x.Phonenumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{9,15}$").WithMessage("Invalid phone number format.")
            .MustAsync(async (phone, cancellation) =>
            {
                var existing = await userManager.Users
                    .Where(u => u.PhoneNumber == phone)
                    .AnyAsync(cancellation);
                return !existing;
            })
            .WithMessage("This phone number is already confirmed.");
    }
}
