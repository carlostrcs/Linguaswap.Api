using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetTopMistakes
{
    public sealed record GetTopMistakesResult(
        Guid VocabItemId,
        string SourceText,
        string TargetText,
        int TotalAttempts,
        int CorrectAttempts,
        int IncorrectAttempts,
        double Accuracy
    );
}
