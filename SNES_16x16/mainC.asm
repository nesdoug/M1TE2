; SNES 16x16 tiles testing how it flips 16x16 tiles.

.p816
.smart

.include "regs.asm"
.include "variables.asm"
.include "macros.asm"
.include "init.asm"





.segment "CODE"

; enters here in forced blank
Main:
.a16 ; the setting from init code
.i16
	phk
	plb
	

	
; DMA from BG_Palette to CGRAM
	A8
	stz CGADD ; $2121 cgram address
	
	stz $4300 ; transfer mode 0 = 1 register write once
	lda #$22  ; $2122
	sta $4301 ; destination, cgram data
	ldx #.loword(BG_Palette)
	stx $4302 ; source
	lda #^BG_Palette
	sta $4304 ; bank
	ldx #32
	stx $4305 ; length
	lda #1
	sta MDMAEN ; $420b start dma, channel 0
	
	
; DMA from Tiles to VRAM	
	lda #V_INC_1 ; the value $80
	sta VMAIN  ; $2115 = set the increment mode +1
	ldx #$0000
	stx VMADDL ; $2116 set an address in the vram of $0000
	
	lda #1
	sta $4300 ; transfer mode, 2 registers 1 write
			  ; $2118 and $2119 are a pair Low/High
	lda #$18  ; $2118
	sta $4301 ; destination, vram data
	ldx #.loword(Tiles)
	stx $4302 ; source
	lda #^Tiles
	sta $4304 ; bank
	ldx #(End_Tiles-Tiles)
	stx $4305 ; length
	lda #1
	sta MDMAEN ; $420b start dma, channel 0
    
    
    
    ldx #$2000
	stx VMADDL ; $2116 set an address in the vram of $2000
	
;	lda #1
;	sta $4300 ; transfer mode, 2 registers 1 write
;			  ; $2118 and $2119 are a pair Low/High
;	lda #$18  ; $2118
;	sta $4301 ; destination, vram data
	ldx #.loword(Tiles2)
	stx $4302 ; source
	lda #^Tiles2
	sta $4304 ; bank
	ldx #(End_Tiles2-Tiles2)
	stx $4305 ; length
	lda #1
	sta MDMAEN ; $420b start dma, channel 0
	
	
	
; DMA from Tilemap to VRAM	
	ldx #$6000
	stx VMADDL ; $2116 set an address in the vram of $6000
	
;	lda #1
;	sta $4300 ; transfer mode, 2 registers 1 write
;			  ; $2118 and $2119 are a pair Low/High
;	lda #$18  ; $2118
;	sta $4301 ; destination, vram data
	ldx #.loword(Tilemap)
	stx $4302 ; source
	lda #^Tilemap
	sta $4304 ; bank
	ldx #896
	stx $4305 ; length
	lda #1
	sta MDMAEN ; $420b start dma, channel 0	
    
    
    ldx #$6800
	stx VMADDL ; $2116 set an address in the vram of $6800
	
;	lda #1
;	sta $4300 ; transfer mode, 2 registers 1 write
;			  ; $2118 and $2119 are a pair Low/High
;	lda #$18  ; $2118
;	sta $4301 ; destination, vram data
	ldx #.loword(Tilemap2)
	stx $4302 ; source
	lda #^Tilemap2
	sta $4304 ; bank
	ldx #896
	stx $4305 ; length
	lda #1
	sta MDMAEN ; $420b start dma, channel 0	
    
    
    ldx #$7000
	stx VMADDL ; $2116 set an address in the vram of $7000
	
;	lda #1
;	sta $4300 ; transfer mode, 2 registers 1 write
;			  ; $2118 and $2119 are a pair Low/High
;	lda #$18  ; $2118
;	sta $4301 ; destination, vram data
	ldx #.loword(Tilemap3)
	stx $4302 ; source
	lda #^Tilemap3
	sta $4304 ; bank
	ldx #896
	stx $4305 ; length
	lda #1
	sta MDMAEN ; $420b start dma, channel 0	
	
	
; a is still 8 bit.
	lda #1|BG_ALL_16x16 ; mode 1, tilesize 16x16
	sta BGMODE ; $2105
	
; 210b = tilesets for bg 1 and bg 2
; (210c for bg 3 and bg 4)
; steps of $1000 -321-321... bg2 bg1
	stz BG12NBA ; $210b BG 1 and 2 tiles at VRAM address $0000
	lda #2
    sta BG34NBA ; $210c BG 3 tiles at VRAM address $2000
    
	lda #$60 ; bg1 map at VRAM address $6000
	sta BG1SC ; $2107
    lda #$68 ; bg2 map at VRAM address $6800
	sta BG2SC ; $2107
    lda #$70 ; bg3 map at VRAM address $7000
	sta BG3SC ; $2107

	lda #BG_ALL_ON	; bg 1,2,3 screens on
	sta TM ; $212c
	
	lda #FULL_BRIGHT ; $0f = turn the screen on (end forced blank)
	sta INIDISP ; $2100


Infinite_Loop:	
	A8
	XY16
	jsr Wait_NMI
	
	;code goes here

	jmp Infinite_Loop
	
	
	
Wait_NMI:
.a8
.i16
;should work fine regardless of size of A
	lda in_nmi ;load A register with previous in_nmi
@check_again:	
	WAI ;wait for an interrupt
	cmp in_nmi	;compare A to current in_nmi
				;wait for it to change
				;make sure it was an nmi interrupt
	beq @check_again
	rts
	
	

.include "header.asm"	


.segment "RODATA1"

BG_Palette:
; 32 bytes
.incbin "M1TE/flip.pal"

Tiles2:
; 2bpp tileset
.incbin "M1TE/flip2bpp.chr"
End_Tiles2:

;896 bytes each
Tilemap:
.incbin "M1TE/flip1.map"
Tilemap2:
.incbin "M1TE/flip2.map"
Tilemap3:
.incbin "M1TE/flip3.map"

.segment "RODATA2"

Tiles:
; 4bpp tileset
.incbin "M1TE/flip4bpp.chr"
End_Tiles:


