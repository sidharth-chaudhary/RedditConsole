using RedditConsole.ServiceRepository;

namespace RedditConsole.Services
{
    public class SubredditService : ISubredditService
    {
        private readonly ISubRedditServiceRepo _serviceRepo;
        
        public SubredditService (ISubRedditServiceRepo serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }
        public async Task GetSubreddit()
        {
            await _serviceRepo.GetSubredditApiAsync();
        }
    }
}
