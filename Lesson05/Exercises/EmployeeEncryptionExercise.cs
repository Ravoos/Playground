using Encryptions;
using Encryptions.Options;
using Models.Employees;
using Seido.Utilities.SeedGenerator;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Playground.Lesson05.Examples;

public static class EmployeeEncryptionExercise
{
    // Credit card obfuscator from CreditCardEncryption pattern
    static Func<CreditCard, (CreditCard original, CreditCard obfuscated)> _ccObfuscator = cc => {
        string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b"; 
        string replacement = "$1**** **** $4";

        return (cc, new CreditCard(
                            cc.CreditCardId,
                            cc.Issuer,
                            Regex.Replace(cc.Number, pattern, replacement),
                            "**",
                            "**"));
    };

    // Employee obfuscation function
    static Func<Employee, Employee> _employeeObfuscator = emp => {
        var obfuscatedFirstName = Regex.Replace(emp.FirstName, @"(?<=.).(?=.)", "*");
        var obfuscatedLastName =  Regex.Replace(emp.LastName, @"(?<=.).(?=.)", "*");
        var obfuscatedHireDate = new DateTime(emp.HireDate.Year, 1, 1); // Keep only year
        var obfuscatedCreditCards = emp.CreditCards
            .Select(cc => _ccObfuscator(cc).obfuscated)
            .ToImmutableList();

        return emp with {
            FirstName = obfuscatedFirstName,
            LastName = obfuscatedLastName,
            HireDate = obfuscatedHireDate,
            CreditCards = obfuscatedCreditCards
        };
    };

    public static void RunExercise()
    {
        Console.WriteLine("\n=== Employee Encryption Exercise ===\n");

        // Part 1: Setup and Data Preparation
        var seeder = new SeedGenerator();
        var employees = seeder.ItemsToList<Employee>(25);
        var encryptor = new EncryptionEngine(AesEncryptionOptions.Default());

        // Display original employees
        Console.WriteLine("1. Original Employees (showing 3):");
        Console.WriteLine(FormatEmployees(employees.Take(3)));

        // Part 2: Employee Obfuscation

        // Part 3: Full Employee Encryption

        // Part 4: Employee Decryption and Verification

        // Data Integrity Verification
        Console.WriteLine("\n=== End of Employee Encryption Exercise ===\n");
    }



    private static string FormatEmployees(IEnumerable<Employee> employees)
    {
        return employees
            .Select(emp => $"Employee: {emp.FirstName} {emp.LastName} ({emp.EmployeeId})\n" +
                          $"  Role: {emp.Role}, Hired: {emp.HireDate:yyyy-MM-dd}\n" +
                          $"  Credit Cards: {emp.CreditCards.Count}\n")
            .Aggregate("", (acc, empInfo) => acc + empInfo);
    }

    private static string FormatEmployeesObfuscated(IEnumerable<Employee> employees)
    {
        return employees
            .Select(emp => $"Employee: {emp.FirstName} {emp.LastName} ({emp.EmployeeId})\n" +
                          $"  Role: {emp.Role}, Hired: {emp.HireDate.Year}-**-**\n" +
                          $"  Credit Cards: {emp.CreditCards.Count} (masked)\n")
            .Aggregate("", (acc, empInfo) => acc + empInfo);
    }

    private static string FormatEncryptedEmployees(IEnumerable<(Guid EmployeeId, string EncryptedData)> encryptedEmployees)
    {
        return encryptedEmployees
            .Select(emp => $"Employee ID: {emp.EmployeeId}\n" +
                          $"Encrypted Data: {emp.EncryptedData.Substring(0, Math.Min(50, emp.EncryptedData.Length))}...\n" +
                          $"Data Size: {emp.EncryptedData.Length} characters\n")
            .Aggregate("", (acc, empInfo) => acc + empInfo);
    }
}