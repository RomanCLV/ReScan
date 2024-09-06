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

--> C/C++ > Additionnal Include Directories: `..\..\ReScanAdditionnals\eigen`;`..\..\ReScanAdditionnals\boost`;`%(AdditionalIncludeDirectories)`

--> C/C++ > Preprocessor: Add `BOOST_ALL_NO_LIB` to all your configurations

--> Linker > Additionnal Include Directories: `..\..\ReScanAdditionnals\boost\stage\lib`;`%(AdditionalLibraryDirectories)`

--> Linker > Input : Additionnal Dependencies : Add `libboost_program_options-vc143-mt-x64-1_84.lib` (for Release x64) or `libboost_program_options-vc143-mt-gd-x64-1_84.lib` (for Debug x64)
End can change depending on your boost version.

## ReScanVisualize C#

Package `HelixToolkit.Wpf.2.24.0` is used. You can download it with NuGet Packet Manager anf look for `HelixToolkit.Wpf` by objo.

# ReScan Usage

## ReScan C++

- Basic usage: `ReScan.exe`

Available options:
```
General Options:
  -h [ --help ]               Produce help message

Run Process Options:
  -c [ --config ] config.ini  Specify one or more config files
  -f [ --file ] file.obj      Specify one or more obj files
  -p [ --prescan ]            Indicates to run the PreScan process

Configuration Options:
  --ccd                       Create a new default config file: config.ini
  --ccif                      Create a new config file adapted for ICNDE
                              (frontal): configFrontal.ini
  --ccil                      Create a new config file adapted for ICNDE
                              (lateral): configLateral.ini
  --ccp                       Create a new default config file for PreScan
                              process: prescan-config.ini
```

If a `create-config` (`--cc_`) option is used, the process is aborted.

You can use `ReScan.exe --ccd --ccif --ccil --ccp` to create all configuration files simultaneously. You can then edit them and use them.

### `ReScan`

Executes the `ReScan process` program. 
If no filename is specified, the `.obj file` name will be requested automatically. 
This step is skipped if you specify it using the `-f filename` option. 
After providing some required information, the process will start.

You can also run process many times in a row by specifying many .obj files `-f file1.obj file2.obj`.

You can customize the output (e.g., export or not certain files) based on your preferences 
using the `-c config` option, which specifies the configuration file to use. 
This option also allows you to automate the process.

You can also run process many times in a row by specifying many config files `-c config1.ini config2.ini`.

### `PreScan`

You must use the `-p` option to indicate that you want to run the `PreScan process`.

Similar to the ReScan process, you can also specify a configuration file (e.g., `ReScan.exe -p -c prescan-config.ini`) to automate the process,
or input the values during program execution.

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
    - `port` must be an integer `from 0 to 65535`.
     
  Once a UDP client is opened, you can communicate and send it command lines to interract with the application without manipulate the mouse and keyboard.

  A recommanded usage is: `ReScanVisualizer.exe -udp o port`. The application is launched and a UDP client is also launch and ready to receive command lines.
- Max Points:
  - Set or reset the max points of graphs added by command lines: `-mp | --max-points opt`
    - To set the max points:
      - `opt` must be an interger strictly `greater than 0`.
    - To reset the max points:
      -  `opt`must be `reset` or `0` (or a negative number)
- Add graph:
  - `-ag | -add-graph type filepath [containsHeader] [scaleFactor] [axisScaleFactor] [pointRadius] [maxPointsDisplayed] [displayBarycenter] [displayAveragePlan] [displayBase] [renderQuality]`
    - Required parameters:
      - `type` is the type of the file. Currently only `csv` is supported.
      - `filepath` is the path of the file to open. Better if it's the absolute path.
    - Additionnal parameters:
      - `containsHeader` indicates if the first line of the file is headers.
        - Accepted values for true: `true`, `t`, `1`
        - Accepted values for false: `false`, `f`, `0`
        - Default value if the parameter is not given: `true`
      - `scaleFactor` indicates the scale factor to apply for the visual representation of the graph.
        - Must be a strictly positive number.
        - Default value if the parameter is not given: `1`
      - `axisScaleFactor` indicates the scale factor to apply for the visual representation of the graph's base.
        - Must be a strictly positive number.
        - Default value if the parameter is not given: `1`
      - `pointRadius` indicates the point's radius to apply for the visual representation of each point.
        - Must be a strictly positive number.
        - Default value if the parameter is not given: `0.25`
      - `maxPointsDisplayed` indicates the maximum number of points that will be displayed.
        - `-1` (or a `negative value`) to set `no limit`
        - `positive integer` (start from 0) to set a limit.
      - `displayBarycenter` show or hide the barycenter of a graph.
        - Accepted values for true: `true`, `t`, `1`
        - Accepted values for false: `false`, `f`, `0`
        - Default value if the parameter is not given: `true`
      - `displayAveragePlan` show or hide the average plan of a graph.
        - Accepted values for true: `true`, `t`, `1`
        - Accepted values for false: `false`, `f`, `0`
        - Default value if the parameter is not given: `true`
      - `displayBase` show or hide the base of a graph.
        - Accepted values for true: `true`, `t`, `1`
        - Accepted values for false: `false`, `f`, `0`
        - Default value if the parameter is not given: `true`
      - `renderQuality` set the render quality.
        - Accepted values: `VeryLow`, `Low`, `Medium`, `High`, `VeryHigh`
        - Default value if the parameter is not given: `High`

  If you want to set an additionnal value, you have to indicates all the previous additionnal values.

  Example: If you want to set the point radius to 2 (and let the other default values) and supposing your file has headers, use:

  `-ag csv path\to\myfile.csv t 1 1 2`

  Important: Add a huge file (e.g. 1000 points or more) can be hard to manage. That's why we recommand to set the max points to add with the command `max points`. See this command above in the file.
  
- Add bases:
  - `-abs | -add-bases filepath isCartesian [containsHeader] [scaleFactor] [axisScaleFactor] [renderQuality]`
    - Required parameters:
      - `filepath` is the path of the file to open. Better if it's the absolute path.
      - `isCartesian` indicates if the bases are expressed in Cartesian or with ZYX Euler's angles.
        - Accepted values for true: `true`, `t`, `1`
        - Accepted values for false: `false`, `f`, `0`
    - Additionnal parameters:
      - `containsHeader` indicates if the first line of the file is headers.
        - Accepted values for true: `true`, `t`, `1`
        - Accepted values for false: `false`, `f`, `0`
        - Default value if the parameter is not given: `true`
      - `scaleFactor` indicates the scale factor to apply for the visual representation of the graph.
        - Must be a strictly positive number.
        - Default value if the parameter is not given: `1`
      - `axisScaleFactor` indicates the scale factor to apply for the visual representation of the graph's base.
        - Must be a strictly positive number.
        - Default value if the parameter is not given: `1`
      - `renderQuality` set the render quality.
        - Accepted values: `VeryLow`, `Low`, `Medium`, `High`, `VeryHigh`
        - Default value if the parameter is not given: `High`
  If you want to set an additionnal value, you have to indicates all the previous additionnal values.

  Example: If you want to set the render quality to low (and let the other default values) and supposing your file has headers, use:

  `-abs path\to\myfile.csv t 1 1 low`
- Clear graphs:
  - `-cg` clear all the added graphs. 
- Clear bases:
  - `-cb` clear all the added bases.
- Kill:
  - `-k | --kill` to shutdown the application (close automatically all UDP client opened)

Several commands can be used at the at a time.
