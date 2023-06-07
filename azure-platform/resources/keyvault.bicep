param vaultName string
param location string

resource keyvault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: vaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
    publicNetworkAccess: 'disabled'
    enablePurgeProtection: true
  }
}

output keyVaultName string = keyvault.name
output keyVaultId string = keyvault.id
output vaultUri string = keyvault.properties.vaultUri
