# GitHub Actions CI/CD Setup Guide

This guide explains how to configure the GitHub Actions workflow for your Event-Driven Architecture project.

## 🔐 Required GitHub Secrets

You need to configure the following secrets in your GitHub repository:

### Setting up GitHub Secrets

1. Go to your GitHub repository: `https://github.com/manojnewgen/Event_driven`
2. Click on **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret** for each of the following:

### Required Secrets:

#### 1. AZURE_CREDENTIALS
```json
{
  "clientId": "your-service-principal-client-id",
  "clientSecret": "your-service-principal-client-secret",
  "subscriptionId": "your-azure-subscription-id",
  "tenantId": "your-azure-tenant-id"
}
```

#### 2. AZURE_ACR_USERNAME
```
acreventdriven8168
```

#### 3. AZURE_ACR_PASSWORD
```
your-acr-admin-password
```

## 🚀 Quick Setup Commands

### 1. Get Azure Container Registry Credentials
```bash
# Get ACR admin password
az acr credential show --name acreventdriven8168 --resource-group rg-event-driven-app
```

### 2. Create Azure Service Principal
```bash
# Create service principal for GitHub Actions
az ad sp create-for-rbac \
  --name "github-actions-event-driven" \
  --role contributor \
  --scopes /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/rg-event-driven-app \
  --sdk-auth
```

This command will output JSON that you'll use for the `AZURE_CREDENTIALS` secret.

### 3. Get Subscription and Tenant IDs
```bash
# Get subscription info
az account show --query "{subscriptionId:id, tenantId:tenantId}"
```

## 📋 Workflow Features

The GitHub Actions workflow includes:

### 🔨 Build and Test
- ✅ .NET 8 build and restore
- ✅ Run unit tests with coverage
- ✅ Cache NuGet packages for faster builds
- ✅ Upload test results and coverage reports

### 🔒 Security
- ✅ Security scanning with Microsoft DevSkim
- ✅ Container vulnerability scanning
- ✅ SARIF security report upload

### 🐳 Docker
- ✅ Multi-platform Docker build (linux/amd64)
- ✅ Automated image tagging with SHA and branch
- ✅ Push to Azure Container Registry
- ✅ Docker layer caching for faster builds

### ☁️ Azure Deployment
- ✅ Deploy to Azure Container Apps
- ✅ Update container app with new image
- ✅ Environment-specific deployments (production)

### 🏥 Health Checks
- ✅ Automated health endpoint testing
- ✅ API connectivity verification
- ✅ Swagger endpoint validation
- ✅ Post-deployment verification

### 🧹 Maintenance
- ✅ Cleanup old container images (keeps latest 10)
- ✅ Automated notifications
- ✅ Deployment status reporting

## 🔄 Workflow Triggers

The workflow runs on:
- **Push** to `main` or `develop` branches
- **Pull Request** to `main` branch

## 📊 Workflow Jobs

| Job | Purpose | Runs On |
|-----|---------|---------|
| `build-and-test` | Build solution and run tests | All triggers |
| `security-scan` | Security analysis | All triggers |
| `docker-build` | Build and push Docker image | `main` branch only |
| `deploy-to-azure` | Deploy to Azure Container Apps | `main` branch only |
| `health-check` | Verify deployment health | After deployment |
| `notify` | Send deployment notifications | After deployment |
| `cleanup` | Remove old container images | After deployment |

## 🎯 Environment Configuration

### Production Environment
- Protected environment requiring manual approval
- Deploys only from `main` branch
- Full health check validation

### Development (Optional)
You can add a development environment by:
1. Creating a `develop` environment in GitHub
2. Adding environment-specific secrets
3. Modifying the workflow conditions

## 📝 Usage Examples

### Manual Workflow Trigger
```bash
# Trigger workflow manually (if manual trigger is enabled)
gh workflow run "dotnet.yaml"
```

### Check Workflow Status
```bash
# List workflow runs
gh run list --workflow="dotnet.yaml"

# View specific run
gh run view <run-id>
```

### Monitor Deployment
```bash
# Watch Azure Container App logs
az containerapp logs show \
  --name app-event-driven \
  --resource-group rg-event-driven-app \
  --follow
```

## 🛠️ Customization

### Adding Tests
To include test projects in the workflow:
1. Add test projects to your solution
2. The workflow will automatically discover and run them
3. Test results will be uploaded as artifacts

### Adding Environments
To add staging or development environments:
1. Create environment in GitHub repository settings
2. Add environment-specific secrets
3. Modify workflow to include new environment

### Custom Health Checks
To customize health checks:
1. Modify the `health-check` job in the workflow
2. Add specific API endpoint tests
3. Configure custom validation logic

## 🔍 Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| **ACR Login Failed** | Verify `AZURE_ACR_USERNAME` and `AZURE_ACR_PASSWORD` secrets |
| **Azure Login Failed** | Check `AZURE_CREDENTIALS` secret format and permissions |
| **Deployment Failed** | Verify resource group and container app names |
| **Health Check Failed** | Check application startup and health endpoint |

### Debug Commands
```bash
# Check Azure resources
az containerapp show --name app-event-driven --resource-group rg-event-driven-app

# Check ACR images
az acr repository show-tags --name acreventdriven8168 --repository event-driven-api

# Test health endpoint manually
curl https://your-app-url.azurecontainerapps.io/health
```

## 🎉 Workflow Benefits

### Automated Quality Assurance
- ✅ Every commit is built and tested
- ✅ Security vulnerabilities are detected early
- ✅ Code coverage is tracked
- ✅ Container images are scanned

### Reliable Deployments
- ✅ Consistent deployment process
- ✅ Automatic rollback on health check failure
- ✅ Zero-downtime deployments
- ✅ Environment isolation

### Maintenance Automation
- ✅ Old images are automatically cleaned up
- ✅ Dependencies are cached for performance
- ✅ Deployment status is reported
- ✅ Infrastructure drift is prevented

---

**Next Steps:**
1. Configure the required GitHub secrets
2. Push changes to trigger the workflow
3. Monitor the deployment in GitHub Actions
4. Verify the application is healthy in Azure

Your CI/CD pipeline is now ready for production use! 🚀
