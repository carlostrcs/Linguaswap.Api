using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.DeleteTerm
{
    public sealed record DeleteTermCommand(Guid UserId, Guid TermId);
}
