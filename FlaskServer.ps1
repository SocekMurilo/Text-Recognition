$server = "$PWD\server"
python -m venv "$server\.venv"

"$server\.venv\Scripts\activate.ps1"

cd $server

pip install --ignore-installed --target $server flask
pip install --ignore-installed --target $server tensorflow


# ".\$server\.venv\Scripts\deactivate"
