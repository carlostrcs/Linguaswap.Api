using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItemDetail
{
    public sealed record VocabTermDto(Guid Id, string LanguageCode, string Text);
}
