@description('Cosmos DB account name')
param accountName string = 'cosmos-${uniqueString(resourceGroup().id)}'

@description('Location for the Cosmos DB account.')
param location string

@description('The name for the Core (SQL) database')
param databaseName string

@description('The name of the keyvault were to store settings')
param keyvaultName string

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: toLower(accountName)
  location: location
  properties: {
    enableFreeTier: true
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }
}

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosAccount
  name: toLower(databaseName)
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 1000
    }
  }
}

output documentEndpoint string = cosmosAccount.properties.documentEndpoint

module setCosmosConnectionString 'keyvault-set-secret.bicep' = {
  name: 'setCosmosConnectionString'
  params: {
    keyVaultName: keyvaultName
    secretName: 'cosmosDb--Key'
    contentType: 'Cosmos DB Primary Master Key'
    secretValue: cosmosAccount.listKeys().primaryMasterKey
  }
}
