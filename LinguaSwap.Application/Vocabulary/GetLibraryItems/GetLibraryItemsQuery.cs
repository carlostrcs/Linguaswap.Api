using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetLibraryItems
{
    public sealed record GetLibraryItemsQuery(Guid LibraryId, Guid? UserId);
}
