# Paprika Functions App

This is the code for <https://paprika.me.uk>, a tool for generating random language according to human-written grammars. It hosts [Paprika.Net][paprikanet] in an Azure Functions App so that it can be stateless and serverless.

[paprikanet]: https://github.com/stegriff/paprika.net

## Repo contents

	/docs  - Documentation
	/env   - Environment (hidden files)
	/src   - Source code
	/test  - Test data and other files to support testing


For some design principles and API, please see [`/docs/api.md`][api]. Take a look at the other [`/docs/`][docs] as well.

[api]: /docs/api.md
[docs]: /docs/

## Prerequisites To Do

- [x] Release Paprika.Net ~~v1.1~~ v1.2.0 on GitHub  
- [x] Publish ~~v1.1~~ v1.2.0 as a NuGet package  
- [x] Update my VS2017 to 15.4  
- [x] Download Azure workload update with project template for Functions App
- [x] Start Functions App and import NuGet package
- [x] Then all we have to is write it ;)

## Development Stages

 0. ~~Static-grammar function app with no auth **MVP**~~
 0. ~~Single user function app environment loading from storage (no auth)~~
 0. ~~Multi-user function app loading per-user grammars from storage~~

## Aims

 * Minimum viable product working in Dec 2017
 * Multi-user service available Jan/Feb 2018

 ## Status

It's done! We went live 6th March 2018!

 ## To Do

 - [x] Anonymous users
 - [x] Default grammars
 - [x] User creation
 - [x] Sensible login workflow
