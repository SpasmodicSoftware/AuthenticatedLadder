namespace AuthenticatedLadder.Middlewares.JWTPayload
{
    public class JWTPayloadMiddlewareSettings
    {
        public string HeaderName { get; set; }
        public string DecodeSecret { get; set; }

        public bool IsValidConfiguration()
        {
            return !string.IsNullOrEmpty(HeaderName)
                   && !string.IsNullOrEmpty(DecodeSecret);
        }
    }
}
