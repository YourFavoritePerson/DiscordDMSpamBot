using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APX
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private readonly List<string> prefixes = new List<string>();

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;

            prefixes = ParsePrefixList();



        }
        private List<string> ParsePrefixList()
        {
            List<string> prefixes = new List<string>();
            string value ="";
            int count = 0;
            while (value != null)
            {
                var prefix = _config["prefix:"+ count];
                if (prefix == null)
                {
                    break;
                }
                else
                {
                    prefixes.Add(prefix);
                }
                count++;
            }
            if (prefixes.Count == 0)
            {
                Environment.Exit(0);
            }
            return prefixes;
        }
        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null) return;
            //if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands
            if (msg.Author.IsBot) return;
            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            int argPos = 0;     // Check if the message has a valid command prefix
            bool HasPrefix = prefixes.Any(o => msg.HasStringPrefix(o, ref argPos, StringComparison.OrdinalIgnoreCase));
            if (HasPrefix || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
