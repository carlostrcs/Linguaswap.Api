using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItemDetail
{
    public sealed record GetVocabItemDetailQuery(Guid UserId, Guid VocabItemId);
}
