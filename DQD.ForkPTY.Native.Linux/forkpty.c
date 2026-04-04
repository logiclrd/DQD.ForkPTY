#define _GNU_SOURCE

#include <signal.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <errno.h>
#include <fcntl.h>
#include <pty.h>

#include "control_chars.h"

// Export
int forkpty_exec(int charWidth, int charHeight, int pixelWidth, int pixelHeight, char *fileName, char **argv, int *masterFD)
{
	*masterFD = 0;

	// Fail fast if fileName is obviously non-executable
	if (access(fileName, X_OK) != 0)
		return -1; // Let the caller read errno

	// Set up terminal parameters
	struct termios term;

	memset(&term, 0, sizeof(term));

	term.c_iflag =
		BRKINT |
		IGNPAR |
		ICRNL |
		IXANY |
		IMAXBEL;

	term.c_oflag =
		ONLCR;

	term.c_cflag =
		CS8 |
		CREAD |
		HUPCL |
		CLOCAL;

	term.c_lflag =
		ISIG |
		ECHO |
		ECHOE |
		ECHOK |
		ECHOCTL |
		IEXTEN;

	char standardControlCharacters[] =
		{ INTR, QUIT, ERASE, KILL, EOF_c, TIME, MIN, SWITCH_none, START, STOP, SUSP, EOL_NUL, REPRINT, DISCARD, WERASE, LNEXT, EOL_NUL };

	for (int i=0; i < sizeof(standardControlCharacters); i++)
		term.c_cc[i] = standardControlCharacters[i];

	struct winsize win;

	win.ws_col = charWidth;
	win.ws_row = charHeight;
	win.ws_xpixel = pixelWidth;
	win.ws_ypixel = pixelHeight;

	// The framework has hooked all sorts of signals, and the handlers pass the signals
	// off to a centralized thread using a pipe. So, even though we don't have a copy of
	// that thread here, if a signal is received here, it'll get received by the parent
	// process, causing all sorts of havoc. So, mask all signals for now.
	//
	// NB: Following this line, the code cannot be debugged conventionally, as debugging
	//     depends on the use of SIGTRAP to interrupt execution.
	sigset_t signal_set, old_signal_set;

	sigfillset(&signal_set);
	pthread_sigmask(SIG_SETMASK, &signal_set, &old_signal_set);

	// Fork the child
	int childPID = forkpty(
		masterFD,
		NULL, // "Nobody knows how much space should be reserved for name."
		&term,
		&win);

	if (childPID != 0)
	{
		// We're not the child. Restore the signal mask before returning.
		pthread_sigmask(SIG_SETMASK, &old_signal_set, NULL);

		return childPID;
	}

	// Hooray, we're a new process. Only this thread has been cloned. We need to exec post-haste.

	// Just in case.
	if (*masterFD != 0)
		close(*masterFD);

	// But first, reinitialize signal handler configuration. Replace all custom handlers with SIG_DFL.
	for (int sig = 1; sig < NSIG; ++sig)
	{
		if ((sig == SIGKILL)
		 || (sig == SIGSTOP))
			continue;

		struct sigaction sa_old;

		if (sigaction(sig, NULL, &sa_old) == 0)
		{
			sighandler_t oldhandler = sa_old.sa_handler;

			if ((oldhandler != SIG_IGN)
			 && (oldhandler != SIG_DFL))
			{
				// It has a custom handler, put the default handler back.
				// We check first to preserve flags on default handlers.
				struct sigaction sa_default = {0};

				sa_default.sa_handler = SIG_DFL;

				sigaction(sig, &sa_default,  NULL);
			}
		}
	}

	// Unmask signals.
	pthread_sigmask(SIG_SETMASK, &old_signal_set, NULL);

	// forkpty has already stuffed the PTY slave FD into stdin, stdout and stderr,
	// so all that's left to do is exec.
	execvp(fileName, argv);

	// Can only get here if an error occurred.
	exit(errno);
}
