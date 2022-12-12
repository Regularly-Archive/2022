#!/bin/bash
mtext=$1 
angle=45 # in degrees counterclockwise from horizontal
grey=0.75 # 0 is black 1 is white

ps2pdf - - <<!
%!PS
/cm { 28.4 mul } bind def
/draft-Bigfont /Helvetica-Bold findfont 36 scalefont def
/draft-copy {
        gsave initgraphics $grey setgray
        5 cm 10 cm moveto
        $angle rotate
        draft-Bigfont setfont
        ($mtext) show grestore
 } def
draft-copy showpage
!