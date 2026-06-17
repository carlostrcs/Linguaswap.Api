using LinguaSwap.Application.Practice.GetLibraryPracticeOptions;
using LinguaSwap.Application.Vocabulary.CreateLibrary;
using LinguaSwap.Application.Vocabulary.DeleteLibrary;
using LinguaSwap.Application.Vocabulary.GetLibraries;
using LinguaSwap.Application.Vocabulary.GetLibraryItems;
using LinguaSwap.Application.Vocabulary.GetPublicLibraries;
using LinguaSwap.Application.Vocabulary.UpdateLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LinguaSwap.Api.Controllers
{
    [ApiController]
    [Route("api/libraries")]
    public sealed class LibrariesController : ControllerBase
    {
        private readonly CreateLibraryHandler _createLibraryHandler;
        private readonly GetLibrariesHandler _getLibrariesHandler;
        private readonly UpdateLibraryHandler _updateLibraryHandler;
        private readonly DeleteLibraryHandler _deleteLibraryHandler;
        private readonly GetLibraryItemsHandler _getLibraryItemsHandler;
        private readonly GetPublicLibrariesHandler _getPublicLibrariesHandler;
        private readonly GetLibraryPracticeOptionsHandler _getLibraryPracticeOptionsHandler;

        public LibrariesController(
            CreateLibraryHandler createLibraryHandler,
            GetLibrariesHandler getLibrariesHandler,
            UpdateLibraryHandler updateLibraryHandler,
            DeleteLibraryHandler deleteLibraryHandler,
            GetLibraryItemsHandler getLibraryItemsHandler,
            GetPublicLibrariesHandler getPublicLibrariesHandler,
            GetLibraryPracticeOptionsHandler getLibraryPracticeOptionsHandler)
        {
            _createLibraryHandler = createLibraryHandler;
            _getLibrariesHandler = getLibrariesHandler;
            _updateLibraryHandler = updateLibraryHandler;
            _deleteLibraryHandler = deleteLibraryHandler;
            _getLibraryItemsHandler = getLibraryItemsHandler;
            _getPublicLibrariesHandler = getPublicLibrariesHandler;
            _getLibraryPracticeOptionsHandler = getLibraryPracticeOptionsHandler;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User identifier missing.");

            return Guid.Parse(userIdString);
        }

        private Guid? GetCurrentUserIdOrNull()
        {
            var value =
                User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }

        public sealed record CreateLibraryRequest(string Name);

        [Authorize]
        [HttpPost]
        public ActionResult<CreateLibraryResult> Create([FromBody] CreateLibraryRequest request)
        {
            var userId = GetUserId();
            var result = _createLibraryHandler.Handle(new CreateLibraryCommand(userId, request.Name));
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<GetLibrariesResult>> GetMine()
        {
            var userId = GetUserId();
            return Ok(_getLibrariesHandler.Handle(new GetLibrariesQuery(userId)));
        }

        [HttpGet("{libraryId:guid}/items")]
        public ActionResult<GetLibraryItemsResult> GetItems(Guid libraryId)
        {
            Guid? userId = null;
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (claim is not null) userId = Guid.Parse(claim);

            var result = _getLibraryItemsHandler.Handle(new GetLibraryItemsQuery(libraryId, userId));
            return Ok(result);
        }

        public sealed record UpdateLibraryRequest(string Name);

        [Authorize]
        [HttpPut("{libraryId:guid}")]
        public ActionResult<UpdateLibraryResult> Update(Guid libraryId, [FromQuery] UpdateLibraryRequest request)
        {
            var userId = GetUserId();

            var result = _updateLibraryHandler.Handle(new UpdateLibraryCommand(
                userId,
                libraryId,
                request.Name
            ));

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{libraryId:guid}")]
        public IActionResult DeleteLibrary(Guid libraryId)
        {
            var userId = GetUserId();

            _deleteLibraryHandler.Handle(new DeleteLibraryCommand(
                userId,
                libraryId
            ));

            return NoContent();
        }

        [HttpGet("public")]
        public ActionResult<GetPublicLibrariesResult> GetPublic()
        {
            return Ok(_getPublicLibrariesHandler.Handle(new GetPublicLibrariesQuery()));
        }

        [HttpGet("{libraryId:guid}/practice-options")]
        [AllowAnonymous]
        public ActionResult<GetLibraryPracticeOptionsResult> GetPracticeOptions(Guid libraryId)
        {
            var userId = GetCurrentUserIdOrNull();

            var result = _getLibraryPracticeOptionsHandler.Handle(new GetLibraryPracticeOptionsQuery(libraryId, userId));

            return Ok(result);
        }
    }
}