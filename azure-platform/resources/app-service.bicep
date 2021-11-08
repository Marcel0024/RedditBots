param appserviceplanId string
param location string
param appservicename string
param apiKey string

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
      use32BitWorkerProcess: true
      webSocketsEnabled: true
      alwaysOn: false
      http20Enabled: true
      autoHealEnabled: true
      netFrameworkVersion: 'v6.0'
    }
    clientAffinityEnabled: false
  }
}
