param appserviceplanId string
param location string
param appservicename string
param apiKey string

param cosmosKey string
param cosmosAccount string
param databaseName string

resource appservice 'Microsoft.Web/sites@2020-12-01' = {
  name: appservicename
  location: location
  properties: {
    serverFarmId: appserviceplanId
    customDomainVerificationId: 'DNS Record verification'
    enabled: true
    siteConfig: {
      appSettings: [
        {
          name: 'CosmosDb:Account'
          value: cosmosAccount
        }
        {
          name: 'CosmosDb:Key'
          value: cosmosKey
        }
        {
          name: 'CosmosDb:DatabaseName'
          value: databaseName
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'Logging:LogLevel:Default'
          value: 'Information'
        }
        {
          name: 'AppSettings:apiKey'
          value: apiKey
        }
      ]
      use32BitWorkerProcess: false
      webSocketsEnabled: true
      alwaysOn: true
      http20Enabled: true
      autoHealEnabled: true
      netFrameworkVersion: 'v6.0'
    }
    clientAffinityEnabled: false
  }
}
