using ElasticsearchIntro.Dtos;
using MediatR;
using Nest;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using static Nest.Infer;

namespace ElasticsearchIntro.Features
{
    public sealed class SearchRequest2 : MediatR.IRequest<Pagination<CompanyInformationDto>>
    {
        public string Term { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        private class Handler : IRequestHandler<SearchRequest2, Pagination<CompanyInformationDto>>
        {
            private readonly IElasticClient _elasticClient;

            public Handler(IElasticClient elasticClient)
            {
                _elasticClient = elasticClient;
            }

            public async Task<Pagination<CompanyInformationDto>> Handle(SearchRequest2 request, CancellationToken cancellationToken)
            {
                //Object Initializer syntax
                int page = request.Page, pageSize = request.PageSize;

                var searchRequest = new Nest.SearchRequest<CompanyInformationDto>
                {
                    From = (page - 1) * pageSize,
                    Size = pageSize,
                    Query = new BoolQuery
                    {
                        Must = new List<QueryContainer>
                        {
                            new TermsQuery
                            {
                                Field = Field<CompanyInformationDto>(f=>f.ComTypeCode.Suffix("keyword")),
                                Terms = new []{ "CT", "NH" }
                            },
                            new MultiMatchQuery
                            {
                                Fields = new []{ "ticker^3", "taxCode.keyword^3", "organName", "en_OrganName", "organShortName", "en_OrganShortName" },
                                Query = (request.Term ?? "").Trim(),
                                Operator = Operator.And,
                                Type = TextQueryType.BestFields
                            }
                        }
                    }
                };

                var searchResponse = await _elasticClient.SearchAsync<CompanyInformationDto>(searchRequest);

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
