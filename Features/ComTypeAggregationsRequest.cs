using ElasticsearchIntro.Dtos;
using MediatR;
using Nest;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticsearchIntro.Features
{
    public sealed class ComTypeAggregationsRequest : MediatR.IRequest<object>
    {
        private class Handler : IRequestHandler<ComTypeAggregationsRequest, object>
        {
            private readonly IElasticClient _elasticClient;

            public Handler(IElasticClient elasticClient)
            {
                _elasticClient = elasticClient;
            }

            public async Task<object> Handle(ComTypeAggregationsRequest request, CancellationToken cancellationToken)
            {
                var searchRespone = await _elasticClient.SearchAsync<CompanyInformationDto>(selector => selector
                    .Size(0)
                    .Aggregations(a => a
                        .Terms("comtype", t => t.Field(f => f.ComTypeCode.Suffix("keyword")).Size(100))
                    )
                );

                searchRespone.Aggregations.TryGetValue("comtype", out var aggregate);

                return (aggregate as BucketAggregate).Items.Select(t =>
                {
                    var bucket = (t as KeyedBucket<object>);
                    return new
                    {
                        bucket.Key,
                        bucket.DocCount
                    };
                }).ToList();
            }
        }
    }
}
