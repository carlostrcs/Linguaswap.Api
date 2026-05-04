using LinguaSwap.Application.Progress.GetProgressByLanguage;
using LinguaSwap.Application.Progress.GetProgressByPair;
using LinguaSwap.Application.Progress.GetProgressHistory;
using LinguaSwap.Application.Progress.GetProgressSummary;
using LinguaSwap.Application.Progress.GetTopMistakes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinguaSwap.Api.Controllers
{
    [ApiController]
    [Route("api/progress")]
    [Authorize]
    public sealed class ProgressController : ControllerBase
    {
        private readonly GetProgressSummaryHandler _getProgressSummaryHandler;
        private readonly GetTopMistakesHandler _getTopMistakesHandler;
        private readonly GetProgressHistoryHandler _getProgressHistoryHandler;
        private readonly GetProgressByPairHandler _getProgressByPairHandler;
        private readonly GetProgressByLanguageHandler _getProgressByLanguageHandler;

        public ProgressController(
            GetProgressSummaryHandler getProgressSummaryHandler,
            GetTopMistakesHandler getTopMistakesHandler,
            GetProgressHistoryHandler getProgressHistoryHandler,
            GetProgressByPairHandler getProgressByPairHandler,
            GetProgressByLanguageHandler getProgressByLanguageHandler)
        {
            _getProgressSummaryHandler = getProgressSummaryHandler;
            _getTopMistakesHandler = getTopMistakesHandler;
            _getProgressHistoryHandler = getProgressHistoryHandler;
            _getProgressByPairHandler = getProgressByPairHandler;
            _getProgressByLanguageHandler = getProgressByLanguageHandler;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User identifier missing.");

            return Guid.Parse(userIdString);
        }

        public sealed record GetProgressSummaryRequest(string Source, string Target);

        [HttpGet("summary")]
        public ActionResult<GetProgressSummaryResult> Summary([FromQuery] GetProgressSummaryRequest request)
        {
            var userId = GetUserId();

            var result = _getProgressSummaryHandler.Handle(new GetProgressSummaryQuery(
                UserId: userId,
                SourceLanguage: request.Source,
                TargetLanguage: request.Target
            ));

            return Ok(result);
        }

        public sealed record GetTopMistakesRequest(string Source, string Target, int Limit);

        [HttpGet("top-mistakes")]
        public ActionResult<List<GetTopMistakesResult>> TopMistakes([FromQuery] GetTopMistakesRequest request)
        {
            var userId = GetUserId();

            var result = _getTopMistakesHandler.Handle(new GetTopMistakesQuery(
                UserId: userId,
                SourceLanguage: request.Source,
                TargetLanguage: request.Target,
                Limit: request.Limit == 0 ? 10 : request.Limit
            ));

            return Ok(result);
        }

        public sealed record GetProgressHistoryRequest(string Source, string Target, int Days);

        [HttpGet("history")]
        public ActionResult<List<GetProgressHistoryResult>> History([FromQuery] GetProgressHistoryRequest request)
        {
            var userId = GetUserId();

            var result = _getProgressHistoryHandler.Handle(new GetProgressHistoryQuery(
                UserId: userId,
                SourceLanguage: request.Source,
                TargetLanguage: request.Target,
                Days: request.Days == 0 ? 30 : request.Days
            ));

            return Ok(result);
        }

        [HttpGet("by-pair")]
        public ActionResult<List<GetProgressByPairResult>> ByPair()
        {
            var userId = GetUserId();
            return Ok(_getProgressByPairHandler.Handle(new GetProgressByPairQuery(userId)));
        }

        [HttpGet("by-language")]
        public ActionResult<List<GetProgressByLanguageResult>> ByLanguage()
        {
            var userId = GetUserId();
            return Ok(_getProgressByLanguageHandler.Handle(new GetProgressByLanguageQuery(userId)));
        }
    }
}