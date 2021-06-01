using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApexDataGrabber
{
    public class APICaller
    {
        static HttpClient client;

        public APICaller()
        {
            setup();
        }

        //setting up the call to the Apex Legends API
        void setup()
        {
            client = new HttpClient();

            client.BaseAddress = new Uri("https://api.mozambiquehe.re/");
            client.DefaultRequestHeaders.Accept.Clear();
        }


        // inputting a name, building a request, returning deserialised product
        public async Task<PlayerData> GetPlayerStatsAsync(string playerName)
        {

            string authKey = "iaRXcyiPqDryd2bSZKCj";

            // send api request
            var data = await client.GetStringAsync("bridge?version=5&platform=PC&player=" + playerName + "&auth=" + authKey);
            //var deserializedProduct = JsonConvert.DeserializeObject(data);

            PlayerData playerData = new PlayerData(data);

            return playerData;

        }
    }
}
