# WoAExplorer
File Explorer for Windows on ARM. The main objective is to write a touch friendly file explorer, following 
the Fluent Design guidelines by Microsoft.

## Compatibility
This program can be compiled for AMD64, ARM32 and ARM64. It will not work on ARM64 and ARM32
since there is a compiler bug that doesn't allow objects of type IStorageItem to be casted to and from.

It requires Windows 10 19H1 / version 1903 as it's the application minimum target at the moment. It might be
possible to lower the minimum requirement, but it needs testing.

## Screenshots
Wide Layout
![Wide version of the app layout](https://github.com/Simizfo/WoAExplorer/blob/master/Screenshots/screenshotwide.png?raw=true)
Mobile Layout
![Mobile version of the app layout](https://github.com/Simizfo/WoAExplorer/blob/master/Screenshots/screenshotnarrow.png?raw=true)
