using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.SubmitAttempt
{
    public sealed record SubmitAttemptCommand(Guid SessionId, Guid WordId, string UserAnswer);
}
