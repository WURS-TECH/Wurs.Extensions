# Wurs.Extensions
A set of .NET extension methods to use in your projects.

## Wurs.Extensions.ServiceCollection

`Wurs.Extensions.ServiceCollection` is a .NET library that provides Microsoft.Extensions.DependencyInjection.IServiceCollection extension methods.

### Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Installation](#installation)
  - [Basic Usage](#basic-usage)
- [Acknowledgments](#acknowledgments)
- [License](#license)

### Introduction
The library has the different extension methods organized as follows:

- `RegisterOptionsPatternExtension` provides an extension method to centralize the dependency injection of your options classes via custom attributes when using [OptionsPattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0) on your .NET applications.

### Features

- **OptionsPattern DI via attributes:** Mark your clases with the `RegisterOptionAttribute` to inject them on DI container.
- **DataAnnotations and ValidateOnStart:** You can decide whether your option classes will use the data annotations for validations and whether those validations should be executed at application startup, passing arguments directly to the `RegisterOptionAttribute`.
- **Environment and configuration:** You can map information directly from the environment or from configuration/settings files.

### Getting Started

#### Installation

You can install the library via NuGet Package Manager:

```c#
# Wurs.Extensions.ServiceCollection
Install-Package Wurs.Extensions.ServiceCollection
```
#### Basic Usage
Here's a simple example of using the Wurs.Extensions.ServiceCollection library.

Add the RegisterOptionAttribute in every class you want to configure using the OptionsPattern:
```c#
using Wurs.Extensions.ServiceCollection.Atributtes;
using Wurs.Extensions.ServiceCollection.Enums;

[RegisterOption(OptionType.Environment)]
public class ExampleOptions
{
    public string FirstSetting { get; set; }

    public int SecondSetting { get; set; }
}
```
The optionType argument is mandatory and define from where the configuration is located, To collect it from a settings or configuration file, use the enumeration : `OptionType.Settings`.

When no additional arguments are passed, DataAnnotations for validations will be ignored.

If you want to use the DataAnnotations add them and pass the second argument as true:
```c#
using Wurs.Extensions.ServiceCollection.Atributtes;
using Wurs.Extensions.ServiceCollection.Enums;

[RegisterOption(OptionType.Settings, true)]
public class ExampleOptions
{
    [Required]
    public string FirstSetting { get; set; }

    [Range(0, 10)]
    public int SecondSetting { get; set; }
}
```
If you want to execute this validations on application start, pass the last argument as true:
```c#
[RegisterOption(OptionType.Settings, true, true)]
```
Finally use the provided extension method on Program.cs or on a custom extension method, passing the configuration and the assemblies containing this clases or all assemblies.
```c#
using Wurs.Extensions.ServiceCollection;
//..
services.RegisterOptionsPatterns(configuration,Assembly.GetAssemblies());
```
## Acknowledgments

### License
This project is licensed under the [MIT License](https://choosealicense.com/licenses/mit/). See the LICENSE file for details.
