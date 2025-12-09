using Application.Services.Images;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Scalar.AspNetCore;
using System.Reflection.Metadata.Ecma335;

namespace Presentation.Controllers.Misc
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _service;

        public ImageController(ImageService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> DownloadImageAsync([FromRoute] int id
            ,CancellationToken cancellationToken = default)
        {
            var response = await _service.GetImageAsync(id, cancellationToken);

            if(response.Value !=null && response.IsSuccess)
            {
                return File(response.Value.ImageStream, response.Value.MimeType
                    ,response.Value.FileName);
            }
            return response.ErrorType switch
            {
                ErrorType.Conflict => Conflict(response.ErrorMessage),
                ErrorType.BadRequest => BadRequest(response.ErrorMessage),
                ErrorType.NotFound => NotFound(response.ErrorMessage),
                ErrorType.Unauthorized => Unauthorized(response.ErrorMessage),
                ErrorType.InternalServerError => new ObjectResult(response.ErrorMessage)
                { StatusCode = 500 },
                _ => Ok(response.Value)
            };
        }

    }
}
