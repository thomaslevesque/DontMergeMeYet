﻿using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    internal interface IPullRequestHandler
    {
        Task HandleWebhookEventAsync(PullRequestEventContext context);
    }
}