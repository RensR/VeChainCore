![Build Status](https://travis-ci.com/RensR/VeChainCore.svg?branch=master) ![Dotnet](https://img.shields.io/badge/.NET%20Core-2.2-blue.svg)

# VeChainCore
VeChainCore is a VeChain API implementation in .NET Core. It simply wraps the already friendly RESTful web API in some C# magic. The project is designed to be used in other applications and speed up the development process by letting you focus on the flashy stuff, not just the invisible back-end processes.

## Prerequisites 

For the application to interact with the VeChain blockchain it is needed that you have http access to a running node instance. To run a node on a device simply install [Thor-Sync](https://github.com/vechain/thor-sync) or run a node from using the VeChain [source code](https://github.com/vechain/thor). 

## Usage
Simply create a new instance of the VeChainClient and direct it to your hosted VeChain instance. Be aware, VeChainCore does not provide the blockchain itself and will not work without an instance of the VeChain blockchain!
```
var vechainClient = new VeChainClient();
vechainClient.SetBlockchainAddress("http://localhost:8669");
```

This client can then be used to interact with the blockchain, for instance getting a block by its block id. The following code retrieves the genesis block for us as a ```Block``` object.
```
Block genesis = vechainClient.GetBlock(0)
```

## Pull Requests
Pull requests are always welcome. Feel free to use this code however you like as long as it contains a reference to this original project.
