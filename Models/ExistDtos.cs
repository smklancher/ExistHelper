using System.Data;

namespace ExistHelper.Models
{
        public class AttributeGroup
        {
            public string Name { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public int Priority { get; set; }
        }

        public class Service
        {
            public string Name { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
        }

        public class AttributeValue
        {
            public DateTime? Date { get; set; } 

            public string DateString => Date?.ToString("yyyy-MM-dd") ?? string.Empty;

            /// <summary>
            /// To understand how this is deserialized, see the JsonConverter in Shared/JsonConverter.cs
            /// <see cref="ExistHelper.Shared.JsonConverter"/>
            /// </summary>
            public object? Value { get; set; } = null;

            public string StringValue=>Value?.ToString() ?? string.Empty;
    }

        public class AttributeResult
        {
            public AttributeGroup Group { get; set; } = new();
            public string Template { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public int Priority { get; set; }
            public bool Manual { get; set; }
            public bool Active { get; set; }
            public int ValueType { get; set; }
            public string ValueTypeDescription { get; set; } = string.Empty;
            public Service Service { get; set; } = new();
            public List<AttributeValue> Values { get; set; } = new();
        }

        public class AttributeResponse
        {
            public int Count { get; set; }
            public string Next { get; set; } = string.Empty;
            public string Previous { get; set; } = string.Empty;
            public List<AttributeResult> Results { get; set; } = new();
        }
    }

