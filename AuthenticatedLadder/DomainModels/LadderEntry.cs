using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace AuthenticatedLadder.DomainModels
{
    public class LadderEntry
    {
        public string LadderId { get; set; }
        public string Platform { get; set; }
        public string Username { get; set; }
        //This represents seconds of gameplay. The less the better
        public long Score { get; set; }
        [NotMapped]
        public long Position { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(LadderEntry))
                return base.Equals(obj);

            var entry = (LadderEntry)obj;
            return entry.LadderId == LadderId
                   && entry.Platform == Platform
                   && entry.Username == Username
                   && entry.Score == Score;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 42;
                hash = hash * 64 + LadderId.GetHashCode();
                hash = hash * 128 + Platform.GetHashCode();
                hash = hash * 256 + Username.GetHashCode();
                hash = hash * 512 + Score.GetHashCode();
                return hash;
            }
        }
    }
}
