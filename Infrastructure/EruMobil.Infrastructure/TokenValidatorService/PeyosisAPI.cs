using EruMobil.Application.Features.Devices.Exceptions.AccessTokenExceptions;
using EruMobil.Application.Interfaces.ObisisService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EruMobil.Infrastructure.TokenValidatorService
{
    public class PeyosisAPI:IPeyosisAPI
    {
        private readonly HttpClient httpClient;
        private readonly string apiUrl;

        public PeyosisAPI(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            apiUrl = configuration["PeyosisApiUrl"];
        }
        public async Task<bool> IsStaffTokenValid(string accessToken)
        {
            try
            {
                if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri))
                {
                    Console.WriteLine("Geçersiz URL formatı: " + apiUrl);
                    return false;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"URL hatası: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Genel hata: {ex.Message}");
                return false;
            }
        }

    }
}
