using System.Diagnostics;

namespace Git
{
	public class ProcessHelper
	{
		public int Id { get; set; }
		public string Command { get; set; }
		public string WorkingDirectory = null;
		public string Arguments = null;
		
		public DataReceivedEventHandler OnOutput;
		public DataReceivedEventHandler OnError;
		
		public void Start()
		{
			var process = new Process();
			
			process.StartInfo.FileName = Command;
			process.StartInfo.Arguments = Arguments;
			process.StartInfo.WorkingDirectory = WorkingDirectory;
			
			process.StartInfo.UseShellExecute = false;
			
			process.OutputDataReceived += OnOutput;
			process.ErrorDataReceived += OnError;
			
			process.Start();
			
			Id = process.Id;
			
			process.WaitForExit();
			process.Dispose();
		}
		
		public void Stop()
		{
			var process = Process.GetProcessById(Id);
			
			process.Kill();
			process.Dispose();
		}
    }
}