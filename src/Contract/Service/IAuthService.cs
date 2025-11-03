using Contract.Dto.Auth;
using Shared;

namespace Contract.Service;

public interface IAuthService
{
    Task<ResponseResult> SignUp(CreateUserDto dto);
    Task<ResponseResult> ConfirmEmail(ConfirmPhoneOrEmailDto dto);
    Task<ResponseResult> ConfirmPhone(ConfirmPhoneOrEmailDto dto);
}