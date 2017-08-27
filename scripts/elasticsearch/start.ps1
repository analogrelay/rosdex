. "$PSScriptRoot\_common.ps1"

Write-Host -ForegroundColor Green "*** Building Image ***"
docker build -t $ImageName $PSScriptRoot

if (!(docker volume ls -q --filter "name=$VolumeName")) {
    Write-Host -ForegroundColor Green "*** Creating Data Volume **"
    docker volume create $VolumeName
}

Write-Host -ForegroundColor Green "*** Starting Elasticsearch Container ***"
Ensure-Container-Removed -Name $ContainerName
docker run -p 9200:9200 -d -v "$($VolumeName):/usr/share/elasticsearch/data" --name $ContainerName $ImageName

Write-Host -ForegroundColor Green "*** Starting Cerebro Container ***"
Ensure-Container-Removed -Name $CerebroContainerName
docker run -p 9000:9000 -d --link "$($ContainerName):elasticsearch" --name $CerebroContainerName $CerebroImageName

Write-Host -ForegroundColor Green "*** Starting Kibana Container ***"
Ensure-Container-Removed -Name $KibanaContainerName
docker run -p 5601:5601 -d --link "$($ContainerName):elasticsearch" --name $KibanaContainerName $KibanaImageName

Write-Host -ForegroundColor Green "Started Elasticsearch container '$ContainerName'"
Write-Host -ForegroundColor Green "Elasticsearch should be available at http://localhost:9200/"
Write-Host -ForegroundColor Green "Cerebro management UI should be available at http://localhost:9000/"
Write-Host -ForegroundColor Green "Kibana UI should be available at http://localhost:5601/"