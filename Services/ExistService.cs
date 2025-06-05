using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ExistHelper.Models;
using ExistHelper.Shared;

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

            return SerializationHelper.Deserialize<AttributeResponse>(json);
        }

        public async Task<List<AttributeResult>> GetLatestNonBlankAttributeResultsAsync()
        {
            var response = await GetAttributesWithValuesAsync(limit: 10000, days: 31);

            if (response?.Results == null)
                return new List<AttributeResult>();

            // Dictionary to hold the latest valid AttributeValue for each attribute name
            var latestByName = new Dictionary<string, (AttributeResult Result, AttributeValue Value, DateTime Date)>();

            foreach (var result in response.Results)
            {
                if (result.Values == null || result.Values.Count == 0)
                    continue;

                foreach (var value in result.Values)
                {
                    // Parse date
                    if (!DateTime.TryParse(value.Date, out var date))
                        continue;

                    // Check value is not null, not blank, not zero
                    if (value.Value == null)
                        continue;

                    // Handle string and numeric types
                    bool isBlankOrZero = value.Value switch
                    {
                        string s => string.IsNullOrWhiteSpace(s),
                        int i => i == 0,
                        long l => l == 0,
                        double d => d == 0,
                        float f => f == 0,
                        decimal m => m == 0,
                        _ => false
                    };
                    if (isBlankOrZero)
                        continue;

                    // Only keep the latest valid value for each attribute name
                    if (!latestByName.TryGetValue(result.Name, out var existing) || date > existing.Date)
                    {
                        latestByName[result.Name] = (result, value, date);
                    }
                }
            }

            // Build filtered AttributeResult list, each with only the latest valid value
            var filtered = new List<AttributeResult>();
            foreach (var entry in latestByName.Values)
            {
                var resultCopy = new AttributeResult
                {
                    Group = entry.Result.Group,
                    Template = entry.Result.Template,
                    Name = entry.Result.Name,
                    Label = entry.Result.Label,
                    Priority = entry.Result.Priority,
                    Manual = entry.Result.Manual,
                    Active = entry.Result.Active,
                    ValueType = entry.Result.ValueType,
                    ValueTypeDescription = entry.Result.ValueTypeDescription,
                    Service = entry.Result.Service,
                    Values = new List<AttributeValue> { entry.Value }
                };
                filtered.Add(resultCopy);
            }

            return filtered;
        }
    }
}