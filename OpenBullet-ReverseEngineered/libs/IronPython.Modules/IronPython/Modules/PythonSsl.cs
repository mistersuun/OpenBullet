// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSsl
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonSsl
{
  public const string __doc__ = "Implementation module for SSL socket operations.  See the socket module\nfor documentation.";
  public const int OPENSSL_VERSION_NUMBER = 9437184 /*0x900000*/;
  public static readonly PythonTuple OPENSSL_VERSION_INFO = PythonTuple.MakeTuple((object) 0, (object) 0, (object) 0, (object) 0, (object) 0);
  public static readonly object _OPENSSL_API_VERSION = (object) PythonSsl.OPENSSL_VERSION_INFO;
  public const string OPENSSL_VERSION = "OpenSSL 0.0.0 (.NET SSL)";
  private static List<Asn1Object> _asn1Objects = new List<Asn1Object>();
  public static PythonType SSLType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonSocket.ssl));
  private const int ClassOffset = 6;
  private const int ClassMask = 192 /*0xC0*/;
  private const int ClassUniversal = 0;
  private const int ClassApplication = 64 /*0x40*/;
  private const int ClassContextSpecific = 128 /*0x80*/;
  private const int ClassPrivate = 192 /*0xC0*/;
  private const int NumberMask = 31 /*0x1F*/;
  private const int UnivesalSequence = 16 /*0x10*/;
  private const int UniversalInteger = 2;
  private const int UniversalOctetString = 4;
  public const int CERT_NONE = 0;
  public const int CERT_OPTIONAL = 1;
  public const int CERT_REQUIRED = 2;
  public const int PROTOCOL_SSLv2 = 0;
  public const int PROTOCOL_SSLv3 = 1;
  public const int PROTOCOL_SSLv23 = 2;
  public const int PROTOCOL_TLS = 2;
  public const int PROTOCOL_TLSv1 = 3;
  public const int PROTOCOL_TLSv1_1 = 4;
  public const int PROTOCOL_TLSv1_2 = 5;
  public const uint OP_ALL = 2147486719;
  public const uint OP_DONT_INSERT_EMPTY_FRAGMENTS = 2048 /*0x0800*/;
  public const int OP_NO_SSLv2 = 16777216 /*0x01000000*/;
  public const int OP_NO_SSLv3 = 33554432 /*0x02000000*/;
  public const int OP_NO_TLSv1 = 67108864 /*0x04000000*/;
  public const int OP_NO_TLSv1_1 = 268435456 /*0x10000000*/;
  public const int OP_NO_TLSv1_2 = 134217728 /*0x08000000*/;
  internal const int OP_NO_COMPRESSION = 131072 /*0x020000*/;
  internal const int OP_NO_ALL = 520224768 /*0x1F020000*/;
  public const int SSL_ERROR_SSL = 1;
  public const int SSL_ERROR_WANT_READ = 2;
  public const int SSL_ERROR_WANT_WRITE = 3;
  public const int SSL_ERROR_WANT_X509_LOOKUP = 4;
  public const int SSL_ERROR_SYSCALL = 5;
  public const int SSL_ERROR_ZERO_RETURN = 6;
  public const int SSL_ERROR_WANT_CONNECT = 7;
  public const int SSL_ERROR_EOF = 8;
  public const int SSL_ERROR_INVALID_ERROR_CODE = 9;
  public const int VERIFY_DEFAULT = 0;
  public const int VERIFY_CRL_CHECK_LEAF = 4;
  public const int VERIFY_CRL_CHECK_CHAIN = 12;
  public const int VERIFY_X509_STRICT = 32 /*0x20*/;
  public const int VERIFY_X509_TRUSTED_FIRST = 32768 /*0x8000*/;
  public const bool HAS_SNI = true;
  public const bool HAS_ECDH = true;
  public const bool HAS_NPN = false;
  public const bool HAS_ALPN = false;
  public const bool HAS_TLS_UNIQUE = false;
  public const bool HAS_TLSv1_3 = false;
  private const int SSL_VERIFY_NONE = 0;
  private const int SSL_VERIFY_PEER = 1;
  private const int SSL_VERIFY_FAIL_IF_NO_PEER_CERT = 2;
  private const int SSL_VERIFY_CLIENT_ONCE = 4;

  static PythonSsl()
  {
    PythonSsl._asn1Objects.AddRange((IEnumerable<Asn1Object>) new Asn1Object[2]
    {
      new Asn1Object("serverAuth", "TLS Web Server Authentication", 129, new byte[9]
      {
        (byte) 1,
        (byte) 3,
        (byte) 6,
        (byte) 1,
        (byte) 5,
        (byte) 5,
        (byte) 7,
        (byte) 3,
        (byte) 1
      }),
      new Asn1Object("clientAuth", "TLS Web Client Authentication", 130, new byte[9]
      {
        (byte) 1,
        (byte) 3,
        (byte) 6,
        (byte) 1,
        (byte) 5,
        (byte) 5,
        (byte) 7,
        (byte) 3,
        (byte) 2
      })
    });
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    PythonModule builtinModule = context.GetBuiltinModule("_socket");
    PythonType socketError = PythonSocket.GetSocketError(context, builtinModule.__dict__);
    PythonType baseType = context.EnsureModuleException((object) "SSLError", socketError, dict, "SSLError", "ssl");
    context.EnsureModuleException((object) "SSLZeroReturnError", baseType, dict, "SSLZeroReturnError", "ssl");
    context.EnsureModuleException((object) "SSLWantWriteError", baseType, dict, "SSLWantWriteError", "ssl");
    context.EnsureModuleException((object) "SSLSyscallError", baseType, dict, "SSLSyscallError", "ssl");
    context.EnsureModuleException((object) "SSLEOFError", baseType, dict, "SSLEOFError", "ssl");
    context.EnsureModuleException((object) "SSLWantReadError", baseType, dict, "SSLWantReadError", "ssl");
  }

  public static void RAND_add(object buf, double entropy)
  {
    switch (buf)
    {
      case null:
        throw PythonOps.TypeError("must be string or read-only buffer, not None");
      case string _:
        break;
      case PythonBuffer _:
        break;
      default:
        throw PythonOps.TypeError("must be string or read-only buffer, not {0}", (object) PythonOps.GetPythonTypeName(buf));
    }
  }

  public static int RAND_status() => 1;

  public static PythonSocket.ssl sslwrap(
    CodeContext context,
    PythonSocket.socket socket,
    bool server_side,
    string keyfile = null,
    string certfile = null,
    int certs_mode = 0,
    int protocol = 50331650 /*0x03000002*/,
    string cacertsfile = null,
    object ciphers = null)
  {
    return new PythonSocket.ssl(context, socket, server_side, keyfile, certfile, certs_mode, protocol, cacertsfile);
  }

  public static object txt2obj(CodeContext context, string txt, object name = false)
  {
    return (object) (((PythonOps.IsTrue(name) ? 1 : 0) == 0 ? PythonSsl._asn1Objects.Where<Asn1Object>((Func<Asn1Object, bool>) (x => txt == x.OIDString)).FirstOrDefault<Asn1Object>() : PythonSsl._asn1Objects.Where<Asn1Object>((Func<Asn1Object, bool>) (x => txt == x.OIDString || txt == x.ShortName || txt == x.LongName)).FirstOrDefault<Asn1Object>()) ?? throw PythonOps.ValueError("unknown object '{0}'", (object) txt)).ToTuple();
  }

  public static object nid2obj(CodeContext context, int nid)
  {
    if (nid < 0)
      throw PythonOps.ValueError("NID must be positive");
    return (object) (PythonSsl._asn1Objects.Where<Asn1Object>((Func<Asn1Object, bool>) (x => x.NID == nid)).FirstOrDefault<Asn1Object>() ?? throw PythonOps.ValueError("unknown NID {0}", (object) nid)).ToTuple();
  }

  public static IronPython.Runtime.List enum_certificates(string store_name)
  {
    X509Store x509Store = (X509Store) null;
    try
    {
      x509Store = new X509Store(store_name, StoreLocation.LocalMachine);
      x509Store.Open(OpenFlags.ReadOnly);
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
      while (enumerator.MoveNext())
      {
        X509Certificate2 current = enumerator.Current;
        string str = !(current.GetFormat() == "X509") ? "unknown" : "x509_asn";
        SetCollection setCollection = new SetCollection();
        bool flag = false;
        foreach (X509Extension extension in current.Extensions)
        {
          if (extension is X509EnhancedKeyUsageExtension keyUsageExtension)
          {
            foreach (Oid enhancedKeyUsage in keyUsageExtension.EnhancedKeyUsages)
              setCollection.add((object) enhancedKeyUsage.Value);
            flag = true;
            break;
          }
        }
        list.Add((object) PythonTuple.MakeTuple((object) new Bytes((IList<byte>) ((IEnumerable<byte>) current.RawData).ToList<byte>()), (object) str, flag ? (object) setCollection : ScriptingRuntimeHelpers.True));
      }
      return list;
    }
    catch
    {
    }
    finally
    {
      x509Store?.Close();
    }
    return new IronPython.Runtime.List();
  }

  public static IronPython.Runtime.List enum_crls(string store_name)
  {
    X509Store x509Store = (X509Store) null;
    try
    {
      x509Store = new X509Store(store_name, StoreLocation.LocalMachine);
      x509Store.Open(OpenFlags.ReadOnly);
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
      while (enumerator.MoveNext())
        enumerator.Current.GetFormat();
    }
    catch
    {
    }
    finally
    {
      x509Store?.Close();
    }
    return new IronPython.Runtime.List();
  }

  internal static PythonType SSLError(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) nameof (SSLError));
  }

  public static PythonDictionary _test_decode_cert(CodeContext context, string path)
  {
    X509Certificate2 cert = PythonSsl.ReadCertificate(context, path);
    return PythonSsl.CertificateToPython(context, cert);
  }

  internal static PythonDictionary CertificateToPython(CodeContext context, X509Certificate cert)
  {
    return cert is X509Certificate2 cert1 ? PythonSsl.CertificateToPython(context, cert1) : PythonSsl.CertificateToPython(context, new X509Certificate2(cert.GetRawCertData()));
  }

  internal static PythonDictionary CertificateToPython(CodeContext context, X509Certificate2 cert)
  {
    CommonDictionaryStorage dictionaryStorage = new CommonDictionaryStorage();
    dictionaryStorage.AddNoLock((object) "notAfter", (object) ToPythonDateFormat(cert.NotAfter));
    dictionaryStorage.AddNoLock((object) "subject", (object) PythonSsl.IssuerToPython(context, cert.Subject));
    dictionaryStorage.AddNoLock((object) "notBefore", (object) ToPythonDateFormat(cert.NotBefore));
    dictionaryStorage.AddNoLock((object) "serialNumber", (object) PythonSsl.SerialNumberToPython(cert));
    dictionaryStorage.AddNoLock((object) "version", (object) cert.Version);
    dictionaryStorage.AddNoLock((object) "issuer", (object) PythonSsl.IssuerToPython(context, cert.Issuer));
    PythonSsl.AddSubjectAltNames(dictionaryStorage, cert);
    return new PythonDictionary((DictionaryStorage) dictionaryStorage);

    static string ToPythonDateFormat(DateTime date)
    {
      string pythonDateFormat = date.ToUniversalTime().ToString("MMM dd HH:mm:ss yyyy", (IFormatProvider) CultureInfo.InvariantCulture) + " GMT";
      if (pythonDateFormat[4] == '0')
        pythonDateFormat = $"{pythonDateFormat.Substring(0, 4)} {pythonDateFormat.Substring(5)}";
      return pythonDateFormat;
    }
  }

  private static void AddSubjectAltNames(CommonDictionaryStorage dict, X509Certificate2 cert2)
  {
    foreach (X509Extension extension in cert2.Extensions)
    {
      if (!(extension.Oid.Value != "2.5.29.17"))
      {
        List<object> objectList = new List<object>();
        StringReader stringReader = new StringReader(extension.Format(true));
        string str1;
        while ((str1 = stringReader.ReadLine()) != null)
        {
          string str2 = str1.Trim();
          char[] chArray1 = new char[1]{ ',' };
          foreach (string str3 in str2.Split(chArray1))
          {
            char[] chArray2 = new char[2]{ ':', '=' };
            string[] strArray = str3.Split(chArray2);
            if (strArray[0].Contains("DNS") && strArray.Length == 2)
              objectList.Add((object) PythonTuple.MakeTuple((object) "DNS", (object) strArray[1]));
          }
        }
        dict.AddNoLock((object) "subjectAltName", (object) PythonTuple.MakeTuple(objectList.ToArray()));
        break;
      }
    }
  }

  private static string SerialNumberToPython(X509Certificate2 cert)
  {
    string serialNumber = cert.SerialNumber;
    for (int index = 0; index < serialNumber.Length; ++index)
    {
      if (serialNumber[index] != '0')
        return serialNumber.Substring(index);
    }
    return serialNumber;
  }

  private static IEnumerable<string> IssuerParts(string issuer)
  {
    bool inQuote = false;
    StringBuilder token = new StringBuilder();
    string str = issuer;
    for (int index = 0; index < str.Length; ++index)
    {
      char ch = str[index];
      if (inQuote)
      {
        if (ch == '"')
          inQuote = false;
        else
          token.Append(ch);
      }
      else
      {
        switch (ch)
        {
          case '"':
            inQuote = true;
            continue;
          case ',':
            yield return token.ToString().Trim();
            token.Length = 0;
            continue;
          default:
            token.Append(ch);
            continue;
        }
      }
    }
    str = (string) null;
    if (token.Length > 0)
      yield return token.ToString().Trim();
  }

  private static PythonTuple IssuerToPython(CodeContext context, string issuer)
  {
    List<object> list = new List<object>();
    foreach (string issuerPart in PythonSsl.IssuerParts(issuer))
    {
      PythonTuple python = PythonSsl.IssuerFieldToPython(context, issuerPart);
      if (python != null)
        list.Add((object) PythonTuple.MakeTuple((object) python));
    }
    return PythonTuple.MakeTuple(list.ToReverseArray<object>());
  }

  private static PythonTuple IssuerFieldToPython(CodeContext context, string p)
  {
    if (string.Compare(p, 0, "CN=", 0, 3) == 0)
      return PythonTuple.MakeTuple((object) "commonName", (object) p.Substring(3));
    if (string.Compare(p, 0, "OU=", 0, 3) == 0)
      return PythonTuple.MakeTuple((object) "organizationalUnitName", (object) p.Substring(3));
    if (string.Compare(p, 0, "O=", 0, 2) == 0)
      return PythonTuple.MakeTuple((object) "organizationName", (object) p.Substring(2));
    if (string.Compare(p, 0, "L=", 0, 2) == 0)
      return PythonTuple.MakeTuple((object) "localityName", (object) p.Substring(2));
    if (string.Compare(p, 0, "S=", 0, 2) == 0)
      return PythonTuple.MakeTuple((object) "stateOrProvinceName", (object) p.Substring(2));
    if (string.Compare(p, 0, "C=", 0, 2) == 0)
      return PythonTuple.MakeTuple((object) "countryName", (object) p.Substring(2));
    if (string.Compare(p, 0, "E=", 0, 2) != 0)
      return (PythonTuple) null;
    return PythonTuple.MakeTuple((object) "email", (object) p.Substring(2));
  }

  internal static X509Certificate2 ReadCertificate(CodeContext context, string filename)
  {
    string[] lines;
    try
    {
      lines = File.ReadAllLines(filename);
    }
    catch (IOException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonSsl.SSLError(context), (object) "Can't open file ", (object) filename);
    }
    X509Certificate2 x509Certificate2 = (X509Certificate2) null;
    RSACryptoServiceProvider cryptoServiceProvider = (RSACryptoServiceProvider) null;
    try
    {
      for (int start = 0; start < lines.Length; ++start)
      {
        if (lines[start] == "-----BEGIN CERTIFICATE-----")
        {
          string end = PythonSsl.ReadToEnd(lines, ref start, "-----END CERTIFICATE-----");
          try
          {
            x509Certificate2 = new X509Certificate2(Convert.FromBase64String(end.ToString()));
          }
          catch (Exception ex)
          {
            throw PythonSsl.ErrorDecoding(context, (object) filename, (object) ex);
          }
        }
        else if (lines[start] == "-----BEGIN RSA PRIVATE KEY-----")
        {
          string end = PythonSsl.ReadToEnd(lines, ref start, "-----END RSA PRIVATE KEY-----");
          try
          {
            byte[] x = Convert.FromBase64String(end.ToString());
            cryptoServiceProvider = PythonSsl.ParsePkcs1DerEncodedPrivateKey(context, filename, x);
          }
          catch (Exception ex)
          {
            throw PythonSsl.ErrorDecoding(context, (object) filename, (object) ex);
          }
        }
      }
    }
    catch (InvalidOperationException ex)
    {
      throw PythonSsl.ErrorDecoding(context, (object) filename, (object) ex.Message);
    }
    if (x509Certificate2 != null)
    {
      if (cryptoServiceProvider != null)
      {
        try
        {
          x509Certificate2.PrivateKey = (AsymmetricAlgorithm) cryptoServiceProvider;
        }
        catch (CryptographicException ex)
        {
          throw PythonSsl.ErrorDecoding(context, (object) filename, (object) "cert and private key are incompatible", (object) ex);
        }
      }
      return x509Certificate2;
    }
    throw PythonSsl.ErrorDecoding(context, (object) filename, (object) "certificate not found");
  }

  private static RSACryptoServiceProvider ParsePkcs1DerEncodedPrivateKey(
    CodeContext context,
    string filename,
    byte[] x)
  {
    if (((int) x[0] & 192 /*0xC0*/) != 0)
      throw PythonSsl.ErrorDecoding(context, (object) filename, (object) "failed to find universal class");
    if (((int) x[0] & 31 /*0x1F*/) != 16 /*0x10*/)
      throw PythonSsl.ErrorDecoding(context, (object) filename, (object) "failed to read sequence header");
    int offset = 1;
    PythonSsl.ReadLength(x, ref offset);
    int num = PythonSsl.ReadUnivesalInt(x, ref offset);
    if (num != 0)
      throw new InvalidOperationException($"bad vesion: {num}");
    RSACryptoServiceProvider encodedPrivateKey = new RSACryptoServiceProvider();
    encodedPrivateKey.ImportParameters(new RSAParameters()
    {
      Modulus = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      Exponent = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      D = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      P = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      Q = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      DP = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      DQ = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset),
      InverseQ = PythonSsl.ReadUnivesalIntAsBytes(x, ref offset)
    });
    return encodedPrivateKey;
  }

  private static byte[] ReadUnivesalIntAsBytes(byte[] x, ref int offset)
  {
    PythonSsl.ReadIntType(x, ref offset);
    int length = PythonSsl.ReadLength(x, ref offset);
    while (x[offset] == (byte) 0)
    {
      --length;
      ++offset;
    }
    byte[] numArray = new byte[length];
    for (int index = 0; index < numArray.Length; ++index)
      numArray[index] = x[offset++];
    return numArray;
  }

  private static void ReadIntType(byte[] x, ref int offset)
  {
    int num = (int) x[offset++];
    if (num != 2)
      throw new InvalidOperationException($"expected version, fonud {num}");
  }

  private static int ReadUnivesalInt(byte[] x, ref int offset)
  {
    PythonSsl.ReadIntType(x, ref offset);
    return PythonSsl.ReadInt(x, ref offset);
  }

  private static int ReadLength(byte[] x, ref int offset)
  {
    int num = (int) x[offset++];
    return (num & 128 /*0x80*/) == 0 ? num : PythonSsl.ReadInt(x, ref offset, num & -129);
  }

  private static int ReadInt(byte[] x, ref int offset, int bytes)
  {
    if (bytes + offset > x.Length)
      throw new InvalidOperationException();
    int num = 0;
    for (int index = 0; index < bytes; ++index)
      num = num << 8 | (int) x[offset++];
    return num;
  }

  private static int ReadInt(byte[] x, ref int offset)
  {
    int bytes = (int) x[offset++];
    return PythonSsl.ReadInt(x, ref offset, bytes);
  }

  private static string ReadToEnd(string[] lines, ref int start, string end)
  {
    StringBuilder stringBuilder = new StringBuilder();
    ++start;
    while (start < lines.Length)
    {
      if (lines[start] == end)
        return stringBuilder.ToString();
      stringBuilder.Append(lines[start]);
      ++start;
    }
    return (string) null;
  }

  private static Exception ErrorDecoding(CodeContext context, params object[] args)
  {
    return PythonExceptions.CreateThrowable(PythonSsl.SSLError(context), ArrayUtils.Insert<object>((object) "Error decoding PEM-encoded file ", args));
  }

  [PythonType]
  public class _SSLContext
  {
    private X509Certificate2Collection _cert_store = new X509Certificate2Collection();
    private string _cafile;
    private int _verify_mode;

    public _SSLContext(CodeContext context, int protocol = 2)
    {
      this.protocol = protocol == 0 || protocol == 2 || protocol == 1 || protocol == 3 || protocol == 4 || protocol == 5 ? protocol : throw PythonOps.ValueError("invalid protocol version");
      if (protocol != 0)
        this.options |= 16777216 /*0x01000000*/;
      if (protocol != 1)
        this.options |= 33554432 /*0x02000000*/;
      this.verify_mode = 0;
      this.check_hostname = false;
    }

    public void set_ciphers(CodeContext context, string ciphers)
    {
    }

    public int options { get; set; }

    public int verify_mode
    {
      get => this._verify_mode;
      set
      {
        this._verify_mode = this._verify_mode == 0 || this._verify_mode == 1 || this._verify_mode == 2 ? value : throw PythonOps.ValueError("invalid value for verify_mode");
      }
    }

    public int protocol { get; set; }

    public bool check_hostname { get; set; }

    public void set_default_verify_paths(CodeContext context)
    {
    }

    public void load_cert_chain(string certfile, string keyfile = null, object password = null)
    {
    }

    public void load_verify_locations(
      CodeContext context,
      string cafile = null,
      string capath = null,
      object cadata = null)
    {
      if (cafile == null && capath == null && cadata == null)
        throw PythonOps.TypeError("cafile, capath and cadata cannot be all omitted");
      if (cafile != null)
      {
        this._cert_store.Add(PythonSsl.ReadCertificate(context, cafile));
        this._cafile = cafile;
      }
      if (cadata == null || !(cadata is IBufferProtocol bufferProtocol))
        return;
      int sourceIndex = 0;
      int? end = new int?();
      X509Certificate2 certificate;
      for (byte[] byteArray = bufferProtocol.ToBytes(0, end).ToByteArray(); sourceIndex < byteArray.Length; sourceIndex += certificate.GetRawCertData().Length)
      {
        byte[] numArray = new byte[byteArray.Length - sourceIndex];
        Array.Copy((Array) byteArray, sourceIndex, (Array) numArray, 0, byteArray.Length - sourceIndex);
        certificate = new X509Certificate2(numArray);
        this._cert_store.Add(certificate);
      }
    }

    public object _wrap_socket(
      CodeContext context,
      PythonSocket.socket sock = null,
      bool server_side = false,
      string server_hostname = null,
      object ssl_sock = null)
    {
      return (object) new PythonSocket.ssl(context, sock, server_side, certfile: this._cafile, certs_mode: this.verify_mode, protocol: this.protocol | this.options, certs: this._cert_store)
      {
        _serverHostName = server_hostname
      };
    }
  }
}
