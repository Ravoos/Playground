# Lesson 03 - Home Exercise Tasks
## Functional Programming Extensions with Friend Models

### Overview
In this exercise, you will practice using functional programming extensions (Map, Tap, Fork, Alt, Compose) and monadic patterns (Maybe, Either) with the Friend model data. You have access to four different Friend objects with varying data completeness:

- `friendNoAddress`: A friend without address information
- `friendNoPets`: A friend with no pets
- `friendNoQuotes`: A friend with no quotes
- `friendHasMany`: A friend with complete data (address, multiple pets, multiple quotes)

### Prerequisites
- Understanding of extension methods in C#
- Basic knowledge of functional programming concepts
- Familiarity with the Friend, Address, Pet, and Quote models

---

## Exercise 1: Map Extension
**Objective**: Transform friend data using the Map extension method.

### Tasks:
1. **Single Transformation**: Use Map to extract the full name from `friendHasMany`
2. **Conditional Mapping**: Use Map to calculate and display age information from birthday data, handling cases where birthday might be null
3. **Multiple Transformations**: Use Map with multiple transformation functions to:
   - Convert `friendNoAddress`'s first and last names to uppercase
   - Convert the email to lowercase
   - Display the updated friend information
4. **Complex Mapping**: Use Map to create an anonymous object summary containing:
   - Full name
   - Whether the friend has an address
   - Number of pets
   - Number of quotes
   - Contact email

---

## Exercise 2: Tap Extension
**Objective**: Add side effects and logging without modifying the data flow.

### Tasks:
1. **Logging Chain**: Create a processing chain using `friendHasMany` tha Logs To Console:
   - Logs "Processing friend: [Name]"
   - Logs email information
   - Logs address availability
   - Logs pet count
   - Logs quote count
   - Maps to a completion message
2. **Validation Logging**: Use `friendNoAddress` to create a validation chain that:
   - Logs the validation start
   - Logs missing address warning
   - Logs pet information (present or missing)

---

## Exercise 3: Fork Extension
**Objective**: Perform parallel operations on the same data.

### Tasks:
1. **Parallel Data Extraction**: Use Fork with `friendHasMany` to:
   - Extract all pet names (first function)
   - Extract all unique quote authors (second function)
   - Combine results into a readable format (combine function)
2. **Score Calculations**: Use Fork with `friendHasMany` to calculate:
   - Contact score: 1 point each for address and email availability
   - Social score: sum of pets count and quotes count
   - Combine into a formatted analysis string
3. **Comparison Analysis**: Use Fork with `friendNoAddress` to:
   - Check pet status ("Has Pets" or "No Pets")
   - Check address status ("Has Address" or "No Address")
   - Combine into a comparison summary

---

## Exercise 4: Alt Extension
**Objective**: Provide fallback values when data is unavailable.

### Tasks:
1. **Address Fallbacks**: Use Alt with `friendNoAddress` to try:
   - Street address (first choice)
   - City name (second choice)
   - "No address available" (final fallback)
2. **Pet Information**: Use Alt with `friendNoPets` to try:
   - First pet's name
   - Pet count summary ("X pets")
   - "No pets found" fallback
3. **Quote Information**: Use Alt with `friendNoQuotes` to try:
   - First quote text
   - First quote author
   - "No quotes available" fallback
4. **Contact Method**: Use Alt with `friendHasMany` to prioritize:
   - Email contact information
   - Address-based contact
   - "No contact method available" fallback

---

## Exercise 5: Compose Extension
**Objective**: Chain functions together for complex transformations.

### Tasks:
1. **Greeting Pipeline**: Create a composed function that:
   - Extracts full name from Friend
   - Converts to uppercase
   - Adds "Mr./Ms." title
   - Adds greeting prefix
   - Test with `friendHasMany`
2. **Pet Analysis Pipeline**: Create a composed function that:
   - Gets pet count from Friend
   - Formats count as "Count: X"
   - Adds pet emoji
   - Test with `friendHasMany`
3. **Email Domain Analysis**: Create a composed function that:
   - Extracts email from Friend
   - Extracts domain part after @
   - Formats as "Domain: [domain]"
   - Test with `friendNoAddress`

---

## Exercise 6: Maybe Monad
**Objective**: Handle nullable values safely using the Maybe monad pattern.

### Tasks:
1. **Address Handling**: Create a `GetAddressInfo` method that:
   - Returns `Nothing<string>` if address is null
   - Returns `Something<string>` with formatted address if present
   - Returns `Error<string>` if exception occurs
   - Test with both `friendNoAddress` and `friendHasMany`

2. **Age Calculation**: Create a `CalculateAge` method that:
   - Returns `Nothing<int>` if birthday is null
   - Returns `Something<int>` with calculated age
   - Handles leap year considerations
   - Returns `Error<int>` on exceptions
   - Test with `friendHasMany`

3. **Pet Selection**: Create a `GetFavoritePet` method that:
   - Returns `Nothing<string>` if no pets exist
   - Returns `Something<string>` with random pet info
   - Includes pet name, kind, and mood in result
   - Test with both `friendNoPets` and `friendHasMany`

4. **Quote Selection**: Create a `GetRandomQuote` method that:
   - Returns `Nothing<string>` if no quotes exist
   - Returns `Something<string>` with formatted quote and author
   - Test with both `friendNoQuotes` and `friendHasMany`

5. **Result Display**: Create a `DisplayMaybeResult` method that:
   - Handles `Something<T>` cases with success message
   - Handles `Nothing<T>` cases with info message
   - Handles `Error<T>` cases with error message
   - Uses appropriate emoji/symbols for each case

---

## Exercise 7: Either Monad
**Objective**: Handle success/error cases using the Either monad pattern.

### Tasks:
1. **Friend Validation**: Create a `ValidateFriend` method that:
   - Returns `Left<string, string>` with error list if validation fails
   - Returns `Right<string, string>` with success message if valid
   - Checks for: first name, last name, email, address
   - Test with both `friendNoAddress` and `friendHasMany`

2. **Email Validation**: Create a `ValidateEmail` method that:
   - Returns `Left<string, string>` for validation failures
   - Returns `Right<string, string>` for valid emails
   - Checks for: empty email, missing @, format issues
   - Test with valid email and "invalid-email" string

3. **Social Score Calculation**: Create a `CalculateSocialScore` method that:
   - Returns `Left<string, int>` if score is too low (< 30)
   - Returns `Right<string, int>` for good social scores
   - Scoring: FirstName(10), Email(10), Address(20), Birthday(10), Pets(5 each), Quotes(3 each)
   - Test with both `friendNoPets` and `friendHasMany`

4. **Result Display**: Create a `DisplayEitherResult` method that:
   - Handles `Right<TLeft, TRight>` cases with success message
   - Handles `Left<TLeft, TRight>` cases with failure message
   - Uses appropriate emoji/symbols for each case

---

## Implementation Guidelines

### Code Structure
- Organize each exercise in its own region (`#region Exercise X`)
- Create private static helper methods for complex operations
- Use meaningful variable names and include comments

### Error Handling
- Always use try-catch blocks in Maybe monad methods
- Provide meaningful error messages
- Handle edge cases (null values, empty collections)

### Output Formatting
- Use consistent console output formatting
- Include exercise headers and descriptions
- Use emojis or symbols to make output more readable
- Display results clearly with context

### Testing
- Test each extension with appropriate Friend objects
- Demonstrate both success and failure scenarios
- Show how data flows through transformations

### Functional Programming Principles
- Avoid side effects except in Tap operations
- Keep functions pure when possible
- Use immutable transformations (with records)
- Chain operations fluently

---

## Expected Learning Outcomes

After completing these exercises, you should understand:

1. **Map Extension**: How to transform data without changing structure
2. **Tap Extension**: How to add side effects while maintaining data flow
3. **Fork Extension**: How to perform parallel operations and combine results
4. **Alt Extension**: How to provide fallback mechanisms for missing data
5. **Compose Extension**: How to build complex operations from simple functions
6. **Maybe Monad**: How to handle nullable values safely without exceptions
7. **Either Monad**: How to represent success/failure states explicitly

These patterns are fundamental in functional programming and provide safer, more expressive alternatives to traditional imperative error handling and data transformation approaches.