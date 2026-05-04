using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetLibraryItems
{
    public sealed record GetLibraryItemsResult(List<GetLibraryItemsItem> Items);

    public sealed record GetLibraryItemsItem(Guid VocabItemId, List<GetLibraryItemsTerm> Terms);

    public sealed record GetLibraryItemsTerm(Guid Id, string LanguageCode, string Text);
}
