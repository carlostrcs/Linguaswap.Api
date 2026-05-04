using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressHistory
{
    public sealed record GetProgressHistoryQuery(
        Guid UserId,
        string SourceLanguage,
        string TargetLanguage,
        int Days = 30
    );
}
