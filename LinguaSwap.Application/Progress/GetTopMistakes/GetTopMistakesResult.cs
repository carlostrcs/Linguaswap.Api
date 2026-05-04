using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetTopMistakes
{
    public sealed record GetTopMistakesResult(
        Guid VocabItemId,
        int Correct,
        int Wrong,
        double Accuracy
    );
}
