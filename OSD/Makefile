HOME_TREE := . 

# TODO: set the prj_name here
# default name is your folder's title
PRJ_NAME := $(notdir $(CURDIR))

# TODO: set the output binary name here
# default name is your folder's tilte
BINARY_NAME := hazys_osd
#$(notdir $(CURDIR))

# enable the following marco in case you want these features
#EXTRA_DEFS = -D_ENABLE_SERIAL_CALLBACK
#EXTRA_DEFS += -D_SERIAL_CUSTOM_RX_ROUTINE_

#includes the common definitions...
#do not remove them unless you know what you are doing
BUILD_ROOT  := build
SOURCE_ROOT := $(HOME_TREE)/src
TOOL_ROOT   := 
#$(HOME_TREE)/bin
OUTPUT_ROOT := $(BUILD_ROOT)
OBJ_ROOT    := $(BUILD_ROOT)/obj
LIB_ROOT    := $(OBJ_ROOT)/lib

REMOVE      := rm -f
MV          := mv -f
MKDIR       := mkdir -p
SED         := sed
FIND        := find
DIRNAME     := dirname

AVR_RUNTIME_LIB  := core.a
AVR_RUNTIME_PATH := $(SOURCE_ROOT)/core
AVR_OBJ_ROOT     := $(OBJ_ROOT)
AVR_OUTPUT_ROOT  := $(OUTPUT_ROOT)
TARGET           := $(AVR_OUTPUT_ROOT)/$(BINARY_NAME)
TARGET_OBJ_ROOT  := $(AVR_OBJ_ROOT)

#################################################################
#The follow are default config, please overide them if necessary.
UPLOAD_PORT := COM3
PORT_RATE := 19200

MCU := atmega16
F_CPU := 16000000

AVRDUDE_PROGRAMMER := stk500v1
BURNING_PROGRAMMER := stk500v2
######################################

AVR_TOOLS_PATH = 
#$(WINAVR_HOME)/bin

FORMAT := ihex

# Debugging format.
# Native formats for AVR-GCC's -g are stabs [default], or dwarf-2.
# AVR (extended) COFF requires stabs, plus an avr-objcopy run.
DEBUG = stabs

OPT = s

# Place -D or -U options here
CDEFS = -DF_CPU=$(F_CPU)UL -D_AVR_ $(EXTRA_DEFS) -DMAVLINK10
CXXDEFS = -DF_CPU=$(F_CPU)UL -D_AVR_ $(EXTRA_DEFS) -DMAVLINK10


# Compiler flag to set the C Standard level.
# c89   - "ANSI" C
# gnu89 - c89 plus GCC extensions
# c99   - ISO C99 standard (not yet fully implemented)
# gnu99 - c99 plus GCC extensions
CSTANDARD = -std=gnu99
CDEBUG = -g$(DEBUG)
CWARN = -Wall -Wstrict-prototypes
CTUNING = -funsigned-char -funsigned-bitfields -fpack-struct -fshort-enums
#CEXTRA = -Wa,-adhlns=$(<:.c=.lst)


CFLAGS = $(CDEBUG) $(CDEFS) $(CINCS) -O$(OPT) $(CWARN) $(CSTANDARD) $(CEXTRA) -ffunction-sections
CXXFLAGS = $(CDEFS) $(CINCS) -O$(OPT)
ASFLAGS = -Wa,-adhlns=$(<:.S=.lst),-gstabs $(CDEFS) $(CINCS)
LDFLAGS = -lm -Wl,--gc-sections -Os

########################################
#Override the default compiler setting

CC = avr-gcc
CXX = avr-g++
OBJCOPY = avr-objcopy
OBJDUMP = avr-objdump
AR  = avr-ar
SIZE = avr-size
NM = avr-nm

# Combine all necessary flags and optional flags.
# Add target processor to flags.

INCLUDE_PATH += -I.

ALL_CFLAGS = -ffunction-sections -fdata-sections -mmcu=$(MCU) $(INCLUDE_PATH) $(CFLAGS)
ALL_CXXFLAGS = -fno-exceptions -ffunction-sections -fdata-sections -mmcu=$(MCU) $(INCLUDE_PATH) $(CXXFLAGS)
ALL_ASFLAGS = -mmcu=$(MCU) $(INCLUDE_PATH) -x assembler-with-cpp $(ASFLAGS)

# Display size of file.
HEXSIZE = $(SIZE) --target=$(FORMAT) $(TARGET).hex
ELFSIZE = $(SIZE)  $(TARGET).elf

# Convert ELF to COFF for use in debugging / simulating in AVR Studio or VMLAB.
COFFCONVERT=$(OBJCOPY) --debugging \
--change-section-address .data-0x800000 \
--change-section-address .bss-0x800000 \
--change-section-address .noinit-0x800000 \
--change-section-address .eeprom-0x810000 

#############################################

# TODO: set the fuse if necessary.
# Fuse setting will be used when using the make fuse command
# These are the default fuse setting for m168 based ArduinoBoard (bootloader enabled)
#  HFUSE=DD
#  LFUSE=FF
#  EFUSE=00
#  LOCKFUSE=00

# TODO: list the C-Source files here
# All of the .c files within this folder will be included by default
CSRC := $(shell find src/ -name "*.c")

# TODO: list the CPP-Source files here
# All of the .cpp files within this folder will be included by default
CXXSRC := $(shell find src/ -name "*.cpp")

# TODO: list the Assmebly-Source files here
# All of the .S files within this folder will be included by default
ASRC := $(shell find src/ -name "*.S")

# TODO: Put the header's path that you want to include
INCLUDE_PATH += -Isrc/FastSerial -Isrc/GCS_MAVLink


all:  build sizebefore sizeafter

burnall: $(TARGET).hex erase fuse burn
	
#include Arduino-Lite building rules.
#include $(HOME_TREE)/scripts/mak_lib.inc

#include standard build logic
#include $(HOME_TREE)/scripts/mak_avr_common.inc

.SUFFIXES:
.SUFFIXES: .o .cpp .s

.PHONY: all make_lib clean addition additionclean make_subs

make_subs:
	@for subdir in $(MAKE_TARGETS) ; do \
	  $(MAKE) -C $$subdir $(MAKECMDGOALS) ; \
	  done

# Define all object files.

_CSRC   = $(filter-out %*.c,$(CSRC))
_CXXSRC = $(filter-out %*.cpp,$(CXXSRC))
_ASRC   = $(filter-out %*.S,$(ASRC))


OBJ += $(patsubst %.c, $(TARGET_OBJ_ROOT)/%.o, $(_CSRC))
OBJ += $(patsubst %.cpp, $(TARGET_OBJ_ROOT)/%.o, $(_CXXSRC))
OBJ += $(patsubst %.S, $(TARGET_OBJ_ROOT)/%.o, $(_ASRC))

OBJ_DUMP = $(patsubst %.c, $(TARGET_OBJ_ROOT)/%.txt, $(_CSRC))
OBJ_DUMP += $(patsubst %.cpp, $(TARGET_OBJ_ROOT)/%.txt, $(_CXXSRC))
OBJ_DUMP += $(patsubst %.S, $(TARGET_OBJ_ROOT)/%.txt, $(_ASRC))

BIN_DUMP = $(TARGET).txt

# Define all listing files.

LST += $(patsubst %.c, $(TARGET_OBJ_ROOT)/%.lst, $(_CSRC))
LST += $(patsubst %.cpp, $(TARGET_OBJ_ROOT)/%.lst, $(_CXXSRC))
LST += $(patsubst %.S, $(TARGET_OBJ_ROOT)/%.lst, $(_ASRC))

DEP_FILE += $(patsubst %.c, $(TARGET_OBJ_ROOT)/%.d, $(_CSRC))
DEP_FILE += $(patsubst %.cpp, $(TARGET_OBJ_ROOT)/%.d, $(_CXXSRC))

build: elf hex 

clean: 
	$(REMOVE) -r $(AVR_OUTPUT_ROOT)/*

elf: $(TARGET).elf
hex: $(TARGET).hex
eep: $(TARGET).eep
lss: $(TARGET).lss 
sym: $(TARGET).sym

# Program the device. 

# Burning command

# BUILD FUSE updating commanding based on the actual configuration
ifdef EFUSE
BURNING_WRITE_FUSE += -U efuse:w:0x$(EFUSE):m
endif

ifdef HFUSE
BURNING_WRITE_FUSE += -U hfuse:w:0x$(HFUSE):m
endif

ifdef LFUSE
BURNING_WRITE_FUSE += -U lfuse:w:0x$(LFUSE):m
endif

ifdef LOCKFUSE
BURNING_WRITE_LOCKFUSE = -U lock:w:0x$(LOCKFUSE):m
endif


# erase the chip and burning the hex file only
burn: $(TARGET).hex
	$(AVRDUDE) $(BURNING_FLAGS) $(BURNING_WRITE_FLASH)

fuse:
	$(AVRDUDE) $(BURNING_FLAGS) $(BURNING_WRITE_FUSE)

lock:
	$(AVRDUDE) $(BURNING_FLAGS) $(BURNING_WRITE_LOCKFUSE)

erase:
	$(AVRDUDE) $(BURNING_FLAGS) $(BURNING_EARSE_CHIP)

# Bootloader command
upload: $(TARGET).hex
	$(AVRDUDE) $(AVRDUDE_FLAGS) $(AVRDUDE_WRITE_FLASH)

usbupload: $(TARGET).hex
	$(HIDDOWNLOADER) $(TARGET).hex

# DEBUGING SUPPORT

# disassembly all the output file (*.o)
dumpobj: $(OBJ_DUMP)
 
# disassembly the elf file
dump: $(BIN_DUMP)

sizebefore:
	@if [ -f $(TARGET).elf ]; then echo; $(ELFSIZE); echo; fi

sizeafter:
	@if [ -f $(TARGET).elf ]; then echo; $(HEXSIZE); echo; fi

coff: $(TARGET).elf
	#$(COFFCONVERT) -O coff-avr $(TARGET).elf $(TARGET).cof


extcoff: $(TARGET).elf
	$(COFFCONVERT) -O coff-ext-avr $(TARGET).elf $(TARGET).cof


.SUFFIXES: .elf .hex .eep .lss .sym

ifneq ($(MAKECMDGOALS), clean)
sinclude $(DEP_FILE)
endif

.elf.hex:
	$(OBJCOPY) -O $(FORMAT) -R .eeprom $< $@

.elf.eep:
	-$(OBJCOPY) -j .eeprom --set-section-flags=.eeprom="alloc,load" \
	--no-change-warnings --change-section-lma .eeprom=0 -O $(FORMAT) $< $@

# Create extended listing file from ELF output file.
.elf.lss:
	$(OBJDUMP) -h -S $< > $@

# Create a symbol table from ELF output file.
.elf.sym:
	$(NM) -n $< > $@

$(TARGET).elf: $(OBJ) $(EXTRA_OBJ)
	$(MKDIR) `$(DIRNAME) $@`
	$(CC) $(ALL_CFLAGS) --cref -o $@ -L. $^ $(LDFLAGS) 

# Compile: create object files from C++ source files.
$(TARGET_OBJ_ROOT)/%.o: %.cpp
	$(MKDIR) `$(DIRNAME) $@`
	$(CXX) -c $(ALL_CXXFLAGS) $< -o $@ 

# Compile: create object files from C source files.
$(TARGET_OBJ_ROOT)/%.o: %.c
	$(MKDIR) `$(DIRNAME) $@`
	$(CC) -c $(ALL_CFLAGS) $< -o $@ 


# Compile: create assembler files from C source files.
$(TARGET_OBJ_ROOT)/%.s: %.c
	$(MKDIR) `$(DIRNAME) $@`
	$(CC) -S $(ALL_CFLAGS) $< -o $@


# Assemble: create object files from assembler source files.
$(TARGET_OBJ_ROOT)/%.o: %.S
	$(MKDIR) `$(DIRNAME) $@`
	$(CC) -c $(ALL_ASFLAGS) $< -o $@


# Automatic dependencies
$(TARGET_OBJ_ROOT)/%.d: %.c
	$(MKDIR) `$(DIRNAME) $@`
	($(CC) -M $(ALL_CFLAGS) $< | $(SED) "s;$(notdir $*).o:;$(TARGET_OBJ_ROOT)/$*.o $(TARGET_OBJ_ROOT)/$*.d:;" ) > $@

$(TARGET_OBJ_ROOT)/%.d: %.cpp
	$(MKDIR) `$(DIRNAME) $@`
	($(CXX) -M $(ALL_CXXFLAGS) $< | $(SED) "s;$(notdir $*).o:;$(TARGET_OBJ_ROOT)/$*.o $(TARGET_OBJ_ROOT)/$*.d:;" ) > $@

# Debugibility Support
$(TARGET_OBJ_ROOT)/%.txt: $(TARGET_OBJ_ROOT)/%.o
	$(OBJDUMP) -d $< > $@

#$(TARGET).txt: $(TARGET).elf lss
#	$(OBJDUMP) -d $< > $@

.PHONY:	all build elf hex eep lss sym program coff extcoff clean applet_files sizebefore sizeafter dump dumpobj
