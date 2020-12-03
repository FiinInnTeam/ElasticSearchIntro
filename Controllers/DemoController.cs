using ElasticsearchIntro.Features;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElasticsearchIntro.Controllers
{
    public class DemoController : BaseController
    {
        private readonly IMediator _mediator;

        public DemoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        public async Task<IActionResult> Search2([FromQuery] SearchRequest2 request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        public async Task<IActionResult> SearchByAge([FromQuery] SearchByAgeRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        public async Task<IActionResult> SearchTotalAssets([FromQuery] SearchTotalAssetsRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        public async Task<IActionResult> SearchTotalAssetsUsd([FromQuery] SearchTotalAssetsUsdRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        public async Task<IActionResult> ComTypeAggregations([FromQuery] ComTypeAggregationsRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
