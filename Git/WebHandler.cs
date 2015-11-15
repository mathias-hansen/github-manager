using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;
using Microsoft.Framework.Configuration;
using System.IO;

namespace Git.Auth
{
    public class WebHandler
    {
        internal static string RedirectUrl { get; set; }
        internal static string ClientId { get; set; }
        internal static string ClientSecret { get; set; }
        internal static string BasePath { get; set; }
        RequestDelegate _next;

        public WebHandler(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            string response = null;
            var path = context.Request.Path.Value;
            var queryString = context.Request.QueryString.Value;
            var meth = context.Request.Method;
            
            await _next(context);
            
            if (path == RedirectUrl)
            {
                response = await Callback(queryString);
            }
            else if (path == "/repo/create" && meth == "POST") 
            {
                response = CreateRepo(queryString);   
            }
            else if (path == "/repo/remove" && meth == "DELETE") 
            {
                response = RemoveRepo(queryString);
            }
            else if (path == "/webhook")
            {
                Console.WriteLine("UPDATED");
                response = UpdateRepo(queryString);
            }
            else if (path == "/auth")
            {
                context.Response.Redirect(
                    "https://github.com/login/oauth/authorize" +
                    "?scope=user,admin:repo_hook,admin:org_hook" +
                    "&client_id=" + ClientId);
            }
            
            if (response != null) 
            {
                await context.Response.WriteAsync(response);
            }
        }
        public string UpdateRepo(string queryString)
        {
            var repo = new Git.Repo {
                BasePath = BasePath,
                HookId = Convert.ToInt32(ParseAttributeValue("hookId", queryString))  
            };
            
            repo.Pull();
            
            return "repo updated";
        }
        public string CreateRepo(string queryString)
        {
            var rm = ParseAttributeValue("repo", queryString);
            
            Console.WriteLine(rm);
            
            var repo = new Git.Repo {
                BasePath = BasePath,
                RepoName = rm,
                HookId = Convert.ToInt32(ParseAttributeValue("hookId", queryString)),
                Branch = ParseAttributeValue("branch", queryString)
            };
            
            Console.WriteLine(repo.RepoName);
            
            repo.Create();
            
            return "repo created";
        }
        public string RemoveRepo(string queryString)
        {
            var repo = new Git.Repo {
                BasePath = BasePath,
                HookId = Convert.ToInt32(ParseAttributeValue("hookId", queryString))
            };
            
            repo.Remove();
            
            return "repo removed";
        }
        public async Task<string> Callback(string queryString)
        {
            var code = ParseAttributeValue("code", queryString);
                
            var accessToken = await new Api(ClientId, ClientSecret).AuthUser(code);
                
            return "<script>" +
                "sessionStorage.setItem('GITHUBACCESSTOKEN', '" + accessToken + "');" +
                "window.location.href='index.html';" +
            "</script>";
        }
        public string ParseAttributeValue(string parameterName, string queryString)
        {
            return Regex.Replace(
                Regex.Match(queryString, parameterName + @"=.+?(?=&|$)", RegexOptions.IgnoreCase).Value, 
                parameterName + "=", "", RegexOptions.IgnoreCase);
        }
    }
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseGitApi(this IApplicationBuilder app, IConfigurationRoot config)
        {
            WebHandler.RedirectUrl = config.GetSection("redirect_url").Value;
            WebHandler.ClientId = config.GetSection("client_id").Value;
            WebHandler.ClientSecret = config.GetSection("client_secret").Value;
            WebHandler.BasePath = config.GetSection("base_path").Value;
            
            return app.UseMiddleware<WebHandler>();
        }
    }
}
