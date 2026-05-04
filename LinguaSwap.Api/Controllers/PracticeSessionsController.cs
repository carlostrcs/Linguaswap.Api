using LinguaSwap.Application.Practice.GetNextPracticeWord;
using LinguaSwap.Application.Practice.GetSessionStats;
using LinguaSwap.Application.Practice.StartPracticeSession;
using LinguaSwap.Application.Practice.SubmitAttempt;
using LinguaSwap.Domain.Practice;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinguaSwap.Api.Controllers
{
    [ApiController]
    [Route("api/practice/sessions")]
    public sealed class PracticeSessionsController : ControllerBase
    {
        private readonly StartPracticeSessionHandler _startPracticeSessionHandler;
        private readonly GetNextPracticeWordHandler _getNextPracticeWordHandler;
        private readonly SubmitAttemptHandler _submitAttemptHandler;
        private readonly GetSessionStatsHandler _getSessionStatsHandler;

        public PracticeSessionsController(
            StartPracticeSessionHandler startPracticeSessionHandler,
            GetNextPracticeWordHandler getNextPracticeWordHandler,
            SubmitAttemptHandler submitAttemptHandler,
            GetSessionStatsHandler getSessionStatsHandler)
        {
            _startPracticeSessionHandler = startPracticeSessionHandler;
            _getNextPracticeWordHandler = getNextPracticeWordHandler;
            _submitAttemptHandler = submitAttemptHandler;
            _getSessionStatsHandler = getSessionStatsHandler;
        }

        public sealed record StartPracticeSessionRequest(
            Guid? LibraryId,
            string SourceLanguage,
            string TargetLanguage,
            PracticeDirection Direction,
            PracticeDifficulty Difficulty
        );

        [HttpPost]
        public ActionResult<StartPracticeSessionResult> Start([FromBody] StartPracticeSessionRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid? userId = userIdClaim is null
                ? null
                : Guid.Parse(userIdClaim);

            var result = _startPracticeSessionHandler.Handle(new StartPracticeSessionCommand(
                UserId: userId,
                SourceLanguage: request.SourceLanguage,
                TargetLanguage: request.TargetLanguage,
                Direction: request.Direction,
                Difficulty: request.Difficulty,
                LibraryId: request.LibraryId
            ));

            return Ok(result);
        }

        [HttpGet("{sessionId:guid}/next")]
        public ActionResult<GetNextPracticeWordResult> Next(Guid sessionId)
        {
            var result = _getNextPracticeWordHandler.Handle(new GetNextPracticeWordQuery(sessionId));
            return Ok(result);
        }

        public sealed record SubmitAttemptRequest(Guid WordId, string UserAnswer);

        [HttpPost("{sessionId:guid}/attempts")]
        public ActionResult<SubmitAttemptResult> Submit(
            Guid sessionId, [FromBody] SubmitAttemptRequest request)
        {
            var result = _submitAttemptHandler.Handle(new SubmitAttemptCommand(sessionId, request.WordId, request.UserAnswer));
            return Ok(result);
        }

        [HttpGet("{sessionId:guid}/stats")]
        public ActionResult<GetSessionStatsResult> Stats(
        Guid sessionId)
        {
            var result = _getSessionStatsHandler.Handle(new GetSessionStatsQuery(sessionId));
            return Ok(result);
        }
    }
}
