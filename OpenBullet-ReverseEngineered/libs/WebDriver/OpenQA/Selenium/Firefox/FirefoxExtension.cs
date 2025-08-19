// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxExtension
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxExtension
{
  private const string EmNamespaceUri = "http://www.mozilla.org/2004/em-rdf#";
  private const string RdfManifestFileName = "install.rdf";
  private const string JsonManifestFileName = "manifest.json";
  private string extensionFileName;
  private string extensionResourceId;

  public FirefoxExtension(string fileName)
    : this(fileName, string.Empty)
  {
  }

  internal FirefoxExtension(string fileName, string resourceId)
  {
    this.extensionFileName = fileName;
    this.extensionResourceId = resourceId;
  }

  public void Install(string profileDirectory)
  {
    DirectoryInfo directoryInfo = new DirectoryInfo(profileDirectory);
    string str1 = Path.Combine(Path.GetTempPath(), directoryInfo.Name + ".staging");
    string str2 = Path.Combine(str1, Path.GetFileName(this.extensionFileName));
    if (Directory.Exists(str2))
      Directory.Delete(str2, true);
    Directory.CreateDirectory(str2);
    using (ZipStorer zipStorer = ZipStorer.Open(ResourceUtilities.GetResourceStream(this.extensionFileName, this.extensionResourceId), FileAccess.Read))
    {
      foreach (ZipStorer.ZipFileEntry zipFileEntry in zipStorer.ReadCentralDirectory())
      {
        string path2 = zipFileEntry.FilenameInZip.Replace('/', Path.DirectorySeparatorChar);
        string destinationFileName = Path.Combine(str2, path2);
        zipStorer.ExtractFile(zipFileEntry, destinationFileName);
      }
    }
    string extensionId = FirefoxExtension.GetExtensionId(str2);
    string str3 = Path.Combine(Path.Combine(profileDirectory, "extensions"), extensionId);
    if (Directory.Exists(str3))
      Directory.Delete(str3, true);
    Directory.CreateDirectory(str3);
    FileUtilities.CopyDirectory(str2, str3);
    FileUtilities.DeleteDirectory(str1);
  }

  private static string GetExtensionId(string root)
  {
    string path = Path.Combine(root, "manifest.json");
    if (File.Exists(Path.Combine(root, "install.rdf")))
      return FirefoxExtension.ReadIdFromInstallRdf(root);
    if (File.Exists(path))
      return FirefoxExtension.ReadIdFromManifestJson(root);
    throw new WebDriverException("Extension should contain either install.rdf or manifest.json metadata file");
  }

  private static string ReadIdFromInstallRdf(string root)
  {
    string filename = Path.Combine(root, "install.rdf");
    string innerText;
    try
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filename);
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
      nsmgr.AddNamespace("em", "http://www.mozilla.org/2004/em-rdf#");
      nsmgr.AddNamespace("RDF", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
      XmlNode xmlNode = xmlDocument.SelectSingleNode("//em:id", nsmgr);
      if (xmlNode == null)
        innerText = (xmlDocument.SelectSingleNode("//RDF:Description", nsmgr).Attributes["id", "http://www.mozilla.org/2004/em-rdf#"] ?? throw new WebDriverException("Cannot locate node containing extension id: " + filename)).Value;
      else
        innerText = xmlNode.InnerText;
      if (string.IsNullOrEmpty(innerText))
        throw new FileNotFoundException("Cannot install extension with ID: " + innerText);
    }
    catch (Exception ex)
    {
      throw new WebDriverException("Error installing extension", ex);
    }
    return innerText;
  }

  private static string ReadIdFromManifestJson(string root)
  {
    string str = (string) null;
    JObject jobject = JObject.Parse(File.ReadAllText(Path.Combine(root, "manifest.json")));
    if (jobject["applications"] != null)
    {
      JToken jtoken1 = jobject["applications"];
      if (jtoken1[(object) "gecko"] != null)
      {
        JToken jtoken2 = jtoken1[(object) "gecko"];
        if (jtoken2[(object) "id"] != null)
          str = jtoken2[(object) "id"].ToString().Trim();
      }
    }
    if (string.IsNullOrEmpty(str))
      str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}", (object) jobject["name"].ToString().Replace(" ", ""), (object) jobject["version"].ToString());
    return str;
  }
}
