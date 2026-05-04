using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetSessionStats
{
    public sealed record GetSessionStatsResult(
    int TotalAttempts,
    int CorrectAttempts,
    double Accuracy
    );
}
