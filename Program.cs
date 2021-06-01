using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApexDataGrabber
{

    class Program
    {
    
        static DiscordSocketClient _client;
        static CommandService _commands;
        static CommandHandler _commandHandler =  new CommandHandler(_client, _commands);

        static IServiceProvider _services;

        static APICaller _apiCaller;
        static Modules _modules;

        static void Setup()
        {

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // How much logging do you want to see?
                LogLevel = LogSeverity.Info,

                // If you or another service needs to do anything with messages
                // (eg. checking Reactions, checking the content of edited/deleted messages),
                // you must set the MessageCacheSize. You may adjust the number as needed.
                MessageCacheSize = 50,

                // If your platform doesn't have native WebSockets,
                // add Discord.Net.Providers.WS4Net from NuGet,
                // add the `using` at the top, and uncomment this line:
                //WebSocketProvider = WS4NetProvider.Instance
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                // Again, log level:
                LogLevel = LogSeverity.Info,

                // There's a few more properties you can set,
                // for example, case-insensitive commands.
                CaseSensitiveCommands = false,
            });

            // Subscribe the logging handler to both the client and the CommandService.
            _client.Log += Log;
            _commands.Log += Log;

            // Setup your DI container.
            _services = ConfigureServices();

            //setup the api calling module
            _modules = new Modules();
        }

        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection();
                // Repeat this for all the service classes
                // and other dependencies that your commands might need.
                //.AddSingleton(new SomeServiceClass());

            // When all your required services are in the collection, build the container.
            // Tip: There's an overload taking in a 'validateScopes' bool to make sure
            // you haven't made any mistakes in your dependency graph.
            return map.BuildServiceProvider();
        }



        //// inputting a name, building a request, returning deserialised product
        //static async Task<PlayerData> GetPlayerStatsAsync(string playerName)
        //{
        //    client.BaseAddress = new Uri("https://api.mozambiquehe.re/");
        //    client.DefaultRequestHeaders.Accept.Clear();

        //    string authKey = "iaRXcyiPqDryd2bSZKCj";

        //    // send api request
        //    var data = await client.GetStringAsync("bridge?version=5&platform=PC&player=" + playerName + "&auth=" + authKey);
        //    //var deserializedProduct = JsonConvert.DeserializeObject(data);

        //    PlayerData playerData = new PlayerData(data);

        //    return playerData;

        //}

   // Example of a logging handler. This can be re-used by addons
    // that ask for a Func<LogMessage, Task>.
    private static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();
        
        // If you get an error saying 'CompletedTask' doesn't exist,
        // your project is targeting .NET 4.5.2 or lower. You'll need
        // to adjust your project's target framework to 4.6 or higher
        // (instructions for this are easily Googled).
        // If you *need* to run on .NET 4.5 for compat/other reasons,
        // the alternative is to 'return Task.Delay(0);' instead.
        return Task.CompletedTask;
    }

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            //trigger running when asked

            Setup();

            _client = new DiscordSocketClient();

            _client.Log += Log;

            await InitCommands();

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = "ODQ5MDE4MzEyNTU3NzIzNjQ5.YLVD6w.mXhF3S-_BEJcYuiXmyiAPuHqsKE";

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
    


        private async Task InitCommands()
        {
            // Either search the program and add all Module classes that can be found.
            // Module classes MUST be marked 'public' or they will be ignored.
            // You also need to pass your 'IServiceProvider' instance now,
            // so make sure that's done before you get here.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Subscribe a handler to see if a message invokes a command.
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix('!', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Create a Command Context.
                var context = new SocketCommandContext(_client, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully).
                var result = await _commands.ExecuteAsync(context, pos, _services);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed.
                // This does not catch errors from commands with 'RunMode.Async',
                // subscribe a handler for '_commands.CommandExecuted' to see those.
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }


        //public class InfoModule : ModuleBase<SocketCommandContext>
        //{
        //    // ~say hello world -> hello world
        //    [Command("lookup")]
        //    [Summary("looks up a player's stats")]
        //    public async Task LookupAsync([Remainder][Summary("The player to look up.")] string player)
        //    {
        //        PlayerData data = await GetPlayerStatsAsync(player);

        //        await Context.Channel.SendMessageAsync("Player name: " + data.playerName);
        //        await Context.Channel.SendMessageAsync("Level: " + data.level.ToString());
        //        await Context.Channel.SendMessageAsync("Is Banned?: " + data.isBanned.ToString());
        //        await Context.Channel.SendMessageAsync("Ranked Score " + data.rankScore.ToString());
        //        await Context.Channel.SendMessageAsync("Rank:" + data.rankName + data.rankDivision);
        //        await Context.Channel.SendMessageAsync(data.rankImg);

        //    }

        //    // ReplyAsync is a method on ModuleBase 
        //}

    }
}
