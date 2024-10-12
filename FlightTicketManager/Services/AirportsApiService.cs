using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FlightTicketManager.Helpers;

namespace FlightTicketManager.Services
{
    public class AirportsApiService
    {
        private readonly HttpClient _httpClient;

        public AirportsApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response> GetAirportsAsync(string name)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.api-ninjas.com/v1/airports?city={name}"),
                Headers =
                {
                    { "X-Api-Key", "d26b+vkhAFp8CEy10dfgPQ==Un1fFh7Q15kfcDcf" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var countries = JsonConvert.DeserializeObject<List<AirportApi>>(result);
                    return new Response
                    {
                        IsSuccess = true,
                        Results = countries.OrderBy(c => c.Name).ToList()
                    };
                }

                return new Response
                {
                    IsSuccess = false,
                    Message = result
                };
            }
        }
    }
}
