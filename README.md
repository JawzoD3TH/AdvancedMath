# Advanced Math

AdvancedMath is a cutting-edge, high-performance generic mathematical library for .NET, designed to compute advanced operations, even parallel processing when feasible.  
Built with modern C# features and targeting .NET 9, it delivers optimal performance and includes a wide range of advanced mathematical functions and utilities. 
The library supports both synchronous and asynchronous methods for various numerical computations.

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