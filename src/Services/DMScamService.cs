using Discord;
using Discord.Commands;
using Discord.WebSocket;
using APX.DataClasses;
using APX.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace APX
{
    public class DMScamService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private static List<Join> UserList = new List<Join>();
        private static object PreprocessingLock = new object();
        private readonly RequestOptions requestOptions = RequestOptions.Default;
        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public DMScamService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;
            _discord.UserJoined += OnUserJoin;
            requestOptions.RetryMode = RetryMode.AlwaysRetry;
            Console.WriteLine("DM Scam Service started");
        }

        public async Task OnUserJoin(SocketGuildUser user) 
        {
            Guid ThisThreadsGuid = Guid.NewGuid();

            
            List<Join> Cunts = new List<Join>();
            lock (PreprocessingLock)
            {
                //add this join to the list
                UserList.Add(new Join(user, DateTime.UtcNow));
                //clear older joins below time threshold.
                UserList = UserList.Where(o => o.UtcTimestamp > DateTime.UtcNow - TimeSpan.FromSeconds(180)).ToList();
                int NumJoinsToBan = Convert.ToInt32(_config["Guilds:" + user.Guild.Id + ":NumberOfJoinsToAutoBan"]);
                if (UserList.Count >= NumJoinsToBan)
                {
                    //only foreach the people who are not already being processed or have been processed already.    
                    var UsersToCheck = UserList.Where(o => o.processingState == Join.ProcessingState.Unprocessed).ToList();
                    foreach (var userToCheck in UsersToCheck)
                    {
                        if (!userToCheck.HasNitro && userToCheck.AccountAgeInDays < 10)
                        {
                            //userToCheck.processingState = Join.ProcessingState.ToBeBanned;
                            userToCheck.SetToBeBannedFlag(ThisThreadsGuid);
                        }
                    }
                    //if their state is to be banned and the guid matches the guild for this thread, stick them on the ban list.
                    Cunts = UserList.Where(o => o.processingState == Join.ProcessingState.ToBeBanned && o.guid==ThisThreadsGuid).ToList();
                }
            }
            //now find the ones marked to be banned in this thread and do it, if any.
            if (Cunts.Count() > 0)
            {
                foreach (var scammer in Cunts)
                {
                    await scammer.Author.BanAsync(0, "Likely DM Scammer", requestOptions);
                    ulong LogChannel = Convert.ToUInt64(_config["Guilds:" + scammer.Author.Guild.Id + ":LogChannel"]);
                    LogMessageToDiscord(scammer.Author.Guild.Id, LogChannel,
                                        "Suspected spammer banned - " + scammer.Author.Mention + Environment.NewLine +
                                        "Account Age - " + scammer.AccountAgeInDays + Environment.NewLine +
                                        "Nitro? - " + scammer.HasNitro);
                    scammer.processingState = Join.ProcessingState.AlreadyBanned;
                }
            }
        }

        private async void LogMessageToDiscord(ulong GuildId, ulong ChannelId, string message)
        {
            var loggingChannel = _discord.GetGuild(GuildId).GetTextChannel(ChannelId);
            using (loggingChannel.EnterTypingState())
            {
                await loggingChannel.SendMessageAsync(message, false, null, requestOptions);
            }
        }
    }
}
