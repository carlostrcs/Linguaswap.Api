using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetLibraryPracticeOptions
{
        public sealed record GetLibraryPracticeOptionsQuery(
            Guid LibraryId,
            Guid? UserId
        );
}
