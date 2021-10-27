;macros for SNES


;mesen-s can use wdm is as a breakpoint
;for debugging purposes
.macro WDM_BREAK number
	.byte $42, number
.endmacro



.macro A8
	sep #$20
.endmacro

.macro A16
	rep #$20
.endmacro

.macro AXY8
	sep #$30
.endmacro

.macro AXY16
	rep #$30
.endmacro

.macro XY8
	sep #$10
.endmacro

.macro XY16
	rep #$10
.endmacro



; memcpy, block move
;for WRAM to WRAM data transfers (can't be done with DMA)
.macro BLOCK_MOVE  length, src_addr, dst_addr
;mnv changes the data bank register, need to preserve it
	phb
.if .asize = 8
	rep #$30
.elseif .isize = 8
	rep #$30
.endif
	lda #(length-1)
	ldx #.loword(src_addr)
	ldy #.loword(dst_addr)	
;	mvn src_bank, dst_bank
	.byte $54, ^dst_addr, ^src_addr
	plb
.endmacro
;originally, the ca65 assembler, mvn assembled
;its operands backwards from standard syntax mvn src,dst
;Fixed in July 27, 2018 build of ca65
;I opted to use a .byte directive to make sure no
;errors from different versions of ca65



;first set vram_adr and vram_inc
;decompresses rle files AND copy to vram
.macro UNPACK_TO_VRAM  src_address
.if .asize = 8
	rep #$30
.elseif .isize = 8
	rep #$30
.endif
	lda #.loword(src_address)
	ldx #^src_address
	jsl Unrle
	jsl DMA_VRAM
.endmacro
