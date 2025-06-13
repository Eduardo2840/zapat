using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace zapat.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        
        public async Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
        {
            var url = $"https://open.er-api.com/v6/latest/{baseCurrency}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JSON recibido:");
            Console.WriteLine(json);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("rates", out JsonElement rates) &&
                rates.TryGetProperty(targetCurrency, out JsonElement rateElement))
            {
                return rateElement.GetDecimal();
            }

            throw new KeyNotFoundException($"No se encontró la tasa de cambio para '{targetCurrency}'. JSON: {json}");
        }

        public List<string> GetSupportedCurrencies()
        {
            return new List<string> { "USD", "EUR", "MXN", "GBP", "PEN" }; // Agrega más si deseas
        }
    }
}