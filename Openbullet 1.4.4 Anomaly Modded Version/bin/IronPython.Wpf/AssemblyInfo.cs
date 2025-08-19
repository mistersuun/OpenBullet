using IronPython.Modules;
using IronPython.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("IronPython.Wpf")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyProduct("IronPython")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("505BA001-53E8-4148-AE1D-1425D0FF7948")]
[assembly: SecurityTransparent]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: PythonModule("_wpf", typeof (Wpf), PlatformsAttribute.PlatformFamily.Windows)]
[assembly: AssemblyFileVersion("2.7.9.1000")]
[assembly: AssemblyInformationalVersion("IronPython 2.7.9 final 0")]
[assembly: AssemblyCompany("IronPython Team")]
[assembly: AssemblyCopyright("© IronPython Contributors.")]
[assembly: AssemblyVersion("2.7.9.0")]
