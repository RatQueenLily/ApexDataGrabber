using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ApexDataGrabber
{
    class PlayerData
    {

        // general player data
        public string playerName;
        public int level;
        public bool isBanned;

        // ranked game data
        public double rankScore;
        public string rankName;
        public int rankDivision;
        public string rankImg;

        // assigning parsed data from API
        public PlayerData(string playerData)
        {
            JObject data = JObject.Parse(playerData);

            playerName = (string)data["global"]["name"];
            level = (int)data["global"]["level"];
            isBanned = (bool)data["global"]["bans"]["isActive"];

            rankScore = (double)data["global"]["rank"]["rankScore"];
            rankName = (string)data["global"]["rank"]["rankName"];
            rankDivision = (int)data["global"]["rank"]["rankDiv"];
            rankImg = (string)data["global"]["rank"]["rankImg"];

        }


    }
}
