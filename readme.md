# Paprika Functions App

This is a WIP attempt to host [Paprika.Net][paprikanet] in an Azure Functions App so that it can be stateless and serverless.

For some design principles and API, please see [`/docs/api.md`][api]

[paprikanet]: https://github.com/stegriff/paprika.net
[api]: /docs/api.md

## Prerequisites To Do

- [x] Release Paprika.Net ~~v1.1~~ v1.2.0 on GitHub  
- [x] Publish ~~v1.1~~ v1.2.0 as a NuGet package  
- [x] Update my VS2017 to 17.4  
- [x] Download Azure workload update with project template for Functions App
- [ ] Start Functions App and import NuGet package
- [ ] Then all we have to is write it ;)

## Development Stages

 0. Static-grammar function app with no auth **MVP**
 0. Single user function app environment loading from storage (no auth)
 0. Multi-user function app loading per-user grammars from storage

## Aims

 * Minimum viable product working in Dec 2017
 * Multi-user service available Jan/Feb 2018
