using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace APX.Modules
{
    [Name("Commands")]
    public class CommandsModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private static readonly Dictionary<ulong, string> AprilFoolsDict = new Dictionary<ulong, string>();
        static CommandsModule()
        {
            
        }
        public CommandsModule(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;
           
        }

        [Command("ping"), Alias("p")]
        [Summary("check if its alive")]
        [RequireContext(ContextType.Guild)]
        public Task Say()
            => ReplyAsync("Whoop Whoop Whoop Whoop Whoop Whoop Whoop Whoop");


       
        
    }
}
