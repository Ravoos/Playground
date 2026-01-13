# Employee Encryption Exercise

## Objective
Create a comprehensive employee encryption system that demonstrates the full encryption/decryption lifecycle for sensitive employee data using LINQ and functional programming patterns.

## Background
In the CreditCardEncryption example, we saw how to encrypt individual credit cards within employee records. This exercise extends that concept to encrypt entire Employee objects, demonstrating how to handle complex object encryption in a functional programming style.

## Requirements

### Part 1: Setup and Data Preparation
1. Create a new static class `EmployeeEncryptionExercise` in the `Lesson05/Examples` folder
2. Generate a list of 20-50 employees using `SeedGenerator`
3. Create an `EncryptionEngine` instance with default AES encryption options

### Part 2: Employee Obfuscation
Create an employee obfuscation function that masks sensitive information:
- **FirstName**: Replace middle characters with asterisks (e.g., "Martin" → "M****n")
- **LastName**: Replace middle characters with asterisks (e.g., "Johnson" → "J*****n") 
- **HireDate**: Keep only the year (e.g. use new DateTime(emp.HireDate.Year, 1, 1))
- **EmployeeId**: Keep unchanged (needed for identification)
- **Role**: Keep unchanged (not sensitive)
- **CreditCards**: Use the existing `_ccObfuscator` from CreditCardEncryption

**LINQ Pattern to Use:**
```csharp
var obfuscatedEmployees = employees
    .Select(emp => emp with { /* obfuscated properties */ })
    .ToList();
```

### Part 3: Full Employee Encryption
Encrypt entire Employee objects to demonstrate complex object serialization:
1. Convert each Employee to an encrypted Base64 string
2. Store the mapping as `(EmployeeId, EncryptedEmployeeData)`
3. Display the first 3 encrypted employee records

**LINQ Pattern to Use:**
```csharp
var encryptedEmployees = employees
    .Select(emp => (emp.EmployeeId, encryptor.AesEncryptToBase64(emp)))
    .ToList();
```

### Part 4: Employee Decryption and Verification
1. Decrypt the encrypted employee data back to Employee objects
2. Compare the decrypted employees with the original employees
3. Use LINQ to verify data integrity

**LINQ Patterns to Use:**
```csharp
var decryptedEmployees = encryptedEmployees
    .Select(encrypted => encryptor.AesDecryptFromBase64<Employee>(encrypted.Item2))
    .ToList();

// Verification using LINQ
var areEqual = originalEmployees
    .OrderBy(e => e.EmployeeId)
    .SequenceEqual(decryptedEmployees.OrderBy(e => e.EmployeeId));
```

### Part 5: Advanced LINQ Operations
Demonstrate LINQ operations on encrypted data:
1. **Group Analysis**: Group employees by role and show count of encrypted records per role
2. **Filter Operations**: Find all encrypted employees hired after 2020
3. **Statistical Analysis**: Calculate average number of credit cards per encrypted employee
4. **Join Operations**: Join original employees with their encrypted counterparts

## Expected Output Format
Your solution should produce output similar to:
```
=== Employee Encryption Exercise ===

1. Original Employees (showing 3):
Employee: John Smith (12345678-1234-1234-1234-123456789012)
  Role: Management, Hired: 2020-05-15
  Credit Cards: 2

2. Obfuscated Employees (showing 3):
Employee: J**n S***h (12345678-1234-1234-1234-123456789012)
  Role: Management, Hired: 2020-**-**
  Credit Cards: 2 (masked)

3. Encrypted Employees (showing 3):
Employee ID: 12345678-1234-1234-1234-123456789012
Encrypted Data: SGVsbG8gV29ybGQh... (truncated)

4. Decrypted Employees (showing 3):
[Same format as original employees]

5. Data Integrity Verification:
✅ Encryption/Decryption cycle preserved all data!

6. Statistical Analysis:
- Total employees processed: 50
- Employees by role: Management: 10, Veterinarian: 15, etc.
- Average credit cards per employee: 2.3
```
