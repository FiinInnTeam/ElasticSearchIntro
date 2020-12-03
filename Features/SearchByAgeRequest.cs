using ElasticsearchIntro.Dtos;
using MediatR;
using Nest;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using static Nest.Infer;

namespace ElasticsearchIntro.Features
{
    public sealed class SearchByAgeRequest : MediatR.IRequest<Pagination<CompanyInformationDto>>
    {
        public int? AgeFrom { get; set; }

        public int? AgeTo { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        private class Handler : IRequestHandler<SearchByAgeRequest, Pagination<CompanyInformationDto>>
        {
            private readonly IElasticClient _elasticClient;

            public Handler(IElasticClient elasticClient)
            {
                _elasticClient = elasticClient;
            }

            public async Task<Pagination<CompanyInformationDto>> Handle(SearchByAgeRequest request, CancellationToken cancellationToken)
            {
                //Object Initializer syntax
                int page = request.Page, pageSize = request.PageSize;

                var searchRequest = new Nest.SearchRequest<CompanyInformationDto>
                {
                    From = (page - 1) * pageSize,
                    Size = pageSize,
                    Query = new DateRangeQuery
                    {
                        Field = Field<CompanyInformationDto>(f => f.IncorporationDate),
                        LessThanOrEqualTo = request.AgeFrom == null ? null : $"now/y-{request.AgeFrom}y/y",
                        GreaterThanOrEqualTo = request.AgeTo == null ? null : $"now/y-{request.AgeTo}y/y"
                    },
                    Sort = new List<ISort>
                    {
                        new FieldSort
                        {
                            Field = Field<CompanyInformationDto>(f => f.IncorporationDate),
                            Order = SortOrder.Ascending
                        }
                    },
                    Source = new SourceFilter
                    {
                        Includes = Fields<CompanyInformationDto>(f => f.OrganCode, f => f.OrganName, f => f.Age, f => f.IncorporationDate)
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
