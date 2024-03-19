using FourSix.Lambda.Authentication.Application;
using FourSix.Lambda.Authentication.Model;
using Microsoft.AspNetCore.Mvc;

namespace FourSix.Lambda.Authentication.Controllers;

[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IAuthManager _authManager;

    public LoginController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    [HttpPost]
    public async Task<ResponseModel> Login([FromForm] string userName, [FromForm] string password)
    {
        try
        {
            var response = await _authManager.SignIn(userName, password);
            return new ResponseModel
            {
                Code="1",
                Message="login Efetuado com sucesso",
                AccesToken=response.AuthenticationResult.AccessToken
            };
        }
        catch (Exception ex)
        {
            return new ResponseModel
            {
                Code = "-1",
                Message = $"Erro Login: {ex.Message}"
            };
        }
    }

    [HttpPost]
    [Route("signup")]
    public async Task<ResponseModel> SignUp([FromForm] string userName, [FromForm]string password, [FromForm]string email)
    {
        try
        {
            var response = await _authManager.SignUp(userName, password, email);
            return new ResponseModel
            {
                Code = "1",
                Message = "Usuario Criado com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResponseModel
            {
                Code = "-1",
                Message = $"Erro Cadastro: {ex.Message}"
            };           
        }
    }
    

    [HttpPost]
    [Route("confirmation")]
    public async Task<ResponseModel> Confirmation([FromForm]string userName, [FromForm]string codeVerification)
    {
        try
        {
            var response = await _authManager.ConfirmationSignUp(userName, codeVerification);
            if (response)
                return new ResponseModel
                {
                    Code = "1",
                    Message = "Email confirmado"
                };
            
            throw new Exception("Não foi possível confirmar o email");
        }
        catch (Exception ex)
        {
            return new ResponseModel
            {
                Code = "-1",
                Message = ex.Message
            };
        }
    }

    [HttpPost]
    [Route("resendcode")]
    public async Task<ResponseModel> ResendCode([FromForm] string userName)
    {
        try
        {
            var response = await _authManager.ResendConfirmationCodeAsync(userName);
            if (response != null)
                return new ResponseModel
                {
                    Code = "1",
                    Message = "Email reenviado"
                };

            throw new Exception("Não foi possível reenviar o email");
        }
        catch (Exception ex)
        {
            return new ResponseModel
            {
                Code = "-1",
                Message = ex.Message
            };
        }
    }

    [HttpGet]
    public string Get()
    {
        return "testeOk_v1";
    }



}