using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressByLanguage
{
    public sealed record GetProgressByLanguageResult(
        string TargetLanguage,
        int DistinctWords,
        int TotalAttempts,
        int CorrectAttempts,
        double Accuracy
    );
}
