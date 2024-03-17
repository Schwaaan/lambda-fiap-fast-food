using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using FourSix.Lambda.Authentication.Model;
using System.Net;

namespace FourSix.Lambda.Authentication.Application
{
    public class CognitoManager : IAuthManager
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;

        private readonly string clientId = "20jmg7ee7pdidqq4pnbog062ht";

        public CognitoManager()
        {
            _cognitoClient = new AmazonCognitoIdentityProviderClient(Amazon.RegionEndpoint.USEast1);
        }

        public async Task<SignUpResponse> SignUp(string userName, string password, string email)
        {
            var userAttrs = new AttributeType
            {
                Name = "email",
                Value = email,
            };

            var userAttrsList = new List<AttributeType>();

            userAttrsList.Add(userAttrs);

            var userSignUpRequest = new SignUpRequest
            {
                UserAttributes = userAttrsList,
                ClientId = clientId,
                Password = password,
                Username = userName
            };

            return await _cognitoClient.SignUpAsync(userSignUpRequest);
            
        }

        public async Task<InitiateAuthResponse> SignIn(string userName, string password)
        {
            var authParameters = new Dictionary<string, string>();
            authParameters.Add("USERNAME", userName);
            authParameters.Add("PASSWORD", password);

            var authRequest = new InitiateAuthRequest

            {
                ClientId = clientId,
                AuthParameters = authParameters,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            };


            return await _cognitoClient.InitiateAuthAsync(authRequest);
        }

        public async Task<bool> ConfirmationSignUp(string userName, string codeVerification)
        {
            var signUpRequest = new ConfirmSignUpRequest
            {
                ClientId = clientId,
                ConfirmationCode = userName,
                Username = codeVerification,
            };

            var response = await _cognitoClient.ConfirmSignUpAsync(signUpRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return true;
            
            return false;
        }

        public async Task<CodeDeliveryDetailsType> ResendConfirmationCodeAsync(string userName)
        {
            var codeRequest = new ResendConfirmationCodeRequest
            {
                ClientId = clientId,
                Username = userName,
            };

            var response = await _cognitoClient.ResendConfirmationCodeAsync(codeRequest);

            return response.CodeDeliveryDetails;
        }


    }
}
