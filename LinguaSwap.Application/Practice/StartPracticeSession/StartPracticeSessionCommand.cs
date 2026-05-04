using LinguaSwap.Domain.Practice;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.StartPracticeSession
{
    public sealed record StartPracticeSessionCommand(
        Guid? UserId,
        string SourceLanguage,
        string TargetLanguage,
        PracticeDirection Direction,
        PracticeDifficulty Difficulty,
        Guid? LibraryId
    );
}
