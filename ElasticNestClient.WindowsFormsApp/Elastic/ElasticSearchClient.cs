using Elasticsearch.Net;
using Nest;
using System;

namespace ElasticNestClient.WindowsFormsApp
{
    public class ElasticSearchClient
    {
        #region Elasticsearch Connection

        public ElasticClient EsClient()
        {
            var nodes = new Uri[]
            {
                    new Uri("http://localhost:9200/"),
            };

            var connectionPool = new StaticConnectionPool(nodes);
            var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming();
            var elasticClient = new ElasticClient(connectionSettings.DefaultIndex("productdetails"));
            return elasticClient;
        }

        #endregion Elasticsearch Connection
    }

}


