$ImageName = "anurse/rosdex-elasticsearch"
$ContainerName = "rosdex_elasticsearch"
$VolumeName = "rosdex_elasticsearch_data"

$CerebroImageName = "yannart/cerebro:latest"
$CerebroContainerName = "rosdex_cerebro"

$KibanaImageName = "docker.elastic.co/kibana/kibana:5.5.2"
$KibanaContainerName = "rosdex_kibana"

function Ensure-Container-Removed($Name) {
    if (docker ps -q --filter "name=$Name" --filter "status=running") {
        Write-Host -ForegroundColor Yellow "Stopping Existing Container"
        docker stop $Name
    }

    if (docker ps -a -q --filter "name=$Name") {
        Write-Host -ForegroundColor Yellow "Removing Existing Container"
        docker rm -f $Name
    }
}
