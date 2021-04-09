$time = Get-Date
$timestring = $time.ToShortDateString() + "_" +  $time.ToShortTimeString()
$timestring = $timestring.Replace("/","_")
$timestring = $timestring.Replace(" ","_")
$timestring = $timestring.Replace(":","_")
$characterdestination = ".\Backup\$timestring\characters"
$worlddestination = ".\Backup\$timestring\worlds"
Copy-Item -Path ".\characters" -Destination $characterdestination -Recurse -Force
Copy-Item -Path ".\worlds" -Destination "$worlddestination" -Recurse -Force