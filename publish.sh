#!/usr/bin/env bash

cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -p:PublishTrimmed=true -r win-x64 -c Release && cd ..
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -p:PublishTrimmed=true -r win-x86 -c Release && cd ..

rm Kaleidoscope-Companion-win-x64.zip
zip -r -j Kaleidoscope-Companion-win-x64.zip Kaleidoscope_Companion/bin/Release/netcoreapp3.0/win-x64/publish/


rm Kaleidoscope-Companion-win-x86.zip
zip -r -j Kaleidoscope-Companion-win-x86.zip Kaleidoscope_Companion/bin/Release/netcoreapp3.0/win-x86/publish/