# Azure Deployment Script
param(
    [string]$ResourceGroup = "rg-event-driven-app",
    [string]$Location = "East US",
    [string]$AcrName = "acreventdriven8168",
    [string]$ImageName = "event-driven-api",
    [string]$AppName = "app-event-driven"
)

Write-Host "Starting Azure Deployment..." -ForegroundColor Green

# Step 1: Verify Azure login
Write-Host "Verifying Azure login..." -ForegroundColor Yellow
$account = az account show --query "user.name" --output tsv 2>$null
if (-not $account) {
    Write-Host "ERROR: Please run 'az login' first" -ForegroundColor Red
    exit 1
}
Write-Host "SUCCESS: Logged in as: $account" -ForegroundColor Green

# Step 2: Set working directory
Set-Location "C:\ThinksysProjects\Event_driven"
Write-Host "Working directory: $(Get-Location)" -ForegroundColor Cyan

# Step 3: Ensure resource group exists
Write-Host "Checking resource group: $ResourceGroup" -ForegroundColor Yellow
$rgExists = az group exists --name $ResourceGroup
if ($rgExists -eq "false") {
    Write-Host "Creating resource group..." -ForegroundColor Yellow
    az group create --name $ResourceGroup --location $Location
    if ($LASTEXITCODE -ne 0) { Write-Host "ERROR: Failed to create resource group" -ForegroundColor Red; exit 1 }
}
Write-Host "SUCCESS: Resource group ready" -ForegroundColor Green

# Step 4: Check ACR exists
Write-Host "Checking Azure Container Registry: $AcrName" -ForegroundColor Yellow
$acrExists = az acr show --name $AcrName --resource-group $ResourceGroup --query "name" --output tsv 2>$null
if (-not $acrExists) {
    Write-Host "Creating ACR..." -ForegroundColor Yellow
    az acr create --resource-group $ResourceGroup --name $AcrName --sku Basic --admin-enabled true
    if ($LASTEXITCODE -ne 0) { Write-Host "ERROR: Failed to create ACR" -ForegroundColor Red; exit 1 }
}
Write-Host "SUCCESS: ACR ready: $AcrName" -ForegroundColor Green

# Step 5: Build Docker image
Write-Host "Building Docker image..." -ForegroundColor Yellow
$imageFull = "$AcrName.azurecr.io/$ImageName`:latest"
docker build -f src/EventDrivenArchitecture.API/Dockerfile -t $imageFull .
if ($LASTEXITCODE -ne 0) { Write-Host "ERROR: Docker build failed" -ForegroundColor Red; exit 1 }
Write-Host "SUCCESS: Docker image built: $imageFull" -ForegroundColor Green

# Step 6: Login to ACR and push
Write-Host "Pushing to ACR..." -ForegroundColor Yellow
az acr login --name $AcrName
if ($LASTEXITCODE -ne 0) { Write-Host "ERROR: ACR login failed" -ForegroundColor Red; exit 1 }

docker push $imageFull
if ($LASTEXITCODE -ne 0) { Write-Host "ERROR: Docker push failed" -ForegroundColor Red; exit 1 }
Write-Host "SUCCESS: Image pushed to ACR" -ForegroundColor Green

# Step 7: Verify image
Write-Host "Verifying image in ACR..." -ForegroundColor Yellow
az acr repository show-tags --name $AcrName --repository $ImageName --output table

# Step 8: Deploy to Container Apps
Write-Host "Deploying to Azure Container Apps..." -ForegroundColor Yellow

# Create Container App Environment if needed
$envName = "env-event-driven"
$envExists = az containerapp env show --name $envName --resource-group $ResourceGroup --query "name" --output tsv 2>$null
if (-not $envExists) {
    Write-Host "Creating Container App Environment..." -ForegroundColor Yellow
    az containerapp env create --name $envName --resource-group $ResourceGroup --location $Location
}

# Deploy Container App
az containerapp create `
  --name $AppName `
  --resource-group $ResourceGroup `
  --environment $envName `
  --image $imageFull `
  --target-port 8080 `
  --ingress external `
  --registry-server "$AcrName.azurecr.io" `
  --env-vars "ASPNETCORE_ENVIRONMENT=Production" `
  --cpu 0.5 `
  --memory 1Gi `
  --min-replicas 1 `
  --max-replicas 3

if ($LASTEXITCODE -eq 0) {
    # Get app URL
    $appUrl = az containerapp show --resource-group $ResourceGroup --name $AppName --query "properties.configuration.ingress.fqdn" --output tsv
    
    Write-Host ""
    Write-Host "DEPLOYMENT SUCCESS!" -ForegroundColor Green
    Write-Host "===================" -ForegroundColor Green
    Write-Host "App URL: https://$appUrl" -ForegroundColor Cyan
    Write-Host "Health: https://$appUrl/health" -ForegroundColor Cyan
    Write-Host "Swagger: https://$appUrl/swagger" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Magenta
    Write-Host "  Check logs: az containerapp logs show --name $AppName --resource-group $ResourceGroup --follow" -ForegroundColor Gray
} else {
    Write-Host "ERROR: Container App deployment failed" -ForegroundColor Red
    exit 1
}
