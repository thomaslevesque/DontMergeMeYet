using Microsoft.AspNetCore.Mvc;

namespace DontMergeMeYet.Models.Github.Webhooks
{
    [ModelBinder(BinderType = typeof(EventPayloadModelBinder))]
    public class EventPayload
    {
        public string DeliveryId { get; set; }
        public string Event { get; set; }
    }
}