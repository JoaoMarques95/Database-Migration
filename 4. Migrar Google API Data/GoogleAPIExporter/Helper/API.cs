using Generator;
using Generator.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Generator
{

    public class API
    {

        public static async Task<PlaceCandidates> SearchCadidates(string name, string seachFields, string searchInputType, string apiKey)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            var streamTask = client.GetStreamAsync($"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?fields={seachFields}&input={name}&inputtype={searchInputType}&key={apiKey}");
            return await JsonSerializer.DeserializeAsync<PlaceCandidates>(await streamTask);
        }
        
        public static async Task<PlaceDetails> GetPlaceDetails(string placeId, string detailsFields, string apiKey)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            var streamTask = client.GetStreamAsync($"https://maps.googleapis.com/maps/api/place/details/json?fields={detailsFields}&place_id={placeId}&key={apiKey}");
            return await JsonSerializer.DeserializeAsync<PlaceDetails>(await streamTask);
        }

        public static async Task<string> GetPlaceDetailsString(string placeId, string detailsFields, string apiKey)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            var stringTask = client.GetStringAsync($"https://maps.googleapis.com/maps/api/place/details/json?fields={detailsFields}&place_id={placeId}&key={apiKey}");

            return await stringTask;
        }
    }
}
