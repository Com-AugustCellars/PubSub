# CoAP.PubSub - An implementation in C# of the PubSub REST API.

[![NuGet Status](https://img.shields.io/nuget/v/Com.AugustCellars.CoAP.png)](https://www.nuget.org/packages/Com.AugustCellars.CoAP)
[![Build Status](https://api.travis-ci.org/jimsch/CoAP-CSharp.png)](https://travis-ci.org/jimsch/CoAP-CSharp)

The Constrained Application Protocol (CoAP) (https://datatracker.ietf.org/doc/draft-ietf-core-coap/) is a RESTful web transfer protocol for resource-constrained networks and nodes.
CoAP.PubSub is a publish/subscription REST API to allow a server to act as the distribution point for data.

The specification for the REST API can be found at [PubSub]{https://datatracker.ietf.org/doc/draft-ietf-core-coap-pubsub/}.


## Copyright

Copyright (c) 2017, Jim Schaad <ietf@augustcellars.com>

## Content

- [Quick Start](#quick-start)
- [Build](#build)
- [License](#license)
- [Acknowledgements](#acknowledgements)

## How to Install

The C# implementation will be available in the NuGet Package Gallery under the name [Com.AugustCellars.CoAP.PubSub](https://www.nuget.org/packages/Com.AugustCellars.CoAP.PubSub).
To install this library as a NuGet package, enter 'Install-Package Com.AugustCellars.CoAP.PubSub' in the NuGet Package Manager Console.

## Documentation

Documentation can be found in two places.
First an XML file is installed as part of the package for inline documentation.

## Quick Start

There is currently only server side code to implement the resources needed.
The client code is expected to use the standard client APIs.

A new CoAP server can be easily built with help of the class [**CoapServer**](CoAP.NET/Server/CoapServer.cs)

```csharp
  static void Main(String[] args)
  {
    CoapServer server = new CoapServer();
    
    server.Add(new PubSubResource("ps"));
    
    server.Start();
    
    Console.ReadKey();
  }
```

See [CoAP Example Server](PubSub.PubSubServer) for more.


## Building the sources

I am currently sync-ed up to Visual Studio 2017 and have started using language features of C# v7.0 that are supported both in Visual Studio and in the latest version of mono.

## License

See [LICENSE](LICENSE) for more info.


