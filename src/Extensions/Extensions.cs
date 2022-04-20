using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace APX.Extensions
{
    public static class Extensions
    {
        public static string GetAuthorDetails(this SocketGuildUser su)
        {
            string alias = su.Username + "#" + su.Discriminator;
            if (su.Nickname != null && su.Nickname.Length > 0)
                alias += " aka " + su.Nickname;
            alias += " (" + su.Id + ")";
            return alias;
        }
    }
}
