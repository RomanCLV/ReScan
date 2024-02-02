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

Available options:

- Help: `-h | --help`
- Config file is specified: `-c | --config config.ini`
- Obj file is specified: `-f | --file objFile.obj`
- Create a new default config file: config.ini: `-cc | --create-config`
- Create a new config file adapted for ICNDE (frontal): configFrontal.ini: `-ccif | --create-config-icnde-frontal`
- Create a new config file adapted for ICNDE (lateral): configLateral.ini: `-ccil | --create-config-icnde-lateral`

Use only one command at a time.

## ReScanVisualizer C#

ReScanVisualizer is a user interface to manipulate graph. You can also use the interface with command lines.

- Basic usage: `ReScanVisualizer.exe`

Available options:

- Help:
  - General help `-h | --help`
  - Help about a specific command `-h | --help command`. Example: `-h ag` will get the help of the `add graph` command.
- UDP:
  - Open or close a UDP client: `-udp | --udp opt port`
    - `opt` can be: `o`, `open` to open a client, or `c`, `close` to close a client.
    - `port` must be an integer between 0 and 65535.
     
  Once a UDP client is opened, you can communicate and send it command lines to interract with the application without manipulate the mouse and keyboard.

  A recommanded usage is: `ReScanVisualizer.exe -udp o port`. The application is launched and a UDP client is also launch and ready to receive command lines.
- Max Points:
  - Set or reset the max points of graphs added by command lines: `-mp | --max-points opt`
    - To set the max points:
      - `opt` must be an interger strictly greater than 0.
    - To reset the max points:
      -  `opt`
- Add graph:
- Add bases:
- Clear graphs:
- Clear bases:

Several commands can be use at the at a time.
