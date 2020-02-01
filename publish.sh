#!/usr/bin/env bash

X64_AUTONOMOUS_ZIP_NAME=Kaleidoscope-Companion-win-x64-self-contained.zip
X86_AUTONOMOUS_ZIP_NAME=Kaleidoscope-Companion-win-x86-self-contained.zip

X64_ZIP_NAME=Kaleidoscope-Companion-win-x64.zip
X86_ZIP_NAME=Kaleidoscope-Companion-win-x86.zip

# Autonomous
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -p:PublishTrimmed=true -r win-x64 -c Release; cd ..
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -p:PublishTrimmed=true -r win-x86 -c Release; cd ..

rm "$X64_AUTONOMOUS_ZIP_NAME"
zip -r -j "$X64_AUTONOMOUS_ZIP_NAME" Kaleidoscope_Companion/bin/Release/netcoreapp3.0/win-x64/publish/

rm "$X86_AUTONOMOUS_ZIP_NAME"
zip -r -j "$X86_AUTONOMOUS_ZIP_NAME" Kaleidoscope_Companion/bin/Release/netcoreapp3.0/win-x86/publish/

rm -rf Kaleidoscope_Companion/bin

# Platform dependent
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -p:PublishTrimmed=true -r win-x64 -c Release --self-contained false; cd ..
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -p:PublishTrimmed=true -r win-x86 -c Release --self-contained false; cd ..
    
rm "$X64_ZIP_NAME"
zip -r -j "$X64_ZIP_NAME" Kaleidoscope_Companion/bin/Release/netcoreapp3.0/win-x64/

rm "$X86_ZIP_NAME"
zip -r -j "$X86_ZIP_NAME" Kaleidoscope_Companion/bin/Release/netcoreapp3.0/win-x86/
