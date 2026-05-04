using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItems
{
    public sealed record GetVocabItemsQuery(Guid UserId, Guid? LibraryId);
}
