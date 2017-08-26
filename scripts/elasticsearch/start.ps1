$ImageName = "anurse/rosdex-elasticsearch"
$ContainerName = "rosdex_elasticsearch"
$VolumeName = "rosdex_elasticsearch_data"

Write-Host -ForegroundColor Green "*** Building Image ***"
docker build -t $ImageName $PSScriptRoot

if (docker volume ls -q --filter "name=$VolumeName") {
    Write-Host -ForegroundColor Green "*** Creating Data Volume **"
    docker volume create $VolumeName
}

if (docker ps -q --filter "name=$ContainerName" --filter "status=running") {
    Write-Host -ForegroundColor Yellow "Stopping Existing Container"
    docker stop $ContainerName
}

if (docker ps -a -q --filter "name=$ContainerName") {
    Write-Host -ForegroundColor Yellow "Removing Existing Container"
    docker rm -f $ContainerName
}

Write-Host -ForegroundColor Green "*** Starting Elasticsearch Container ***"
docker run -p 9200:9200 -d -v "$($VolumeName):/usr/share/elasticsearch/data" --name $ContainerName $ImageName

Write-Host -ForegroundColor Green "Started Elasticsearch container '$ContainerName'"
Write-Host -ForegroundColor Green "Elasticsearch should be available at http://localhost:9200/"