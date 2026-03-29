using System.Runtime.InteropServices;

namespace DQD.ForkPTY;

static partial class NativeMethods
{
	[DllImport("c", CallingConvention = CallingConvention.Cdecl)]
	public static extern int waitpid(int pid, out int wstatus, WaitFlags flags);
}
