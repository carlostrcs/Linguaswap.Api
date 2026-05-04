using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.AddTerm
{
    public sealed record AddTermCommand(
        Guid UserId,
        Guid VocabItemId,
        string LanguageCode,
        string Text
    );
}
