using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Contract.Dto.Auth;
using Contract.Service;
using Core.Entities;
using Core.Persistence;
using Shared;

namespace Application.Service;

public class AuthService : IAuthService
{
    private readonly IMainRepository _mainRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IValidator<CreateUserDto> _validator;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    public AuthService(IMainRepository mainRepository, RoleManager<AppRole> roleManager, UserManager<User> userManager, IValidator<CreateUserDto> validator, IEmailService emailService, ISmsService smsService)
    {
        _mainRepository = mainRepository;
        _roleManager = roleManager;
        _userManager = userManager;
        _validator = validator;
        _emailService = emailService;
        _smsService = smsService;
    }

    public async Task<ResponseResult> SignUp(CreateUserDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join(", ", validation.Errors
                .Select(e => e.ErrorMessage));

            return ResponseResult.CreateError(errors, ErrorConstant.SignUpValidateError);
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Username,
            PhoneNumber = dto.Phonenumber,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var identityErrors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ResponseResult.CreateError(identityErrors, ErrorConstant.SignUpCreateUser);
        }

        const string defaultRole = "User";
        if (await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _userManager.AddToRoleAsync(user, defaultRole);
        }

        await _mainRepository.UnitOfWork.CommitAsync();
        await SendEmailConfirmation(user);

        return ResponseResult.CreateSuccess();
    }

    public async Task<ResponseResult> ConfirmEmail(ConfirmPhoneOrEmailDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return ResponseResult.CreateError("User was not found", ErrorConstant.UserNotFound);

        var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ResponseResult.CreateError(errors, ErrorConstant.EmailConfirmFailed);
        }

        return ResponseResult.CreateSuccess();
    }

    public async Task<ResponseResult> ConfirmPhone(ConfirmPhoneOrEmailDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return ResponseResult.CreateError("User was not found", ErrorConstant.UserNotFound);

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber!, dto.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ResponseResult.CreateError(errors, ErrorConstant.PhoneConfirmFailed);
        }

        await SendEmailConfirmation(user);

        return ResponseResult.CreateSuccess();
    }

    private async Task SendPhoneConfirmation(User user)
    {
        var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
        await _smsService.SendAsync(user.PhoneNumber!, code);
    }

    private async Task SendEmailConfirmation(User user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendConfirmationEmailAsync(user.Email!, code);
    }
}