namespace AcmeCorp.Core.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly List<string> _validApiKeys;

        public ApiKeyService()
        {

            _validApiKeys = new List<string>
        {
            "customerkey1",
            "customerinfokey1",
            "orderskey1"
        };
        }

        public Task<bool> ValidateApiKeyAsync(string apiKey)
        {
            bool isValidApiKey = _validApiKeys.Contains(apiKey);
            return Task.FromResult(isValidApiKey);
        }
    }
}
