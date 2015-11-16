using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Git;

namespace Api.Controllers
{
    [Route("Repo")]
    public class RepoController : Controller
    {
        public static string BasePath { get; set; }
        [Route("Create"), HttpPost]
        public void Create(string repoName, int hookId, string branch)
        {
            var repo = new Repo {
                BasePath = BasePath,
                RepoName = repoName,
                HookId = hookId,
                Branch = branch
            };
            
            repo.Create();
        }
        [Route("Remove"), HttpDelete]
        public void Remove(int hookId)
        {
            var repo = new Repo {
                BasePath = BasePath,
                HookId = hookId
            };
            
            repo.Remove();
        } 
    }
}
