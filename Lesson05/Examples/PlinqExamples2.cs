using Encryptions;
using Encryptions.Options;
using Models.Employees;
using Seido.Utilities.SeedGenerator;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Playground.Lesson05.Examples;

public static class PlinqExamples2
{
    public static void RunExamples()
    {
        Console.WriteLine("\n=== Credit Card Encryption Plinq Examples ===\n");

        var seeder = new SeedGenerator();
        var creditcards = seeder.ItemsToList<CreditCard>(1_000_000);

        var timer = new Stopwatch();

        // LINQ Encryption Example
        Console.WriteLine("---- LINQ Encryption Example ----");
        CreditCardEncryptionLinq(timer, creditcards);
        
        // PLINQ Encryption Example
        Console.WriteLine("---- PLINQ Encryption Example ----");
        CreditCardEncryptionPlinq(timer, creditcards);

        Console.WriteLine("\n=== End of Credit Card Encryption Plinq Examples ===\n");
    }
    
    private static void CreditCardEncryptionLinq(Stopwatch timer, IList<CreditCard> creditCards)
    {
        var encryptor = new EncryptionEngine(AesEncryptionOptions.Default());

        System.Console.WriteLine("Credit Cards:");
        Console.WriteLine(FormatCreditCards(creditCards.Take(5)));

        timer.Start();
        var encryptedCreditCards = creditCards.Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc))).ToImmutableList();
        timer.Stop();

        System.Console.WriteLine("Encrypted Credit Cards:");
        Console.WriteLine(FormatEncryptedCreditCards(encryptedCreditCards.Take(5)));
        Console.WriteLine($"LINQ Encryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms\n");

        timer.Reset();
        timer.Start();
        var empDecryptedCC = encryptedCreditCards.Select(cc => encryptor.AesDecryptFromBase64<CreditCard>(cc.Item2))
                            .ToImmutableList();
        timer.Stop();

        System.Console.WriteLine("Decrypted Credit Cards:");
        Console.WriteLine(FormatCreditCards(empDecryptedCC.Take(5)));
        Console.WriteLine($"LINQ Decryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms\n");


        // Data Integrity Verification
        Console.WriteLine("Data Integrity Verification:");
        Console.WriteLine(VerifyDataIntegrity(creditCards, empDecryptedCC)
            ? "   Data integrity verified: Decrypted data matches original."
            : "   Data integrity check failed: Decrypted data does not match original.");
    }

    private static void CreditCardEncryptionPlinq(Stopwatch timer, IList<CreditCard> creditCards)
    {
        var encryptor = new EncryptionEngine(AesEncryptionOptions.Default());

        System.Console.WriteLine("Credit Cards:");
        Console.WriteLine(FormatCreditCards(creditCards.Take(5)));

        timer.Start();
        var encryptedCreditCards = creditCards.AsParallel().Select(cc => (cc.CreditCardId, encryptor.AesEncryptToBase64(cc))).ToImmutableList();
        timer.Stop();

        System.Console.WriteLine("Encrypted Credit Cards:");
        Console.WriteLine(FormatEncryptedCreditCards(encryptedCreditCards.Take(5)));
        Console.WriteLine($"PLINQ Encryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms\n");

        timer.Reset();
        timer.Start();
        var empDecryptedCC = encryptedCreditCards.AsParallel().Select(cc => encryptor.AesDecryptFromBase64<CreditCard>(cc.Item2))
                            .ToImmutableList();
        timer.Stop();

        System.Console.WriteLine("Decrypted Credit Cards:");
        Console.WriteLine(FormatCreditCards(empDecryptedCC.Take(5)));
        Console.WriteLine($"PLINQ Decryption of {encryptedCreditCards.Count:N0} items completed in {timer.ElapsedMilliseconds} ms\n");

        // Data Integrity Verification
        Console.WriteLine("Data Integrity Verification:");
        Console.WriteLine(VerifyDataIntegrity(creditCards, empDecryptedCC)
            ? "   Data integrity verified: Decrypted data matches original."
            : "   Data integrity check failed: Decrypted data does not match original.");
    }

    private static string FormatCreditCards(IEnumerable<CreditCard> creditCards)
    {
        return creditCards
            .Select(cc => $"  - {cc.Issuer}: {cc.Number} (Expires: {cc.ExpirationMonth}/{cc.ExpirationYear})")
            .Aggregate("", (acc, ccInfo) => acc + ccInfo + "\n");
    }

    private static string FormatEncryptedCreditCards(IEnumerable<(Guid creditCardId, string encryptedToken)> encryptedCreditCards)
    {
        return encryptedCreditCards
                .Select(cc => $"    - CreditCard ID: {cc.creditCardId}, Encrypted Data: {cc.encryptedToken}")
                .Aggregate("", (acc, ccInfo) => acc + ccInfo + "\n");
    }

    public static bool VerifyDataIntegrity(IEnumerable<CreditCard> originalCreditCards, IEnumerable<CreditCard> decryptedCreditCards)
    {   
        var originalSorted = originalCreditCards.OrderBy(e => e.CreditCardId).ToList();
        var decryptedSorted = decryptedCreditCards.OrderBy(e => e.CreditCardId).ToList();

        return originalSorted.SequenceEqual(decryptedSorted);
    }
}