namespace API.Helpers
{
    public class MessageParams: PaginationParams
    {
        //currently logged in user
        public string Username { get; set; }
        //return by default unread messages to the user
        public string Container { get; set; } = "Unread";
    }
}