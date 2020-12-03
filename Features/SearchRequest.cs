using ElasticsearchIntro.Dtos;
using MediatR;
using Nest;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticsearchIntro.Features
{
    public sealed class SearchRequest : MediatR.IRequest<Pagination<CompanyInformationDto>>
    {
        public string Term { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        private class Handler : IRequestHandler<SearchRequest, Pagination<CompanyInformationDto>>
        {
            private readonly IElasticClient _elasticClient;

            public Handler(IElasticClient elasticClient)
            {
                _elasticClient = elasticClient;
            }

            public async Task<Pagination<CompanyInformationDto>> Handle(SearchRequest request, CancellationToken cancellationToken)
            {
                //Fluent DSL (DSL: Domain Specific Language)
                int page = request.Page, pageSize = request.PageSize;
                var searchResponse = await _elasticClient.SearchAsync<CompanyInformationDto>(selector => selector
                   .From((page - 1) * pageSize)
                   .Size(pageSize)
                   .Query(q => q.Bool(b => b.Must(
                        m => m.Terms(t => t.Field(f => f.ComTypeCode.Suffix("keyword")).Terms("CT", "NH")),
                        m => m.MultiMatch(mt => mt
                            .Fields(f => f.Fields("ticker^3", "taxCode.keyword^3", "organName", "en_OrganName", "organShortName", "en_OrganShortName"))
                            .Query((request.Term ?? "").Trim())
                            .Operator(Operator.And)
                            .Type(TextQueryType.BestFields))
                    ))));

                return new Pagination<CompanyInformationDto>
                {
                    Items = searchResponse.Documents,
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = searchResponse.Total
                };
            }
        }
    }
}
