#!/bin/bash
set -ev
echo -en 'travis_fold:start:nuget\\r'
mono nuget.exe restore $SLN
echo -en 'travis_fold:end:nuget\\r'

msbuild /p:Configuration=$VERSION $SLN

mono ./testrunner/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe ./PubSub.Tests/bin/$VERSION/$TARGET/PubSub.Tests.dll
