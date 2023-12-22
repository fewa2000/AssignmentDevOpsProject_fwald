# Variables
$resourceGroupName = "AssignmentDevOpsProjectRessourceGroup"
$templateFile = "./azurefunctions.bicep"
$location = "northeurope"
$functionAppPath = "./AssignmentDevOpsProject_fwald/AssignmentDevOpsProject_fwald.csproj"
$functionAppName = "AssignmentDevOpsProject_fwald" # Manually specify your Function App name

# Authenticate with Azure (if necessary)
#Connect-AzAccount

# Deploy Bicep template
Write-Host "Deploying Azure resources..."
$deploymentResult = az deployment group create `
  --resource-group $resourceGroupName `
  --template-file $templateFile `
  --parameters location=$location

if ($LASTEXITCODE -ne 0) {
    Write-Host "Deployment failed"
    exit
}

# Publish Azure Function
Write-Host "Publishing Azure Function..."
dotnet publish $functionAppPath --configuration Release --output .\publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publishing failed"
    exit
}

# Deploy the function app using Azure CLI
Write-Host "Deploying Function App to Azure..."
az functionapp deployment source config-zip `
  --resource-group $resourceGroupName `
  --name $functionAppName `
  --src .\publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Function App deployment failed"
    exit
}

Write-Host "Deployment completed successfully."
