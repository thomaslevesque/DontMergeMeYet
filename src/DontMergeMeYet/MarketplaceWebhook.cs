using DontMergeMeYet.Extensions;
using DontMergeMeYet.Services.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DontMergeMeYet
{
    public class MarketplaceWebhook
    {
        private readonly IGithubPayloadValidator _payloadValidator;

        public MarketplaceWebhook(IGithubPayloadValidator payloadValidator)
        {
            _payloadValidator = payloadValidator;
        }

        [FunctionName(nameof(MarketplaceWebhook))]
        public async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage request,
            ILogger logger)
        {
            string eventName = request.Headers.GetValueOrDefault("X-GitHub-Event");
            string deliveryId = request.Headers.GetValueOrDefault("X-GitHub-Delivery");
            string signature = request.Headers.GetValueOrDefault("X-Hub-Signature");

            logger.LogInformation("Webhook delivery: Delivery id = '{DeliveryId}', Event name = '{EventName}'", deliveryId, eventName);

            var payloadBytes = await request.Content.ReadAsByteArrayAsync();
            if (!_payloadValidator.IsPayloadSignatureValid(payloadBytes, signature))
            {
                logger.LogWarning("Invalid signature");
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Invalid signature"));
            }

            return request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
