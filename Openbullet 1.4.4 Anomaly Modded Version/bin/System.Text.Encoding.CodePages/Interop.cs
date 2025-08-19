// Decompiled with JetBrains decompiler
// Type: Interop
// Assembly: System.Text.Encoding.CodePages, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D2B4F262-31A4-4E80-9CFB-26A2249A735E
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Text.Encoding.CodePages.dll

using System.Runtime.InteropServices;

#nullable disable
internal static class Interop
{
  internal static class Kernel32
  {
    internal const uint CP_ACP = 0;
    internal const uint WC_NO_BEST_FIT_CHARS = 1024 /*0x0400*/;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern unsafe int GetCPInfoExW(
      uint CodePage,
      uint dwFlags,
      Interop.Kernel32.CPINFOEXW* lpCPInfoEx);

    internal static unsafe int GetLeadByteRanges(int codePage, byte[] leadByteRanges)
    {
      int leadByteRanges1 = 0;
      Interop.Kernel32.CPINFOEXW cpinfoexw;
      if (Interop.Kernel32.GetCPInfoExW((uint) codePage, 0U, &cpinfoexw) != 0)
      {
        for (int index = 0; index < 10 && leadByteRanges[index] != (byte) 0; index += 2)
        {
          leadByteRanges[index] = cpinfoexw.LeadByte[index];
          leadByteRanges[index + 1] = cpinfoexw.LeadByte[index + 1];
          ++leadByteRanges1;
        }
      }
      return leadByteRanges1;
    }

    internal static unsafe bool TryGetACPCodePage(out int codePage)
    {
      Interop.Kernel32.CPINFOEXW cpinfoexw;
      if (Interop.Kernel32.GetCPInfoExW(0U, 0U, &cpinfoexw) != 0)
      {
        codePage = (int) cpinfoexw.CodePage;
        return true;
      }
      codePage = 0;
      return false;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CPINFOEXW
    {
      internal uint MaxCharSize;
      internal unsafe fixed byte DefaultChar[2];
      internal unsafe fixed byte LeadByte[12];
      internal char UnicodeDefaultChar;
      internal uint CodePage;
      internal unsafe fixed byte CodePageName[260];
    }
  }

  internal static class Libraries
  {
    internal const string Advapi32 = "advapi32.dll";
    internal const string BCrypt = "BCrypt.dll";
    internal const string CoreComm_L1_1_1 = "api-ms-win-core-comm-l1-1-1.dll";
    internal const string Crypt32 = "crypt32.dll";
    internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";
    internal const string HttpApi = "httpapi.dll";
    internal const string IpHlpApi = "iphlpapi.dll";
    internal const string Kernel32 = "kernel32.dll";
    internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";
    internal const string Mswsock = "mswsock.dll";
    internal const string NCrypt = "ncrypt.dll";
    internal const string NtDll = "ntdll.dll";
    internal const string Odbc32 = "odbc32.dll";
    internal const string OleAut32 = "oleaut32.dll";
    internal const string PerfCounter = "perfcounter.dll";
    internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";
    internal const string Secur32 = "secur32.dll";
    internal const string Shell32 = "shell32.dll";
    internal const string SspiCli = "sspicli.dll";
    internal const string User32 = "user32.dll";
    internal const string Version = "version.dll";
    internal const string WebSocket = "websocket.dll";
    internal const string WinHttp = "winhttp.dll";
    internal const string Ws2_32 = "ws2_32.dll";
    internal const string Wtsapi32 = "wtsapi32.dll";
    internal const string CompressionNative = "clrcompression.dll";
  }
}
