using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DontMergeMeYet.Models.Github.Webhooks
{
    public class EventPayloadModelBinder : IModelBinder
    {
        private readonly ILogger<EventPayloadModelBinder> _logger;

        public EventPayloadModelBinder(ILogger<EventPayloadModelBinder> logger)
        {
            _logger = logger;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                // TODO: validate signature

                var payload = await DeserializePayload(bindingContext.HttpContext);
                bindingContext.Result = ModelBindingResult.Success(payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to bind model");
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }

        private async Task<EventPayload> DeserializePayload(HttpContext context)
        {
            var body = await ReadBodyAsJsonAsync(context.Request.Body);
            string eventName = context.Request.Headers["X-GitHub-Event"];
            string deliveryId = context.Request.Headers["X-GitHub-Delivery"];
            _logger.LogInformation($"Received payload of type {eventName} (deliveryId: {deliveryId})");
            EventPayload payload;
            switch (eventName)
            {
                case "pull_request":
                    payload = body.ToObject<PullRequestEventPayload>();
                    break;
                case "installation":
                    payload = body.ToObject<InstallationEventPayload>();
                    break;
                default:
                    return null;
            }

            payload.DeliveryId = deliveryId;
            payload.Event = eventName;
            return payload;
        }

        private static async Task<JObject> ReadBodyAsJsonAsync(Stream body)
        {
            using (var streamReader = new StreamReader(body))
            {
                string json = await streamReader.ReadToEndAsync();
                return JObject.Parse(json);
            }
        }
    }
}