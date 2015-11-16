using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Api.Controllers
{
    [Route("Github")]
    public class GithubController : Controller
    {   
        public static string ClientId { get; set; }
        public static string ClientSecret { get; set; }
        
        [Route("Auth"), HttpGet]
        public RedirectResult Auth()
        {
            return Redirect(
                "https://github.com/login/oauth/authorize" +
                "?scope=user,admin:repo_hook,admin:org_hook" +
                "&client_id=" + ClientId);
        }
        [Route("Callback"), HttpGet]
        public async Task<RedirectResult> Callback(string code)
        {
            var githubApi = new Git.Api(ClientId, ClientSecret);
            var accessToken = await githubApi.AuthUser(code);

            return Redirect("/store-token.html?token=" + accessToken);
        }
        [Route("Webhook"), HttpPost]
        public void Webhook()
        {
            
        } 
    }
}
