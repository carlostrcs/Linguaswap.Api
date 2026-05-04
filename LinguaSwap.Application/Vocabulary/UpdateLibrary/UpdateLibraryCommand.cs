using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.UpdateLibrary
{
    public sealed record UpdateLibraryCommand(Guid UserId, Guid LibraryId, string Name);
}
