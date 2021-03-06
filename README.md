# Veldrid

Veldrid is a cross-platform, graphics API-agnostic rendering and compute library for .NET. It provides a powerful, unified interface to a system's GPU and includes more advanced features than any other .NET library. Unlike other platform- or vendor-specific technologies, Veldrid can be used to create high-performance 3D applications that are truly portable.

Supported backends:

* Direct3D 11
* Vulkan
* Metal
* OpenGL 3

[Veldrid documentation site](https://mellinoe.github.io/veldrid-docs/)

Veldrid is available on NuGet:

[![NuGet](https://img.shields.io/nuget/v/Veldrid.svg)](https://www.nuget.org/packages/Veldrid)

Pre-release versions of Veldrid are also available from MyGet: https://www.myget.org/feed/mellinoe/package/nuget/Veldrid

![Sponza](https://i.imgur.com/QDaXwWL.jpg)

### Build instructions

Veldrid  uses the standard .NET Core tooling. [Install the tools](https://www.microsoft.com/net/download/core) and build normally (`dotnet build`).

Run the RenderDemo program to see a quick demonstration of the rendering capabilities of the library.
