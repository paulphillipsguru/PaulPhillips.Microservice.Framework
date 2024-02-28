# PaulPhillips.Framework.Feature (Microservice Framework)

A small framework that combines a set of common features that a typical Microservice would access which supports Vertical Slice Architecture. 

Build in support for:

1. Framework to supports Vertical Slice Architecture
2. Idempotency
3. Event Management (Rabbit MQ)
4. Structured Logging (Jeager), Error Handling and Security
5. Security (JWT)

### Terminology 

1. Feature: This embodies the vertical slice of the framework, encapsulating individual core logic.
2. Command: Signifies an action to be executed on the server, in line with the CQRS pattern.
3. Query: Denotes read-only actions, also integral to the CQRS pattern.

A feature and only be a Command OR a Query, not both.

### Requirements

1. NET 8
2. Redis
3. Jeager
4. Rabbit MQ

Though the above are requirements, you can swap Redis, Jeager and RabbitMQ to alternative platforms if required.

## Getting Started

Create a .NET Minimal API (.NET) project and install (controllers are not required by the framework)

```
dotnet add package PaulPhillips.Framework.Feature --version 1.0.12-alpha
Package: NuGet\Install-Package PaulPhillips.Framework.Feature -Version 1.0.12-alpha
```

To start with, your program.cs should something like below:

```c#
using PaulPhillips.Framework.Feature.Core;
using PaulPhillips.Framework.Feature.Helpers;
using PaulPhillips.Framework.Feature.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterFeatureAll();

var app = builder.Build();

app.UseMiddleware<FeatureHealthMiddleware>();
#if !DEBUG
app.UseMiddleware<FeatureSecurityMiddleware>(); Include this middleware if you want to test with JWT
#endif
app.UseMiddleware<FeaterCoreMiddleware>();

app.Run();
```

Next, we need to add some configuration to AppSettings.json, this will configure endpoints for Redis etc.

```json
  "Health": {
    "Alive": "Health/Alive",
    "Ready": "Health/Ready"
  },
  "Security": {
    "Issuer": "http://localhost:8480/realms/Paul",
    "Audience": "account",
    "Key": "ZUhBMVhVakppWkJMNXkxWVVjWVNYNllTTC15YkxIRTlQemJ3ZkpFVE00Zw==",
    "RequireHttpsMetadata": false,
    "Enabled": true
  },
  "Idempotency": {
    "Host": "localhost"
  },
  "Events": {
    "Host": "127.0.0.1",
    "UserName": "guest",
    "Password": "guest"
  }
```

Security: Standard JWT configuration

Idempotency: Redis instance

Events: Rabbit MQ Instance

## Creating a Command Feature

A feature group collates all code, assets etc into a single folder (the feature). 

There are no rules how are you create your folder structure, this is up to you, but in the example I will be creating a folder called FeatureDemo. This example, I will be simply return back 

Please follow the following steps:

1. Create a folder called "Features"
2. Under the folder "Features", Create Folder caller "FeatureDemo"
3. Under the "Feature" create a folder called "Models"
4. Under Models, create a class called RequestModel with a public string property called "FullName".
5. Under Models create another called ResponseModel with a public string property called "FirstName"
6. Under the Feature folder, create a class called CreateCustomerFeature that inherits from Command<RequestModel,ResponseModel>
7. Last step, you will need to override a single method ProcessAsync (this is where your core logic will reside)

Your Command Feature should look something like:

```c#
using OpenTracing;
using PaulPhillips.Feature.BasicExample.Features.FeatureDemo.Models;
using PaulPhillips.Framework.Feature.Commands;

namespace PaulPhillips.Feature.BasicExample.Features.FeatureDemo
{
    public class CreateCustomerFeature : Command<RequestModel,ResponseModel>
    {
        public override  async Task<dynamic> ProcessAsync(ISpan tracingSpan)
        {
            if (!string.IsNullOrWhiteSpace(Request?.FullName))
            {
                var nameToSplit = Request.FullName.Split(" ");

                var responseModel = new ResponseModel { FirstName = nameToSplit[0] };

                return await Task.FromResult(responseModel);
            }

            return await Task.FromResult(new ResponseModel());

        }
    }
}
```

The above is just an example at this stage and will not cover every aspect of the framework at this stage.

Now we need to register the feature with the core framework, this can be done in the program class.

```c#
var app = builder.Build();

FeatureFactory.Features.Add("CreateCustomer", typeof(CreateCustomerFeature));

```

That is it, a new feature has been created.

Use postman or any other client to perform a post.

```http
@PaulPhillips.Feature.BasicExample_HostAddress = http://localhost:5062

POST {{PaulPhillips.Feature.BasicExample_HostAddress}}/CreateCustomer
Accept: application/json

{
  "FullName": "Paul Phillips"
}

###

```

Result:

```json
{
  "ValidationResult": null,
  "Response": {
    "FirstName": "Paul"
  }
}
```

