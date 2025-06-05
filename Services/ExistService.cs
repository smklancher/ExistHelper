using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ExistHelper.Models;

namespace ExistHelper.Services
{
    public class ExistService
    {
        private readonly ApiService _apiService;

        public ExistService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<AttributeResponse?> GetAttributesWithValuesAsync(
            int? page = null,
            int? limit = null,
            int? days = null,
            string? date_max = null,
            string? groups = null,
            string? attributes = null,
            string? templates = null,
            bool? manual = null)
        {
            var parameters = new Dictionary<string, string>();

            if (page.HasValue) parameters["page"] = page.Value.ToString();
            if (limit.HasValue) parameters["limit"] = limit.Value.ToString();
            if (days.HasValue) parameters["days"] = days.Value.ToString();
            if (!string.IsNullOrWhiteSpace(date_max)) parameters["date_max"] = date_max;
            if (!string.IsNullOrWhiteSpace(groups)) parameters["groups"] = groups;
            if (!string.IsNullOrWhiteSpace(attributes)) parameters["attributes"] = attributes;
            if (!string.IsNullOrWhiteSpace(templates)) parameters["templates"] = templates;
            if (manual.HasValue) parameters["manual"] = manual.Value.ToString().ToLower();

            var json = await _apiService.GetApiResultsAsync("https://exist.io/api/2/attributes/with-values/", parameters);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<AttributeResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}