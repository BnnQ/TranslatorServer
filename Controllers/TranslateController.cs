using Microsoft.AspNetCore.Mvc;
using TranslatorServer.Models.Dto;
using TranslatorServer.Services.Abstractions;

namespace TranslatorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private readonly ILogger<TranslateController> logger;
        private readonly ITranslator translator;

        public TranslateController(ILogger<TranslateController> logger, ITranslator translator)
        {
            this.logger = logger;
            this.translator = translator;
        }

        // POST: api/Translate
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromQuery] string fromLanguage, [FromQuery] string toLanguage, [FromBody] TranslationRequestDto translationRequest)
        {
            string translatedText;
            try
            {
                translatedText = await translator.TranslateAsync(fromLanguage, toLanguage, translationRequest.Text);
            }
            catch (InvalidOperationException exception)
            {
                logger.LogWarning("[POST] error during translating text: {ErrorMessage}", exception.Message);
                return BadRequest();
            }
            
            logger.LogInformation("[POST] successfully processed a request; returning translated text");
            return new JsonResult(new TranslationContentResultDto { TranslatedText = translatedText});
        }
        
    }
}