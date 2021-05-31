using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ApexDataGrabber
{
    
    //public class Product
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public decimal Price { get; set; }
    //    public string Category { get; set; }
    //}
    //#endregion

    class Program
    {
    
        static HttpClient client = new HttpClient();



        // inputting a name, building a request, returning deserialised product
        static async Task<PlayerData> GetPlayerStatsAsync(string playerName)
        {
            string authKey = "iaRXcyiPqDryd2bSZKCj";

            // send api request
            var data = await client.GetStringAsync("bridge?version=5&platform=PC&player=" + playerName + "&auth=" + authKey);
            //var deserializedProduct = JsonConvert.DeserializeObject(data);

            PlayerData playerData = new PlayerData(data);

            return playerData;

        }




        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }


        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://api.mozambiquehe.re/");
            client.DefaultRequestHeaders.Accept.Clear();


            try
            {

                PlayerData data = await GetPlayerStatsAsync("RatQueenLily");
                



                Console.WriteLine(data);


            }
            catch (Exception e)
            {
                // error message here
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

    }
}
