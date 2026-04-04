using System.Runtime.InteropServices;

namespace DQD.ForkPTY;

static partial class NativeMethods
{
	public static class FreeBSD
	{
		[DllImport("native/libDQD.ForkPTY.Native.FreeBSD.so", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern int forkpty_exec(
			int charWidth,
			int charHeight,
			int pixelWidth,
			int pixelHeight,
			[In, MarshalAs(UnmanagedType.LPStr)] string fileName,
			[In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string?[] argv_nullTerminated,
			out int masterFD);
	}
}
