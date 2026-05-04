using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.CreateLibrary
{
    public sealed record CreateLibraryCommand(Guid UserId, string Name);
}
