using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ElasticNestClient.WindowsFormsApp.FRM
{
    public struct ESConnections
    {


        public static Dictionary<ESIndexType, ESConnectionSettings> ESConnectionSettings
        {
            get
            {
                try
                {
                    var filename = AppDomain.CurrentDomain.BaseDirectory + "ElasticSearchConnectionSettings.json";

                    File.WriteAllText(filename, JsonConvert.SerializeObject(!File.Exists(filename) ? new KeyValuePair<ESIndexType, ESConnectionSettings>[] {
                        new KeyValuePair<ESIndexType,ESConnectionSettings>(ESIndexType.Document,new ESConnectionSettings{ DefaultIndex="ArianLibrary" }),
                        new KeyValuePair<ESIndexType,ESConnectionSettings>(ESIndexType.FileContent,new ESConnectionSettings{ DefaultIndex="ArianLibrary_FileContent" }),
                        new KeyValuePair<ESIndexType,ESConnectionSettings>(ESIndexType.Registery,new ESConnectionSettings{ DefaultIndex="ArianLibrary_Registery" }),
                        new KeyValuePair<ESIndexType,ESConnectionSettings>(ESIndexType.Transaction,new ESConnectionSettings{ DefaultIndex="ArianLibrary_Transaction" }),

                    }.ToDictionary(k => k.Key, k => k.Value) : JsonConvert.DeserializeObject<Dictionary<ESIndexType, ESConnectionSettings>>(File.ReadAllText(filename))));

                    return JsonConvert.DeserializeObject<Dictionary<ESIndexType, ESConnectionSettings>>(File.ReadAllText(filename));
                }
                catch
                {
                    return ESConnectionSettings;

                }
            }
        }
        public static Dictionary<ESIndexType, ConnectionSettings> ConnectionSettings
        => ESConnectionSettings.ToDictionary(k => k.Key, k => new ConnectionSettings(connectionPool: new SingleNodeConnectionPool(k.Value.Uri),
            sourceSerializer: (builtIn, settings) =>
            {

                return new JsonNetSerializer(builtIn, settings, () =>
                {
                    var serializerSettings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

                    serializerSettings.Converters.Add(new StringEnumConverter());

                    return serializerSettings;
                });
            })
        .DefaultIndex(k.Value.DefaultIndex.ToLower())

        );
        public static string GetQueryJsonString<T>(ElasticClient client, T data)
        {
            var ms = new MemoryStream();
            client.RequestResponseSerializer.Serialize(data, ms);
            return Encoding.UTF8.GetString(ms.GetBuffer());
        }

        public static Dictionary<ESIndexType, ElasticClient> Clients = new Dictionary<ESIndexType, ElasticClient>();
        public static void RegisterClients()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(k => k.DefinedTypes
            .Where(t => t.ImplementedInterfaces.Any(i => i == typeof(IElasticClientDocument)))).ToList();
            types.ForEach(type =>
            {
                var k = (IElasticClientDocument)Activator.CreateInstance(type);
                Clients.TryAdd(k.IndexType, k.Register());
            });
        }




        private static ElasticClient GetClient(ESIndexType type)
        {
            Clients.TryGetValue(type, out ElasticClient client);
            if (client == null)
            {
                RegisterClients();
                return GetClient(type);
            }
            return client;
        }

        public static ElasticClient EditableDocumentClient => GetClient(ESIndexType.Document);
        public static ElasticClient FileContentClient => GetClient(ESIndexType.FileContent);
        public static ElasticClient RegisteryClient => GetClient(ESIndexType.Registery);
        public static ElasticClient TransactionClient => GetClient(ESIndexType.Transaction);
    }
}
