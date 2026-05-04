using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressSummary
{
    public sealed record GetProgressSummaryResult(
        int TotalAttempts,
        int CorrectAttempts,
        double Accuracy,
        int DistinctWords
    );
}
