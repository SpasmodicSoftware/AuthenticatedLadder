using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
