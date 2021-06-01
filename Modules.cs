using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexDataGrabber
{
    public class Modules
    {
        public Modules()
        {
            setup();
        }

        public static APICaller _apiCaller;

        //setting up the call to the Apex Legends API
        void setup()
        {

            // setting up the api calling module
            _apiCaller = new APICaller();
        }



        public class InfoModule : ModuleBase<SocketCommandContext>
        {
            // ~say hello world -> hello world
            [Command("lookup")]
            [Summary("looks up a player's stats")]
            public async Task LookupAsync([Remainder][Summary("The player to look up.")] string player)
            {

                // calling API
                PlayerData data = await _apiCaller.GetPlayerStatsAsync(player);

                await Context.Channel.SendMessageAsync("Player name: " + data.playerName);
                await Context.Channel.SendMessageAsync("Level: " + data.level.ToString());
                await Context.Channel.SendMessageAsync("Is Banned?: " + data.isBanned.ToString());
                await Context.Channel.SendMessageAsync("Ranked Score " + data.rankScore.ToString());
                await Context.Channel.SendMessageAsync("Rank:" + data.rankName + data.rankDivision);
                await Context.Channel.SendMessageAsync(data.rankImg);

            }

            // ReplyAsync is a method on ModuleBase 
        }

    }

   
}
