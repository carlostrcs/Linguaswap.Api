using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.DeleteVocabItem
{
    public sealed record DeleteVocabItemCommand(Guid UserId, Guid VocabItemId);
}
