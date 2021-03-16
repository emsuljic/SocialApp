namespace API.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        //default page size
        private int _pageSize = 10;

        //get current pageSize, and when we set this property, 
        //take pageSize and if it's gratter than MaxPageSize,
        // set it on MaxPageSize, if not set it to value
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}