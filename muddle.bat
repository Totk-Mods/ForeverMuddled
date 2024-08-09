@ECHO OFF

ECHO Muddling enemies...

dotnet run %1 --project ./script/ForeverMuddled/ForeverMuddled.csproj

ECHO Enemies muddled!
PAUSE