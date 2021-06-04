targetScope = 'subscription'

param appservicename string
param apiKey string

var location = 'westeurope'

resource resourcegroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: appservicename
  location: location
}

module appserviceplan './resources/app-service-plan.bicep' = {
  name: '${appservicename}-plan'
  scope: resourcegroup
  params: {
    location: location
    appservicename: appservicename
  }
}

module appservice './resources/app-service.bicep' = {
  name: appservicename
  scope: resourcegroup
  params: {
    appserviceplanId: appserviceplan.outputs.appServicePlanId
    location: location
    appservicename: appservicename
    apiKey: apiKey
  }
}
