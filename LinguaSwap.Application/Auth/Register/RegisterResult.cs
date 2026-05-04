using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Auth.Register
{
    public sealed record RegisterResult(Guid UserId, string Email);
}
