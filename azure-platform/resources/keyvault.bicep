param vaultName string
param location string

resource keyvault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: vaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: false
    enablePurgeProtection: true
    accessPolicies: []
  }
}

output keyVaultName string = keyvault.name
output keyVaultId string = keyvault.id
output vaultUri string = keyvault.properties.vaultUri
output keyvault object = keyvault
