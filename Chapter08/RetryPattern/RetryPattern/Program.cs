using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RetryPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sends a request to a cloud service
            SendRequest();
        }

        static async void SendRequest()
        {
            HttpClient httpClient = new HttpClient();
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);
            await RetryPattern.RetryOnExceptionAsync<HttpRequestException>
                (maxRetryAttempts, pauseBetweenFailures, async () =>
                {
                    var response = await httpClient.GetAsync(
            "https://mycloudservice.com/api/items/1");
                    response.EnsureSuccessStatusCode();
                });
        }
    }
}
