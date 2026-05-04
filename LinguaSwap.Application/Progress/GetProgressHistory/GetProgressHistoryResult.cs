using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressHistory
{
    public sealed record GetProgressHistoryResult(
        DateTime Day,
        int Attempts,
        int Correct
    );
}
