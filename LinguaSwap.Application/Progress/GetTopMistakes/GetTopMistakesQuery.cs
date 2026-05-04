using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetTopMistakes
{
    public sealed record GetTopMistakesQuery(
        Guid UserId,
        string SourceLanguage,
        string TargetLanguage,
        int Limit = 10
    );
}
