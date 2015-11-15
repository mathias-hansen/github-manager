using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Git
{
    public class HttpHelper
    {
        public async Task<string> SendRequest(string url, string accessToken = null, List<KeyValuePair<string, string>> parameters = null) 
        {
            using (var client = new HttpClient()) 
            {   
                client.DefaultRequestHeaders.Add("User-Agent", "github-website-manager");
                                
                if (accessToken != null) 
                {
                    client.DefaultRequestHeaders.Add("Authorization", "token " + accessToken);
                }
                                
                if (parameters == null)
                {
                    return await _GetRequest(url, client); 
                } 
                else
                {
                    var query = new FormUrlEncodedContent(parameters);
                                
                    return await _PostRequest(url, query, client);
                }
            }
        }
        private async Task<string> _GetRequest(string url, HttpClient client)
        {
            using (var response = await client.GetAsync(url))
            {
                using (var content = response.Content)
                {
                    return await content.ReadAsStringAsync();   
                }
            }
        }
        private async Task<string> _PostRequest(string url, FormUrlEncodedContent query, HttpClient client)
        {
            using (var response = await client.PostAsync(url, query))
            {   
                using (var content = response.Content)
                {
                    return await content.ReadAsStringAsync();
                }
            }
        }
    }
}