// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockTCP
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using RuriLib.LS;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockTCP : BlockBase
{
  private TCPCommand tcpCommand;
  private string host = "";
  private string port = "";
  private bool useSSL = true;
  private bool webSocket;
  private bool waitForHello = true;
  private string message = "";
  private string variableName = "";
  private bool isCapture;

  public TCPCommand TCPCommand
  {
    get => this.tcpCommand;
    set
    {
      this.tcpCommand = value;
      this.OnPropertyChanged(nameof (TCPCommand));
    }
  }

  public string Host
  {
    get => this.host;
    set
    {
      this.host = value;
      this.OnPropertyChanged(nameof (Host));
    }
  }

  public string Port
  {
    get => this.port;
    set
    {
      this.port = value;
      this.OnPropertyChanged(nameof (Port));
    }
  }

  public bool UseSSL
  {
    get => this.useSSL;
    set
    {
      this.useSSL = value;
      this.OnPropertyChanged(nameof (UseSSL));
    }
  }

  public bool WebSocket
  {
    get => this.webSocket;
    set
    {
      this.webSocket = value;
      this.OnPropertyChanged(nameof (WebSocket));
    }
  }

  public bool WaitForHello
  {
    get => this.waitForHello;
    set
    {
      this.waitForHello = value;
      this.OnPropertyChanged(nameof (WaitForHello));
    }
  }

  public string Message
  {
    get => this.message;
    set
    {
      this.message = value;
      this.OnPropertyChanged(nameof (Message));
    }
  }

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public bool IsCapture
  {
    get => this.isCapture;
    set
    {
      this.isCapture = value;
      this.OnPropertyChanged(nameof (IsCapture));
    }
  }

  public BlockTCP() => this.Label = "TCP";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    // ISSUE: reference to a compiler-generated field
    if (BlockTCP.\u003C\u003Eo__37.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockTCP.\u003C\u003Eo__37.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, TCPCommand>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (TCPCommand), typeof (BlockTCP)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.TCPCommand = BlockTCP.\u003C\u003Eo__37.\u003C\u003Ep__0.Target((CallSite) BlockTCP.\u003C\u003Eo__37.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "Command", typeof (TCPCommand)));
    switch (this.TCPCommand)
    {
      case TCPCommand.Connect:
        this.Host = LineParser.ParseLiteral(ref input, "Host");
        this.Port = LineParser.ParseLiteral(ref input, "Port");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        break;
      case TCPCommand.Send:
        this.Message = LineParser.ParseLiteral(ref input, "Message");
        if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
        {
          LineParser.SetBool(ref input, (object) this);
          break;
        }
        break;
    }
    if (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Arrow, false) == "")
      return (BlockBase) this;
    try
    {
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (!(token.ToUpper() == "VAR"))
      {
        if (!(token.ToUpper() == "CAP"))
          goto label_16;
      }
      this.IsCapture = token.ToUpper() == "CAP";
    }
    catch
    {
      throw new ArgumentException("Invalid or missing variable type");
    }
label_16:
    try
    {
      this.VariableName = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Literal, true);
    }
    catch
    {
      throw new ArgumentException("Variable name not specified");
    }
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "TCP").Token((object) this.TCPCommand);
    switch (this.TCPCommand)
    {
      case TCPCommand.Connect:
        blockWriter.Literal(this.Host).Literal(this.Port).Boolean(this.UseSSL, "UseSSL").Boolean(this.WaitForHello, "WaitForHello");
        break;
      case TCPCommand.Send:
        blockWriter.Literal(this.Message).Boolean(this.WebSocket, "WebSocket");
        break;
    }
    if (!blockWriter.CheckDefault((object) this.VariableName, "VariableName"))
      blockWriter.Arrow().Token(this.IsCapture ? (object) "CAP" : (object) "VAR").Literal(this.VariableName);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    TcpClient tcpClient1 = data.TCPClient;
    NetworkStream netStream = data.NETStream;
    SslStream sslStream = data.SSLStream;
    byte[] numArray1 = new byte[2048 /*0x0800*/];
    string logString = "";
    switch (this.TCPCommand)
    {
      case TCPCommand.Connect:
        string str1 = BlockBase.ReplaceValues(this.host, data);
        int port = int.Parse(BlockBase.ReplaceValues(this.port, data));
        TcpClient tcpClient2 = new TcpClient();
        tcpClient2.Connect(str1, port);
        if (tcpClient2.Connected)
        {
          NetworkStream stream = tcpClient2.GetStream();
          if (this.UseSSL)
          {
            sslStream = new SslStream((Stream) stream);
            sslStream.AuthenticateAsClient(str1);
          }
          if (this.WaitForHello)
          {
            int count = !this.UseSSL ? stream.Read(numArray1, 0, numArray1.Length) : sslStream.Read(numArray1, 0, numArray1.Length);
            logString = Encoding.ASCII.GetString(numArray1, 0, count);
          }
          data.TCPClient = tcpClient2;
          data.NETStream = stream;
          data.SSLStream = sslStream;
          data.TCPSSL = this.UseSSL;
          data.Log(new LogEntry($"Succesfully connected to host {str1} on port {port}. The server says:", Colors.Green));
          data.Log(new LogEntry(logString, Colors.GreenYellow));
        }
        if (!(this.VariableName != ""))
          break;
        data.Variables.Set(new CVar(this.VariableName, logString, this.IsCapture));
        data.Log(new LogEntry("Saved Response in variable " + this.VariableName, Colors.White));
        break;
      case TCPCommand.Disconnect:
        if (tcpClient1 == null)
          throw new Exception("Make a connection first!");
        tcpClient1.Close();
        netStream?.Close();
        sslStream?.Close();
        data.Log(new LogEntry("Succesfully closed the stream", Colors.GreenYellow));
        break;
      case TCPCommand.Send:
        if (tcpClient1 == null)
          throw new Exception("Make a connection first!");
        string str2 = BlockBase.ReplaceValues(this.Message, data);
        byte[] numArray2 = new byte[0];
        byte[] bytes = Encoding.ASCII.GetBytes(str2.Replace("\\r\\n", "\r\n"));
        byte[] buffer;
        if (this.WebSocket)
        {
          List<byte> byteList = new List<byte>();
          byteList.Add((byte) 129);
          ulong length = (ulong) bytes.Length;
          if (length <= 125UL)
            byteList.Add((byte) (length + 128UL /*0x80*/));
          else if (length <= (ulong) ushort.MaxValue)
          {
            byteList.Add((byte) 254);
            byteList.Add((byte) (length >> 8));
            byteList.Add((byte) (length % (ulong) byte.MaxValue));
          }
          else if (length <= ulong.MaxValue)
          {
            byteList.Add(byte.MaxValue);
            byteList.Add((byte) (length >> 24));
            byteList.Add((byte) ((length >> 16 /*0x10*/) % (ulong) byte.MaxValue));
            byteList.Add((byte) ((length >> 8) % (ulong) byte.MaxValue));
            byteList.Add((byte) (length % (ulong) byte.MaxValue));
          }
          byte[] collection = new byte[4]
          {
            (byte) 61,
            (byte) 84,
            (byte) 35,
            (byte) 6
          };
          byteList.AddRange((IEnumerable<byte>) collection);
          for (int index = 0; index < bytes.Length; ++index)
            byteList.Add((byte) ((uint) bytes[index] ^ (uint) collection[index % 4]));
          buffer = byteList.ToArray();
        }
        else
          buffer = bytes;
        data.Log(new LogEntry("> " + str2, Colors.White));
        int count1;
        if (data.TCPSSL)
        {
          sslStream.Write(buffer);
          count1 = sslStream.Read(numArray1, 0, numArray1.Length);
        }
        else
        {
          netStream.Write(buffer, 0, buffer.Length);
          count1 = netStream.Read(numArray1, 0, numArray1.Length);
        }
        string str3 = Encoding.ASCII.GetString(numArray1, 0, count1);
        data.Log(new LogEntry("> " + str3, Colors.GreenYellow));
        if (!(this.VariableName != ""))
          break;
        data.Variables.Set(new CVar(this.VariableName, str3, this.IsCapture));
        data.Log(new LogEntry($"Saved Response in variable {this.VariableName}.", Colors.White));
        break;
    }
  }
}
