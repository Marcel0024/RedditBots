targetScope = 'subscription'

param projectname string
param location string = 'westeurope'

@secure()
param apiKey string

var databaseName = '${projectname}-logs'

resource resourcegroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: projectname
  location: location
}

module cosmosDb 'resources/cosmos-db.bicep' = {
  scope: resourcegroup
  name: '${projectname}-cosmos'
  params: {
    databaseName: databaseName
  }
}

module appserviceplan './resources/app-service-plan.bicep' = {
  name: '${projectname}-plan'
  scope: resourcegroup
  params: {
    location: location
    appservicename: projectname
  }
}

module appservice './resources/app-service.bicep' = {
  name: projectname
  scope: resourcegroup
  params: {
    cosmosKey: cosmosDb.outputs.key
    cosmosAccount: cosmosDb.outputs.documentEndpoint
    databaseName: databaseName
    appserviceplanId: appserviceplan.outputs.appServicePlanId
    location: location
    appservicename: projectname
    apiKey: apiKey
  }
}
