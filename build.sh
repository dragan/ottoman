#! /bin/sh

echo Running Build
xbuild ottoman.msbuild /t:"$@"
s