param location string
param appservicename string

resource appserviceplan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: '${appservicename}-plan'
  location: location
  properties: {
    reserved: true
    perSiteScaling: false
  }
  sku: {
    name: 'B1'
    tier: 'Basic'
    capacity: 1
  }
  kind: 'linux'
}

output appServicePlanId string = appserviceplan.id
