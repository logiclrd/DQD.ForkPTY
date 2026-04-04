build: debug

clean:
	rm -rf bin obj

release:
	export CFLAGS=-O3; \
	export CONF=Release; \
	$(MAKE) build__target

debug:
	export CFLAGS=-g; \
	export CONF=Debug; \
	$(MAKE) build__target

build__target: bin/$$CONF/DQD.ForkPTY.Native.Linux.so

SRCS := $(wildcard *.c)
OBJS := $(patsubst %.c, obj/$$CONF/%.o, $(SRCS))

bin/$$CONF/DQD.ForkPTY.Native.Linux.so: exports.map $(OBJS)
	mkdir -p bin/$$CONF
	gcc -shared -o bin/$$CONF/libDQD.ForkPTY.Native.Linux.so -Wl,--version-script,exports.map $(OBJS)
	export RID=$$(./runtime-identifier); \
	export DEPLOYDIR=../NativeBinaries/runtimes/$$RID/native; \
	mkdir -p $$DEPLOYDIR; \
	cp bin/$$CONF/libDQD.ForkPTY.Native.Linux.so $$DEPLOYDIR/

obj/$$CONF/%.o: %.c
	mkdir -p obj/$$CONF
	gcc -o $@ -fPIC $$CFLAGS -c $<

.PHONY: clean release debug
