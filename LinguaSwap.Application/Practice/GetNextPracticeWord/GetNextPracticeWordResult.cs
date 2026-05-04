using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetNextPracticeWord
{
    public sealed record GetNextPracticeWordResult(Guid WordId, string Prompt);
}
