$Script:BackupFolder = ".\Backup"

function GetFolders()
{
    Param(
        [Parameter(Mandatory=$True)][String]$SourceFolder
    )

    return (Get-ChildItem -Path $SourceFolder) | Sort-Object LastWriteTime -Descending
}

Function CreateMenu (){
    
    Param(
        [Parameter(Mandatory=$True)][String]$MenuTitle,
        [Parameter(Mandatory=$True)][array]$MenuOptions
    )

    $MaxValue = $MenuOptions.count-1
    $Selection = 0
    $EnterPressed = $False
    
    Clear-Host

    While($EnterPressed -eq $False){
        
        Write-Host "$MenuTitle"

        For ($i=0; $i -le $MaxValue; $i++){
            
            If ($i -eq $Selection){
                Write-Host -BackgroundColor Cyan -ForegroundColor Black "[ $($MenuOptions[$i]) ]"
            } Else {
                Write-Host "  $($MenuOptions[$i])  "
            }

        }

        $KeyInput = $host.ui.rawui.readkey("NoEcho,IncludeKeyDown").virtualkeycode

        Switch($KeyInput){
            13{
                $EnterPressed = $True
                Return $Selection
                Clear-Host
                break
            }

            38{
                If ($Selection -eq 0){
                    $Selection = $MaxValue
                } Else {
                    $Selection -= 1
                }
                Clear-Host
                break
            }

            40{
                If ($Selection -eq $MaxValue){
                    $Selection = 0
                } Else {
                    $Selection +=1
                }
                Clear-Host
                break
            }
            Default{
                Clear-Host
            }
        }
    }
}

function Main()
{


    $folders = GetFolders -SourceFolder $Script:BackupFolder
    $selectedIndex = CreateMenu -MenuTitle "Pick Folder to restore from" -MenuOptions $folders
    $selectedFolder = $folders[$selectedIndex]
    
    $TypeOptions = @("Exit", "Characters", "Worlds", "Both")
    Write-Host "What would you like to restore"
    $selectedIndex = CreateMenu -MenuTitle "Restore Type" -MenuOptions $TypeOptions
    $selectedOption = $TypeOptions[$selectedIndex]

    $restoreOption = "Restore $selectedOption from $selectedFolder"
    $selectedIndex = CreateMenu -MenuTitle $restoreOption -MenuOptions @("Yes","No, exit")

    switch($selectedIndex)
    {
        0 {
            if($selectedOption.ToString().ToLower() -eq "characters" -or $selectedOption.ToString().ToLower() -eq "both")
            {
                    Copy-Item -Path ".\backup\$selectedFolder\characters" -Destination ".\" -Recurse -Force
            }

            if($selectedOption.ToString().ToLower() -eq "worlds" -or $selectedOption.ToString().ToLower() -eq "both")
            {
                    Copy-Item -Path ".\backup\$selectedFolder\worlds" -Destination ".\" -Recurse -Force
            }
          
            }
        1 {
            # Do nothing
          }
    }

}

main