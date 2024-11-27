namespace RG35XX.Libraries
{
    public class DeviceInfo
    {
        public string GetArchitecture()
        {
            //TODO: Implement Me
            return "ARM-64";
        }

        public async Task<bool> IsInternetConnected()
        {
            try
            {
                // Create handler that ignores SSL validation
                using HttpClientHandler handler = new()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                // Create and configure client
                using HttpClient client = new(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                // Use HEAD request to minimize data transfer
                using HttpRequestMessage request = new(HttpMethod.Head, "http://google.com");
                using HttpResponseMessage response = await client.SendAsync(request);

                if (response.Headers.Date.HasValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}