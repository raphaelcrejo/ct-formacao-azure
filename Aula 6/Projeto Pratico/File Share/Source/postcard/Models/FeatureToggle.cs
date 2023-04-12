using System;
using Azure;
using Azure.Data.Tables;

namespace postcard.Models
{
	public class FeatureToggle : ITableEntity
    {
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public string ativo { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}

