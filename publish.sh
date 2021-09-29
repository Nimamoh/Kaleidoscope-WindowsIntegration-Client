#!/usr/bin/env bash

read -p 'version (e.g: v1.0.1): ' version
if [[ -z "$version" ]]; then
  echo >&2 "Version is empty. Exiting"
  exit 1
fi


X64_AUTONOMOUS_ZIP_NAME=Kaleidoscope-Companion-win-x64-$version-self-contained.zip
X86_AUTONOMOUS_ZIP_NAME=Kaleidoscope-Companion-win-x86-$version-self-contained.zip

X64_ZIP_NAME=Kaleidoscope-Companion-win-x64-$version.zip
X86_ZIP_NAME=Kaleidoscope-Companion-win-x86-$version.zip

# Autonomous
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -r win-x64 -c Release; cd ..
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -r win-x86 -c Release; cd ..

rm "$X64_AUTONOMOUS_ZIP_NAME" || true
zip -r -j "$X64_AUTONOMOUS_ZIP_NAME" Kaleidoscope_Companion/bin/Release/net5.0-windows/win-x64/publish/

rm "$X86_AUTONOMOUS_ZIP_NAME" || true
zip -r -j "$X86_AUTONOMOUS_ZIP_NAME" Kaleidoscope_Companion/bin/Release/net5.0-windows/win-x86/publish/

rm -rf Kaleidoscope_Companion/bin

# Platform dependent
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -r win-x64 -c Release --self-contained false; cd ..
cd Kaleidoscope_Companion && dotnet.exe publish -p:PublishSingleFile=true -r win-x86 -c Release --self-contained false; cd ..
    
rm "$X64_ZIP_NAME" || true
zip -r -j "$X64_ZIP_NAME" Kaleidoscope_Companion/bin/Release/net5.0-windows/win-x64/publish

rm "$X86_ZIP_NAME" || true
zip -r -j "$X86_ZIP_NAME" Kaleidoscope_Companion/bin/Release/net5.0-windows/win-x86/publish

git tag -d $version || true
echo "Create git tag $version"
git tag $version
