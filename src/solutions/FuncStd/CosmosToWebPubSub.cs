using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FuncStd
{
    public class CosmosToWebPubSub
    {
        private readonly ILogger _logger;

        public CosmosToWebPubSub(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CosmosToWebPubSub>();
        }

        [Function("CosmosToWebPubSub")]
        [WebPubSubOutput(Hub = "%WPS_HUB_NAME%", Connection = "WPS_CONNECTION_STRING")]
        public SendToAllAction? Run(
            [CosmosDBTrigger(
                databaseName: "%COSMOS_DB_DATABASE_NAME%",
                containerName: "%COSMOS_DB_CONTAINER_ID%",
                Connection = "COSMOS_DB_CONNECTION_STRING",
                CreateLeaseContainerIfNotExists = true,
                LeaseContainerName = "leases")
            ] IReadOnlyList<Transcription> input
        )
        {
            if (input != null && input.Count > 0)
            {
                _logger.LogInformation("Document Id: " + input[0].id);

                return new SendToAllAction
                {
                    Data = BinaryData.FromString(JsonSerializer.Serialize(input[0])),
                    DataType = WebPubSubDataType.Json
                };
            }

            return null;
        }
    }
}
