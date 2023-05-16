# Fulma Demo [![Build status](https://ci.appveyor.com/api/projects/status/0wcqxjaog7igxfgr/branch/master?svg=true)](https://ci.appveyor.com/project/MangelMaxime/fulma-demo/branch/master)

[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io/#https://github.com/MangelMaxime/fulma-demo)

# Build for production

Run: `dotnet fsi build.fsx`

All the files needed for deployment are under the `src/dist/` folder.

# Watch mode

Run: `dotnet fsi build.fsx --watch`

# Debugging in VS Code

* Install [Debugger For Chrome](https://marketplace.visualstudio.com/items?itemName=msjsdiag.debugger-for-chrome) in vscode
* Press F5 in vscode
* After all the .fs files are compiled, the browser will be launched
* Set a breakpoint in F#
* Either press F5 in Chrome or restart debugging in VS Code with Ctrl+Shift+F5 (Cmd+Shift+F5 on macOS)
* The breakpoint will be caught in vscode
