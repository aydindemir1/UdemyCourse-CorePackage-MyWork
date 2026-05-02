using Core.ElasticSearch.Models;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch
{
    public class ElasticSearchService : IElasticSearchService
    {

        private readonly ElasticClient _client;

        public ElasticSearchService(IConfiguration configuration)
        {
            var config = configuration.GetSection("ElasticSearch").Get<ElasticSearchConfig>() ?? throw new InvalidOperationException("ElasticSearch configuration is missing");

            var settings = new ConnectionSettings(new Uri(config.ConnectionString))
                .DefaultFieldNameInferrer(p => p)
                .PrettyJson()
                .DisableDirectStreaming()
                .DefaultMappingFor<object>(m => m.IdProperty("Id"));

            _client = new ElasticClient(settings);
        }


        public async Task<ElasticSearchResult> CreateIndexAsync(string indexName)
        {
            var exists = await _client.Indices.ExistsAsync(indexName);
            if (exists.Exists)
                return ElasticSearchResult.Fail("Index already exists");
            var response = await _client.Indices.CreateAsync(indexName);
            return ElasticSearchResult.FromResponse(response);

        }

        public async Task<ElasticSearchResult> InsertAsync(InsertOrUpdateModel model)
        {
            var response = await _client.IndexAsync(model.Item, i => i.Index(model.IndexName).Id(model.ElasticId).Refresh(Elasticsearch.Net.Refresh.True));
            return ElasticSearchResult.FromResponse(response);
        }

        public async Task<ElasticSearchResult> UpdateAsync(InsertOrUpdateModel model)
        {
            var response = await _client.UpdateAsync<object>(model.ElasticId, u => u.Index(model.IndexName).Doc(model.Item));
            return ElasticSearchResult.FromResponse(response);
        }

        public async Task<ElasticSearchResult> DeleteAsync(DeleteModel model)
        {
            var response = await _client.DeleteAsync<object>(model.DocumentId, d => d.Index(model.IndexName));
            return ElasticSearchResult.FromResponse(response);
        }


        public async Task<List<ElasticSearchGetModel<T>>> SearchAsync<T>(SearchParameters parameters) where T : class
        {
            var response = await _client.SearchAsync<T>(s => s.Index(parameters.IndexName)

            .From(parameters.From)

            .Size(parameters.Size)

            .Query(q =>
                string.IsNullOrWhiteSpace(parameters.Keyword)
                ? q.MatchAll()
                : q.Bool(b => b
                .Should(
                    sh => sh.MultiMatch(m => m
                    .Fields(f => f.Field("*"))
                    .Query(parameters.Keyword)
                    .Fuzziness(Fuzziness.Auto)
                    ),
                sh => sh.Prefix(p => p
                .Field("*")
                .Value(parameters.Keyword.ToLower())
                )

                ).MinimumShouldMatch(1)

                )
            )

        );

            return response.Hits.Select(hit => new ElasticSearchGetModel<T>(hit.Id, hit.Source)).ToList();

        }



        public async Task<T?> GetAsync<T>(string id, string indexName) where T : class
        {
            var response = await _client.GetAsync<T>(id, g => g.Index(indexName));

            if (!response.Found)
                return null;
            return response.Source;
        }

    }
}
