printf "Muddling enemies...\n"

dotnet run $1 --project ./script/ForeverMuddled/ForeverMuddled.csproj

printf "Enemies muddled!\n"
read -n 1 -s -r -p "Press any key to continue . . . "
printf "\n"