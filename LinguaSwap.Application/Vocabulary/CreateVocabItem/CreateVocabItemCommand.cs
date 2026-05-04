using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.CreateVocabItem
{
    public sealed record CreateVocabItemCommand(
        Guid UserId,
        Guid LibraryId,
        List<TermInput> Terms
    );

    public sealed record TermInput(string LanguageCode, string Text);
}
