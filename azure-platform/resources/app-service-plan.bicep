param location string
param appservicename string

resource appserviceplan 'Microsoft.Web/serverfarms@2019-08-01' = {
  name: '${appservicename}-plan'
  location: location
  properties: {
    perSiteScaling: false
  }
  sku: {
    name: 'B1'
    tier: 'Basic'
    capacity: 1
  }
}

output appServicePlanId string = appserviceplan.id
