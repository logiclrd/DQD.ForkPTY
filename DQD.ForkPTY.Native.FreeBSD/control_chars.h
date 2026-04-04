#ifndef CONTROL_CHARS_H
#define CONTROL_CHARS_H

typedef enum : char
{
	INTR = 3,
	QUIT = 28,
	ERASE = 127,
	KILL = 21,
	EOF_c = 4,
	TIME = 0,
	MIN = 1,
	SWITCH_none = 0,
	START = 19,
	STOP = 17,
	SUSP = 26,
	EOL_NUL = 0,
	REPRINT = 18,
	DISCARD = 15,
	WERASE = 23,
	LNEXT = 22,
} char_t;

typedef struct
{
	int index;
	char_t ch;
} control_char_mapping_t;

extern const control_char_mapping_t ControlChars[];

#endif // CONTROL_CHARS_H