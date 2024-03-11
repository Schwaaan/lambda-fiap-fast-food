using FourSix.Lambda.Authentication.Model;
using Microsoft.AspNetCore.Mvc;

namespace FourSix.Lambda.Authentication.Controllers;

[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public void Post([FromBody] LoginModel login)
    {
    }

    // GET api/values/5
    [HttpGet]
    public string Get()
    {
        return "testeOk";
    }

}