using Nest;

namespace ElasticNestClient.WindowsFormsApp.FRM
{
    public abstract class ElasticClientDocument<T> where T : class, new()
    {
        public abstract ESIndexType IndexType { get; }

        public virtual ElasticClient Register()
        {
            var settings = ESConnections.ConnectionSettings[IndexType];
            var client = new ElasticClient(settings);
            var indexName = client.ConnectionSettings.DefaultIndex;
            var createdResponse = client.Indices.Create(indexName);

            client.Map<T>(m => m.AutoMap());

            client.Indices.UpdateSettings(indexName, u =>
            u.IndexSettings(i => i
                .Setting("index.max_result_window", 10000000)
                .Setting("index.mapping.total_fields.limit", 5000)
            ));
            return client;
        }
    }
}
