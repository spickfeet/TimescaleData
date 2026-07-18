using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Const;
using WebAPI.DTOs.CsvDTOs;
using WebAPI.DTOs.ResultDTOs;
using WebAPI.DTOs.ValueDTOs;
using WebAPI.Features.Features.GetLastValues;
using WebAPI.Features.Features.GetResults;
using WebAPI.Features.Features.UploadCsv;

namespace WebAPI.Controllers
{
    [ApiController]
    public class TimescaleController : Controller
    {
        private readonly ILogger<TimescaleController> _logger;
        private readonly IMediator _mediator;

        public TimescaleController(ILogger<TimescaleController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(ServiceRoutes.Timescale.Upload)]
        public async Task<IActionResult> UploadCsv([FromForm] CsvUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest(new { Errors = new[] { "Файл не выбран" } });

            var fileName = dto.File.FileName;

            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest(new { Errors = new[] { "Некорректное имя файла" } });

            using var stream = dto.File.OpenReadStream();

            var command = new UploadCsvCommand(fileName, stream);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Errors = result.Error });
            }

            return Ok(new { Message = "Файл успешно обработан" });
        }

        [HttpGet(ServiceRoutes.Timescale.Results)]
        public async Task<ActionResult<List<ResultDtoResponse>>> GetResults([FromQuery] ResultFilter resultFilter)
        {
            var command = new GetResultsQuery(resultFilter);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Errors = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpGet(ServiceRoutes.Timescale.ValuesByFileName)]
        public async Task<ActionResult<List<ValueDtoResponse>>> GetValuesByFileName(string fileName)
        {
            var command = new GetValuesQuery(fileName);

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Errors = result.Error });
            }

            return Ok(result.Value);
        }
    }
}
