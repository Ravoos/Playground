# Lazy Loading Exercise - Lesson 05

## Objective
Examine the existing `MusicGroup` model in `Models.Music.Lazy` to understand how lazy loading works with complex object relationships, and then create your own `EmployeeLazy` model.

## Background
In many applications, loading all related data immediately (eager loading) can be expensive and wasteful if not all data is used. Lazy loading defers the loading of expensive data until it's actually needed.

## Exercise Tasks

### Preparation 1: Examine the MusicGroup Lazy Implementation
- Look at the `MusicGroup` record in `Models/MusicLazy/MusicGroup.cs`
- Notice how `Albums` and `Artists` are wrapped in `Lazy<ImmutableList<T>>`
- Observe the lazy initialization in the `Seed` method
- Run the `LazyMusicGroupExamples.RunExamples()` to see lazy loading in action

### Exercise 1: Create Your Own EmployeeLazy Model
Based on what you learned from examining `MusicGroup`, create an `EmployeeLazy` model that:
- Has the same basic properties as the regular `Employee` model
- Wraps `CreditCards` in `Lazy<ImmutableList<CreditCard>>`
- Implements proper lazy initialization in the `Seed` method
- Includes console output to show when lazy loading occurs

### Preparation 2: Key Concepts to Understand
From examining the MusicGroup lazy implementation, focus on:
- `Albums.IsValueCreated` / `Artists.IsValueCreated` - Check if lazy value has been loaded
- `Albums.Value` / `Artists.Value` - Access the lazy value (triggers loading if not loaded)
- Console output showing when lazy loading occurs
- How the `Seed` method creates `Lazy<T>` instances with factory functions

### Exercise 2: Implement EmployeeLazy Exercise Methods
Create methods to demonstrate:
- Selective loading of credit cards based on employee criteria
- Conditional loading based on work roles (Management, Veterinarian, etc.)

