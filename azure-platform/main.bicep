targetScope = 'subscription'

param projectname string
param location string = 'westeurope'

@secure()
param apiKey string

var databaseName = '${projectname}-logs'
var vaultName = '${projectname}-keyvault'

resource resourcegroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: projectname
  location: location
}

module keyvault 'resources/keyvault.bicep' = {
  name: vaultName
  scope: resourcegroup
  params: {
    location: location
    vaultName: vaultName
  }
}

module setApiKey 'resources/keyvault-set-secret.bicep' = {
  name: 'apiKey'
  scope: resourcegroup
  params: {
    keyVaultName: vaultName
    secretName: 'logsApiKey'
    contentType: 'Secret Key'
    secretValue: apiKey
  }
  dependsOn: [
    keyvault
  ]
}

module cosmosDb 'resources/cosmos-db.bicep' = {
  scope: resourcegroup
  name: '${projectname}-cosmos'
  params: {
    location: location
    databaseName: databaseName
    keyvaultName: keyvault.outputs.keyVaultName
  }
  dependsOn: [
    keyvault
  ]
}

module appserviceplan './resources/app-service-plan.bicep' = {
  name: '${projectname}-plan'
  scope: resourcegroup
  params: {
    location: location
    appservicename: projectname
  }
  dependsOn: [
    cosmosDb
  ]
}

module appservice './resources/app-service.bicep' = {
  name: projectname
  scope: resourcegroup
  params: {
    appserviceplanId: appserviceplan.outputs.appServicePlanId
    location: location
    appservicename: projectname

    cosmosAccount: cosmosDb.outputs.documentEndpoint
    databaseName: databaseName
    vaultUri: keyvault.outputs.vaultUri
  }
  dependsOn: [
    appserviceplan
  ]
}
