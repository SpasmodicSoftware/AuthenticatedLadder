namespace AuthenticatedLadder.Persistence
{
    public class LadderRepositorySettings
    {
        public int Length { get; set; }

        public bool IsValidConfiguration()
        {
            return Length > 0;
        }
    }
}
