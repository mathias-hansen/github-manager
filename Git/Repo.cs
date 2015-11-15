using System;
using System.IO;
using System.Text;

namespace Git
{
	public class Repo
	{
		public string Owner { get; set; }
		public string RepoName { get; set; }
		public int HookId { get; set; }
		public string Branch { get; set; }
		public string BasePath { get; set; }
		public void Create() 
		{
			if (!Directory.Exists($"{BasePath}/{HookId}")) 
			{
				var repoDataPath = $"{BasePath}/{HookId}/branch.dat";
				
				Clone();

				//File.WriteAllText(repoDataPath, Branch, Encoding.ASCII);	
			}
			else
			{
				throw new Exception("repo already exists");
			}
		}
		public void Remove()
		{
			if (Directory.Exists($"{BasePath}/{HookId}")) 
			{
				Directory.Delete($"{BasePath}/{HookId}", true);
			}
		}
		public void Clone()
		{
			var ph = new ProcessHelper {
				Command = "git",
				Arguments = $"clone git@github.com:{Owner}/{RepoName}.git {HookId}",
				WorkingDirectory = BasePath
			};
			
			ph.OnOutput = (sender, e) => {
				Console.WriteLine("[clone]", e.Data);
			};
			
			ph.OnError = (sender, e) => {
				ph.Stop();
				
				throw new Exception("error " + e.Data);
			};
			
			ph.Start();
		}
		public void Pull()
		{
			var ph = new ProcessHelper {
				Command = "git",
				Arguments = "pull",
				WorkingDirectory = $"{BasePath}/{HookId}"
			};
			
			ph.OnError = (sender, e) => {
				ph.Stop();
				
				throw new Exception("error " + e.Data);
			};
			
			ph.Start();
		}
	}
}