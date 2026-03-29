using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Win32.SafeHandles;

namespace DQD.ForkPTY;

[UnsupportedOSPlatform(PlatformNames.Windows)]
public class ForkResult(int childPID, Stream ptyStream, WaitHandle processExit) : IDisposable
{
	public int ChildProcessID = childPID;
	public Stream PTYStream = ptyStream;
	public WaitHandle ProcessExit => processExit;

	public static ForkResult FromResult(int childPID, int masterFD)
	{
		var masterFDHandle = new SafeFileHandle(masterFD, ownsHandle: true);

		if (childPID < 0)
		{
			masterFDHandle.Dispose();
			throw new Win32Exception();
		}

		var processExit = ReapProcessOnChildSignal(childPID);

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

		return new ForkResult(childPID, ptyStream, processExit);
	}

	static WaitHandle ReapProcessOnChildSignal(int childPID)
	{
		PosixSignalRegistration? signalRegistration = null;
		ManualResetEvent processExit = new ManualResetEvent(initialState: false);

		void ReapChild(PosixSignalContext signalContext)
		{
			int result = NativeMethods.waitpid(childPID, out var status, WaitFlags.WNOHANG);

			if (result > 0)
			{
				signalRegistration?.Dispose();
				processExit.Set();
			}
		}

		signalRegistration = PosixSignalRegistration.Create(PosixSignal.SIGCHLD, ReapChild);

		return processExit;
	}

	public void Dispose()
	{
		PTYStream?.Dispose();
	}
}