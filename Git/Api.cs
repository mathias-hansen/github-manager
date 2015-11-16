using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Git
{
    public class Api
    {
        string _ClientId { get; }
        string _ClientSecret { get; }
        HttpHelper _HttpHelper { get; }
        public Api(string clientId, string clientSecret) 
        {
            _ClientId = clientId;
            _ClientSecret = clientSecret;
            _HttpHelper = new HttpHelper();
        }
        
        public async Task<string> AuthUser(string code, string redirectUri = "", string state = "") 
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _ClientId),
                new KeyValuePair<string, string>("client_secret", _ClientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("state", state)
            };
            
            var response = await _HttpHelper.SendRequest("https://github.com/login/oauth/access_token", null, parameters);
            
            return Regex.Replace(Regex.Match(response, @"^access_token=\w+").Value, "^access_token=", "");     
        }
    }
}
