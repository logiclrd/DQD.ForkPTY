using System.Runtime.Versioning;

using DQD.ForkPTY;

[UnsupportedOSPlatform("Windows")]
class Program
{
	static void Main()
	{
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
				"/usr/bin/bash");

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