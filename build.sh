#!/bin/bash
set -ev
mono nuget.exe restore $SLN

msbuild /p:Configuration=$VERSION $SLN

mono ./testrunner/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe ./PubSub.Tests/bin/$VERSION/$TARGET/PubSub.Tests.dll
