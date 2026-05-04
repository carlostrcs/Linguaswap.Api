using LinguaSwap.Application.Auth.Login;
using LinguaSwap.Application.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace LinguaSwap.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly RegisterHandler _registerHandler;
        private readonly LoginHandler _loginHandler;

        public AuthController(RegisterHandler registerHandler, LoginHandler loginHandler)
        {
            _registerHandler = registerHandler;
            _loginHandler = loginHandler;
        }

        public sealed record RegisterRequest(string Email, string Password);
        public sealed record LoginRequest(string Email, string Password);

        [HttpPost("register")]
        public ActionResult<RegisterResult> Register([FromBody] RegisterRequest request)
        {
            var result = _registerHandler.Handle(new RegisterCommand(request.Email, request.Password));
            return Ok(result);
        }

        [HttpPost("login")]
        public ActionResult<LoginResult> Login([FromBody] LoginRequest request)
        {
            var result = _loginHandler.Handle(new LoginCommand(request.Email, request.Password));
            return Ok(result);
        }
    }
}