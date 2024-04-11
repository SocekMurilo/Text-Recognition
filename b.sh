count=0
unit=0
deci=0

while [ $count -le 62 ]
do
    if [ unit -ge 10 ]
    then
        unit = 0
        deci = $deci + 1
    fi
    mkdir ./data/$count
    mv ./data/Img/*0$deci$unit-*.png ./data/$count
    ((count++))
done