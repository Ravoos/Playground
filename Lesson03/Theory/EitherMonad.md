# Either Type - Functional Programming Explanation

## Overview

The [EitherType.cs](Extensions/EitherType.cs) implements the **Either** type, a fundamental functional programming construct that represents a value that can be one of two possible types. Either is typically used for error handling where the left side represents an error state and the right side represents a success state. This provides a type-safe, explicit way to handle operations that can fail without using exceptions or null values.

## Implementation Analysis

```csharp
public abstract class Either<T1, T2> { }

public class Left<T1, T2> : Either<T1, T2>
{
    public Left(T1 value) => Value = value;
    public T1 Value { get; init; }
}

public class Right<T1, T2> : Either<T1, T2>
{
    public Right(T2 value) => Value = value;
    public T2 Value { get; init; }
}
```

### Key Components:

1. **Abstract Base**: `Either<T1, T2>` - Represents a value that is either of type `T1` or type `T2`
2. **Left<T1, T2>**: Traditionally represents the "error" or "failure" path, containing a value of type `T1`
3. **Right<T1, T2>**: Traditionally represents the "success" or "happy" path, containing a value of type `T2`
4. **Biased Design**: By convention, Either is "right-biased" - the right side is the expected success case

## Functional Programming Relevance

### 1. **Explicit Error Handling**
Either makes error handling explicit in the type system, forcing developers to handle both success and failure cases:

```csharp
// From EmployeeEitherExamples - explicit error or success handling
var validationResult = ValidateEmployee(employee);
switch (validationResult)
{
    case Left<string, Employee> error:
        Console.WriteLine($"Employee validation failed: {error.Value}");  // Error case
        break;
    case Right<string, Employee> success:
        Console.WriteLine($"Employee validation passed: {success.Value.FirstName}"); // Success case
        break;
}
```

### 2. **Type-Safe Error Representation**
Either provides type-safe error representation without exceptions:

```csharp
// Traditional exception-based approach (breaks control flow)
public Employee ValidateEmployee(Employee emp) {
    if (string.IsNullOrEmpty(emp.FirstName)) 
        throw new ArgumentException("First name is required");
    return emp;
}

// Either approach (maintains control flow)
public Either<string, Employee> ValidateEmployee(Employee emp) {
    if (string.IsNullOrEmpty(emp.FirstName))
        return new Left<string, Employee>("First name is required");
    return new Right<string, Employee>(emp);
}
```

### 3. **Composable Error Handling**
Either enables chaining operations while preserving error information:

```csharp
// Conceptual chaining (would require additional extension methods)
var result = ValidateEmployee(employee)
    .Bind(CalculateSalary)
    .Bind(CheckPromotion)
    .Map(FormatResult);
// Chain stops at first Left (error), propagating the error through the pipeline
```

### 4. **Rich Error Information**
Either can carry detailed error information while maintaining type safety:

```csharp
// From the examples - detailed business validation
static Either<string, Employee> ValidateEmployee(Employee employee)
{
    if (string.IsNullOrEmpty(employee.FirstName))
        return new Left<string, Employee>("First name is required");
    
    if (employee.HireDate > DateTime.Now)
        return new Left<string, Employee>("Hire date cannot be in the future");
    
    if (employee.HireDate < DateTime.Now.AddYears(-50))
        return new Left<string, Employee>("Hire date seems too old - please verify");
    
    return new Right<string, Employee>(employee); // Success case
}
```

## Advanced Use Cases from the Codebase

### Business Logic Validation
The examples demonstrate comprehensive business rule validation:

```csharp
static Either<string, decimal> CalculateEmployeeSalary(Employee employee)
{
    var tenure = DateTime.Now.Year - employee.HireDate.Year;
    
    // Input validation
    if (tenure < 0)
        return new Left<string, decimal>("Invalid tenure calculation");
    
    var baseSalary = employee.Role switch
    {
        WorkRole.Management => 85000m,
        WorkRole.Veterinarian => 75000m,
        WorkRole.ProgramCoordinator => 60000m,
        WorkRole.AnimalCare => 45000m,
        WorkRole.Maintenance => 40000m,
        _ => 0m
    };
    
    // Business rule validation
    if (baseSalary == 0)
        return new Left<string, decimal>($"No salary information available for role: {employee.Role}");
    
    // Success calculation
    var experienceBonus = tenure * 2000m;
    var totalSalary = baseSalary + experienceBonus;
    
    return new Right<string, decimal>(totalSalary);
}
```

### Complex Decision Making
Either handles sophisticated business decisions with clear success/failure paths:

```csharp
static Either<string, string> CheckEmployeePromotion(Employee employee)
{
    var tenure = DateTime.Now.Year - employee.HireDate.Year;
    
    // Eligibility check
    if (tenure < 1)
        return new Left<string, string>("Employee must have at least 1 year of service");
    
    // Complex pattern matching for promotion rules
    var promotion = (employee.Role, tenure) switch
    {
        (WorkRole.AnimalCare, >= 5) => "Senior Animal Care Specialist - $52,000 base salary",
        (WorkRole.AnimalCare, >= 3) => "Lead Animal Care Technician - $48,000 base salary",
        (WorkRole.Veterinarian, >= 8) => "Chief Veterinarian - $95,000 base salary",
        (WorkRole.ProgramCoordinator, >= 6) => "Senior Program Manager - $72,000 base salary",
        (WorkRole.Maintenance, >= 10) => "Facilities Manager - $55,000 base salary",
        (WorkRole.Management, >= 7) => "Senior Management - $100,000 base salary",
        _ => null
    };
    
    // Result based on rule evaluation
    if (promotion == null)
        return new Left<string, string>($"No promotion opportunities available for {employee.Role} with {tenure} years experience");
    
    return new Right<string, string>(promotion);
}
```

### Revenue Calculation with Error Handling
Either provides robust error handling for complex calculations:

```csharp
static Either<string, decimal> CalculateGroupRevenue(MusicGroup group)
{
    // Precondition validation
    if (!group.Albums.Any())
        return new Left<string, decimal>("No albums available for revenue calculation");
    
    try
    {
        // Complex revenue calculations
        var digitalSales = group.Albums.Sum(a => a.CopiesSold * 8.99m);
        var physicalSales = group.Albums.Sum(a => a.CopiesSold * 0.3m * 15.99m);
        var streamingRevenue = group.Albums.Sum(a => a.Tracks.Count * 1000 * 0.003m);
        
        var totalRevenue = digitalSales + physicalSales + streamingRevenue;
        
        return new Right<string, decimal>(totalRevenue);
    }
    catch (OverflowException)
    {
        return new Left<string, decimal>("Revenue calculation resulted in overflow - sales figures may be too large");
    }
}
```

### Business Negotiation Logic
Either excels at modeling business processes with multiple outcomes:

```csharp
static Either<string, string> NegotiateRecordDeal(MusicGroup group)
{
    var totalSales = group.Albums.Sum(a => a.CopiesSold);
    var yearsActive = DateTime.Now.Year - group.EstablishedYear;
    
    // Eligibility screening
    if (yearsActive < 2)
        return new Left<string, string>("Group too new for major record deal consideration");
    
    if (totalSales < 5000)
        return new Left<string, string>("Insufficient sales history for record deal");
    
    // Tiered deal structure based on success metrics
    var dealTerms = (totalSales, yearsActive) switch
    {
        (> 1_000_000, > 15) => "Major label deal: 3 albums, $2M advance, 15% royalties",
        (> 500_000, > 10) => "Independent label deal: 2 albums, $500K advance, 18% royalties",
        (> 100_000, > 5) => "Regional label deal: 1 album, $100K advance, 20% royalties",
        (> 25_000, > 3) => "Indie label deal: 1 album, $25K advance, 25% royalties",
        _ => "Demo deal: EP only, no advance, 30% royalties"
    };
    
    return new Right<string, string>(dealTerms);
}
```

## Functional Programming Benefits

### 1. **Explicit Error Propagation**
Either makes error handling explicit and prevents errors from being ignored:
- Compiler forces handling of both cases
- No hidden exceptions or null values
- Clear contract about what can go wrong

### 2. **Composability**
Either operations can be composed and chained (with proper extension methods):
```csharp
// Conceptual pipeline - each step can fail
var result = input
    .Validate()           // Either<Error, ValidInput>
    .Bind(Process)        // Either<Error, ProcessedData>
    .Bind(Transform)      // Either<Error, TransformedData>
    .Map(Finalize);       // Either<Error, FinalResult>
```

### 3. **Type Safety**
Either provides compile-time guarantees about error handling:
```csharp
// Method signatures tell the complete story
Either<string, Employee> ValidateEmployee(Employee emp);     // Can fail with string error
Either<ValidationError, decimal> CalculateSalary(Employee); // Can fail with structured error
Employee CreateEmployee(string name);                       // Cannot fail (or throws)
```

### 4. **Performance Benefits**
- No exception throwing/catching overhead
- Predictable control flow
- Clear branching logic

### 5. **Testability**
- Easy to test both success and failure paths
- Deterministic error conditions
- No hidden side effects

## Mathematical Foundation

Either is based on the **sum type** or **tagged union** from type theory:

```
Either<A, B> = Left<A> + Right<B>
```

This represents exactly one of two possible values, following these properties:
- **Totality**: Every Either is either Left or Right, never both, never neither
- **Discriminated Union**: The type system can distinguish between the cases
- **Right-Biased**: By convention, Right represents success

## Comparison with Other Patterns

### vs. Maybe/Option
```csharp
// Maybe - represents presence/absence
Maybe<Employee> employee = FindEmployee(id);  // Something, Nothing, or Error

// Either - represents success/failure with error details
Either<string, Employee> employee = ValidateEmployee(data);  // Left(error) or Right(employee)
```

### vs. Exception Handling
```csharp
// Exception-based (breaks control flow)
try {
    var employee = ValidateEmployee(data);
    var salary = CalculateSalary(employee);
    return ProcessSalary(salary);
} catch (ValidationException ex) {
    return HandleValidationError(ex);
} catch (CalculationException ex) {
    return HandleCalculationError(ex);
}

// Either-based (normal control flow)
return ValidateEmployee(data)
    .Bind(CalculateSalary)
    .Bind(ProcessSalary)
    .Match(
        error => HandleError(error),
        success => success
    );
```

### vs. Result Pattern
```csharp
// Result pattern (typically boolean success flag)
var result = TryValidateEmployee(data);
if (result.Success) { /* use result.Value */ }
else { /* handle result.Error */ }

// Either (type-safe, composable)
var result = ValidateEmployee(data);
switch (result) {
    case Left<string, Employee> error: /* handle error.Value */ break;
    case Right<string, Employee> success: /* use success.Value */ break;
}
```

## Enhanced Either Implementation

In comming lessons I will be adding these extension methods:

```csharp
public static class EitherExtensions
{
    // Functor (Map) - transforms the Right value
    public static Either<TLeft, TRightNew> Map<TLeft, TRight, TRightNew>(
        this Either<TLeft, TRight> either, 
        Func<TRight, TRightNew> func)
    {
        return either switch
        {
            Left<TLeft, TRight> left => new Left<TLeft, TRightNew>(left.Value),
            Right<TLeft, TRight> right => new Right<TLeft, TRightNew>(func(right.Value)),
            _ => throw new InvalidOperationException()
        };
    }

    // Monad (Bind) - chains Either operations
    public static Either<TLeft, TRightNew> Bind<TLeft, TRight, TRightNew>(
        this Either<TLeft, TRight> either,
        Func<TRight, Either<TLeft, TRightNew>> func)
    {
        return either switch
        {
            Left<TLeft, TRight> left => new Left<TLeft, TRightNew>(left.Value),
            Right<TLeft, TRight> right => func(right.Value),
            _ => throw new InvalidOperationException()
        };
    }

    // Match - provides exhaustive case handling
    public static TResult Match<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TResult> onLeft,
        Func<TRight, TResult> onRight)
    {
        return either switch
        {
            Left<TLeft, TRight> left => onLeft(left.Value),
            Right<TLeft, TRight> right => onRight(right.Value),
            _ => throw new InvalidOperationException()
        };
    }

    // Check if Either is Right
    public static bool IsRight<TLeft, TRight>(this Either<TLeft, TRight> either) =>
        either is Right<TLeft, TRight>;

    // Check if Either is Left
    public static bool IsLeft<TLeft, TRight>(this Either<TLeft, TRight> either) =>
        either is Left<TLeft, TRight>;
}
```

## Usage Patterns

### Error Accumulation
```csharp
// For collecting multiple validation errors
public class ValidationErrors
{
    public List<string> Errors { get; init; } = new();
}

public static Either<ValidationErrors, Employee> ValidateEmployeeFull(Employee emp)
{
    var errors = new ValidationErrors();
    
    if (string.IsNullOrEmpty(emp.FirstName))
        errors.Errors.Add("First name is required");
    
    if (string.IsNullOrEmpty(emp.LastName))
        errors.Errors.Add("Last name is required");
    
    if (emp.HireDate > DateTime.Now)
        errors.Errors.Add("Hire date cannot be in future");
    
    return errors.Errors.Any() 
        ? new Left<ValidationErrors, Employee>(errors)
        : new Right<ValidationErrors, Employee>(emp);
}
```

### Railway-Oriented Programming
```csharp
// Chaining operations that can fail
public static Either<string, ProcessedEmployee> ProcessEmployee(Employee emp) =>
    ValidateEmployee(emp)
        .Bind(CalculateBenefits)
        .Bind(AssignWorkspace)
        .Bind(CreateBadge)
        .Map(FinalizeEmployeeRecord);
```

## Best Practices

1. **Use Descriptive Error Types**: Instead of just `string`, use specific error types
2. **Follow Right-Bias Convention**: Right for success, Left for failure
3. **Make Errors Actionable**: Include enough information to handle the error
4. **Avoid Exceptions in Either Functions**: Either should not throw exceptions
5. **Use Pattern Matching**: Leverage C# pattern matching for clean Either handling
6. **Consider Error Hierarchies**: Use inheritance for structured error types

## Error Type Design

```csharp
// Structured error types instead of strings
public abstract record ValidationError;
public record NameRequiredError : ValidationError;
public record InvalidDateError(DateTime ProvidedDate) : ValidationError;
public record InvalidRoleError(string ProvidedRole) : ValidationError;

// Usage with structured errors
Either<ValidationError, Employee> ValidateEmployee(Employee emp) =>
    string.IsNullOrEmpty(emp.FirstName) 
        ? new Left<ValidationError, Employee>(new NameRequiredError())
        : new Right<ValidationError, Employee>(emp);
```

## Integration with Other Functional Patterns

Either works seamlessly with other functional programming constructs:

```csharp
// With Maybe for optional operations
Either<string, Maybe<Employee>> result = 
    ValidateInput(data)
        .Map(FindEmployee)  // May return Maybe<Employee>
        .Bind(ProcessEmployeeIfFound);

// With collections for batch processing
var results = employees
    .Select(ValidateEmployee)
    .Partition(e => e.IsRight());  // Separate successes from failures

// With async operations
async Task<Either<string, Employee>> ValidateEmployeeAsync(Employee emp) =>
    await ValidateInDatabaseAsync(emp)
        ? new Right<string, Employee>(emp)
        : new Left<string, Employee>("Database validation failed");
```

The Either type is a powerful functional programming construct that brings type-safe, explicit, and composable error handling to C#. It eliminates entire classes of runtime errors while making business logic more expressive and maintainable. The implementation shown provides the foundation for building robust, predictable systems with excellent error handling characteristics.