using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace APX.DataClasses
{
    public class Join
    {
        public enum ProcessingState { Unprocessed,ToBeBanned,AlreadyBanned }
        public SocketGuildUser Author;
        public ProcessingState processingState = ProcessingState.Unprocessed;
        public Guid guid = Guid.NewGuid();
        public double AccountAgeInDays;
        public bool HasNitro;
        public DateTime UtcTimestamp;
        public Join(SocketGuildUser author, DateTime timestamp)
        {
            Author = author;
            UtcTimestamp = timestamp;
            AccountAgeInDays = (DateTime.UtcNow - author.CreatedAt).TotalDays;
            HasNitro = author.PremiumSince.HasValue;
        }
        public void SetToBeBannedFlag(Guid g)
        {
            guid = g;
            processingState = ProcessingState.ToBeBanned;
        }
    }
}
