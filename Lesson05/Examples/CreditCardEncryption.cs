using Encryptions;
using Encryptions.Options;
using Models.Employees;
using Seido.Utilities.SeedGenerator;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Playground.Lesson05.Examples;

public static class EncryptionExamples
{
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

    public static void RunExamples()
    {
        Console.WriteLine("\n=== Credit Card Encryption Examples ===\n");

        var seeder = new SeedGenerator();
        var employees = seeder.ItemsToList<Employee>(100);

        CreditCardEncryption(employees);

        Console.WriteLine("\n=== End of Credit Card Encryption Examples ===\n");
    }

    private static void CreditCardEncryption(IList<Employee> employees)
    {
        var encryptor = new EncryptionEngine(AesEncryptionOptions.Default());

        System.Console.WriteLine("Employees with Credit Cards:");
        Console.WriteLine(FormatEmployeesWithCreditCards(employees.Take(5)));

        var empWithObfuscatedCC = employees
            .Select(e => 
                e with { CreditCards = e.CreditCards
                    .Select( cc => _ccObfuscator(cc).obfuscated )
                    .ToImmutableList() });

        System.Console.WriteLine("Employees with Obfuscated Credit Cards:");
        Console.WriteLine(FormatEmployeesWithCreditCards(empWithObfuscatedCC.Take(5)));


        var empEncryptedCC = employees
            .Select(e => 
                (e.EmployeeId,  
                 e.CreditCards.Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc))).ToImmutableList()));

        System.Console.WriteLine("Employees with Encrypted Credit Cards:");
        Console.WriteLine(FormatEmployeesWithEncryptedCreditCards(empEncryptedCC.Take(5)));


        var empDecryptedCC = employees.Join(empEncryptedCC, 
                e => e.EmployeeId, e => e.EmployeeId, 
                (original, encrypted) => original with { CreditCards = encrypted.Item2
                            .Select(cc => encryptor.AesDecryptFromBase64<CreditCard>(cc.Item2))
                            .ToImmutableList()});

        System.Console.WriteLine("Employees with Decrypted Credit Cards:");
        Console.WriteLine(FormatEmployeesWithCreditCards(empDecryptedCC.OrderBy(e => e.EmployeeId).Take(2)));


        // Data Integrity Verification
        Console.WriteLine("Data Integrity Verification:");
        Console.WriteLine(EncryptionExamples.VerifyDataIntegrity(employees, empDecryptedCC)
            ? "   Data integrity verified: Decrypted data matches original."
            : "   Data integrity check failed: Decrypted data does not match original.");
        Console.WriteLine("\n=== End of Employee Encryption Exercise ===\n");

    }

    private static string FormatEmployeesWithCreditCards(IEnumerable<Employee> employees)
    {
        return employees
            .Select(emp => $"\nEmployee: {emp.FirstName} {emp.LastName} ({emp.EmployeeId})\n  Has {emp.CreditCards.Count} credit card(s):\n" +
            emp.CreditCards
                .Select(cc => $"    - {cc.Issuer}: {cc.Number} (Expires: {cc.ExpirationMonth}/{cc.ExpirationYear})")
                .Aggregate("", (acc, ccInfo) => acc + ccInfo + "\n"))
            .Aggregate("", (acc, empInfo) => acc + empInfo);
    }

    private static string FormatEmployeesWithEncryptedCreditCards(IEnumerable<(Guid EmployeeId, ImmutableList<(Guid creditCardId, string encryptedToken)>)> employees)
    {
        return employees
            .Select(emp => $"\nEmployee ID: {emp.EmployeeId}\n  Has {emp.Item2.Count} encrypted credit card(s):\n" +
            emp.Item2
                .Select(cc => $"    - CreditCard ID: {cc.creditCardId}, Encrypted Data: {cc.encryptedToken}")
                .Aggregate("", (acc, ccInfo) => acc + ccInfo + "\n"))
            .Aggregate("", (acc, empInfo) => acc + empInfo);
    }

    public static bool VerifyDataIntegrity(IEnumerable<Employee> originalEmployees, IEnumerable<Employee> decryptedEmployees)
    {   
        var originalSorted = originalEmployees.OrderBy(e => e.EmployeeId).ToList();
        var decryptedSorted = decryptedEmployees.OrderBy(e => e.EmployeeId).ToList();
        
        bool dataIntegrityPreserved = originalSorted.Count == decryptedSorted.Count &&
            originalSorted.Zip(decryptedSorted, (orig, decr) => 

                (orig.EmployeeId, orig.FirstName, orig.LastName, orig.HireDate, orig.Role, orig.Seeded, orig.CreditCards.Count) ==
                (decr.EmployeeId, decr.FirstName, decr.LastName, decr.HireDate, decr.Role, decr.Seeded, decr.CreditCards.Count) &&
                 orig.CreditCards.OrderBy(cc => cc.CreditCardId).SequenceEqual(decr.CreditCards.OrderBy(cc => cc.CreditCardId))
    
            ).All(match => match);

        return dataIntegrityPreserved;  
    }
}