@echo off

set name="SNES16"
set name2="SNES16RLE"
set name3="FLIP"

set path=%path%;..\bin\

set CC65_HOME=..\

ca65 main.asm -g
ca65 mainB.asm -g
ca65 mainC.asm -g

ld65 -C lorom256k.cfg -o %name%.sfc main.o -Ln labels.txt
ld65 -C lorom256k.cfg -o %name2%.sfc mainB.o
ld65 -C lorom256k.cfg -o %name3%.sfc mainC.o

pause

del *.o

