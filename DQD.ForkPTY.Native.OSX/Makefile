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

ifneq ($(CONF),)

build__target: bin/$(CONF)/DQD.ForkPTY.Native.OSX.dylib

SRCS := $(wildcard *.c)
OBJS := $(patsubst %.c, obj/$(CONF)/%.o, $(SRCS))
UNITS := $(patsubst %.c, %, $(SRCS))

bin/$(CONF)/DQD.ForkPTY.Native.OSX.dylib: exports.list ensure__dirs $(OBJS)
	gcc -shared -dynamiclib -o bin/$(CONF)/libDQD.ForkPTY.Native.OSX.dylib -Wl,-exported_symbols_list,exports.list $(OBJS) -lutil
	export RID=$$(./runtime-identifier); \
	export DEPLOYDIR=../NativeBinaries/runtimes/$$RID/native; \
	mkdir -p $$DEPLOYDIR; \
	cp bin/$(CONF)/libDQD.ForkPTY.Native.OSX.dylib $$DEPLOYDIR/

ensure__dirs:
	mkdir -p obj/$(CONF) bin/$(CONF)

define BUILDRULE

obj/$(2)/$(1).o: $(1).c
	gcc -o obj/$(2)/$(1).o -fPIC $(CFLAGS) -c $(1).c 

endef

$(foreach unit,$(UNITS),$(eval $(call BUILDRULE,$(unit),$(CONF))))

endif

.PHONY: clean release debug
