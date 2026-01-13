# Encryption in C#

## What is Encryption?

Encryption is the process of converting readable data (plaintext) into an unreadable format (ciphertext) to protect sensitive information from unauthorized access. Only those with the correct key can decrypt the data back to its original form. This is essential for:

- **Data Security** - Protecting sensitive information like credit card numbers, passwords, and personal data
- **Compliance** - Meeting regulatory requirements (GDPR, PCI DSS, HIPAA)
- **Privacy Protection** - Ensuring user data remains confidential
- **Secure Communication** - Protecting data in transit and at rest

## Encryption vs. Obfuscation

Our examples demonstrate both concepts:

### Obfuscation
**Purpose:** Hide data from casual viewing, but easily reversible
**Example from our code:**
```csharp
static Func<CreditCard, (CreditCard original, CreditCard obfuscated)> _ccObfuscator = cc => {
    string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b"; 
    string replacement = "$1**** **** **** $4";

    return (cc, new CreditCard(
                cc.CreditCardId,
                cc.Issuer,
                Regex.Replace(cc.Number, pattern, replacement),
                "**",
                "**"));
};
```

**Result:** `1234 5678 9012 3456` becomes `1234 **** **** 3456`

### Encryption
**Purpose:** Truly secure data using cryptographic algorithms
**Example from our code:**
```csharp
var encryptedCC = employees
    .Select(e => 
        (e.EmployeeId,  
         e.CreditCards.Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc))).ToImmutableList()));
```

**Result:** Credit card data becomes `eyJDcmVkaXRDYXJkSWQiOiJhYmMxMjMtZGVmNDU2...` (Base64-encoded encrypted data)

## AES Encryption Implementation

### Core Components

#### 1. AesEncryptionOptions ([AesEcryptionOptions.cs](../../Encryptions/Options/AesEcryptionOptions.cs))
```csharp
public class AesEncryptionOptions
{
    public string Key { get; set; }      // Encryption key
    public string Iv { get; set; }       // Initialization Vector
    public string Salt { get; set; }     // Salt for key derivation
    public int Iterations { get; set; }  // PBKDF2 iterations
    
    public byte[] KeyHash { get; private set; }
    public byte[] IvHash { get; private set; }
}
```

**Key Elements:**
- **Key**: The secret key used for encryption/decryption
- **IV (Initialization Vector)**: Ensures the same plaintext produces different ciphertext
- **Salt**: Adds randomness to key derivation
- **Iterations**: Number of PBKDF2 iterations for key strengthening

#### 2. EncryptionEngine ([EncryptionEngine.cs](../../Encryptions/EncryptionEngine.cs))

**Encryption Process:**
```csharp
public string AesEncryptToBase64<T>(T sourceToEncrypt)
{
    // 1. Serialize object to JSON
    string stringToEncrypt = JsonConvert.SerializeObject(sourceToEncrypt);
    
    // 2. Convert to bytes
    byte[] dataset = System.Text.Encoding.Unicode.GetBytes(stringToEncrypt);

    // 3. Encrypt using AES
    byte[] encryptedBytes;
    using (SymmetricAlgorithm algorithm = Aes.Create())
    using (ICryptoTransform encryptor = algorithm.CreateEncryptor(_aesOption.KeyHash, _aesOption.IvHash))
    {
        encryptedBytes = encryptor.TransformFinalBlock(dataset, 0, dataset.Length);
    }

    // 4. Return as Base64 string
    return Convert.ToBase64String(encryptedBytes);
}
```

**Decryption Process:**
```csharp
public T AesDecryptFromBase64<T>(string encryptedBase64, JsonSerializerSettings jsonSettings = null)
{
    // 1. Convert from Base64
    byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

    // 2. Decrypt using AES
    byte[] decryptedBytes;
    using (SymmetricAlgorithm algorithm = Aes.Create())
    using (ICryptoTransform decryptor = algorithm.CreateDecryptor(_aesOption.KeyHash, _aesOption.IvHash))
    {
        decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
    }

    // 3. Convert bytes back to string
    string decryptedString = System.Text.Encoding.Unicode.GetString(decryptedBytes);
    
    // 4. Deserialize JSON back to object
    T decryptedObject = JsonConvert.DeserializeObject<T>(decryptedString, jsonSettings);

    return decryptedObject;
}
```

## Practical Example: Credit Card Encryption ([CreditCardEncryption.cs](../Examples/CreditCardEncryption.cs))

### Step 1: Original Data
```csharp
Employee: John Doe (abc123-def456)
  Has 2 credit card(s):
    - Visa: 1234 5678 9012 3456 (Expires: 12/25)
    - MasterCard: 9876 5432 1098 7654 (Expires: 06/26)
```

### Step 2: Obfuscated Data
```csharp
var empWithObfuscatedCC = employees
    .Select(e => 
        e with { CreditCards = e.CreditCards
            .Select( cc => _ccObfuscator(cc).obfuscated )
            .ToImmutableList() });
```

**Result:**
```csharp
Employee: John Doe (abc123-def456)
  Has 2 credit card(s):
    - Visa: 1234 **** **** 3456 (Expires: **/*)
    - MasterCard: 9876 **** **** 7654 (Expires: **/*)
```

### Step 3: Encrypted Data
```csharp
var empEncryptedCC = employees
    .Select(e => 
        (e.EmployeeId,  
         e.CreditCards.Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc))).ToImmutableList()));
```

**Result:**
```csharp
Employee ID: abc123-def456
  Has 2 encrypted credit card(s):
    - CreditCard ID: cc123-456, Encrypted Data: eyJDcmVkaXRDYXJkSWQiOiJjYzEyMy00NTYi...
    - CreditCard ID: cc789-012, Encrypted Data: eyJDcmVkaXRDYXJkSWQiOiJjYzc4OS0wMTIi...
```

### Step 4: Decryption & Verification
```csharp
var empDecryptedCC = employees.Join(empEncryptedCC, 
        e => e.EmployeeId, e => e.EmployeeId, 
        (original, encrypted) => original with { CreditCards = encrypted.Item2
                    .Select(cc => encryptor.AesDecryptFromBase64<CreditCard>(cc.Item2))
                    .ToImmutableList()});

// Data integrity verification
bool integrityVerified = VerifyDataIntegrity(employees, empDecryptedCC);
```

## Data Integrity Verification

The example includes a comprehensive integrity check:

```csharp
public static bool VerifyDataIntegrity(IEnumerable<Employee> originalEmployees, IEnumerable<Employee> decryptedEmployees)
{   
    var originalSorted = originalEmployees.OrderBy(e => e.EmployeeId).ToList();
    var decryptedSorted = decryptedEmployees.OrderBy(e => e.EmployeeId).ToList();
    
    bool dataIntegrityPreserved = originalSorted.Count == decryptedSorted.Count &&
        originalSorted.Zip(decryptedSorted, (orig, decr) => 
            // Check all employee properties match
            (orig.EmployeeId, orig.FirstName, orig.LastName, orig.HireDate, orig.Role, orig.Seeded, orig.CreditCards.Count) ==
            (decr.EmployeeId, decr.FirstName, decr.LastName, decr.HireDate, decr.Role, decr.Seeded, decr.CreditCards.Count) &&
            // Check all credit cards match exactly
             orig.CreditCards.OrderBy(cc => cc.CreditCardId).SequenceEqual(decr.CreditCards.OrderBy(cc => cc.CreditCardId))

        ).All(match => match);

    return dataIntegrityPreserved;  
}
```

This verification ensures:
- Same number of employees
- All employee properties are identical
- All credit card data is exactly preserved
- Encryption/decryption is lossless

## Key Security Concepts

### 1. PBKDF2 Key Derivation
```csharp
private byte[] Pbkdf2HashToBytes (int nrBytes, string password)
{
    byte[] registeredPasswordKeyDerivation = KeyDerivation.Pbkdf2(
        password: password,
        salt: Encoding.UTF8.GetBytes(_aesOption.Salt),
        prf: KeyDerivationPrf.HMACSHA512,
        iterationCount: _aesOption.Iterations,
        numBytesRequested: nrBytes);

    return registeredPasswordKeyDerivation;
}
```

**Benefits:**
- Strengthens keys through multiple iterations
- Resistant to rainbow table attacks
- Uses cryptographically secure salt

### 2. AES Symmetric Encryption
**Advantages:**
- Fast and efficient for large data
- Industry standard (approved by NSA)
- Same key for encryption and decryption

**Use Cases:**
- Encrypting data at rest
- Bulk data encryption
- When both parties have secure access to the key

### 3. Initialization Vector (IV)
- Ensures same plaintext produces different ciphertext each time
- Prevents pattern analysis in encrypted data
- Must be unique for each encryption operation

## Best Practices

### ✅ Do
- Use strong, randomly generated keys
- Store keys securely (Key Vault, Secret Manager)
- Use unique IVs for each encryption
- Implement proper key rotation
- Verify data integrity after decryption
- Use established cryptographic libraries

### ❌ Don't
- Hard-code encryption keys in source code
- Reuse IVs with the same key
- Use weak or predictable keys
- Implement custom cryptographic algorithms
- Store keys with encrypted data
- Skip integrity verification

### ⚠️ Production Considerations

The example includes this important warning:
```csharp
//Note: These are just example values for simplicity. Do not use in production.
//Always use securely generated keys, IVs, and salts stored in a safe manner, 
//i.e. user secret manager or cloud key vaults.
//See SeidoStackSimple project for correct implementation examples.
```

**Production Requirements:**
- Use Azure Key Vault, AWS KMS, or similar
- Implement proper key lifecycle management
- Use Hardware Security Modules (HSMs) for high-security scenarios
- Regular security audits and key rotation
- Comply with relevant regulations (PCI DSS for credit cards)

## Functional Programming Aspects

The encryption examples demonstrate functional programming principles:

### 1. Immutability
```csharp
var empWithObfuscatedCC = employees
    .Select(e => 
        e with { CreditCards = e.CreditCards  // Creates new Employee with modified CreditCards
            .Select( cc => _ccObfuscator(cc).obfuscated )
            .ToImmutableList() });
```

### 2. Pure Functions
The `_ccObfuscator` function is pure - same input always produces same output with no side effects.

### 3. Function Composition
```csharp
// Chain of transformations: Employee -> Encrypted -> Decrypted -> Verified
employees
    .Select(encrypt)
    .Select(decrypt)
    .Where(verify)
```

### 4. Type Safety
Generic methods ensure compile-time type safety:
```csharp
public string AesEncryptToBase64<T>(T sourceToEncrypt)
public T AesDecryptFromBase64<T>(string encryptedBase64)
```

## Summary

Encryption is a critical security practice that transforms readable data into protected format. Our examples demonstrate:

- **Obfuscation** for hiding data from casual viewing
- **AES encryption** for true cryptographic security  
- **Key management** through PBKDF2 key derivation
- **Data integrity verification** to ensure encryption/decryption accuracy
- **Functional programming approaches** using immutable transformations

The implementation shows practical patterns for encrypting sensitive data like credit card information while maintaining data integrity and following security best practices. Remember that production systems require proper key management, secure storage, and compliance with relevant security standards.