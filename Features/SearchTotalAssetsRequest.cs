using ElasticsearchIntro.Dtos;
using MediatR;
using Nest;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticsearchIntro.Features
{
    public sealed class SearchTotalAssetsRequest : MediatR.IRequest<Pagination<CompanyInformationDto>>
    {
        public int YearReport { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        private class Handler : IRequestHandler<SearchTotalAssetsRequest, Pagination<CompanyInformationDto>>
        {
            private readonly IElasticClient _elasticClient;

            public Handler(IElasticClient elasticClient)
            {
                _elasticClient = elasticClient;
            }

            public async Task<Pagination<CompanyInformationDto>> Handle(SearchTotalAssetsRequest request, CancellationToken cancellationToken)
            {
                int page = request.Page, pageSize = request.PageSize;

                var searchResponse = await _elasticClient.SearchAsync<CompanyInformationDto>(selector => selector
                     .From((page - 1) * pageSize)
                     .Size(pageSize)
                     .Query(q => q.Nested(n => n
                           .Path(f => f.FinancialStatements)
                           .Query(nq => nq.Bool(b => b.Must(
                                m => m.Range(r => r.Field(f => f.FinancialStatements[0].BSA53).GreaterThanOrEquals(request.From).LessThanOrEquals(request.To)),
                                m => m.Term(t => t.FinancialStatements[0].YearReport, request.YearReport)
                            )))
                     ))
                     .Source(s => s.Includes(i => i.Fields(f => f.OrganCode, f => f.en_OrganName, f => f.FinancialStatements[0].BSA53, f => f.FinancialStatements[0].YearReport)))
                     .Sort(s => s
                        .Field(f => f
                            .Field(ff => ff.FinancialStatements[0].BSA53)
                            .Nested(n => n
                                .Path(p => p.FinancialStatements)
                                .Filter(ft => ft.Term(t => t.FinancialStatements[0].YearReport, request.YearReport))
                            )
                            .Order(SortOrder.Descending)
                        )
                     )
                 );

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
