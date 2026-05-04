using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Auth.Register
{
    public sealed record RegisterCommand(string Email, string Password);
}
