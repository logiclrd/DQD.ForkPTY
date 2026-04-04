#include <termios.h>

#include "control_chars.h"

const control_char_mapping_t ControlChars[] =
	{
		{ VINTR, INTR },
		{ VQUIT, QUIT },
		{ VERASE, ERASE },
		{ VKILL, KILL },
		{ VEOF, EOF_c },
		{ VTIME, TIME },
		{ VMIN, MIN },
		{ VSTART, START },
		{ VSTOP, STOP },
		{ VSUSP, SUSP },
		{ VEOL, 0 /* NUL */ },
		{ VREPRINT, REPRINT },
		{ VDISCARD, DISCARD },
		{ VWERASE, WERASE },
		{ VLNEXT, LNEXT },

		{ 0, 0 },
	};

