namespace ExistHelper.Models
{
    public class ParameterDtos
    {
    }

    public class PaginationParameters
    {
        /// <summary>Number of values to return per page. Optional, max is 100.</summary>
        public int? Limit { get; set; }
        /// <summary>Page index. Optional, default is 1.</summary>
        public int? Page { get; set; }
        /// <summary>Most recent date (inclusive) of results to be returned, in format YYYY-mm-dd. Optional.</summary>
        public DateTime? DateMax { get; set; }
    }

    public class AttributesWithValuesParameters
    {
        public PaginationParameters Pagination { get; set; } = new();
        /// <summary>Integer defining how many day values to include in values, max 31, default 1</summary>
        public int? Days { get; set; } = 1;
        /// <summary>Comma-separated list of groups to filter by, e.g. activity,workouts</summary>
        public string Groups { get; set; } = string.Empty;
        /// <summary>Comma-separated list of attributes to filter by</summary>
        public string Attributes { get; set; } = string.Empty;
        /// <summary>Comma-separated list of attribute templates to filter by</summary>
        public string Templates { get; set; } = string.Empty;
        /// <summary>Boolean flag, set to true to only show manual attributes</summary>
        public bool? Manual { get; set; }
    }
}
