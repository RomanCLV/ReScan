# ReScan Configuration

## ReScan C++


### Additionnal libs to download:

Eigen : `https://gitlab.com/libeigen/eigen/-/archive/3.4.0/eigen-3.4.0.zip`

Once the folder has been downloaded and unzipped, it's already ready to use. You can also can rename it as `eigen`.

Boost : `https://www.boost.org/users/history/version_1_84_0.html`

Once the folder has been downloaded and unzipped (you can also can rename it as `boost`), run `bootstrap.bat`. This will generate `b2.exe`, which you'll also need to run.


### Configure Microsoft Visual Studio

General :

- C++ Language Standard: `ISO C++20 Standard (/std:c++20)`

- C/C++ > General :

- Additionnal Include Directories: `eigen directory`;`boost directory`;`%(AdditionalIncludeDirectories)`

For example, if you have a similar configuration on your computer:
```
Global directories work & project
  |-- ReScan
        |-- ReScan
        |-- ReScanVisualizer
        ...
  |-- ReScanAdditionnals
        |-- boost
        |-- eigen
```

--> Additionnal Include Directories: `..\..\ReScanAdditionnals\eigen`;`..\..\ReScanAdditionnals\boost`;`%(AdditionalIncludeDirectories)`

- C/C++ > Preprocessor: Add `BOOST_ALL_NO_LIB` to all your configurations

## ReScanVisualize C#

Package `HelixToolkit.Wpf.2.24.0` is used. You can download it with NuGet Packet Manager anf look for `HelixToolkit.Wpf` by objo.

# ReScan Usage

## ReScan C++

- Basic usage: `ReScan.exe`
- Help: `ReScan.exe [-h|--help]`
- Config file is specified: `ReScan.exe [-c|--config] config.ini`
- Obj file is specified: `ReScan.exe [-f|--file] objFile.obj`
- Create a new default config file: config.ini: `ReScan.exe [-cc|--create-config]`
- Create a new config file adapted for ICNDE (frontal): configFrontal.ini: `ReScan.exe [-ccif|--create-config-icnde-frontal]`
- Create a new config file adapted for ICNDE (lateral): configLateral.ini: `ReScan.exe [-ccil|--create-config-icnde-lateral]`

Use only one command at a time.

## ReScanVisualize C#
