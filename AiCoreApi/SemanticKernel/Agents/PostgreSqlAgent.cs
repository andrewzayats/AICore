using System.Text.Json;
using Microsoft.SemanticKernel;
using AiCoreApi.Models.DbModels;
using System.Web;
using AiCoreApi.Common;
using AiCoreApi.Data.Processors;
using Npgsql;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class PostgreSqlAgent : BaseAgent, IPostgreSqlAgent
    {
        private const string DebugMessageSenderName = "PostgreSqlAgent";

        private static class AgentContentParameters
        {
            public const string ConnectionName = "connectionName";
            public const string SqlQuery = "sqlQuery";
        }

        private readonly IConnectionProcessor _connectionProcessor;
        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;

        public PostgreSqlAgent(
            IConnectionProcessor connectionProcessor,
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor,
            ExtendedConfig extendedConfig,
            ILogger<PostgreSqlAgent> logger) : base(requestAccessor, extendedConfig, logger)
        {
            _connectionProcessor = connectionProcessor;
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor; 
        }
        
        public override async Task<string> DoCall(
            AgentModel agent, 
            Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var connectionName = agent.Content[AgentContentParameters.ConnectionName].Value;
            var sqlQuery = ApplyParameters(agent.Content[AgentContentParameters.SqlQuery].Value, parameters);

            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "DoCall Request", sqlQuery);
            var connections = await _connectionProcessor.List();
            var connection = GetConnection(_requestAccessor, _responseAccessor, connections, ConnectionType.PostgreSql, DebugMessageSenderName, connectionName: connectionName);
            var result = ExecuteScript(sqlQuery, connection.Content["connectionString"]);
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "DoCall Response", result);
            return result;
        }

        private string ExecuteScript(string script, string connectionString)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(script, connection);
            using var reader = command.ExecuteReader();
            var tables = new List<List<Dictionary<string, object>>>();
            do
            {
                var rows = new List<Dictionary<string, object>>();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    rows.Add(row);
                }
                if (rows.Count > 0)
                {
                    tables.Add(rows);
                }

            } while (reader.NextResult());
            return JsonSerializer.Serialize(tables);
        }
    }

    public interface IPostgreSqlAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
