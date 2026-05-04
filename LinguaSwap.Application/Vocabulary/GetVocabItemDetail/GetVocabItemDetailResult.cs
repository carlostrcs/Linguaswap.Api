using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItemDetail
{
    public sealed record GetVocabItemDetailResult(Guid Id, Guid LibraryId, List<VocabTermDto> Terms);
}
