using System.Runtime.Versioning;

using DQD.ForkPTY;

[UnsupportedOSPlatform("Windows")]
class Program
{
	static string FindShell()
	{
		string[] candidates = [ "bash", "csh", "zsh", "ash" ];
		string[] possiblePaths = [ "/usr/bin", "/usr/sbin", "/bin" ];

		const UnixFileMode AnyExecute =
			UnixFileMode.UserExecute |
			UnixFileMode.GroupExecute |
			UnixFileMode.OtherExecute;

		foreach (string candidate in candidates)
		{
			foreach (var container in possiblePaths)
			{
				string path = Path.Combine(container, candidate);

				if (File.Exists(path)
				 && ((File.GetUnixFileMode(path) & AnyExecute) != 0))
					return path;
			}
		}

		Console.Error.WriteLine("Failed to locate a shell to run");
		throw new NotSupportedException();
	}

	static void Main()
	{
		string fileName = FindShell();

		Console.WriteLine("Shell: {0}", fileName);

		int charWidth = Console.BufferWidth;
		int charHeight = Console.BufferHeight;

		if (charWidth < 1)
			charWidth = 80;
		if (charHeight < 1)
			charHeight = 25;

		int glyphWidth = 8;
		int glyphHeight = 16;

		Console.WriteLine("Console size: {0}x{1}", charWidth, charHeight);
		Console.WriteLine("Console pixel size: {0}x{1}", charWidth * glyphWidth, charHeight * glyphHeight);

		ForkResult result;

		try
		{
			Console.WriteLine("About to call ForkPTYAndExec");
			Console.Out.Flush();

			result = Forker.ForkPTYAndExec(
				new PTYConfiguration()
				{
					CharacterSize = (charWidth, charHeight),
					PixelSize = (charWidth * glyphWidth, charHeight * glyphHeight),
				},
				fileName);

			Console.WriteLine("ForkPTYAndExec returned, child is {0}", result.ChildProcessID);
			Console.Out.Flush();
		}
		catch (Exception e)
		{
			Console.WriteLine("FAILED: " + e);
			return;
		}

		using (result)
		{
			var receiveTask = Task.Run(() => result.PTYStream.CopyTo(Console.OpenStandardOutput()));
			var sendTask = Task.Run(() => Console.OpenStandardInput().CopyTo(result.PTYStream));

			Console.WriteLine("Waiting for process exit");

			result.ProcessExit.WaitOne();

			Console.WriteLine("Process exited, waiting on I/O tasks");

			try
			{
				Task.WaitAny(receiveTask, sendTask);
			}
			catch (AggregateException e)
			{
				string plural = e.InnerExceptions.Count == 1 ? "exception" : "exceptions";

				Console.WriteLine("Exiting on {0}:", plural);

				for (int i=0; i < e.InnerExceptions.Count; i++)
					Console.WriteLine("[{0}]: {1}", i, e.InnerExceptions[i]);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exiting on exception: {0}", e);
			}
		}
	}
}
