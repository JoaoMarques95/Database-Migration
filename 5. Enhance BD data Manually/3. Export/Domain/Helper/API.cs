using Domain;
using Domain.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Domain.Helper
{

    public class API
    {

        public static async Task<PlaceCandidates> SearchCadidates(string name)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            var streamTask = client.GetStreamAsync($"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?fields={GenericProperties.seachFields}&input={name}&inputtype={GenericProperties.searchInputType}&key={GenericProperties.apiKey}");
            return await JsonSerializer.DeserializeAsync<PlaceCandidates>(await streamTask);
        }
        
        public static async Task<PlaceDetails> GetPlaceDetails(string placeId)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            var streamTask = client.GetStreamAsync($"https://maps.googleapis.com/maps/api/place/details/json?fields={GenericProperties.detailsFields}&place_id={placeId}&key={GenericProperties.apiKey}");
            return await JsonSerializer.DeserializeAsync<PlaceDetails>(await streamTask);
        }

        public static async Task<string> GetPlaceDetailsString(string placeId)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            // Pass the handler to httpclient(from you are calling api)
            HttpClient client = new HttpClient(clientHandler);

            var stringTask = client.GetStringAsync($"https://maps.googleapis.com/maps/api/place/details/json?fields={GenericProperties.detailsFields}&place_id={placeId}&key={GenericProperties.apiKey}");

            return await stringTask;
        }
    }
}
