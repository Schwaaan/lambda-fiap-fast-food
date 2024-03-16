using Amazon.CognitoIdentityProvider.Model;
using FourSix.Lambda.Authentication.Model;

namespace FourSix.Lambda.Authentication.Application
{
    public interface IAuthManager
    {
        Task<SignUpResponse> SignUp(string userName, string password, string email);
        Task<InitiateAuthResponse> SignIn(string userName, string password);
        Task<bool> ConfirmationSignUp(string userName, string codeVerification);
        Task<CodeDeliveryDetailsType> ResendConfirmationCodeAsync(string userName);
    }
}
