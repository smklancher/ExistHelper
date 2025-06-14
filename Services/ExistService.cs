using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ExistHelper.Models;
using ExistHelper.Shared;
using System.Data;

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
            AttributesWithValuesParameters parameters,
            bool sanitizeNames = false)
        {
            var apiParams = new Dictionary<string, string>();

            if (parameters.Pagination.Page.HasValue) apiParams["page"] = parameters.Pagination.Page.Value.ToString();
            if (parameters.Pagination.Limit.HasValue) apiParams["limit"] = parameters.Pagination.Limit.Value.ToString();
            if (parameters.Days.HasValue) apiParams["days"] = parameters.Days.Value.ToString();
            if (parameters.Pagination.DateMax.HasValue) apiParams["date_max"] = parameters.Pagination.DateMax.Value.ToString("yyyy-MM-dd");
            if (!string.IsNullOrWhiteSpace(parameters.Groups)) apiParams["groups"] = parameters.Groups;
            if (!string.IsNullOrWhiteSpace(parameters.Attributes)) apiParams["attributes"] = parameters.Attributes;
            if (!string.IsNullOrWhiteSpace(parameters.Templates)) apiParams["templates"] = parameters.Templates;
            if (parameters.Manual.HasValue) apiParams["manual"] = parameters.Manual.Value.ToString().ToLower();

            var json = await _apiService.GetApiResultsAsync("https://exist.io/api/2/attributes/with-values/", apiParams);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var result = SerializationHelper.Deserialize<AttributeResponse>(json);

            if (sanitizeNames && result?.Results != null)
            {
                SanitizeNames(result.Results);
            }

            return result;
        }

        private void SanitizeNames(List<AttributeResult> results)
        {
            int counter = 1;
            foreach (var attr in results)
            {
                if (string.IsNullOrEmpty(attr.Template))
                {
                    attr.Name = $"Attribute{counter}";
                    attr.Label = $"Attribute{counter}";
                    counter++;
                }
            }
        }

        public async Task<DataTable> LatestNonBlankAttributeResultsAsync2()
        {
            var param = new AttributesWithValuesParameters
            {
                Pagination = new PaginationParameters { Limit = 10000 },
                Days = 31
            };

            var response = await GetAttributesWithValuesAsync(param);

            var table = new DataTable();
            table.Columns.Add("Group", typeof(string));
            table.Columns.Add("Attribute", typeof(string));
            table.Columns.Add("Service", typeof(string));
            table.Columns.Add("Last Usage Date", typeof(string));
            table.Columns.Add("Days Since", typeof(int));
            table.Columns.Add("Value", typeof(object));

            if (response?.Results == null)
            {
                return table;
            }

            // Dictionary to hold the latest valid AttributeValue for each attribute name
            var latestByName = new Dictionary<string, (AttributeResult Result, AttributeValue Value, DateTime Date)>();

            foreach (var result in response.Results)
            {
                if (result.Values == null || result.Values.Count == 0)
                {
                    continue;
                }

                foreach (var value in result.Values)
                {
                    // Parse date
                    if (!DateTime.TryParse(value.DateString, out var date))
                    {
                        continue;
                    }

                    // Check value is not null, not blank, not zero
                    if (value.Value == null)
                    {
                        continue;
                    }

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
                    {
                        continue;
                    }

                    // Only keep the latest valid value for each attribute name
                    if (!latestByName.TryGetValue(result.Name, out var existing) || date > existing.Date)
                    {
                        latestByName[result.Name] = (result, value, date);
                    }
                }
            }

            // Add rows to DataTable
            foreach (var entry in latestByName.Values)
            {
                var item = entry.Result;
                var value = entry.Value;
                var lastDate = entry.Date;
                int daysSince = (int)(DateTime.UtcNow.Date - lastDate.Date).TotalDays;
                table.Rows.Add(
                    item.Group?.Label ?? "",
                    item.Label,
                    item.Service?.Label ?? "",
                    lastDate == default ? "" : lastDate.ToString("yyyy-MM-dd"),
                    daysSince,
                    value?.Value
                );
            }

            table.DefaultView.Sort="[Last Usage Date] DESC, [Group] ASC, [Attribute] ASC";

            return table.DefaultView.ToTable();
        }

        public async Task<List<AttributeResult>> LatestNonBlankAttributeResultsAsync()
        {
            var param = new AttributesWithValuesParameters
            {
                Pagination = new PaginationParameters { Limit = 10000 },
                Days = 31
            };

            var response = await GetAttributesWithValuesAsync(param);

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
                    if (!DateTime.TryParse(value.DateString, out var date))
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

        public delegate Task<List<AttributeResult>> CustomFunctionDelegate();

        /// <summary>
        /// Maps a CustomFunctions enum value to a method in this class and returns its MethodInfo.
        /// </summary>
        private CustomFunctionDelegate? CustomFunctionByName(CustomFunctions function)
        {
            var name = function switch
            {
                CustomFunctions.DaysSince => nameof(LatestNonBlankAttributeResultsAsync),
                CustomFunctions.None => string.Empty,
                _ => string.Empty,
            };

            var method = typeof(ExistService).GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null || method.ReturnType != typeof(Task<List<AttributeResult>>))
            {
                return null;
            }

            return (CustomFunctionDelegate)Delegate.CreateDelegate(typeof(CustomFunctionDelegate), this, method);
        }
        /// <summary>
        /// Maps a CustomFunctions enum value to a method in this class and returns its MethodInfo.
        /// </summary>
        private MethodInfo? CustomFunctionByName2(CustomFunctions function)
        {
            var name = function switch
            {
                CustomFunctions.DaysSince => nameof(LatestNonBlankAttributeResultsAsync2),
                CustomFunctions.None => string.Empty,
                _ => string.Empty,
            };

            var method = typeof(ExistService).GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method == null || method.ReturnType != typeof(Task<DataTable>))
            {
                return null;
            }

            return method;
        }

        /// <summary>
        /// Calls a method on this service by CustomFunctions enum value.
        /// The method name must match the enum value.
        /// </summary>
        public Task<List<AttributeResult>> CallByEnum(CustomFunctions function)
        {
            var method = CustomFunctionByName(function);
            if (method == null)
            {
                throw new InvalidOperationException($"No method found for function '{function}'.");
            }
            var result = method.Invoke();
            return result;
        }

        /// <summary>
        /// Calls a method on this service by CustomFunctions enum value.
        /// The method name must match the enum value.
        /// </summary>
        public Task<DataTable> CallByEnum2(CustomFunctions function)
        {
            var method = CustomFunctionByName2(function);
            if (method == null)
            {
                throw new InvalidOperationException($"No method found for function '{function}'.");
            }
            var result = method.Invoke(null, []);
            return (Task<DataTable>)result!;
        }
    }
}