$timeStamp = Get-Date -UFormat "%Y.%m.%d_%H.%M.%S"
git commit -a -m "nuget release commit $timeStamp"
git tag $timeStamp
git push origin
git push origin $timeStamp