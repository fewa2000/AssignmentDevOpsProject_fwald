param location string = resourceGroup().location

// Existing resource names
var storageAccountName = 'assignmstorageaccount' // Replace with your actual storage account name
var functionAppName = 'AssignmentDevOpsProjectfwald' // Replace with your actual function app name

// Storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  kind: 'StorageV2'
  location: location
  name: storageAccountName
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
  }
}

// Blob Container
resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-08-01' = {
  name: '${storageAccountName}/default/generated-images'
}

// Queue
resource queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-08-01' = {
  name: '${storageAccountName}/default/imageprocessqueue'
}

// Function App
resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    clientAffinityEnabled: false
    httpsOnly: true
  }
  resource functionAppConfig 'config@2021-03-01' = {
    name: 'appsettings'
    properties: {
      'AzureWebJobsStorage': storageAccount.properties.primaryEndpoints.blob
      'FUNCTIONS_EXTENSION_VERSION': '~4'
      'FUNCTIONS_WORKER_RUNTIME': 'dotnet-isolated'
      'STORAGE_ACCOUNT_NAME': storageAccountName
      'QUEUE_NAME': queue.name
      'UNSPLASH_API_KEY': 'Twjdz-5NyIcw1FhzPbgOnXR_xSF90h-UXxNbGESq0vw'
      'BLOB_CONTAINER_NAME': blobContainer.name 
    }
  }
}

