using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressByPair
{
    public sealed record GetProgressByPairResult(
        string SourceLanguage,
        string TargetLanguage,
        int DistinctWords,
        int TotalAttempts,
        int CorrectAttempts,
        int IncorrectAttempts,
        double Accuracy
    );
}
