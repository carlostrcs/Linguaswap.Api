using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressSummary
{
    public sealed record GetProgressSummaryQuery(
        Guid UserId,
        string SourceLanguage,
        string TargetLanguage
    );
}
