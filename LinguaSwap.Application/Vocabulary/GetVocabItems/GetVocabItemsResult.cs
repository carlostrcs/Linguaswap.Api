using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItems
{
    public sealed record GetVocabItemsResult(Guid Id, Guid LibraryId, int TermCount);
}
