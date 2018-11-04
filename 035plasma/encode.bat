@echo off
set ffmpeg=c:\bin\ffmpeg.exe
%ffmpeg% -framerate 25 -i out%%04d.png -f avi -vcodec msmpeg4v2 -q:v 3 -y out.avi

rem =====================================
rem   Doporucovane kodeky (-vcodec xxx):
rem "msmpeg4v2" - MPEG-4 kodek, ktery implicitne umeji Windows
rem "xvid"      - free-verze kodeku pro DivX prehravac
rem "mpeg"      - MPEG-1
rem =====================================
