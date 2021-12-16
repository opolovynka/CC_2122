#r "System.Net.WebClient"

#r "nuget: Nake.Meta, 3.0.0-beta-01"
#r "nuget: Nake.Utility, 3.0.0-beta-01"

using System;                       //
using System.IO;                    //      standard C# namespace imports
using System.Linq;                  //     (these are imported by default)
using System.Text;                  //  
using System.Threading.Tasks;       //    
using System.Collections.Generic;   //  

using static System.IO.Path;        //    C# "using static members" feature 
using static System.Console;        //      will make you scripts more terse

[Nake] async Task Build(string config = "Debug")  => await                                         
    $@"dotnet build \
    /p:Configuration={config} /v:d";

[Nake] async Task Test()  => await                                         
    $@"dotnet test Tests.csproj --filter Test=Unit";