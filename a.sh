count=0

while [ $count -le 62 ]
do
    mkdir ./data/$count
    mv ./data/Img/*$count-*.png ./data/$count
    ((count++))
done