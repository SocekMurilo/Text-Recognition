deactivate

# Path to your Python virtual environment
$venvPath = "$PWD\venv"

python -m venv "$PWD\venv"

# Activate the virtual environment
& "$venvPath\Scripts\Activate.ps1"

python model.py
# Now you're in the virtual environment
# You can execute Python scripts or commands here
