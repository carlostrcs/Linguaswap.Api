using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetLibraryPracticeOptions
{
    public sealed record PracticeLanguagePairResult(
        string SourceLanguage,
        string TargetLanguage,
        int VocabItemCount
    );

    public sealed record GetLibraryPracticeOptionsResult(
        Guid LibraryId,
        IReadOnlyList<string> Languages,
        IReadOnlyList<PracticeLanguagePairResult> Pairs
    );
}
