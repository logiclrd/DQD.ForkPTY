.OBJDIR: ${.CURDIR}

build: debug

clean:
	rm -rf bin obj

release:
	export CFLAGS=-O3; \
	export CONF=Release; \
	export MAKEOBJDIR=obj/$$CONF; \
	mkdir -p $$MAKEOBJDIR; \
	$(MAKE) build__target

debug:
	export CFLAGS=-g; \
	export CONF=Debug; \
	export MAKEOBJDIR=obj/$$CONF; \
	mkdir -p $$MAKEOBJDIR; \
	$(MAKE) build__target

build__target: bin/$(CONF)/DQD.ForkPTY.Native.FreeBSD.so

SRCS != ls *.c
OBJFNS := $(SRCS:.c=.o)
OBJS := $(OBJFNS:S|^|obj/$(CONF)/|)

bin/$(CONF)/DQD.ForkPTY.Native.FreeBSD.so: exports.map $(OBJS)
	mkdir -p bin/$$CONF
	gcc -shared -o bin/$$CONF/libDQD.ForkPTY.Native.FreeBSD.so -Wl,--version-script,exports.map $(OBJS) -lutil
	export RID=$$(./runtime-identifier); \
	export DEPLOYDIR=../NativeBinaries/runtimes/$$RID/native; \
	mkdir -p $$DEPLOYDIR; \
	cp bin/$$CONF/libDQD.ForkPTY.Native.FreeBSD.so $$DEPLOYDIR/

.for src in ${SRCS}

$(src:%.c=obj/$(CONF)/%.o): ${src}
	mkdir -p obj/$$CONF
	gcc -o ${.TARGET} -fPIC $$CFLAGS -c ${src}

.endfor

.SUFFIXES: .o .c

.PHONY: clean release debug
