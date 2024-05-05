using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedditConsole.Model;

namespace RedditConsole.ServiceRepository
{

    public class SubRedditServiceRepo : ISubRedditServiceRepo
    {
        private readonly ILogger<SubRedditServiceRepo> _logger;
        private readonly IConfiguration _config;
        public SubRedditServiceRepo(ILogger<SubRedditServiceRepo> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task GetSubredditApiAsync()
        {
            while (true)
            {
                Console.Clear();
                try
                {
                    
                    using (var _httpClient = new HttpClient())
                    {
                        //Set header info and url
                        string apiUrl = SetHeaderAndUrl(_httpClient);
                        var response = await _httpClient.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode == true)
                        {
                            _logger.LogInformation("Get API call to Subreddit-worldnews is successful.");
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var redditData = JsonSerializer.Deserialize<SubRedditData>(responseBody);
                            if (redditData != null && redditData.data != null && redditData.data.children != null)
                            {
                                CalculateData(redditData);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Get API call to Subreddit-worldnews failed." + response.StatusCode);
                            Console.WriteLine(response.StatusCode);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error fetching data from Reddit: {ex.Message}");
                    _logger.LogError($"Error fetching data from Reddit: {ex.Message}");
                }

                // Wait for 1 minute before making the next request
                await Task.Delay(Int32.Parse(_config["RefreshAPI"] ?? "60000")); 
            }
        }

        private string SetHeaderAndUrl(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            string oauthToken =  _config["OAuthToken"]?? "";
            string escapeDataString = _config["EscapeDataString"] ?? "";
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + oauthToken);
            httpClient.DefaultRequestHeaders.Add("User-Agent", Uri.EscapeDataString(escapeDataString));
            string apiUrl = _config["RedditUri"] ?? "" + _config["RateLimit"] ?? "100";
            return apiUrl;
        }

        private void CalculateData(SubRedditData redditData)
        {

            // Track calculated data
            Dictionary<string, int> userPostCount = new Dictionary<string, int>();
            int maxUpvotes = 0;
            string topPostTitle = "";

            foreach (var child in redditData.data.children)
            {
                // Track users with most posts
                if (!userPostCount.ContainsKey(child.data.author))
                {
                    userPostCount[child.data.author] = 1;
                }
                else
                {
                    userPostCount[child.data.author]++;
                }

                // Track post with most upvotes
                if (child.data.ups > maxUpvotes)
                {
                    maxUpvotes = child.data.ups;
                    topPostTitle = child.data.title;
                }
            }
            //Print data to Console
            
            PostDataToConsole(topPostTitle, maxUpvotes);

            int sequence = 1;
            foreach (var kvp in userPostCount)
            {
                Console.WriteLine($"{sequence}. {kvp.Key}: {kvp.Value} posts");
                sequence++;
            }
        }

        private void PostDataToConsole(string topPostTitle, int maxUpvotes)
        {
            
            Console.WriteLine("World news detail mentioned below:");
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"Posts with most up votes: {topPostTitle}.");
            Console.WriteLine($"Posts with most up votes count:{maxUpvotes}");
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Users with most posts:");
        }
    }
}

