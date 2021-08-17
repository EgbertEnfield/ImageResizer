using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ImageResizer")]
[assembly: AssemblyDescription("画像の拡大縮小をするコマンドツール")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ImageResizer")]
[assembly: AssemblyCopyright("Copyright ©  2021  Семён Мошенко")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3ebad68a-2bec-484f-9fac-b9e3e8eb129a")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.1.*")]
[assembly: AssemblyFileVersion("1.0.1")]

// Attributes to show this application usage and license
[assembly: CommandLine.Text.AssemblyLicense("\n  Released under the MIT License.", "  (https://github.com/Sovietball1922/ImageResizer/blob/master/LICENSE)", "  ")]
[assembly: CommandLine.Text.AssemblyUsage("  IMAGERESIZER source dest ratio [-c] [-o] [-j path] \n")]
