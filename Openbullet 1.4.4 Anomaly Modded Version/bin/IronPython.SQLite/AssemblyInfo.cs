using IronPython.Runtime;
using IronPython.SQLite;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: Extension]
[assembly: AssemblyTitle("IronPython.SQLite")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyProduct("IronPython")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("225ca84b-ef0f-409e-a3d4-42ab1fd899cc")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: PythonModule("_sqlite3", typeof (PythonSQLite), new PlatformID[] {})]
[assembly: AssemblyFileVersion("2.7.9.1000")]
[assembly: AssemblyInformationalVersion("IronPython 2.7.9 final 0")]
[assembly: AssemblyCompany("IronPython Team")]
[assembly: AssemblyCopyright("© IronPython Contributors.")]
[assembly: AssemblyVersion("2.7.9.0")]
