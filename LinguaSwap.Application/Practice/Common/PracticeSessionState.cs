using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.Common
{
    public sealed class PracticeSessionState
    {
        public List<Guid> ActiveVocabItemIds { get; } = new();
        public Queue<Guid> CurrentIterationQueue { get; } = new();

        public Dictionary<Guid, WordPracticeStats> StatsByWordId { get; } = new();

        public int GrowthVersion { get; set; } = 0;
    }
}
