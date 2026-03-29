using System.ComponentModel;

using Microsoft.Win32.SafeHandles;

namespace DQD.ForkPTY;

public class ForkResult(int childPID, Stream ptyStream) : IDisposable
{
	public int ChildProcessID = childPID;
	public Stream PTYStream = ptyStream;

	public static ForkResult FromResult(int childPID, int masterFD)
	{
		var masterFDHandle = new SafeFileHandle(masterFD, ownsHandle: true);

		if (childPID < 0)
		{
			masterFDHandle.Dispose();
			throw new Win32Exception();
		}

		Stream ptyStream;

		try
		{
			ptyStream = new FileStream(masterFDHandle, FileAccess.ReadWrite, bufferSize: 1);
		}
		catch
		{
			masterFDHandle.Dispose();
			throw;
		}

		return new ForkResult(childPID, ptyStream);
	}

	public void Dispose()
	{
		PTYStream?.Dispose();
	}
}