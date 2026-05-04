using LinguaSwap.Application.Practice.Common;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetSessionStats
{
    public sealed class GetSessionStatsHandler
    {
        private readonly LinguaSwapDbContext _db;

        public GetSessionStatsHandler(LinguaSwapDbContext db)
        {
            _db = db;
        }

        public GetSessionStatsResult Handle(GetSessionStatsQuery query)
        {
            var total = _db.Attempts.Count(a => a.SessionId == query.SessionId);
            var correct = _db.Attempts.Count(a => a.SessionId == query.SessionId && a.IsCorrect);
            var accuracy = total == 0 ? 0.0 : (double)correct / total;

            return new GetSessionStatsResult(total, correct, accuracy);
        }
    }
}
