using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetPublicLibraries
{
    public sealed record GetPublicLibrariesResult(List<GetPublicLibrariesItem> Items);

    public sealed record GetPublicLibrariesItem(Guid Id, string Name);
}
