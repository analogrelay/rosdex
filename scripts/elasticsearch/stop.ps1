. "$PSScriptRoot\_common.ps1"

Write-Host -ForegroundColor Green "*** Removing Cerebro Container ***"
Ensure-Container-Removed -Name $CerebroContainerName

Write-Host -ForegroundColor Green "*** Removing Kibana Container ***"
Ensure-Container-Removed -Name $KibanaContainerName

Write-Host -ForegroundColor Green "*** Removing Elasticsearch Container ***"
Ensure-Container-Removed -Name $ContainerName