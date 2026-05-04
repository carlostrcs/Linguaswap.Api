using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.DeleteLibrary
{
    public sealed record DeleteLibraryCommand(Guid UserId, Guid LibraryId);
}
