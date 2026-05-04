using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.Common
{
    public sealed class WordPracticeStats
    {
        public int TimesShown { get; set; } = 0;
        public int CorrectCount { get; set; } = 0;

        // Para “mostrar aprendidas solo 1 vez después de crecer el set”
        public int LastReviewGrowthVersion { get; set; } = -1;

        public double Accuracy => TimesShown == 0 ? 0.0 : (double)CorrectCount / TimesShown;

        public bool IsLearned =>
            TimesShown >= 3 && Accuracy >= 0.8;
    }
}
