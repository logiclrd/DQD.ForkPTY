.OBJDIR: ${.CURDIR}

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

build__target: bin/$(CONF)/DQD.ForkPTY.Native.FreeBSD.so

bin/$(CONF)/DQD.ForkPTY.Native.FreeBSD.so: obj/$(CONF)/forkpty.o
	mkdir -p bin/$$CONF
	gcc -shared -o bin/$$CONF/libDQD.ForkPTY.Native.FreeBSD.so obj/$$CONF/forkpty.o -lutil
	export RID=$$(./runtime-identifier); \
	export DEPLOYDIR=../NativeBinaries/runtimes/$$RID/native; \
	mkdir -p $$DEPLOYDIR; \
	cp bin/$$CONF/libDQD.ForkPTY.Native.FreeBSD.so $$DEPLOYDIR/

obj/$(CONF)/forkpty.o: forkpty.c
	mkdir -p obj/$$CONF
	gcc -o obj/$$CONF/forkpty.o -fPIC $$CFLAGS -c forkpty.c

.PHONY: clean release debug
