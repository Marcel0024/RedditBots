param appserviceplanId string
param location string
param appservicename string

param cosmosAccount string
param databaseName string
param vaultUri string

resource appservice 'Microsoft.Web/sites@2022-09-01' = {
  name: appservicename
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appserviceplanId
    customDomainVerificationId: 'DNS Record verification'
    enabled: true
    siteConfig: {
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'Logging:LogLevel:Default'
          value: 'Information'
        }
        {
          name: 'CosmosDb:Account'
          value: cosmosAccount
        }
        {
          name: 'CosmosDb:DatabaseName'
          value: databaseName
        }
        {
          name: 'KeyVault:Uri'
          value: vaultUri
        }
      ]
      use32BitWorkerProcess: false
      webSocketsEnabled: true
      alwaysOn: true
      http20Enabled: true
      autoHealEnabled: true
      netFrameworkVersion: 'v7.0'
      ftpsState: 'Disabled'
    }
    clientAffinityEnabled: false
    httpsOnly: true
  }
}

output principalId string = appservice.identity.principalId
