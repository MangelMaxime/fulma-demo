# Fulma Demo [![Build status](https://ci.appveyor.com/api/projects/status/0wcqxjaog7igxfgr/branch/master?svg=true)](https://ci.appveyor.com/project/MangelMaxime/fulma-demo/branch/master)

# Build for production

Run: `./fake.sh build`

All the files needed for deployment are under the `output` folder.

# Watch mode

Run: `./fake.sh build -t Watch`

# Running Fable without FAKE

- Install NPM dependencies: `yarn`
- Install Nuget dependencies: `dotnet restore build.proj`
- Building for development: `dotnet fable webpack-dev-server`
- Building for production: `dotnet fable webpack-cli`

# Debugging in VS Code

* Install [Debugger For Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) in vscode
* Press F5 in vscode
* After all the .fs files are compiled, the browser will be launched
* Set a breakpoint in F#
* Either press F5 in Chrome or restart debugging in VS Code with Ctrl+Shift+F5 (Cmd+Shift+F5 on macOS)
* The breakpoint will be caught in vscode

