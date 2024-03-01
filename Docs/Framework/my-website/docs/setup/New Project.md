---
sidebar_label: 'New Project'
sidebar_position: 2
---



## Getting Started

Create a .NET Minimal API (.NET) project and install (controllers are not required by the framework)

```
dotnet add package PaulPhillips.Framework.Feature --version 1.0.14-alpha
Package: NuGet\Install-Package PaulPhillips.Framework.Feature -Version 1.0.14-alpha
```

To start with, your program.cs should something like below:

```c#
using PaulPhillips.Framework.Feature.Core;
using PaulPhillips.Framework.Feature.Helpers;
using PaulPhillips.Framework.Feature.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterFeatureAll();

var app = builder.Build();

app.UseFeatureFramework();

app.Run();
```

Next, we need to add some configuration to AppSettings.json, this will configure endpoints for Redis etc.

