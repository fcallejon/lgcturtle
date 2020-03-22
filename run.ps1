Write-Host "Restoring"
dotnet restore
Write-Host "Bulding"
dotnet build -c release
Write-Host "Publishing"
dotnet publish -r win-x64 -c release  -o .\publish

Write-Host "To The Exit! ->"
.\publish\turtle.exe .\small.txt .\toExit.txt
Write-Host "----------"

Write-Host "To a bomb! ->"
.\publish\turtle.exe .\small.txt .\toABomb.txt
Write-Host "----------"

Write-Host "Out of bounds! ->"
.\publish\turtle.exe .\small.txt .\toOutBounds.txt
Write-Host "----------"

Write-Host "Still in danger! ->"
.\publish\turtle.exe .\small.txt .\notOut.txt
Write-Host "----------"

Write-Host "Circle! ->"
.\publish\turtle.exe .\small.txt .\circle.txt
Write-Host "----------"

Write-Host "No Edge Start Win! ->"
.\publish\turtle.exe .\small-NoEdgeStart.txt .\noEdgeStart-win.txt
Write-Host "----------"

Remove-Item -Path .\obj -Recurse
Remove-Item -Path .\bin -Recurse
Remove-Item -Path .\publish -Recurse
