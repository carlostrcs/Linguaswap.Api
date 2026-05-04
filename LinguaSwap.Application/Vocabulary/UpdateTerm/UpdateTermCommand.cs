using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.UpdateTerm
{
    public sealed record UpdateTermCommand(
        Guid UserId,
        Guid TermId,
        string LanguageCode,
        string Text
    );
}
