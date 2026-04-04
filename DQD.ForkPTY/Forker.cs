using System.Runtime.InteropServices;

namespace DQD.ForkPTY;

public static class Forker
{
	public static ForkResult ForkPTYAndExec(PTYConfiguration configuration, string fileName, string?[]? argv = null)
	{
		if (argv == null)
			argv = [fileName, null];
		else
		{
			int nullTerminatorIndex = Array.IndexOf(argv, null);

			if (nullTerminatorIndex != argv.Length - 1)
			{
				if (nullTerminatorIndex < 0)
					nullTerminatorIndex = argv.Length;

				// If adding an element, it will be null by default.
				Array.Resize(ref argv, nullTerminatorIndex + 1);
			}
		}

		int childPTY, masterFD;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			childPTY = NativeMethods.Linux.forkpty_exec(
				configuration.CharacterSize.Width,
				configuration.CharacterSize.Height,
				configuration.PixelSize.Width,
				configuration.PixelSize.Height,
				fileName,
				argv,
				out masterFD);
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
		{
			childPTY = NativeMethods.FreeBSD.forkpty_exec(
				configuration.CharacterSize.Width,
				configuration.CharacterSize.Height,
				configuration.PixelSize.Width,
				configuration.PixelSize.Height,
				fileName,
				argv,
				out masterFD);
		}
		else
			throw new PlatformNotSupportedException();

		return ForkResult.FromResult(childPTY, masterFD);
	}
}
