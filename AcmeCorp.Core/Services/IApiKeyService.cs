﻿namespace AcmeCorp.Core.Services
{
    public interface IApiKeyService
    {
        Task<bool> ValidateApiKeyAsync(string apiKey);
    }

}
