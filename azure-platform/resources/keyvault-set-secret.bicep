param keyVaultName string
param contentType string
param secretName string
@secure()
param secretValue string

resource secret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  name: '${keyVaultName}/${secretName}'
  properties: {
    value: secretValue
    contentType: contentType
    attributes: {
      enabled: true
    }
  }
}
