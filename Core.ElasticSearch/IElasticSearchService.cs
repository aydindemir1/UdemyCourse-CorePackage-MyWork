using Core.ElasticSearch.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch
{
    public interface IElasticSearchService
    {
        Task<ElasticSearchResult> CreateIndexAsync(string indexName);
        Task<ElasticSearchResult> InsertAsync(InsertOrUpdateModel model);
        Task<ElasticSearchResult> UpdateAsync(InsertOrUpdateModel model);
        Task<ElasticSearchResult> DeleteAsync(DeleteModel model);
        Task<List<ElasticSearchGetModel<T>>> SearchAsync<T>(SearchParameters parameters) where T : class;
        Task<T?> GetAsync<T>(string id, string indexName) where T : class;
    }
}
