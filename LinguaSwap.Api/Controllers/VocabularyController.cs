using LinguaSwap.Application.Vocabulary.AddTerm;
using LinguaSwap.Application.Vocabulary.CreateVocabItem;
using LinguaSwap.Application.Vocabulary.DeleteTerm;
using LinguaSwap.Application.Vocabulary.DeleteVocabItem;
using LinguaSwap.Application.Vocabulary.GetVocabItemDetail;
using LinguaSwap.Application.Vocabulary.GetVocabItems;
using LinguaSwap.Application.Vocabulary.UpdateTerm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinguaSwap.Api.Controllers
{
    [ApiController]
    [Route("api/vocab")]
    public sealed class VocabularyController : ControllerBase
    {
        private readonly CreateVocabItemHandler _createVocabItemHandler;
        private readonly AddTermHandler _addTermHandler;
        private readonly DeleteVocabItemHandler _deleteVocabItemHandler;
        private readonly UpdateTermHandler _updateTermHandler;
        private readonly DeleteTermHandler _deleteTermHandler;
        private readonly GetVocabItemsHandler _getVocabItemsHandler;
        private readonly GetVocabItemDetailHandler _getVocabItemDetailHandler;

        public VocabularyController(
            CreateVocabItemHandler createVocabItemHandler,
            AddTermHandler addTermHandler,
            DeleteVocabItemHandler deleteVocabItemHandler,
            UpdateTermHandler updateTermHandler,
            DeleteTermHandler deleteTermHandler,
            GetVocabItemsHandler getVocabItemsHandler,
            GetVocabItemDetailHandler getVocabItemDetailHandler)
        {
            _createVocabItemHandler = createVocabItemHandler;
            _addTermHandler = addTermHandler;
            _deleteVocabItemHandler = deleteVocabItemHandler;
            _updateTermHandler = updateTermHandler;
            _deleteTermHandler = deleteTermHandler;
            _getVocabItemsHandler = getVocabItemsHandler;
            _getVocabItemDetailHandler = getVocabItemDetailHandler;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User identifier missing.");

            return Guid.Parse(userIdString);
        }

        public sealed record CreateVocabItemRequest(Guid LibraryId, List<TermDto> Terms);
        public sealed record TermDto(string LanguageCode, string Text);

        [Authorize]
        [HttpPost("items")]
        public ActionResult<CreateVocabItemResult> CreateItem([FromBody] CreateVocabItemRequest request)
        {
            var userId = GetUserId();

            var terms = request.Terms
                .Select(t => new TermInput(t.LanguageCode, t.Text))
                .ToList();

            var result = _createVocabItemHandler.Handle(new CreateVocabItemCommand(
                UserId: userId,
                LibraryId: request.LibraryId,
                Terms: terms
            ));
            return Ok(result);
        }

        public sealed record AddTermRequest(string LanguageCode, string Text);

        [Authorize]
        [HttpPost("items/{vocabItemId:guid}/terms")]
        public ActionResult<AddTermResult> AddTerm(Guid vocabItemId, [FromBody] AddTermRequest request)
        {
            var userId = GetUserId();

            var result = _addTermHandler.Handle(new AddTermCommand(
                UserId: userId,
                VocabItemId: vocabItemId,
                LanguageCode: request.LanguageCode,
                Text: request.Text
            ));

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("items/{vocabItemId:guid}")]
        public IActionResult DeleteItem(Guid vocabItemId)
        {
            var userId = GetUserId();

            _deleteVocabItemHandler.Handle(new DeleteVocabItemCommand(
                UserId: userId,
                VocabItemId: vocabItemId
            ));

            return NoContent();
        }

        public sealed record UpdateTermRequest(string LanguageCode, string Text);

        [Authorize]
        [HttpPut("terms/{termId:guid}")]
        public ActionResult<UpdateTermResult> UpdateTerm(Guid termId, [FromBody] UpdateTermRequest request)
        {
            var userId = GetUserId();

            var result = _updateTermHandler.Handle(new UpdateTermCommand(
                UserId: userId,
                TermId: termId,
                LanguageCode: request.LanguageCode,
                Text: request.Text
            ));

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("terms/{termId:guid}")]
        public IActionResult DeleteTerm(Guid termId)
        {
            var userId = GetUserId();

            _deleteTermHandler.Handle(new DeleteTermCommand(
                UserId: userId,
                TermId: termId
            ));

            return NoContent();
        }

        public sealed record GetVocabItemsRequest(Guid? LibraryId);

        [Authorize]
        [HttpGet("items")]
        public ActionResult<List<GetVocabItemsResult>> GetItems([FromQuery] GetVocabItemsRequest request)
        {
            var userId = GetUserId();

            var items = _getVocabItemsHandler.Handle(new GetVocabItemsQuery(
                UserId: userId,
                LibraryId: request.LibraryId
            ));

            return Ok(items);
        }

        [Authorize]
        [HttpGet("items/{vocabItemId:guid}")]
        public ActionResult<GetVocabItemDetailResult> GetItemDetail(Guid vocabItemId)
        {
            var userId = GetUserId();

            var result = _getVocabItemDetailHandler.Handle(new GetVocabItemDetailQuery(
                UserId: userId,
                VocabItemId: vocabItemId
            ));

            return Ok(result);
        }
    }
}