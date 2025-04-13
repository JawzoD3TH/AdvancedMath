# Advanced Math

AdvancedMath is a high-performance, generic mathematical library for .NET, designed to handle advanced mathematical operations with support for parallel processing where feasible.
It leverages modern C# features and targets .NET 9 for optimal performance. It provides advanced mathematical functions and utilities. It supports both synchronous and asynchronous
methods for various numerical operations.

## Features

•	Generic Math Support: Operates on any numeric type implementing INumber<T>.
•	Asynchronous Operations: Async versions of key methods for non-blocking execution.

- Standard Deviation
- Square Root
- Power
- Coefficient of Variation
- Z-Score
- Extension Methods for Numerical Operations

## Usage Example

```csharp

using AdvancedMath;

// Synchronous
var numbers = new double[] { 10, 20, 30 };
var result = Generic.CoefficientOfVariation(numbers);

// Asynchronous
var asyncResult = await Generic.CoefficientOfVariationAsync(numbers);

```

## Installation

To install AdvancedMath, add the project to your solution and reference it in your projects.