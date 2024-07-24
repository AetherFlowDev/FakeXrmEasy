# FakeXrmEasy for AetherFlow

## Overview
FakeXrmEasy for AetherFlow is a forked version of FakeXrmEasy by Jordi Montana, tailored specifically for AetherFlow's needs. It is a framework for developing and testing plugins for Microsoft Dataverse in a simplified manner.

## Features
- Simplified unit testing for Dataverse plugins
- Support for early-bound and late-bound entities
- Easy setup and configuration

## Getting Started
### Prerequisites
- .NET Framework 4.6.2 or later
- Visual Studio 2019 or later

### Installation
#### Use the NuGet package:
1. Download the NuGet package from the nuget.org
2. Install the NuGet package in your project

#### Manual installation:
```sh
git clone https://github.com/AetherFlowDev/FakeXrmEasy.git
```
Build the solution in Visual Studio:
1. Open `AetherFlow.FakeXrmEasy.Plugins.sln`
2. Build the solution

## Usage
1. Create a test project in your solution.
2. Add references to the `FakeXrmEasy` library.
3. Write unit tests for your plugins.

### Example Test
```csharp
[TestClass]
public class MyPluginTests
{
    [TestMethod]
    public void TestPluginExecution()
    {
        var context = new XrmFakedContext();
        var plugin = new MyPlugin();
        
        // Setup context and entities
        
        context.ExecutePluginWith<MyPlugin>(pluginContext);
        
        // Assert results
    }
}
```

## Contributing
1. Fork the repository.
2. Create your feature branch (`git checkout -b feature/AmazingFeature`).
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4. Push to the branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements
- [Jordi Montana](https://github.com/jordimontana82/FakeXrmEasy) for the original FakeXrmEasy framework.

For more details, visit the [GitHub repository](https://github.com/AetherFlowDev/FakeXrmEasy/).