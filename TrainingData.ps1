deactivate

# Path to your Python virtual environment
$venvPath = "$PWD\venv"

python -m venv "$PWD\venv"

# Activate the virtual environment
& "$venvPath\Scripts\Activate.ps1"

$zipFile = "$PWD\data\archive (6).zip"
$destination = "$PWD\data\"

Write-Output "Extracting Zip File"
Expand-Archive -Path $zipFile -DestinationPath $destination

Write-Output "Installing Python Libraries"
pip install --ignore-installed flask
pip install --ignore-installed load-bar termcolor
pip install --ignore-installed matplotlib
pip install --ignore-installed numpy
pip install --ignore-installed opencv-python
pip install --ignore-installed tensorflow

Write-Output "Classifying Images"
python "$PWD\OrderingData\dataOrder.py"

Write-Output "Applying width changes to images"
python "$PWD\OrderingData\LetterWidthEnlargement.py"

Write-Output "Applying size changes to images"
python "$PWD\OrderingData\Resize.py"

Write-Output "Deleting useless folders"
Remove-Item -Path "$destination\Img" -Recurse
Remove-Item -Path "$destination\english.csv" -Recurse

Write-Output "Finished!"
