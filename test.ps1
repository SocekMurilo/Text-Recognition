$zipFile = "$PWD\data\archive (6).zip"
$destination = "$PWD\data\"

Write-Output "Extracting Zip File"
Expand-Archive -Path $zipFile -DestinationPath $destination

Write-Output "Classifying Images"
python "$PWD\dataOrder.py"

Write-Output "Applying width changes to images"
python "$PWD\LetterWidthEnlargement.py"

Write-Output "Deleting useless folders"
Remove-Item -Path "$destination\Img" -Recurse
Remove-Item -Path "$destination\english.csv" -Recurse

Write-Output "Finished!"
