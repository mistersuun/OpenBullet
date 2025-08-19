// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.HtmlEntityProvider
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Html;

public sealed class HtmlEntityProvider : IEntityProvider
{
  private readonly Dictionary<char, Dictionary<string, string>> _entities;
  public static readonly IEntityProvider Resolver = (IEntityProvider) new HtmlEntityProvider();

  private HtmlEntityProvider()
  {
    this._entities = new Dictionary<char, Dictionary<string, string>>()
    {
      {
        'a',
        this.GetSymbolLittleA()
      },
      {
        'A',
        this.GetSymbolBigA()
      },
      {
        'b',
        this.GetSymbolLittleB()
      },
      {
        'B',
        this.GetSymbolBigB()
      },
      {
        'c',
        this.GetSymbolLittleC()
      },
      {
        'C',
        this.GetSymbolBigC()
      },
      {
        'd',
        this.GetSymbolLittleD()
      },
      {
        'D',
        this.GetSymbolBigD()
      },
      {
        'e',
        this.GetSymbolLittleE()
      },
      {
        'E',
        this.GetSymbolBigE()
      },
      {
        'f',
        this.GetSymbolLittleF()
      },
      {
        'F',
        this.GetSymbolBigF()
      },
      {
        'g',
        this.GetSymbolLittleG()
      },
      {
        'G',
        this.GetSymbolBigG()
      },
      {
        'h',
        this.GetSymbolLittleH()
      },
      {
        'H',
        this.GetSymbolBigH()
      },
      {
        'i',
        this.GetSymbolLittleI()
      },
      {
        'I',
        this.GetSymbolBigI()
      },
      {
        'j',
        this.GetSymbolLittleJ()
      },
      {
        'J',
        this.GetSymbolBigJ()
      },
      {
        'k',
        this.GetSymbolLittleK()
      },
      {
        'K',
        this.GetSymbolBigK()
      },
      {
        'l',
        this.GetSymbolLittleL()
      },
      {
        'L',
        this.GetSymbolBigL()
      },
      {
        'm',
        this.GetSymbolLittleM()
      },
      {
        'M',
        this.GetSymbolBigM()
      },
      {
        'n',
        this.GetSymbolLittleN()
      },
      {
        'N',
        this.GetSymbolBigN()
      },
      {
        'o',
        this.GetSymbolLittleO()
      },
      {
        'O',
        this.GetSymbolBigO()
      },
      {
        'p',
        this.GetSymbolLittleP()
      },
      {
        'P',
        this.GetSymbolBigP()
      },
      {
        'q',
        this.GetSymbolLittleQ()
      },
      {
        'Q',
        this.GetSymbolBigQ()
      },
      {
        'r',
        this.GetSymbolLittleR()
      },
      {
        'R',
        this.GetSymbolBigR()
      },
      {
        's',
        this.GetSymbolLittleS()
      },
      {
        'S',
        this.GetSymbolBigS()
      },
      {
        't',
        this.GetSymbolLittleT()
      },
      {
        'T',
        this.GetSymbolBigT()
      },
      {
        'u',
        this.GetSymbolLittleU()
      },
      {
        'U',
        this.GetSymbolBigU()
      },
      {
        'v',
        this.GetSymbolLittleV()
      },
      {
        'V',
        this.GetSymbolBigV()
      },
      {
        'w',
        this.GetSymbolLittleW()
      },
      {
        'W',
        this.GetSymbolBigW()
      },
      {
        'x',
        this.GetSymbolLittleX()
      },
      {
        'X',
        this.GetSymbolBigX()
      },
      {
        'y',
        this.GetSymbolLittleY()
      },
      {
        'Y',
        this.GetSymbolBigY()
      },
      {
        'z',
        this.GetSymbolLittleZ()
      },
      {
        'Z',
        this.GetSymbolBigZ()
      }
    };
  }

  private Dictionary<string, string> GetSymbolLittleA()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "aacute;", HtmlEntityProvider.Convert(225));
    HtmlEntityProvider.AddSingle(symbols, "abreve;", HtmlEntityProvider.Convert(259));
    HtmlEntityProvider.AddSingle(symbols, "ac;", HtmlEntityProvider.Convert(8766));
    HtmlEntityProvider.AddSingle(symbols, "acd;", HtmlEntityProvider.Convert(8767));
    HtmlEntityProvider.AddSingle(symbols, "acE;", HtmlEntityProvider.Convert(8766, 819));
    HtmlEntityProvider.AddBoth(symbols, "acirc;", HtmlEntityProvider.Convert(226));
    HtmlEntityProvider.AddBoth(symbols, "acute;", HtmlEntityProvider.Convert(180));
    HtmlEntityProvider.AddSingle(symbols, "acy;", HtmlEntityProvider.Convert(1072));
    HtmlEntityProvider.AddBoth(symbols, "aelig;", HtmlEntityProvider.Convert(230));
    HtmlEntityProvider.AddSingle(symbols, "af;", HtmlEntityProvider.Convert(8289));
    HtmlEntityProvider.AddSingle(symbols, "afr;", HtmlEntityProvider.Convert(120094));
    HtmlEntityProvider.AddBoth(symbols, "agrave;", HtmlEntityProvider.Convert(224 /*0xE0*/));
    HtmlEntityProvider.AddSingle(symbols, "alefsym;", HtmlEntityProvider.Convert(8501));
    HtmlEntityProvider.AddSingle(symbols, "aleph;", HtmlEntityProvider.Convert(8501));
    HtmlEntityProvider.AddSingle(symbols, "alpha;", HtmlEntityProvider.Convert(945));
    HtmlEntityProvider.AddSingle(symbols, "amacr;", HtmlEntityProvider.Convert(257));
    HtmlEntityProvider.AddSingle(symbols, "amalg;", HtmlEntityProvider.Convert(10815));
    HtmlEntityProvider.AddBoth(symbols, "amp;", HtmlEntityProvider.Convert(38));
    HtmlEntityProvider.AddSingle(symbols, "and;", HtmlEntityProvider.Convert(8743));
    HtmlEntityProvider.AddSingle(symbols, "andand;", HtmlEntityProvider.Convert(10837));
    HtmlEntityProvider.AddSingle(symbols, "andd;", HtmlEntityProvider.Convert(10844));
    HtmlEntityProvider.AddSingle(symbols, "andslope;", HtmlEntityProvider.Convert(10840));
    HtmlEntityProvider.AddSingle(symbols, "andv;", HtmlEntityProvider.Convert(10842));
    HtmlEntityProvider.AddSingle(symbols, "ang;", HtmlEntityProvider.Convert(8736));
    HtmlEntityProvider.AddSingle(symbols, "ange;", HtmlEntityProvider.Convert(10660));
    HtmlEntityProvider.AddSingle(symbols, "angle;", HtmlEntityProvider.Convert(8736));
    HtmlEntityProvider.AddSingle(symbols, "angmsd;", HtmlEntityProvider.Convert(8737));
    HtmlEntityProvider.AddSingle(symbols, "angmsdaa;", HtmlEntityProvider.Convert(10664));
    HtmlEntityProvider.AddSingle(symbols, "angmsdab;", HtmlEntityProvider.Convert(10665));
    HtmlEntityProvider.AddSingle(symbols, "angmsdac;", HtmlEntityProvider.Convert(10666));
    HtmlEntityProvider.AddSingle(symbols, "angmsdad;", HtmlEntityProvider.Convert(10667));
    HtmlEntityProvider.AddSingle(symbols, "angmsdae;", HtmlEntityProvider.Convert(10668));
    HtmlEntityProvider.AddSingle(symbols, "angmsdaf;", HtmlEntityProvider.Convert(10669));
    HtmlEntityProvider.AddSingle(symbols, "angmsdag;", HtmlEntityProvider.Convert(10670));
    HtmlEntityProvider.AddSingle(symbols, "angmsdah;", HtmlEntityProvider.Convert(10671));
    HtmlEntityProvider.AddSingle(symbols, "angrt;", HtmlEntityProvider.Convert(8735));
    HtmlEntityProvider.AddSingle(symbols, "angrtvb;", HtmlEntityProvider.Convert(8894));
    HtmlEntityProvider.AddSingle(symbols, "angrtvbd;", HtmlEntityProvider.Convert(10653));
    HtmlEntityProvider.AddSingle(symbols, "angsph;", HtmlEntityProvider.Convert(8738 /*0x2222*/));
    HtmlEntityProvider.AddSingle(symbols, "angst;", HtmlEntityProvider.Convert(197));
    HtmlEntityProvider.AddSingle(symbols, "angzarr;", HtmlEntityProvider.Convert(9084));
    HtmlEntityProvider.AddSingle(symbols, "aogon;", HtmlEntityProvider.Convert(261));
    HtmlEntityProvider.AddSingle(symbols, "aopf;", HtmlEntityProvider.Convert(120146));
    HtmlEntityProvider.AddSingle(symbols, "ap;", HtmlEntityProvider.Convert(8776));
    HtmlEntityProvider.AddSingle(symbols, "apacir;", HtmlEntityProvider.Convert(10863));
    HtmlEntityProvider.AddSingle(symbols, "apE;", HtmlEntityProvider.Convert(10864));
    HtmlEntityProvider.AddSingle(symbols, "ape;", HtmlEntityProvider.Convert(8778));
    HtmlEntityProvider.AddSingle(symbols, "apid;", HtmlEntityProvider.Convert(8779));
    HtmlEntityProvider.AddSingle(symbols, "apos;", HtmlEntityProvider.Convert(39));
    HtmlEntityProvider.AddSingle(symbols, "approx;", HtmlEntityProvider.Convert(8776));
    HtmlEntityProvider.AddSingle(symbols, "approxeq;", HtmlEntityProvider.Convert(8778));
    HtmlEntityProvider.AddBoth(symbols, "aring;", HtmlEntityProvider.Convert(229));
    HtmlEntityProvider.AddSingle(symbols, "ascr;", HtmlEntityProvider.Convert(119990));
    HtmlEntityProvider.AddSingle(symbols, "ast;", HtmlEntityProvider.Convert(42));
    HtmlEntityProvider.AddSingle(symbols, "asymp;", HtmlEntityProvider.Convert(8776));
    HtmlEntityProvider.AddSingle(symbols, "asympeq;", HtmlEntityProvider.Convert(8781));
    HtmlEntityProvider.AddBoth(symbols, "atilde;", HtmlEntityProvider.Convert(227));
    HtmlEntityProvider.AddBoth(symbols, "auml;", HtmlEntityProvider.Convert(228));
    HtmlEntityProvider.AddSingle(symbols, "awconint;", HtmlEntityProvider.Convert(8755));
    HtmlEntityProvider.AddSingle(symbols, "awint;", HtmlEntityProvider.Convert(10769));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigA()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Aogon;", HtmlEntityProvider.Convert(260));
    HtmlEntityProvider.AddSingle(symbols, "Aopf;", HtmlEntityProvider.Convert(120120));
    HtmlEntityProvider.AddSingle(symbols, "ApplyFunction;", HtmlEntityProvider.Convert(8289));
    HtmlEntityProvider.AddBoth(symbols, "Aring;", HtmlEntityProvider.Convert(197));
    HtmlEntityProvider.AddSingle(symbols, "Ascr;", HtmlEntityProvider.Convert(119964));
    HtmlEntityProvider.AddSingle(symbols, "Assign;", HtmlEntityProvider.Convert(8788));
    HtmlEntityProvider.AddBoth(symbols, "Atilde;", HtmlEntityProvider.Convert(195));
    HtmlEntityProvider.AddBoth(symbols, "Auml;", HtmlEntityProvider.Convert(196));
    HtmlEntityProvider.AddBoth(symbols, "Aacute;", HtmlEntityProvider.Convert(193));
    HtmlEntityProvider.AddSingle(symbols, "Abreve;", HtmlEntityProvider.Convert(258));
    HtmlEntityProvider.AddBoth(symbols, "Acirc;", HtmlEntityProvider.Convert(194));
    HtmlEntityProvider.AddSingle(symbols, "Acy;", HtmlEntityProvider.Convert(1040));
    HtmlEntityProvider.AddBoth(symbols, "AElig;", HtmlEntityProvider.Convert(198));
    HtmlEntityProvider.AddSingle(symbols, "Afr;", HtmlEntityProvider.Convert(120068));
    HtmlEntityProvider.AddBoth(symbols, "Agrave;", HtmlEntityProvider.Convert(192 /*0xC0*/));
    HtmlEntityProvider.AddSingle(symbols, "Alpha;", HtmlEntityProvider.Convert(913));
    HtmlEntityProvider.AddSingle(symbols, "Amacr;", HtmlEntityProvider.Convert(256 /*0x0100*/));
    HtmlEntityProvider.AddBoth(symbols, "AMP;", HtmlEntityProvider.Convert(38));
    HtmlEntityProvider.AddSingle(symbols, "And;", HtmlEntityProvider.Convert(10835));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleB()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "backcong;", HtmlEntityProvider.Convert(8780));
    HtmlEntityProvider.AddSingle(symbols, "backepsilon;", HtmlEntityProvider.Convert(1014));
    HtmlEntityProvider.AddSingle(symbols, "backprime;", HtmlEntityProvider.Convert(8245));
    HtmlEntityProvider.AddSingle(symbols, "backsim;", HtmlEntityProvider.Convert(8765));
    HtmlEntityProvider.AddSingle(symbols, "backsimeq;", HtmlEntityProvider.Convert(8909));
    HtmlEntityProvider.AddSingle(symbols, "barvee;", HtmlEntityProvider.Convert(8893));
    HtmlEntityProvider.AddSingle(symbols, "barwed;", HtmlEntityProvider.Convert(8965));
    HtmlEntityProvider.AddSingle(symbols, "barwedge;", HtmlEntityProvider.Convert(8965));
    HtmlEntityProvider.AddSingle(symbols, "bbrk;", HtmlEntityProvider.Convert(9141));
    HtmlEntityProvider.AddSingle(symbols, "bbrktbrk;", HtmlEntityProvider.Convert(9142));
    HtmlEntityProvider.AddSingle(symbols, "bcong;", HtmlEntityProvider.Convert(8780));
    HtmlEntityProvider.AddSingle(symbols, "bcy;", HtmlEntityProvider.Convert(1073));
    HtmlEntityProvider.AddSingle(symbols, "bdquo;", HtmlEntityProvider.Convert(8222));
    HtmlEntityProvider.AddSingle(symbols, "becaus;", HtmlEntityProvider.Convert(8757));
    HtmlEntityProvider.AddSingle(symbols, "because;", HtmlEntityProvider.Convert(8757));
    HtmlEntityProvider.AddSingle(symbols, "bemptyv;", HtmlEntityProvider.Convert(10672));
    HtmlEntityProvider.AddSingle(symbols, "bepsi;", HtmlEntityProvider.Convert(1014));
    HtmlEntityProvider.AddSingle(symbols, "bernou;", HtmlEntityProvider.Convert(8492));
    HtmlEntityProvider.AddSingle(symbols, "beta;", HtmlEntityProvider.Convert(946));
    HtmlEntityProvider.AddSingle(symbols, "beth;", HtmlEntityProvider.Convert(8502));
    HtmlEntityProvider.AddSingle(symbols, "between;", HtmlEntityProvider.Convert(8812));
    HtmlEntityProvider.AddSingle(symbols, "bfr;", HtmlEntityProvider.Convert(120095));
    HtmlEntityProvider.AddSingle(symbols, "bigcap;", HtmlEntityProvider.Convert(8898));
    HtmlEntityProvider.AddSingle(symbols, "bigcirc;", HtmlEntityProvider.Convert(9711));
    HtmlEntityProvider.AddSingle(symbols, "bigcup;", HtmlEntityProvider.Convert(8899));
    HtmlEntityProvider.AddSingle(symbols, "bigodot;", HtmlEntityProvider.Convert(10752));
    HtmlEntityProvider.AddSingle(symbols, "bigoplus;", HtmlEntityProvider.Convert(10753));
    HtmlEntityProvider.AddSingle(symbols, "bigotimes;", HtmlEntityProvider.Convert(10754));
    HtmlEntityProvider.AddSingle(symbols, "bigsqcup;", HtmlEntityProvider.Convert(10758));
    HtmlEntityProvider.AddSingle(symbols, "bigstar;", HtmlEntityProvider.Convert(9733));
    HtmlEntityProvider.AddSingle(symbols, "bigtriangledown;", HtmlEntityProvider.Convert(9661));
    HtmlEntityProvider.AddSingle(symbols, "bigtriangleup;", HtmlEntityProvider.Convert(9651));
    HtmlEntityProvider.AddSingle(symbols, "biguplus;", HtmlEntityProvider.Convert(10756));
    HtmlEntityProvider.AddSingle(symbols, "bigvee;", HtmlEntityProvider.Convert(8897));
    HtmlEntityProvider.AddSingle(symbols, "bigwedge;", HtmlEntityProvider.Convert(8896));
    HtmlEntityProvider.AddSingle(symbols, "bkarow;", HtmlEntityProvider.Convert(10509));
    HtmlEntityProvider.AddSingle(symbols, "blacklozenge;", HtmlEntityProvider.Convert(10731));
    HtmlEntityProvider.AddSingle(symbols, "blacksquare;", HtmlEntityProvider.Convert(9642));
    HtmlEntityProvider.AddSingle(symbols, "blacktriangle;", HtmlEntityProvider.Convert(9652));
    HtmlEntityProvider.AddSingle(symbols, "blacktriangledown;", HtmlEntityProvider.Convert(9662));
    HtmlEntityProvider.AddSingle(symbols, "blacktriangleleft;", HtmlEntityProvider.Convert(9666));
    HtmlEntityProvider.AddSingle(symbols, "blacktriangleright;", HtmlEntityProvider.Convert(9656));
    HtmlEntityProvider.AddSingle(symbols, "blank;", HtmlEntityProvider.Convert(9251));
    HtmlEntityProvider.AddSingle(symbols, "blk12;", HtmlEntityProvider.Convert(9618));
    HtmlEntityProvider.AddSingle(symbols, "blk14;", HtmlEntityProvider.Convert(9617));
    HtmlEntityProvider.AddSingle(symbols, "blk34;", HtmlEntityProvider.Convert(9619));
    HtmlEntityProvider.AddSingle(symbols, "block;", HtmlEntityProvider.Convert(9608));
    HtmlEntityProvider.AddSingle(symbols, "bne;", HtmlEntityProvider.Convert(61, 8421));
    HtmlEntityProvider.AddSingle(symbols, "bnequiv;", HtmlEntityProvider.Convert(8801, 8421));
    HtmlEntityProvider.AddSingle(symbols, "bNot;", HtmlEntityProvider.Convert(10989));
    HtmlEntityProvider.AddSingle(symbols, "bnot;", HtmlEntityProvider.Convert(8976));
    HtmlEntityProvider.AddSingle(symbols, "bopf;", HtmlEntityProvider.Convert(120147));
    HtmlEntityProvider.AddSingle(symbols, "bot;", HtmlEntityProvider.Convert(8869));
    HtmlEntityProvider.AddSingle(symbols, "bottom;", HtmlEntityProvider.Convert(8869));
    HtmlEntityProvider.AddSingle(symbols, "bowtie;", HtmlEntityProvider.Convert(8904));
    HtmlEntityProvider.AddSingle(symbols, "boxbox;", HtmlEntityProvider.Convert(10697));
    HtmlEntityProvider.AddSingle(symbols, "boxDL;", HtmlEntityProvider.Convert(9559));
    HtmlEntityProvider.AddSingle(symbols, "boxDl;", HtmlEntityProvider.Convert(9558));
    HtmlEntityProvider.AddSingle(symbols, "boxdL;", HtmlEntityProvider.Convert(9557));
    HtmlEntityProvider.AddSingle(symbols, "boxdl;", HtmlEntityProvider.Convert(9488));
    HtmlEntityProvider.AddSingle(symbols, "boxDR;", HtmlEntityProvider.Convert(9556));
    HtmlEntityProvider.AddSingle(symbols, "boxDr;", HtmlEntityProvider.Convert(9555));
    HtmlEntityProvider.AddSingle(symbols, "boxdR;", HtmlEntityProvider.Convert(9554));
    HtmlEntityProvider.AddSingle(symbols, "boxdr;", HtmlEntityProvider.Convert(9484));
    HtmlEntityProvider.AddSingle(symbols, "boxH;", HtmlEntityProvider.Convert(9552));
    HtmlEntityProvider.AddSingle(symbols, "boxh;", HtmlEntityProvider.Convert(9472));
    HtmlEntityProvider.AddSingle(symbols, "boxHD;", HtmlEntityProvider.Convert(9574));
    HtmlEntityProvider.AddSingle(symbols, "boxHd;", HtmlEntityProvider.Convert(9572));
    HtmlEntityProvider.AddSingle(symbols, "boxhD;", HtmlEntityProvider.Convert(9573));
    HtmlEntityProvider.AddSingle(symbols, "boxhd;", HtmlEntityProvider.Convert(9516));
    HtmlEntityProvider.AddSingle(symbols, "boxHU;", HtmlEntityProvider.Convert(9577));
    HtmlEntityProvider.AddSingle(symbols, "boxHu;", HtmlEntityProvider.Convert(9575));
    HtmlEntityProvider.AddSingle(symbols, "boxhU;", HtmlEntityProvider.Convert(9576));
    HtmlEntityProvider.AddSingle(symbols, "boxhu;", HtmlEntityProvider.Convert(9524));
    HtmlEntityProvider.AddSingle(symbols, "boxminus;", HtmlEntityProvider.Convert(8863));
    HtmlEntityProvider.AddSingle(symbols, "boxplus;", HtmlEntityProvider.Convert(8862));
    HtmlEntityProvider.AddSingle(symbols, "boxtimes;", HtmlEntityProvider.Convert(8864));
    HtmlEntityProvider.AddSingle(symbols, "boxUL;", HtmlEntityProvider.Convert(9565));
    HtmlEntityProvider.AddSingle(symbols, "boxUl;", HtmlEntityProvider.Convert(9564));
    HtmlEntityProvider.AddSingle(symbols, "boxuL;", HtmlEntityProvider.Convert(9563));
    HtmlEntityProvider.AddSingle(symbols, "boxul;", HtmlEntityProvider.Convert(9496));
    HtmlEntityProvider.AddSingle(symbols, "boxUR;", HtmlEntityProvider.Convert(9562));
    HtmlEntityProvider.AddSingle(symbols, "boxUr;", HtmlEntityProvider.Convert(9561));
    HtmlEntityProvider.AddSingle(symbols, "boxuR;", HtmlEntityProvider.Convert(9560));
    HtmlEntityProvider.AddSingle(symbols, "boxur;", HtmlEntityProvider.Convert(9492));
    HtmlEntityProvider.AddSingle(symbols, "boxV;", HtmlEntityProvider.Convert(9553));
    HtmlEntityProvider.AddSingle(symbols, "boxv;", HtmlEntityProvider.Convert(9474));
    HtmlEntityProvider.AddSingle(symbols, "boxVH;", HtmlEntityProvider.Convert(9580));
    HtmlEntityProvider.AddSingle(symbols, "boxVh;", HtmlEntityProvider.Convert(9579));
    HtmlEntityProvider.AddSingle(symbols, "boxvH;", HtmlEntityProvider.Convert(9578));
    HtmlEntityProvider.AddSingle(symbols, "boxvh;", HtmlEntityProvider.Convert(9532));
    HtmlEntityProvider.AddSingle(symbols, "boxVL;", HtmlEntityProvider.Convert(9571));
    HtmlEntityProvider.AddSingle(symbols, "boxVl;", HtmlEntityProvider.Convert(9570));
    HtmlEntityProvider.AddSingle(symbols, "boxvL;", HtmlEntityProvider.Convert(9569));
    HtmlEntityProvider.AddSingle(symbols, "boxvl;", HtmlEntityProvider.Convert(9508));
    HtmlEntityProvider.AddSingle(symbols, "boxVR;", HtmlEntityProvider.Convert(9568));
    HtmlEntityProvider.AddSingle(symbols, "boxVr;", HtmlEntityProvider.Convert(9567));
    HtmlEntityProvider.AddSingle(symbols, "boxvR;", HtmlEntityProvider.Convert(9566));
    HtmlEntityProvider.AddSingle(symbols, "boxvr;", HtmlEntityProvider.Convert(9500));
    HtmlEntityProvider.AddSingle(symbols, "bprime;", HtmlEntityProvider.Convert(8245));
    HtmlEntityProvider.AddSingle(symbols, "breve;", HtmlEntityProvider.Convert(728));
    HtmlEntityProvider.AddBoth(symbols, "brvbar;", HtmlEntityProvider.Convert(166));
    HtmlEntityProvider.AddSingle(symbols, "bscr;", HtmlEntityProvider.Convert(119991));
    HtmlEntityProvider.AddSingle(symbols, "bsemi;", HtmlEntityProvider.Convert(8271));
    HtmlEntityProvider.AddSingle(symbols, "bsim;", HtmlEntityProvider.Convert(8765));
    HtmlEntityProvider.AddSingle(symbols, "bsime;", HtmlEntityProvider.Convert(8909));
    HtmlEntityProvider.AddSingle(symbols, "bsol;", HtmlEntityProvider.Convert(92));
    HtmlEntityProvider.AddSingle(symbols, "bsolb;", HtmlEntityProvider.Convert(10693));
    HtmlEntityProvider.AddSingle(symbols, "bsolhsub;", HtmlEntityProvider.Convert(10184));
    HtmlEntityProvider.AddSingle(symbols, "bull;", HtmlEntityProvider.Convert(8226));
    HtmlEntityProvider.AddSingle(symbols, "bullet;", HtmlEntityProvider.Convert(8226));
    HtmlEntityProvider.AddSingle(symbols, "bump;", HtmlEntityProvider.Convert(8782));
    HtmlEntityProvider.AddSingle(symbols, "bumpE;", HtmlEntityProvider.Convert(10926));
    HtmlEntityProvider.AddSingle(symbols, "bumpe;", HtmlEntityProvider.Convert(8783));
    HtmlEntityProvider.AddSingle(symbols, "bumpeq;", HtmlEntityProvider.Convert(8783));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigB()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Backslash;", HtmlEntityProvider.Convert(8726));
    HtmlEntityProvider.AddSingle(symbols, "Barv;", HtmlEntityProvider.Convert(10983));
    HtmlEntityProvider.AddSingle(symbols, "Barwed;", HtmlEntityProvider.Convert(8966));
    HtmlEntityProvider.AddSingle(symbols, "Bcy;", HtmlEntityProvider.Convert(1041));
    HtmlEntityProvider.AddSingle(symbols, "Because;", HtmlEntityProvider.Convert(8757));
    HtmlEntityProvider.AddSingle(symbols, "Bernoullis;", HtmlEntityProvider.Convert(8492));
    HtmlEntityProvider.AddSingle(symbols, "Beta;", HtmlEntityProvider.Convert(914));
    HtmlEntityProvider.AddSingle(symbols, "Bfr;", HtmlEntityProvider.Convert(120069));
    HtmlEntityProvider.AddSingle(symbols, "Bopf;", HtmlEntityProvider.Convert(120121));
    HtmlEntityProvider.AddSingle(symbols, "Breve;", HtmlEntityProvider.Convert(728));
    HtmlEntityProvider.AddSingle(symbols, "Bscr;", HtmlEntityProvider.Convert(8492));
    HtmlEntityProvider.AddSingle(symbols, "Bumpeq;", HtmlEntityProvider.Convert(8782));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleC()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "cacute;", HtmlEntityProvider.Convert(263));
    HtmlEntityProvider.AddSingle(symbols, "cap;", HtmlEntityProvider.Convert(8745));
    HtmlEntityProvider.AddSingle(symbols, "capand;", HtmlEntityProvider.Convert(10820));
    HtmlEntityProvider.AddSingle(symbols, "capbrcup;", HtmlEntityProvider.Convert(10825));
    HtmlEntityProvider.AddSingle(symbols, "capcap;", HtmlEntityProvider.Convert(10827));
    HtmlEntityProvider.AddSingle(symbols, "capcup;", HtmlEntityProvider.Convert(10823));
    HtmlEntityProvider.AddSingle(symbols, "capdot;", HtmlEntityProvider.Convert(10816));
    HtmlEntityProvider.AddSingle(symbols, "caps;", HtmlEntityProvider.Convert(8745, 65024));
    HtmlEntityProvider.AddSingle(symbols, "caret;", HtmlEntityProvider.Convert(8257));
    HtmlEntityProvider.AddSingle(symbols, "caron;", HtmlEntityProvider.Convert(711));
    HtmlEntityProvider.AddSingle(symbols, "ccaps;", HtmlEntityProvider.Convert(10829));
    HtmlEntityProvider.AddSingle(symbols, "ccaron;", HtmlEntityProvider.Convert(269));
    HtmlEntityProvider.AddBoth(symbols, "ccedil;", HtmlEntityProvider.Convert(231));
    HtmlEntityProvider.AddSingle(symbols, "ccirc;", HtmlEntityProvider.Convert(265));
    HtmlEntityProvider.AddSingle(symbols, "ccups;", HtmlEntityProvider.Convert(10828));
    HtmlEntityProvider.AddSingle(symbols, "ccupssm;", HtmlEntityProvider.Convert(10832));
    HtmlEntityProvider.AddSingle(symbols, "cdot;", HtmlEntityProvider.Convert(267));
    HtmlEntityProvider.AddBoth(symbols, "cedil;", HtmlEntityProvider.Convert(184));
    HtmlEntityProvider.AddSingle(symbols, "cemptyv;", HtmlEntityProvider.Convert(10674));
    HtmlEntityProvider.AddBoth(symbols, "cent;", HtmlEntityProvider.Convert(162));
    HtmlEntityProvider.AddSingle(symbols, "centerdot;", HtmlEntityProvider.Convert(183));
    HtmlEntityProvider.AddSingle(symbols, "cfr;", HtmlEntityProvider.Convert(120096));
    HtmlEntityProvider.AddSingle(symbols, "chcy;", HtmlEntityProvider.Convert(1095));
    HtmlEntityProvider.AddSingle(symbols, "check;", HtmlEntityProvider.Convert(10003));
    HtmlEntityProvider.AddSingle(symbols, "checkmark;", HtmlEntityProvider.Convert(10003));
    HtmlEntityProvider.AddSingle(symbols, "chi;", HtmlEntityProvider.Convert(967));
    HtmlEntityProvider.AddSingle(symbols, "cir;", HtmlEntityProvider.Convert(9675));
    HtmlEntityProvider.AddSingle(symbols, "circ;", HtmlEntityProvider.Convert(710));
    HtmlEntityProvider.AddSingle(symbols, "circeq;", HtmlEntityProvider.Convert(8791));
    HtmlEntityProvider.AddSingle(symbols, "circlearrowleft;", HtmlEntityProvider.Convert(8634));
    HtmlEntityProvider.AddSingle(symbols, "circlearrowright;", HtmlEntityProvider.Convert(8635));
    HtmlEntityProvider.AddSingle(symbols, "circledast;", HtmlEntityProvider.Convert(8859));
    HtmlEntityProvider.AddSingle(symbols, "circledcirc;", HtmlEntityProvider.Convert(8858));
    HtmlEntityProvider.AddSingle(symbols, "circleddash;", HtmlEntityProvider.Convert(8861));
    HtmlEntityProvider.AddSingle(symbols, "circledR;", HtmlEntityProvider.Convert(174));
    HtmlEntityProvider.AddSingle(symbols, "circledS;", HtmlEntityProvider.Convert(9416));
    HtmlEntityProvider.AddSingle(symbols, "cirE;", HtmlEntityProvider.Convert(10691));
    HtmlEntityProvider.AddSingle(symbols, "cire;", HtmlEntityProvider.Convert(8791));
    HtmlEntityProvider.AddSingle(symbols, "cirfnint;", HtmlEntityProvider.Convert(10768));
    HtmlEntityProvider.AddSingle(symbols, "cirmid;", HtmlEntityProvider.Convert(10991));
    HtmlEntityProvider.AddSingle(symbols, "cirscir;", HtmlEntityProvider.Convert(10690));
    HtmlEntityProvider.AddSingle(symbols, "clubs;", HtmlEntityProvider.Convert(9827));
    HtmlEntityProvider.AddSingle(symbols, "clubsuit;", HtmlEntityProvider.Convert(9827));
    HtmlEntityProvider.AddSingle(symbols, "colon;", HtmlEntityProvider.Convert(58));
    HtmlEntityProvider.AddSingle(symbols, "colone;", HtmlEntityProvider.Convert(8788));
    HtmlEntityProvider.AddSingle(symbols, "coloneq;", HtmlEntityProvider.Convert(8788));
    HtmlEntityProvider.AddSingle(symbols, "comma;", HtmlEntityProvider.Convert(44));
    HtmlEntityProvider.AddSingle(symbols, "commat;", HtmlEntityProvider.Convert(64 /*0x40*/));
    HtmlEntityProvider.AddSingle(symbols, "comp;", HtmlEntityProvider.Convert(8705));
    HtmlEntityProvider.AddSingle(symbols, "compfn;", HtmlEntityProvider.Convert(8728));
    HtmlEntityProvider.AddSingle(symbols, "complement;", HtmlEntityProvider.Convert(8705));
    HtmlEntityProvider.AddSingle(symbols, "complexes;", HtmlEntityProvider.Convert(8450));
    HtmlEntityProvider.AddSingle(symbols, "cong;", HtmlEntityProvider.Convert(8773));
    HtmlEntityProvider.AddSingle(symbols, "congdot;", HtmlEntityProvider.Convert(10861));
    HtmlEntityProvider.AddSingle(symbols, "conint;", HtmlEntityProvider.Convert(8750));
    HtmlEntityProvider.AddSingle(symbols, "copf;", HtmlEntityProvider.Convert(120148));
    HtmlEntityProvider.AddSingle(symbols, "coprod;", HtmlEntityProvider.Convert(8720));
    HtmlEntityProvider.AddBoth(symbols, "copy;", HtmlEntityProvider.Convert(169));
    HtmlEntityProvider.AddSingle(symbols, "copysr;", HtmlEntityProvider.Convert(8471));
    HtmlEntityProvider.AddSingle(symbols, "crarr;", HtmlEntityProvider.Convert(8629));
    HtmlEntityProvider.AddSingle(symbols, "cross;", HtmlEntityProvider.Convert(10007));
    HtmlEntityProvider.AddSingle(symbols, "cscr;", HtmlEntityProvider.Convert(119992));
    HtmlEntityProvider.AddSingle(symbols, "csub;", HtmlEntityProvider.Convert(10959));
    HtmlEntityProvider.AddSingle(symbols, "csube;", HtmlEntityProvider.Convert(10961));
    HtmlEntityProvider.AddSingle(symbols, "csup;", HtmlEntityProvider.Convert(10960));
    HtmlEntityProvider.AddSingle(symbols, "csupe;", HtmlEntityProvider.Convert(10962));
    HtmlEntityProvider.AddSingle(symbols, "ctdot;", HtmlEntityProvider.Convert(8943));
    HtmlEntityProvider.AddSingle(symbols, "cudarrl;", HtmlEntityProvider.Convert(10552));
    HtmlEntityProvider.AddSingle(symbols, "cudarrr;", HtmlEntityProvider.Convert(10549));
    HtmlEntityProvider.AddSingle(symbols, "cuepr;", HtmlEntityProvider.Convert(8926));
    HtmlEntityProvider.AddSingle(symbols, "cuesc;", HtmlEntityProvider.Convert(8927));
    HtmlEntityProvider.AddSingle(symbols, "cularr;", HtmlEntityProvider.Convert(8630));
    HtmlEntityProvider.AddSingle(symbols, "cularrp;", HtmlEntityProvider.Convert(10557));
    HtmlEntityProvider.AddSingle(symbols, "cup;", HtmlEntityProvider.Convert(8746));
    HtmlEntityProvider.AddSingle(symbols, "cupbrcap;", HtmlEntityProvider.Convert(10824));
    HtmlEntityProvider.AddSingle(symbols, "cupcap;", HtmlEntityProvider.Convert(10822));
    HtmlEntityProvider.AddSingle(symbols, "cupcup;", HtmlEntityProvider.Convert(10826));
    HtmlEntityProvider.AddSingle(symbols, "cupdot;", HtmlEntityProvider.Convert(8845));
    HtmlEntityProvider.AddSingle(symbols, "cupor;", HtmlEntityProvider.Convert(10821));
    HtmlEntityProvider.AddSingle(symbols, "cups;", HtmlEntityProvider.Convert(8746, 65024));
    HtmlEntityProvider.AddSingle(symbols, "curarr;", HtmlEntityProvider.Convert(8631));
    HtmlEntityProvider.AddSingle(symbols, "curarrm;", HtmlEntityProvider.Convert(10556));
    HtmlEntityProvider.AddSingle(symbols, "curlyeqprec;", HtmlEntityProvider.Convert(8926));
    HtmlEntityProvider.AddSingle(symbols, "curlyeqsucc;", HtmlEntityProvider.Convert(8927));
    HtmlEntityProvider.AddSingle(symbols, "curlyvee;", HtmlEntityProvider.Convert(8910));
    HtmlEntityProvider.AddSingle(symbols, "curlywedge;", HtmlEntityProvider.Convert(8911));
    HtmlEntityProvider.AddBoth(symbols, "curren;", HtmlEntityProvider.Convert(164));
    HtmlEntityProvider.AddSingle(symbols, "curvearrowleft;", HtmlEntityProvider.Convert(8630));
    HtmlEntityProvider.AddSingle(symbols, "curvearrowright;", HtmlEntityProvider.Convert(8631));
    HtmlEntityProvider.AddSingle(symbols, "cuvee;", HtmlEntityProvider.Convert(8910));
    HtmlEntityProvider.AddSingle(symbols, "cuwed;", HtmlEntityProvider.Convert(8911));
    HtmlEntityProvider.AddSingle(symbols, "cwconint;", HtmlEntityProvider.Convert(8754));
    HtmlEntityProvider.AddSingle(symbols, "cwint;", HtmlEntityProvider.Convert(8753));
    HtmlEntityProvider.AddSingle(symbols, "cylcty;", HtmlEntityProvider.Convert(9005));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigC()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Cacute;", HtmlEntityProvider.Convert(262));
    HtmlEntityProvider.AddSingle(symbols, "Cap;", HtmlEntityProvider.Convert(8914));
    HtmlEntityProvider.AddSingle(symbols, "CapitalDifferentialD;", HtmlEntityProvider.Convert(8517));
    HtmlEntityProvider.AddSingle(symbols, "Cayleys;", HtmlEntityProvider.Convert(8493));
    HtmlEntityProvider.AddSingle(symbols, "Ccaron;", HtmlEntityProvider.Convert(268));
    HtmlEntityProvider.AddBoth(symbols, "Ccedil;", HtmlEntityProvider.Convert(199));
    HtmlEntityProvider.AddSingle(symbols, "Ccirc;", HtmlEntityProvider.Convert(264));
    HtmlEntityProvider.AddSingle(symbols, "Cconint;", HtmlEntityProvider.Convert(8752));
    HtmlEntityProvider.AddSingle(symbols, "Cdot;", HtmlEntityProvider.Convert(266));
    HtmlEntityProvider.AddSingle(symbols, "Cedilla;", HtmlEntityProvider.Convert(184));
    HtmlEntityProvider.AddSingle(symbols, "CenterDot;", HtmlEntityProvider.Convert(183));
    HtmlEntityProvider.AddSingle(symbols, "Cfr;", HtmlEntityProvider.Convert(8493));
    HtmlEntityProvider.AddSingle(symbols, "CHcy;", HtmlEntityProvider.Convert(1063));
    HtmlEntityProvider.AddSingle(symbols, "Chi;", HtmlEntityProvider.Convert(935));
    HtmlEntityProvider.AddSingle(symbols, "CircleDot;", HtmlEntityProvider.Convert(8857));
    HtmlEntityProvider.AddSingle(symbols, "CircleMinus;", HtmlEntityProvider.Convert(8854));
    HtmlEntityProvider.AddSingle(symbols, "CirclePlus;", HtmlEntityProvider.Convert(8853));
    HtmlEntityProvider.AddSingle(symbols, "CircleTimes;", HtmlEntityProvider.Convert(8855));
    HtmlEntityProvider.AddSingle(symbols, "ClockwiseContourIntegral;", HtmlEntityProvider.Convert(8754));
    HtmlEntityProvider.AddSingle(symbols, "CloseCurlyDoubleQuote;", HtmlEntityProvider.Convert(8221));
    HtmlEntityProvider.AddSingle(symbols, "CloseCurlyQuote;", HtmlEntityProvider.Convert(8217));
    HtmlEntityProvider.AddSingle(symbols, "Colon;", HtmlEntityProvider.Convert(8759));
    HtmlEntityProvider.AddSingle(symbols, "Colone;", HtmlEntityProvider.Convert(10868));
    HtmlEntityProvider.AddSingle(symbols, "Congruent;", HtmlEntityProvider.Convert(8801));
    HtmlEntityProvider.AddSingle(symbols, "Conint;", HtmlEntityProvider.Convert(8751));
    HtmlEntityProvider.AddSingle(symbols, "ContourIntegral;", HtmlEntityProvider.Convert(8750));
    HtmlEntityProvider.AddSingle(symbols, "Copf;", HtmlEntityProvider.Convert(8450));
    HtmlEntityProvider.AddSingle(symbols, "Coproduct;", HtmlEntityProvider.Convert(8720));
    HtmlEntityProvider.AddBoth(symbols, "COPY;", HtmlEntityProvider.Convert(169));
    HtmlEntityProvider.AddSingle(symbols, "CounterClockwiseContourIntegral;", HtmlEntityProvider.Convert(8755));
    HtmlEntityProvider.AddSingle(symbols, "Cross;", HtmlEntityProvider.Convert(10799));
    HtmlEntityProvider.AddSingle(symbols, "Cscr;", HtmlEntityProvider.Convert(119966));
    HtmlEntityProvider.AddSingle(symbols, "Cup;", HtmlEntityProvider.Convert(8915));
    HtmlEntityProvider.AddSingle(symbols, "CupCap;", HtmlEntityProvider.Convert(8781));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleD()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "dagger;", HtmlEntityProvider.Convert(8224));
    HtmlEntityProvider.AddSingle(symbols, "daleth;", HtmlEntityProvider.Convert(8504));
    HtmlEntityProvider.AddSingle(symbols, "dArr;", HtmlEntityProvider.Convert(8659));
    HtmlEntityProvider.AddSingle(symbols, "darr;", HtmlEntityProvider.Convert(8595));
    HtmlEntityProvider.AddSingle(symbols, "dash;", HtmlEntityProvider.Convert(8208));
    HtmlEntityProvider.AddSingle(symbols, "dashv;", HtmlEntityProvider.Convert(8867));
    HtmlEntityProvider.AddSingle(symbols, "dbkarow;", HtmlEntityProvider.Convert(10511));
    HtmlEntityProvider.AddSingle(symbols, "dblac;", HtmlEntityProvider.Convert(733));
    HtmlEntityProvider.AddSingle(symbols, "dcaron;", HtmlEntityProvider.Convert(271));
    HtmlEntityProvider.AddSingle(symbols, "dcy;", HtmlEntityProvider.Convert(1076));
    HtmlEntityProvider.AddSingle(symbols, "dd;", HtmlEntityProvider.Convert(8518));
    HtmlEntityProvider.AddSingle(symbols, "ddagger;", HtmlEntityProvider.Convert(8225));
    HtmlEntityProvider.AddSingle(symbols, "ddarr;", HtmlEntityProvider.Convert(8650));
    HtmlEntityProvider.AddSingle(symbols, "ddotseq;", HtmlEntityProvider.Convert(10871));
    HtmlEntityProvider.AddBoth(symbols, "deg;", HtmlEntityProvider.Convert(176 /*0xB0*/));
    HtmlEntityProvider.AddSingle(symbols, "delta;", HtmlEntityProvider.Convert(948));
    HtmlEntityProvider.AddSingle(symbols, "demptyv;", HtmlEntityProvider.Convert(10673));
    HtmlEntityProvider.AddSingle(symbols, "dfisht;", HtmlEntityProvider.Convert(10623));
    HtmlEntityProvider.AddSingle(symbols, "dfr;", HtmlEntityProvider.Convert(120097));
    HtmlEntityProvider.AddSingle(symbols, "dHar;", HtmlEntityProvider.Convert(10597));
    HtmlEntityProvider.AddSingle(symbols, "dharl;", HtmlEntityProvider.Convert(8643));
    HtmlEntityProvider.AddSingle(symbols, "dharr;", HtmlEntityProvider.Convert(8642));
    HtmlEntityProvider.AddSingle(symbols, "diam;", HtmlEntityProvider.Convert(8900));
    HtmlEntityProvider.AddSingle(symbols, "diamond;", HtmlEntityProvider.Convert(8900));
    HtmlEntityProvider.AddSingle(symbols, "diamondsuit;", HtmlEntityProvider.Convert(9830));
    HtmlEntityProvider.AddSingle(symbols, "diams;", HtmlEntityProvider.Convert(9830));
    HtmlEntityProvider.AddSingle(symbols, "die;", HtmlEntityProvider.Convert(168));
    HtmlEntityProvider.AddSingle(symbols, "digamma;", HtmlEntityProvider.Convert(989));
    HtmlEntityProvider.AddSingle(symbols, "disin;", HtmlEntityProvider.Convert(8946));
    HtmlEntityProvider.AddSingle(symbols, "div;", HtmlEntityProvider.Convert(247));
    HtmlEntityProvider.AddBoth(symbols, "divide;", HtmlEntityProvider.Convert(247));
    HtmlEntityProvider.AddSingle(symbols, "divideontimes;", HtmlEntityProvider.Convert(8903));
    HtmlEntityProvider.AddSingle(symbols, "divonx;", HtmlEntityProvider.Convert(8903));
    HtmlEntityProvider.AddSingle(symbols, "djcy;", HtmlEntityProvider.Convert(1106));
    HtmlEntityProvider.AddSingle(symbols, "dlcorn;", HtmlEntityProvider.Convert(8990));
    HtmlEntityProvider.AddSingle(symbols, "dlcrop;", HtmlEntityProvider.Convert(8973));
    HtmlEntityProvider.AddSingle(symbols, "dollar;", HtmlEntityProvider.Convert(36));
    HtmlEntityProvider.AddSingle(symbols, "dopf;", HtmlEntityProvider.Convert(120149));
    HtmlEntityProvider.AddSingle(symbols, "dot;", HtmlEntityProvider.Convert(729));
    HtmlEntityProvider.AddSingle(symbols, "doteq;", HtmlEntityProvider.Convert(8784));
    HtmlEntityProvider.AddSingle(symbols, "doteqdot;", HtmlEntityProvider.Convert(8785));
    HtmlEntityProvider.AddSingle(symbols, "dotminus;", HtmlEntityProvider.Convert(8760));
    HtmlEntityProvider.AddSingle(symbols, "dotplus;", HtmlEntityProvider.Convert(8724));
    HtmlEntityProvider.AddSingle(symbols, "dotsquare;", HtmlEntityProvider.Convert(8865));
    HtmlEntityProvider.AddSingle(symbols, "doublebarwedge;", HtmlEntityProvider.Convert(8966));
    HtmlEntityProvider.AddSingle(symbols, "downarrow;", HtmlEntityProvider.Convert(8595));
    HtmlEntityProvider.AddSingle(symbols, "downdownarrows;", HtmlEntityProvider.Convert(8650));
    HtmlEntityProvider.AddSingle(symbols, "downharpoonleft;", HtmlEntityProvider.Convert(8643));
    HtmlEntityProvider.AddSingle(symbols, "downharpoonright;", HtmlEntityProvider.Convert(8642));
    HtmlEntityProvider.AddSingle(symbols, "drbkarow;", HtmlEntityProvider.Convert(10512));
    HtmlEntityProvider.AddSingle(symbols, "drcorn;", HtmlEntityProvider.Convert(8991));
    HtmlEntityProvider.AddSingle(symbols, "drcrop;", HtmlEntityProvider.Convert(8972));
    HtmlEntityProvider.AddSingle(symbols, "dscr;", HtmlEntityProvider.Convert(119993));
    HtmlEntityProvider.AddSingle(symbols, "dscy;", HtmlEntityProvider.Convert(1109));
    HtmlEntityProvider.AddSingle(symbols, "dsol;", HtmlEntityProvider.Convert(10742));
    HtmlEntityProvider.AddSingle(symbols, "dstrok;", HtmlEntityProvider.Convert(273));
    HtmlEntityProvider.AddSingle(symbols, "dtdot;", HtmlEntityProvider.Convert(8945));
    HtmlEntityProvider.AddSingle(symbols, "dtri;", HtmlEntityProvider.Convert(9663));
    HtmlEntityProvider.AddSingle(symbols, "dtrif;", HtmlEntityProvider.Convert(9662));
    HtmlEntityProvider.AddSingle(symbols, "duarr;", HtmlEntityProvider.Convert(8693));
    HtmlEntityProvider.AddSingle(symbols, "duhar;", HtmlEntityProvider.Convert(10607));
    HtmlEntityProvider.AddSingle(symbols, "dwangle;", HtmlEntityProvider.Convert(10662));
    HtmlEntityProvider.AddSingle(symbols, "dzcy;", HtmlEntityProvider.Convert(1119));
    HtmlEntityProvider.AddSingle(symbols, "dzigrarr;", HtmlEntityProvider.Convert(10239));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigD()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Dagger;", HtmlEntityProvider.Convert(8225));
    HtmlEntityProvider.AddSingle(symbols, "Darr;", HtmlEntityProvider.Convert(8609));
    HtmlEntityProvider.AddSingle(symbols, "Dashv;", HtmlEntityProvider.Convert(10980));
    HtmlEntityProvider.AddSingle(symbols, "Dcaron;", HtmlEntityProvider.Convert(270));
    HtmlEntityProvider.AddSingle(symbols, "Dcy;", HtmlEntityProvider.Convert(1044));
    HtmlEntityProvider.AddSingle(symbols, "DD;", HtmlEntityProvider.Convert(8517));
    HtmlEntityProvider.AddSingle(symbols, "DDotrahd;", HtmlEntityProvider.Convert(10513));
    HtmlEntityProvider.AddSingle(symbols, "Del;", HtmlEntityProvider.Convert(8711));
    HtmlEntityProvider.AddSingle(symbols, "Delta;", HtmlEntityProvider.Convert(916));
    HtmlEntityProvider.AddSingle(symbols, "Dfr;", HtmlEntityProvider.Convert(120071));
    HtmlEntityProvider.AddSingle(symbols, "DiacriticalAcute;", HtmlEntityProvider.Convert(180));
    HtmlEntityProvider.AddSingle(symbols, "DiacriticalDot;", HtmlEntityProvider.Convert(729));
    HtmlEntityProvider.AddSingle(symbols, "DiacriticalDoubleAcute;", HtmlEntityProvider.Convert(733));
    HtmlEntityProvider.AddSingle(symbols, "DiacriticalGrave;", HtmlEntityProvider.Convert(96 /*0x60*/));
    HtmlEntityProvider.AddSingle(symbols, "DiacriticalTilde;", HtmlEntityProvider.Convert(732));
    HtmlEntityProvider.AddSingle(symbols, "Diamond;", HtmlEntityProvider.Convert(8900));
    HtmlEntityProvider.AddSingle(symbols, "DifferentialD;", HtmlEntityProvider.Convert(8518));
    HtmlEntityProvider.AddSingle(symbols, "DJcy;", HtmlEntityProvider.Convert(1026));
    HtmlEntityProvider.AddSingle(symbols, "Dopf;", HtmlEntityProvider.Convert(120123));
    HtmlEntityProvider.AddSingle(symbols, "Dot;", HtmlEntityProvider.Convert(168));
    HtmlEntityProvider.AddSingle(symbols, "DotDot;", HtmlEntityProvider.Convert(8412));
    HtmlEntityProvider.AddSingle(symbols, "DotEqual;", HtmlEntityProvider.Convert(8784));
    HtmlEntityProvider.AddSingle(symbols, "DoubleContourIntegral;", HtmlEntityProvider.Convert(8751));
    HtmlEntityProvider.AddSingle(symbols, "DoubleDot;", HtmlEntityProvider.Convert(168));
    HtmlEntityProvider.AddSingle(symbols, "DoubleDownArrow;", HtmlEntityProvider.Convert(8659));
    HtmlEntityProvider.AddSingle(symbols, "DoubleLeftArrow;", HtmlEntityProvider.Convert(8656));
    HtmlEntityProvider.AddSingle(symbols, "DoubleLeftRightArrow;", HtmlEntityProvider.Convert(8660));
    HtmlEntityProvider.AddSingle(symbols, "DoubleLeftTee;", HtmlEntityProvider.Convert(10980));
    HtmlEntityProvider.AddSingle(symbols, "DoubleLongLeftArrow;", HtmlEntityProvider.Convert(10232));
    HtmlEntityProvider.AddSingle(symbols, "DoubleLongLeftRightArrow;", HtmlEntityProvider.Convert(10234));
    HtmlEntityProvider.AddSingle(symbols, "DoubleLongRightArrow;", HtmlEntityProvider.Convert(10233));
    HtmlEntityProvider.AddSingle(symbols, "DoubleRightArrow;", HtmlEntityProvider.Convert(8658));
    HtmlEntityProvider.AddSingle(symbols, "DoubleRightTee;", HtmlEntityProvider.Convert(8872));
    HtmlEntityProvider.AddSingle(symbols, "DoubleUpArrow;", HtmlEntityProvider.Convert(8657));
    HtmlEntityProvider.AddSingle(symbols, "DoubleUpDownArrow;", HtmlEntityProvider.Convert(8661));
    HtmlEntityProvider.AddSingle(symbols, "DoubleVerticalBar;", HtmlEntityProvider.Convert(8741));
    HtmlEntityProvider.AddSingle(symbols, "DownArrow;", HtmlEntityProvider.Convert(8595));
    HtmlEntityProvider.AddSingle(symbols, "Downarrow;", HtmlEntityProvider.Convert(8659));
    HtmlEntityProvider.AddSingle(symbols, "DownArrowBar;", HtmlEntityProvider.Convert(10515));
    HtmlEntityProvider.AddSingle(symbols, "DownArrowUpArrow;", HtmlEntityProvider.Convert(8693));
    HtmlEntityProvider.AddSingle(symbols, "DownBreve;", HtmlEntityProvider.Convert(785));
    HtmlEntityProvider.AddSingle(symbols, "DownLeftRightVector;", HtmlEntityProvider.Convert(10576));
    HtmlEntityProvider.AddSingle(symbols, "DownLeftTeeVector;", HtmlEntityProvider.Convert(10590));
    HtmlEntityProvider.AddSingle(symbols, "DownLeftVector;", HtmlEntityProvider.Convert(8637));
    HtmlEntityProvider.AddSingle(symbols, "DownLeftVectorBar;", HtmlEntityProvider.Convert(10582));
    HtmlEntityProvider.AddSingle(symbols, "DownRightTeeVector;", HtmlEntityProvider.Convert(10591));
    HtmlEntityProvider.AddSingle(symbols, "DownRightVector;", HtmlEntityProvider.Convert(8641));
    HtmlEntityProvider.AddSingle(symbols, "DownRightVectorBar;", HtmlEntityProvider.Convert(10583));
    HtmlEntityProvider.AddSingle(symbols, "DownTee;", HtmlEntityProvider.Convert(8868));
    HtmlEntityProvider.AddSingle(symbols, "DownTeeArrow;", HtmlEntityProvider.Convert(8615));
    HtmlEntityProvider.AddSingle(symbols, "Dscr;", HtmlEntityProvider.Convert(119967));
    HtmlEntityProvider.AddSingle(symbols, "DScy;", HtmlEntityProvider.Convert(1029));
    HtmlEntityProvider.AddSingle(symbols, "Dstrok;", HtmlEntityProvider.Convert(272));
    HtmlEntityProvider.AddSingle(symbols, "DZcy;", HtmlEntityProvider.Convert(1039));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleE()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "eacute;", HtmlEntityProvider.Convert(233));
    HtmlEntityProvider.AddSingle(symbols, "easter;", HtmlEntityProvider.Convert(10862));
    HtmlEntityProvider.AddSingle(symbols, "ecaron;", HtmlEntityProvider.Convert(283));
    HtmlEntityProvider.AddSingle(symbols, "ecir;", HtmlEntityProvider.Convert(8790));
    HtmlEntityProvider.AddBoth(symbols, "ecirc;", HtmlEntityProvider.Convert(234));
    HtmlEntityProvider.AddSingle(symbols, "ecolon;", HtmlEntityProvider.Convert(8789));
    HtmlEntityProvider.AddSingle(symbols, "ecy;", HtmlEntityProvider.Convert(1101));
    HtmlEntityProvider.AddSingle(symbols, "eDDot;", HtmlEntityProvider.Convert(10871));
    HtmlEntityProvider.AddSingle(symbols, "eDot;", HtmlEntityProvider.Convert(8785));
    HtmlEntityProvider.AddSingle(symbols, "edot;", HtmlEntityProvider.Convert(279));
    HtmlEntityProvider.AddSingle(symbols, "ee;", HtmlEntityProvider.Convert(8519));
    HtmlEntityProvider.AddSingle(symbols, "efDot;", HtmlEntityProvider.Convert(8786));
    HtmlEntityProvider.AddSingle(symbols, "efr;", HtmlEntityProvider.Convert(120098));
    HtmlEntityProvider.AddSingle(symbols, "eg;", HtmlEntityProvider.Convert(10906));
    HtmlEntityProvider.AddBoth(symbols, "egrave;", HtmlEntityProvider.Convert(232));
    HtmlEntityProvider.AddSingle(symbols, "egs;", HtmlEntityProvider.Convert(10902));
    HtmlEntityProvider.AddSingle(symbols, "egsdot;", HtmlEntityProvider.Convert(10904));
    HtmlEntityProvider.AddSingle(symbols, "el;", HtmlEntityProvider.Convert(10905));
    HtmlEntityProvider.AddSingle(symbols, "elinters;", HtmlEntityProvider.Convert(9191));
    HtmlEntityProvider.AddSingle(symbols, "ell;", HtmlEntityProvider.Convert(8467));
    HtmlEntityProvider.AddSingle(symbols, "els;", HtmlEntityProvider.Convert(10901));
    HtmlEntityProvider.AddSingle(symbols, "elsdot;", HtmlEntityProvider.Convert(10903));
    HtmlEntityProvider.AddSingle(symbols, "emacr;", HtmlEntityProvider.Convert(275));
    HtmlEntityProvider.AddSingle(symbols, "empty;", HtmlEntityProvider.Convert(8709));
    HtmlEntityProvider.AddSingle(symbols, "emptyset;", HtmlEntityProvider.Convert(8709));
    HtmlEntityProvider.AddSingle(symbols, "emptyv;", HtmlEntityProvider.Convert(8709));
    HtmlEntityProvider.AddSingle(symbols, "emsp;", HtmlEntityProvider.Convert(8195));
    HtmlEntityProvider.AddSingle(symbols, "emsp13;", HtmlEntityProvider.Convert(8196));
    HtmlEntityProvider.AddSingle(symbols, "emsp14;", HtmlEntityProvider.Convert(8197));
    HtmlEntityProvider.AddSingle(symbols, "eng;", HtmlEntityProvider.Convert(331));
    HtmlEntityProvider.AddSingle(symbols, "ensp;", HtmlEntityProvider.Convert(8194));
    HtmlEntityProvider.AddSingle(symbols, "eogon;", HtmlEntityProvider.Convert(281));
    HtmlEntityProvider.AddSingle(symbols, "eopf;", HtmlEntityProvider.Convert(120150));
    HtmlEntityProvider.AddSingle(symbols, "epar;", HtmlEntityProvider.Convert(8917));
    HtmlEntityProvider.AddSingle(symbols, "eparsl;", HtmlEntityProvider.Convert(10723));
    HtmlEntityProvider.AddSingle(symbols, "eplus;", HtmlEntityProvider.Convert(10865));
    HtmlEntityProvider.AddSingle(symbols, "epsi;", HtmlEntityProvider.Convert(949));
    HtmlEntityProvider.AddSingle(symbols, "epsilon;", HtmlEntityProvider.Convert(949));
    HtmlEntityProvider.AddSingle(symbols, "epsiv;", HtmlEntityProvider.Convert(1013));
    HtmlEntityProvider.AddSingle(symbols, "eqcirc;", HtmlEntityProvider.Convert(8790));
    HtmlEntityProvider.AddSingle(symbols, "eqcolon;", HtmlEntityProvider.Convert(8789));
    HtmlEntityProvider.AddSingle(symbols, "eqsim;", HtmlEntityProvider.Convert(8770));
    HtmlEntityProvider.AddSingle(symbols, "eqslantgtr;", HtmlEntityProvider.Convert(10902));
    HtmlEntityProvider.AddSingle(symbols, "eqslantless;", HtmlEntityProvider.Convert(10901));
    HtmlEntityProvider.AddSingle(symbols, "equals;", HtmlEntityProvider.Convert(61));
    HtmlEntityProvider.AddSingle(symbols, "equest;", HtmlEntityProvider.Convert(8799));
    HtmlEntityProvider.AddSingle(symbols, "equiv;", HtmlEntityProvider.Convert(8801));
    HtmlEntityProvider.AddSingle(symbols, "equivDD;", HtmlEntityProvider.Convert(10872));
    HtmlEntityProvider.AddSingle(symbols, "eqvparsl;", HtmlEntityProvider.Convert(10725));
    HtmlEntityProvider.AddSingle(symbols, "erarr;", HtmlEntityProvider.Convert(10609));
    HtmlEntityProvider.AddSingle(symbols, "erDot;", HtmlEntityProvider.Convert(8787));
    HtmlEntityProvider.AddSingle(symbols, "escr;", HtmlEntityProvider.Convert(8495));
    HtmlEntityProvider.AddSingle(symbols, "esdot;", HtmlEntityProvider.Convert(8784));
    HtmlEntityProvider.AddSingle(symbols, "esim;", HtmlEntityProvider.Convert(8770));
    HtmlEntityProvider.AddSingle(symbols, "eta;", HtmlEntityProvider.Convert(951));
    HtmlEntityProvider.AddBoth(symbols, "eth;", HtmlEntityProvider.Convert(240 /*0xF0*/));
    HtmlEntityProvider.AddBoth(symbols, "euml;", HtmlEntityProvider.Convert(235));
    HtmlEntityProvider.AddSingle(symbols, "euro;", HtmlEntityProvider.Convert(8364));
    HtmlEntityProvider.AddSingle(symbols, "excl;", HtmlEntityProvider.Convert(33));
    HtmlEntityProvider.AddSingle(symbols, "exist;", HtmlEntityProvider.Convert(8707));
    HtmlEntityProvider.AddSingle(symbols, "expectation;", HtmlEntityProvider.Convert(8496));
    HtmlEntityProvider.AddSingle(symbols, "exponentiale;", HtmlEntityProvider.Convert(8519));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigE()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "Eacute;", HtmlEntityProvider.Convert(201));
    HtmlEntityProvider.AddSingle(symbols, "Ecaron;", HtmlEntityProvider.Convert(282));
    HtmlEntityProvider.AddBoth(symbols, "Ecirc;", HtmlEntityProvider.Convert(202));
    HtmlEntityProvider.AddSingle(symbols, "Ecy;", HtmlEntityProvider.Convert(1069));
    HtmlEntityProvider.AddSingle(symbols, "Edot;", HtmlEntityProvider.Convert(278));
    HtmlEntityProvider.AddSingle(symbols, "Efr;", HtmlEntityProvider.Convert(120072));
    HtmlEntityProvider.AddBoth(symbols, "Egrave;", HtmlEntityProvider.Convert(200));
    HtmlEntityProvider.AddSingle(symbols, "Element;", HtmlEntityProvider.Convert(8712));
    HtmlEntityProvider.AddSingle(symbols, "Emacr;", HtmlEntityProvider.Convert(274));
    HtmlEntityProvider.AddSingle(symbols, "EmptySmallSquare;", HtmlEntityProvider.Convert(9723));
    HtmlEntityProvider.AddSingle(symbols, "EmptyVerySmallSquare;", HtmlEntityProvider.Convert(9643));
    HtmlEntityProvider.AddSingle(symbols, "ENG;", HtmlEntityProvider.Convert(330));
    HtmlEntityProvider.AddSingle(symbols, "Eogon;", HtmlEntityProvider.Convert(280));
    HtmlEntityProvider.AddSingle(symbols, "Eopf;", HtmlEntityProvider.Convert(120124));
    HtmlEntityProvider.AddSingle(symbols, "Epsilon;", HtmlEntityProvider.Convert(917));
    HtmlEntityProvider.AddSingle(symbols, "Equal;", HtmlEntityProvider.Convert(10869));
    HtmlEntityProvider.AddSingle(symbols, "EqualTilde;", HtmlEntityProvider.Convert(8770));
    HtmlEntityProvider.AddSingle(symbols, "Equilibrium;", HtmlEntityProvider.Convert(8652));
    HtmlEntityProvider.AddSingle(symbols, "Escr;", HtmlEntityProvider.Convert(8496));
    HtmlEntityProvider.AddSingle(symbols, "Esim;", HtmlEntityProvider.Convert(10867));
    HtmlEntityProvider.AddSingle(symbols, "Eta;", HtmlEntityProvider.Convert(919));
    HtmlEntityProvider.AddBoth(symbols, "ETH;", HtmlEntityProvider.Convert(208 /*0xD0*/));
    HtmlEntityProvider.AddBoth(symbols, "Euml;", HtmlEntityProvider.Convert(203));
    HtmlEntityProvider.AddSingle(symbols, "Exists;", HtmlEntityProvider.Convert(8707));
    HtmlEntityProvider.AddSingle(symbols, "ExponentialE;", HtmlEntityProvider.Convert(8519));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleF()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "fallingdotseq;", HtmlEntityProvider.Convert(8786));
    HtmlEntityProvider.AddSingle(symbols, "fcy;", HtmlEntityProvider.Convert(1092));
    HtmlEntityProvider.AddSingle(symbols, "female;", HtmlEntityProvider.Convert(9792));
    HtmlEntityProvider.AddSingle(symbols, "ffilig;", HtmlEntityProvider.Convert(64259));
    HtmlEntityProvider.AddSingle(symbols, "fflig;", HtmlEntityProvider.Convert(64256));
    HtmlEntityProvider.AddSingle(symbols, "ffllig;", HtmlEntityProvider.Convert(64260));
    HtmlEntityProvider.AddSingle(symbols, "ffr;", HtmlEntityProvider.Convert(120099));
    HtmlEntityProvider.AddSingle(symbols, "filig;", HtmlEntityProvider.Convert(64257));
    HtmlEntityProvider.AddSingle(symbols, "fjlig;", HtmlEntityProvider.Convert(102, 106));
    HtmlEntityProvider.AddSingle(symbols, "flat;", HtmlEntityProvider.Convert(9837));
    HtmlEntityProvider.AddSingle(symbols, "fllig;", HtmlEntityProvider.Convert(64258));
    HtmlEntityProvider.AddSingle(symbols, "fltns;", HtmlEntityProvider.Convert(9649));
    HtmlEntityProvider.AddSingle(symbols, "fnof;", HtmlEntityProvider.Convert(402));
    HtmlEntityProvider.AddSingle(symbols, "fopf;", HtmlEntityProvider.Convert(120151));
    HtmlEntityProvider.AddSingle(symbols, "forall;", HtmlEntityProvider.Convert(8704));
    HtmlEntityProvider.AddSingle(symbols, "fork;", HtmlEntityProvider.Convert(8916));
    HtmlEntityProvider.AddSingle(symbols, "forkv;", HtmlEntityProvider.Convert(10969));
    HtmlEntityProvider.AddSingle(symbols, "fpartint;", HtmlEntityProvider.Convert(10765));
    HtmlEntityProvider.AddBoth(symbols, "frac12;", HtmlEntityProvider.Convert(189));
    HtmlEntityProvider.AddSingle(symbols, "frac13;", HtmlEntityProvider.Convert(8531));
    HtmlEntityProvider.AddBoth(symbols, "frac14;", HtmlEntityProvider.Convert(188));
    HtmlEntityProvider.AddSingle(symbols, "frac15;", HtmlEntityProvider.Convert(8533));
    HtmlEntityProvider.AddSingle(symbols, "frac16;", HtmlEntityProvider.Convert(8537));
    HtmlEntityProvider.AddSingle(symbols, "frac18;", HtmlEntityProvider.Convert(8539));
    HtmlEntityProvider.AddSingle(symbols, "frac23;", HtmlEntityProvider.Convert(8532));
    HtmlEntityProvider.AddSingle(symbols, "frac25;", HtmlEntityProvider.Convert(8534));
    HtmlEntityProvider.AddBoth(symbols, "frac34;", HtmlEntityProvider.Convert(190));
    HtmlEntityProvider.AddSingle(symbols, "frac35;", HtmlEntityProvider.Convert(8535));
    HtmlEntityProvider.AddSingle(symbols, "frac38;", HtmlEntityProvider.Convert(8540));
    HtmlEntityProvider.AddSingle(symbols, "frac45;", HtmlEntityProvider.Convert(8536));
    HtmlEntityProvider.AddSingle(symbols, "frac56;", HtmlEntityProvider.Convert(8538));
    HtmlEntityProvider.AddSingle(symbols, "frac58;", HtmlEntityProvider.Convert(8541));
    HtmlEntityProvider.AddSingle(symbols, "frac78;", HtmlEntityProvider.Convert(8542));
    HtmlEntityProvider.AddSingle(symbols, "frasl;", HtmlEntityProvider.Convert(8260));
    HtmlEntityProvider.AddSingle(symbols, "frown;", HtmlEntityProvider.Convert(8994));
    HtmlEntityProvider.AddSingle(symbols, "fscr;", HtmlEntityProvider.Convert(119995));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigF()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Fcy;", HtmlEntityProvider.Convert(1060));
    HtmlEntityProvider.AddSingle(symbols, "Ffr;", HtmlEntityProvider.Convert(120073));
    HtmlEntityProvider.AddSingle(symbols, "FilledSmallSquare;", HtmlEntityProvider.Convert(9724));
    HtmlEntityProvider.AddSingle(symbols, "FilledVerySmallSquare;", HtmlEntityProvider.Convert(9642));
    HtmlEntityProvider.AddSingle(symbols, "Fopf;", HtmlEntityProvider.Convert(120125));
    HtmlEntityProvider.AddSingle(symbols, "ForAll;", HtmlEntityProvider.Convert(8704));
    HtmlEntityProvider.AddSingle(symbols, "Fouriertrf;", HtmlEntityProvider.Convert(8497));
    HtmlEntityProvider.AddSingle(symbols, "Fscr;", HtmlEntityProvider.Convert(8497));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleG()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "gacute;", HtmlEntityProvider.Convert(501));
    HtmlEntityProvider.AddSingle(symbols, "gamma;", HtmlEntityProvider.Convert(947));
    HtmlEntityProvider.AddSingle(symbols, "gammad;", HtmlEntityProvider.Convert(989));
    HtmlEntityProvider.AddSingle(symbols, "gap;", HtmlEntityProvider.Convert(10886));
    HtmlEntityProvider.AddSingle(symbols, "gbreve;", HtmlEntityProvider.Convert(287));
    HtmlEntityProvider.AddSingle(symbols, "gcirc;", HtmlEntityProvider.Convert(285));
    HtmlEntityProvider.AddSingle(symbols, "gcy;", HtmlEntityProvider.Convert(1075));
    HtmlEntityProvider.AddSingle(symbols, "gdot;", HtmlEntityProvider.Convert(289));
    HtmlEntityProvider.AddSingle(symbols, "gE;", HtmlEntityProvider.Convert(8807));
    HtmlEntityProvider.AddSingle(symbols, "ge;", HtmlEntityProvider.Convert(8805));
    HtmlEntityProvider.AddSingle(symbols, "gEl;", HtmlEntityProvider.Convert(10892));
    HtmlEntityProvider.AddSingle(symbols, "gel;", HtmlEntityProvider.Convert(8923));
    HtmlEntityProvider.AddSingle(symbols, "geq;", HtmlEntityProvider.Convert(8805));
    HtmlEntityProvider.AddSingle(symbols, "geqq;", HtmlEntityProvider.Convert(8807));
    HtmlEntityProvider.AddSingle(symbols, "geqslant;", HtmlEntityProvider.Convert(10878));
    HtmlEntityProvider.AddSingle(symbols, "ges;", HtmlEntityProvider.Convert(10878));
    HtmlEntityProvider.AddSingle(symbols, "gescc;", HtmlEntityProvider.Convert(10921));
    HtmlEntityProvider.AddSingle(symbols, "gesdot;", HtmlEntityProvider.Convert(10880));
    HtmlEntityProvider.AddSingle(symbols, "gesdoto;", HtmlEntityProvider.Convert(10882));
    HtmlEntityProvider.AddSingle(symbols, "gesdotol;", HtmlEntityProvider.Convert(10884));
    HtmlEntityProvider.AddSingle(symbols, "gesl;", HtmlEntityProvider.Convert(8923, 65024));
    HtmlEntityProvider.AddSingle(symbols, "gesles;", HtmlEntityProvider.Convert(10900));
    HtmlEntityProvider.AddSingle(symbols, "gfr;", HtmlEntityProvider.Convert(120100));
    HtmlEntityProvider.AddSingle(symbols, "gg;", HtmlEntityProvider.Convert(8811));
    HtmlEntityProvider.AddSingle(symbols, "ggg;", HtmlEntityProvider.Convert(8921));
    HtmlEntityProvider.AddSingle(symbols, "gimel;", HtmlEntityProvider.Convert(8503));
    HtmlEntityProvider.AddSingle(symbols, "gjcy;", HtmlEntityProvider.Convert(1107));
    HtmlEntityProvider.AddSingle(symbols, "gl;", HtmlEntityProvider.Convert(8823));
    HtmlEntityProvider.AddSingle(symbols, "gla;", HtmlEntityProvider.Convert(10917));
    HtmlEntityProvider.AddSingle(symbols, "glE;", HtmlEntityProvider.Convert(10898));
    HtmlEntityProvider.AddSingle(symbols, "glj;", HtmlEntityProvider.Convert(10916));
    HtmlEntityProvider.AddSingle(symbols, "gnap;", HtmlEntityProvider.Convert(10890));
    HtmlEntityProvider.AddSingle(symbols, "gnapprox;", HtmlEntityProvider.Convert(10890));
    HtmlEntityProvider.AddSingle(symbols, "gnE;", HtmlEntityProvider.Convert(8809));
    HtmlEntityProvider.AddSingle(symbols, "gne;", HtmlEntityProvider.Convert(10888));
    HtmlEntityProvider.AddSingle(symbols, "gneq;", HtmlEntityProvider.Convert(10888));
    HtmlEntityProvider.AddSingle(symbols, "gneqq;", HtmlEntityProvider.Convert(8809));
    HtmlEntityProvider.AddSingle(symbols, "gnsim;", HtmlEntityProvider.Convert(8935));
    HtmlEntityProvider.AddSingle(symbols, "gopf;", HtmlEntityProvider.Convert(120152));
    HtmlEntityProvider.AddSingle(symbols, "grave;", HtmlEntityProvider.Convert(96 /*0x60*/));
    HtmlEntityProvider.AddSingle(symbols, "gscr;", HtmlEntityProvider.Convert(8458));
    HtmlEntityProvider.AddSingle(symbols, "gsim;", HtmlEntityProvider.Convert(8819));
    HtmlEntityProvider.AddSingle(symbols, "gsime;", HtmlEntityProvider.Convert(10894));
    HtmlEntityProvider.AddSingle(symbols, "gsiml;", HtmlEntityProvider.Convert(10896));
    HtmlEntityProvider.AddBoth(symbols, "gt;", HtmlEntityProvider.Convert(62));
    HtmlEntityProvider.AddSingle(symbols, "gtcc;", HtmlEntityProvider.Convert(10919));
    HtmlEntityProvider.AddSingle(symbols, "gtcir;", HtmlEntityProvider.Convert(10874));
    HtmlEntityProvider.AddSingle(symbols, "gtdot;", HtmlEntityProvider.Convert(8919));
    HtmlEntityProvider.AddSingle(symbols, "gtlPar;", HtmlEntityProvider.Convert(10645));
    HtmlEntityProvider.AddSingle(symbols, "gtquest;", HtmlEntityProvider.Convert(10876));
    HtmlEntityProvider.AddSingle(symbols, "gtrapprox;", HtmlEntityProvider.Convert(10886));
    HtmlEntityProvider.AddSingle(symbols, "gtrarr;", HtmlEntityProvider.Convert(10616));
    HtmlEntityProvider.AddSingle(symbols, "gtrdot;", HtmlEntityProvider.Convert(8919));
    HtmlEntityProvider.AddSingle(symbols, "gtreqless;", HtmlEntityProvider.Convert(8923));
    HtmlEntityProvider.AddSingle(symbols, "gtreqqless;", HtmlEntityProvider.Convert(10892));
    HtmlEntityProvider.AddSingle(symbols, "gtrless;", HtmlEntityProvider.Convert(8823));
    HtmlEntityProvider.AddSingle(symbols, "gtrsim;", HtmlEntityProvider.Convert(8819));
    HtmlEntityProvider.AddSingle(symbols, "gvertneqq;", HtmlEntityProvider.Convert(8809, 65024));
    HtmlEntityProvider.AddSingle(symbols, "gvnE;", HtmlEntityProvider.Convert(8809, 65024));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigG()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Gamma;", HtmlEntityProvider.Convert(915));
    HtmlEntityProvider.AddSingle(symbols, "Gammad;", HtmlEntityProvider.Convert(988));
    HtmlEntityProvider.AddSingle(symbols, "Gbreve;", HtmlEntityProvider.Convert(286));
    HtmlEntityProvider.AddSingle(symbols, "Gcedil;", HtmlEntityProvider.Convert(290));
    HtmlEntityProvider.AddSingle(symbols, "Gcirc;", HtmlEntityProvider.Convert(284));
    HtmlEntityProvider.AddSingle(symbols, "Gcy;", HtmlEntityProvider.Convert(1043));
    HtmlEntityProvider.AddSingle(symbols, "Gdot;", HtmlEntityProvider.Convert(288));
    HtmlEntityProvider.AddSingle(symbols, "Gfr;", HtmlEntityProvider.Convert(120074));
    HtmlEntityProvider.AddSingle(symbols, "Gg;", HtmlEntityProvider.Convert(8921));
    HtmlEntityProvider.AddSingle(symbols, "GJcy;", HtmlEntityProvider.Convert(1027));
    HtmlEntityProvider.AddSingle(symbols, "Gopf;", HtmlEntityProvider.Convert(120126));
    HtmlEntityProvider.AddSingle(symbols, "GreaterEqual;", HtmlEntityProvider.Convert(8805));
    HtmlEntityProvider.AddSingle(symbols, "GreaterEqualLess;", HtmlEntityProvider.Convert(8923));
    HtmlEntityProvider.AddSingle(symbols, "GreaterFullEqual;", HtmlEntityProvider.Convert(8807));
    HtmlEntityProvider.AddSingle(symbols, "GreaterGreater;", HtmlEntityProvider.Convert(10914));
    HtmlEntityProvider.AddSingle(symbols, "GreaterLess;", HtmlEntityProvider.Convert(8823));
    HtmlEntityProvider.AddSingle(symbols, "GreaterSlantEqual;", HtmlEntityProvider.Convert(10878));
    HtmlEntityProvider.AddSingle(symbols, "GreaterTilde;", HtmlEntityProvider.Convert(8819));
    HtmlEntityProvider.AddSingle(symbols, "Gscr;", HtmlEntityProvider.Convert(119970));
    HtmlEntityProvider.AddBoth(symbols, "GT;", HtmlEntityProvider.Convert(62));
    HtmlEntityProvider.AddSingle(symbols, "Gt;", HtmlEntityProvider.Convert(8811));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleH()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "hairsp;", HtmlEntityProvider.Convert(8202));
    HtmlEntityProvider.AddSingle(symbols, "half;", HtmlEntityProvider.Convert(189));
    HtmlEntityProvider.AddSingle(symbols, "hamilt;", HtmlEntityProvider.Convert(8459));
    HtmlEntityProvider.AddSingle(symbols, "hardcy;", HtmlEntityProvider.Convert(1098));
    HtmlEntityProvider.AddSingle(symbols, "hArr;", HtmlEntityProvider.Convert(8660));
    HtmlEntityProvider.AddSingle(symbols, "harr;", HtmlEntityProvider.Convert(8596));
    HtmlEntityProvider.AddSingle(symbols, "harrcir;", HtmlEntityProvider.Convert(10568));
    HtmlEntityProvider.AddSingle(symbols, "harrw;", HtmlEntityProvider.Convert(8621));
    HtmlEntityProvider.AddSingle(symbols, "hbar;", HtmlEntityProvider.Convert(8463));
    HtmlEntityProvider.AddSingle(symbols, "hcirc;", HtmlEntityProvider.Convert(293));
    HtmlEntityProvider.AddSingle(symbols, "hearts;", HtmlEntityProvider.Convert(9829));
    HtmlEntityProvider.AddSingle(symbols, "heartsuit;", HtmlEntityProvider.Convert(9829));
    HtmlEntityProvider.AddSingle(symbols, "hellip;", HtmlEntityProvider.Convert(8230));
    HtmlEntityProvider.AddSingle(symbols, "hercon;", HtmlEntityProvider.Convert(8889));
    HtmlEntityProvider.AddSingle(symbols, "hfr;", HtmlEntityProvider.Convert(120101));
    HtmlEntityProvider.AddSingle(symbols, "hksearow;", HtmlEntityProvider.Convert(10533));
    HtmlEntityProvider.AddSingle(symbols, "hkswarow;", HtmlEntityProvider.Convert(10534));
    HtmlEntityProvider.AddSingle(symbols, "hoarr;", HtmlEntityProvider.Convert(8703));
    HtmlEntityProvider.AddSingle(symbols, "homtht;", HtmlEntityProvider.Convert(8763));
    HtmlEntityProvider.AddSingle(symbols, "hookleftarrow;", HtmlEntityProvider.Convert(8617));
    HtmlEntityProvider.AddSingle(symbols, "hookrightarrow;", HtmlEntityProvider.Convert(8618));
    HtmlEntityProvider.AddSingle(symbols, "hopf;", HtmlEntityProvider.Convert(120153));
    HtmlEntityProvider.AddSingle(symbols, "horbar;", HtmlEntityProvider.Convert(8213));
    HtmlEntityProvider.AddSingle(symbols, "hscr;", HtmlEntityProvider.Convert(119997));
    HtmlEntityProvider.AddSingle(symbols, "hslash;", HtmlEntityProvider.Convert(8463));
    HtmlEntityProvider.AddSingle(symbols, "hstrok;", HtmlEntityProvider.Convert(295));
    HtmlEntityProvider.AddSingle(symbols, "hybull;", HtmlEntityProvider.Convert(8259));
    HtmlEntityProvider.AddSingle(symbols, "hyphen;", HtmlEntityProvider.Convert(8208));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigH()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Hacek;", HtmlEntityProvider.Convert(711));
    HtmlEntityProvider.AddSingle(symbols, "HARDcy;", HtmlEntityProvider.Convert(1066));
    HtmlEntityProvider.AddSingle(symbols, "Hat;", HtmlEntityProvider.Convert(94));
    HtmlEntityProvider.AddSingle(symbols, "Hcirc;", HtmlEntityProvider.Convert(292));
    HtmlEntityProvider.AddSingle(symbols, "Hfr;", HtmlEntityProvider.Convert(8460));
    HtmlEntityProvider.AddSingle(symbols, "HilbertSpace;", HtmlEntityProvider.Convert(8459));
    HtmlEntityProvider.AddSingle(symbols, "Hopf;", HtmlEntityProvider.Convert(8461));
    HtmlEntityProvider.AddSingle(symbols, "HorizontalLine;", HtmlEntityProvider.Convert(9472));
    HtmlEntityProvider.AddSingle(symbols, "Hscr;", HtmlEntityProvider.Convert(8459));
    HtmlEntityProvider.AddSingle(symbols, "Hstrok;", HtmlEntityProvider.Convert(294));
    HtmlEntityProvider.AddSingle(symbols, "HumpDownHump;", HtmlEntityProvider.Convert(8782));
    HtmlEntityProvider.AddSingle(symbols, "HumpEqual;", HtmlEntityProvider.Convert(8783));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleI()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "iacute;", HtmlEntityProvider.Convert(237));
    HtmlEntityProvider.AddSingle(symbols, "ic;", HtmlEntityProvider.Convert(8291));
    HtmlEntityProvider.AddBoth(symbols, "icirc;", HtmlEntityProvider.Convert(238));
    HtmlEntityProvider.AddSingle(symbols, "icy;", HtmlEntityProvider.Convert(1080));
    HtmlEntityProvider.AddSingle(symbols, "iecy;", HtmlEntityProvider.Convert(1077));
    HtmlEntityProvider.AddBoth(symbols, "iexcl;", HtmlEntityProvider.Convert(161));
    HtmlEntityProvider.AddSingle(symbols, "iff;", HtmlEntityProvider.Convert(8660));
    HtmlEntityProvider.AddSingle(symbols, "ifr;", HtmlEntityProvider.Convert(120102));
    HtmlEntityProvider.AddBoth(symbols, "igrave;", HtmlEntityProvider.Convert(236));
    HtmlEntityProvider.AddSingle(symbols, "ii;", HtmlEntityProvider.Convert(8520));
    HtmlEntityProvider.AddSingle(symbols, "iiiint;", HtmlEntityProvider.Convert(10764));
    HtmlEntityProvider.AddSingle(symbols, "iiint;", HtmlEntityProvider.Convert(8749));
    HtmlEntityProvider.AddSingle(symbols, "iinfin;", HtmlEntityProvider.Convert(10716));
    HtmlEntityProvider.AddSingle(symbols, "iiota;", HtmlEntityProvider.Convert(8489));
    HtmlEntityProvider.AddSingle(symbols, "ijlig;", HtmlEntityProvider.Convert(307));
    HtmlEntityProvider.AddSingle(symbols, "imacr;", HtmlEntityProvider.Convert(299));
    HtmlEntityProvider.AddSingle(symbols, "image;", HtmlEntityProvider.Convert(8465));
    HtmlEntityProvider.AddSingle(symbols, "imagline;", HtmlEntityProvider.Convert(8464));
    HtmlEntityProvider.AddSingle(symbols, "imagpart;", HtmlEntityProvider.Convert(8465));
    HtmlEntityProvider.AddSingle(symbols, "imath;", HtmlEntityProvider.Convert(305));
    HtmlEntityProvider.AddSingle(symbols, "imof;", HtmlEntityProvider.Convert(8887));
    HtmlEntityProvider.AddSingle(symbols, "imped;", HtmlEntityProvider.Convert(437));
    HtmlEntityProvider.AddSingle(symbols, "in;", HtmlEntityProvider.Convert(8712));
    HtmlEntityProvider.AddSingle(symbols, "incare;", HtmlEntityProvider.Convert(8453));
    HtmlEntityProvider.AddSingle(symbols, "infin;", HtmlEntityProvider.Convert(8734));
    HtmlEntityProvider.AddSingle(symbols, "infintie;", HtmlEntityProvider.Convert(10717));
    HtmlEntityProvider.AddSingle(symbols, "inodot;", HtmlEntityProvider.Convert(305));
    HtmlEntityProvider.AddSingle(symbols, "int;", HtmlEntityProvider.Convert(8747));
    HtmlEntityProvider.AddSingle(symbols, "intcal;", HtmlEntityProvider.Convert(8890));
    HtmlEntityProvider.AddSingle(symbols, "integers;", HtmlEntityProvider.Convert(8484));
    HtmlEntityProvider.AddSingle(symbols, "intercal;", HtmlEntityProvider.Convert(8890));
    HtmlEntityProvider.AddSingle(symbols, "intlarhk;", HtmlEntityProvider.Convert(10775));
    HtmlEntityProvider.AddSingle(symbols, "intprod;", HtmlEntityProvider.Convert(10812));
    HtmlEntityProvider.AddSingle(symbols, "iocy;", HtmlEntityProvider.Convert(1105));
    HtmlEntityProvider.AddSingle(symbols, "iogon;", HtmlEntityProvider.Convert(303));
    HtmlEntityProvider.AddSingle(symbols, "iopf;", HtmlEntityProvider.Convert(120154));
    HtmlEntityProvider.AddSingle(symbols, "iota;", HtmlEntityProvider.Convert(953));
    HtmlEntityProvider.AddSingle(symbols, "iprod;", HtmlEntityProvider.Convert(10812));
    HtmlEntityProvider.AddBoth(symbols, "iquest;", HtmlEntityProvider.Convert(191));
    HtmlEntityProvider.AddSingle(symbols, "iscr;", HtmlEntityProvider.Convert(119998));
    HtmlEntityProvider.AddSingle(symbols, "isin;", HtmlEntityProvider.Convert(8712));
    HtmlEntityProvider.AddSingle(symbols, "isindot;", HtmlEntityProvider.Convert(8949));
    HtmlEntityProvider.AddSingle(symbols, "isinE;", HtmlEntityProvider.Convert(8953));
    HtmlEntityProvider.AddSingle(symbols, "isins;", HtmlEntityProvider.Convert(8948));
    HtmlEntityProvider.AddSingle(symbols, "isinsv;", HtmlEntityProvider.Convert(8947));
    HtmlEntityProvider.AddSingle(symbols, "isinv;", HtmlEntityProvider.Convert(8712));
    HtmlEntityProvider.AddSingle(symbols, "it;", HtmlEntityProvider.Convert(8290));
    HtmlEntityProvider.AddSingle(symbols, "itilde;", HtmlEntityProvider.Convert(297));
    HtmlEntityProvider.AddSingle(symbols, "iukcy;", HtmlEntityProvider.Convert(1110));
    HtmlEntityProvider.AddBoth(symbols, "iuml;", HtmlEntityProvider.Convert(239));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigI()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "Iacute;", HtmlEntityProvider.Convert(205));
    HtmlEntityProvider.AddBoth(symbols, "Icirc;", HtmlEntityProvider.Convert(206));
    HtmlEntityProvider.AddSingle(symbols, "Icy;", HtmlEntityProvider.Convert(1048));
    HtmlEntityProvider.AddSingle(symbols, "Idot;", HtmlEntityProvider.Convert(304));
    HtmlEntityProvider.AddSingle(symbols, "IEcy;", HtmlEntityProvider.Convert(1045));
    HtmlEntityProvider.AddSingle(symbols, "Ifr;", HtmlEntityProvider.Convert(8465));
    HtmlEntityProvider.AddBoth(symbols, "Igrave;", HtmlEntityProvider.Convert(204));
    HtmlEntityProvider.AddSingle(symbols, "IJlig;", HtmlEntityProvider.Convert(306));
    HtmlEntityProvider.AddSingle(symbols, "Im;", HtmlEntityProvider.Convert(8465));
    HtmlEntityProvider.AddSingle(symbols, "Imacr;", HtmlEntityProvider.Convert(298));
    HtmlEntityProvider.AddSingle(symbols, "ImaginaryI;", HtmlEntityProvider.Convert(8520));
    HtmlEntityProvider.AddSingle(symbols, "Implies;", HtmlEntityProvider.Convert(8658));
    HtmlEntityProvider.AddSingle(symbols, "Int;", HtmlEntityProvider.Convert(8748));
    HtmlEntityProvider.AddSingle(symbols, "Integral;", HtmlEntityProvider.Convert(8747));
    HtmlEntityProvider.AddSingle(symbols, "Intersection;", HtmlEntityProvider.Convert(8898));
    HtmlEntityProvider.AddSingle(symbols, "InvisibleComma;", HtmlEntityProvider.Convert(8291));
    HtmlEntityProvider.AddSingle(symbols, "InvisibleTimes;", HtmlEntityProvider.Convert(8290));
    HtmlEntityProvider.AddSingle(symbols, "IOcy;", HtmlEntityProvider.Convert(1025));
    HtmlEntityProvider.AddSingle(symbols, "Iogon;", HtmlEntityProvider.Convert(302));
    HtmlEntityProvider.AddSingle(symbols, "Iopf;", HtmlEntityProvider.Convert(120128));
    HtmlEntityProvider.AddSingle(symbols, "Iota;", HtmlEntityProvider.Convert(921));
    HtmlEntityProvider.AddSingle(symbols, "Iscr;", HtmlEntityProvider.Convert(8464));
    HtmlEntityProvider.AddSingle(symbols, "Itilde;", HtmlEntityProvider.Convert(296));
    HtmlEntityProvider.AddSingle(symbols, "Iukcy;", HtmlEntityProvider.Convert(1030));
    HtmlEntityProvider.AddBoth(symbols, "Iuml;", HtmlEntityProvider.Convert(207));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleJ()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "jcirc;", HtmlEntityProvider.Convert(309));
    HtmlEntityProvider.AddSingle(symbols, "jcy;", HtmlEntityProvider.Convert(1081));
    HtmlEntityProvider.AddSingle(symbols, "jfr;", HtmlEntityProvider.Convert(120103));
    HtmlEntityProvider.AddSingle(symbols, "jmath;", HtmlEntityProvider.Convert(567));
    HtmlEntityProvider.AddSingle(symbols, "jopf;", HtmlEntityProvider.Convert(120155));
    HtmlEntityProvider.AddSingle(symbols, "jscr;", HtmlEntityProvider.Convert(119999));
    HtmlEntityProvider.AddSingle(symbols, "jsercy;", HtmlEntityProvider.Convert(1112));
    HtmlEntityProvider.AddSingle(symbols, "jukcy;", HtmlEntityProvider.Convert(1108));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigJ()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Jcirc;", HtmlEntityProvider.Convert(308));
    HtmlEntityProvider.AddSingle(symbols, "Jcy;", HtmlEntityProvider.Convert(1049));
    HtmlEntityProvider.AddSingle(symbols, "Jfr;", HtmlEntityProvider.Convert(120077));
    HtmlEntityProvider.AddSingle(symbols, "Jopf;", HtmlEntityProvider.Convert(120129));
    HtmlEntityProvider.AddSingle(symbols, "Jscr;", HtmlEntityProvider.Convert(119973));
    HtmlEntityProvider.AddSingle(symbols, "Jsercy;", HtmlEntityProvider.Convert(1032));
    HtmlEntityProvider.AddSingle(symbols, "Jukcy;", HtmlEntityProvider.Convert(1028));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleK()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "kappa;", HtmlEntityProvider.Convert(954));
    HtmlEntityProvider.AddSingle(symbols, "kappav;", HtmlEntityProvider.Convert(1008));
    HtmlEntityProvider.AddSingle(symbols, "kcedil;", HtmlEntityProvider.Convert(311));
    HtmlEntityProvider.AddSingle(symbols, "kcy;", HtmlEntityProvider.Convert(1082));
    HtmlEntityProvider.AddSingle(symbols, "kfr;", HtmlEntityProvider.Convert(120104));
    HtmlEntityProvider.AddSingle(symbols, "kgreen;", HtmlEntityProvider.Convert(312));
    HtmlEntityProvider.AddSingle(symbols, "khcy;", HtmlEntityProvider.Convert(1093));
    HtmlEntityProvider.AddSingle(symbols, "kjcy;", HtmlEntityProvider.Convert(1116));
    HtmlEntityProvider.AddSingle(symbols, "kopf;", HtmlEntityProvider.Convert(120156));
    HtmlEntityProvider.AddSingle(symbols, "kscr;", HtmlEntityProvider.Convert(120000));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigK()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Kappa;", HtmlEntityProvider.Convert(922));
    HtmlEntityProvider.AddSingle(symbols, "Kcedil;", HtmlEntityProvider.Convert(310));
    HtmlEntityProvider.AddSingle(symbols, "Kcy;", HtmlEntityProvider.Convert(1050));
    HtmlEntityProvider.AddSingle(symbols, "Kfr;", HtmlEntityProvider.Convert(120078));
    HtmlEntityProvider.AddSingle(symbols, "KHcy;", HtmlEntityProvider.Convert(1061));
    HtmlEntityProvider.AddSingle(symbols, "KJcy;", HtmlEntityProvider.Convert(1036));
    HtmlEntityProvider.AddSingle(symbols, "Kopf;", HtmlEntityProvider.Convert(120130));
    HtmlEntityProvider.AddSingle(symbols, "Kscr;", HtmlEntityProvider.Convert(119974));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleL()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "lAarr;", HtmlEntityProvider.Convert(8666));
    HtmlEntityProvider.AddSingle(symbols, "lacute;", HtmlEntityProvider.Convert(314));
    HtmlEntityProvider.AddSingle(symbols, "laemptyv;", HtmlEntityProvider.Convert(10676));
    HtmlEntityProvider.AddSingle(symbols, "lagran;", HtmlEntityProvider.Convert(8466));
    HtmlEntityProvider.AddSingle(symbols, "lambda;", HtmlEntityProvider.Convert(955));
    HtmlEntityProvider.AddSingle(symbols, "lang;", HtmlEntityProvider.Convert(10216));
    HtmlEntityProvider.AddSingle(symbols, "langd;", HtmlEntityProvider.Convert(10641));
    HtmlEntityProvider.AddSingle(symbols, "langle;", HtmlEntityProvider.Convert(10216));
    HtmlEntityProvider.AddSingle(symbols, "lap;", HtmlEntityProvider.Convert(10885));
    HtmlEntityProvider.AddBoth(symbols, "laquo;", HtmlEntityProvider.Convert(171));
    HtmlEntityProvider.AddSingle(symbols, "lArr;", HtmlEntityProvider.Convert(8656));
    HtmlEntityProvider.AddSingle(symbols, "larr;", HtmlEntityProvider.Convert(8592));
    HtmlEntityProvider.AddSingle(symbols, "larrb;", HtmlEntityProvider.Convert(8676));
    HtmlEntityProvider.AddSingle(symbols, "larrbfs;", HtmlEntityProvider.Convert(10527));
    HtmlEntityProvider.AddSingle(symbols, "larrfs;", HtmlEntityProvider.Convert(10525));
    HtmlEntityProvider.AddSingle(symbols, "larrhk;", HtmlEntityProvider.Convert(8617));
    HtmlEntityProvider.AddSingle(symbols, "larrlp;", HtmlEntityProvider.Convert(8619));
    HtmlEntityProvider.AddSingle(symbols, "larrpl;", HtmlEntityProvider.Convert(10553));
    HtmlEntityProvider.AddSingle(symbols, "larrsim;", HtmlEntityProvider.Convert(10611));
    HtmlEntityProvider.AddSingle(symbols, "larrtl;", HtmlEntityProvider.Convert(8610));
    HtmlEntityProvider.AddSingle(symbols, "lat;", HtmlEntityProvider.Convert(10923));
    HtmlEntityProvider.AddSingle(symbols, "lAtail;", HtmlEntityProvider.Convert(10523));
    HtmlEntityProvider.AddSingle(symbols, "latail;", HtmlEntityProvider.Convert(10521));
    HtmlEntityProvider.AddSingle(symbols, "late;", HtmlEntityProvider.Convert(10925));
    HtmlEntityProvider.AddSingle(symbols, "lates;", HtmlEntityProvider.Convert(10925, 65024));
    HtmlEntityProvider.AddSingle(symbols, "lBarr;", HtmlEntityProvider.Convert(10510));
    HtmlEntityProvider.AddSingle(symbols, "lbarr;", HtmlEntityProvider.Convert(10508));
    HtmlEntityProvider.AddSingle(symbols, "lbbrk;", HtmlEntityProvider.Convert(10098));
    HtmlEntityProvider.AddSingle(symbols, "lbrace;", HtmlEntityProvider.Convert(123));
    HtmlEntityProvider.AddSingle(symbols, "lbrack;", HtmlEntityProvider.Convert(91));
    HtmlEntityProvider.AddSingle(symbols, "lbrke;", HtmlEntityProvider.Convert(10635));
    HtmlEntityProvider.AddSingle(symbols, "lbrksld;", HtmlEntityProvider.Convert(10639));
    HtmlEntityProvider.AddSingle(symbols, "lbrkslu;", HtmlEntityProvider.Convert(10637));
    HtmlEntityProvider.AddSingle(symbols, "lcaron;", HtmlEntityProvider.Convert(318));
    HtmlEntityProvider.AddSingle(symbols, "lcedil;", HtmlEntityProvider.Convert(316));
    HtmlEntityProvider.AddSingle(symbols, "lceil;", HtmlEntityProvider.Convert(8968));
    HtmlEntityProvider.AddSingle(symbols, "lcub;", HtmlEntityProvider.Convert(123));
    HtmlEntityProvider.AddSingle(symbols, "lcy;", HtmlEntityProvider.Convert(1083));
    HtmlEntityProvider.AddSingle(symbols, "ldca;", HtmlEntityProvider.Convert(10550));
    HtmlEntityProvider.AddSingle(symbols, "ldquo;", HtmlEntityProvider.Convert(8220));
    HtmlEntityProvider.AddSingle(symbols, "ldquor;", HtmlEntityProvider.Convert(8222));
    HtmlEntityProvider.AddSingle(symbols, "ldrdhar;", HtmlEntityProvider.Convert(10599));
    HtmlEntityProvider.AddSingle(symbols, "ldrushar;", HtmlEntityProvider.Convert(10571));
    HtmlEntityProvider.AddSingle(symbols, "ldsh;", HtmlEntityProvider.Convert(8626));
    HtmlEntityProvider.AddSingle(symbols, "lE;", HtmlEntityProvider.Convert(8806));
    HtmlEntityProvider.AddSingle(symbols, "le;", HtmlEntityProvider.Convert(8804));
    HtmlEntityProvider.AddSingle(symbols, "leftarrow;", HtmlEntityProvider.Convert(8592));
    HtmlEntityProvider.AddSingle(symbols, "leftarrowtail;", HtmlEntityProvider.Convert(8610));
    HtmlEntityProvider.AddSingle(symbols, "leftharpoondown;", HtmlEntityProvider.Convert(8637));
    HtmlEntityProvider.AddSingle(symbols, "leftharpoonup;", HtmlEntityProvider.Convert(8636));
    HtmlEntityProvider.AddSingle(symbols, "leftleftarrows;", HtmlEntityProvider.Convert(8647));
    HtmlEntityProvider.AddSingle(symbols, "leftrightarrow;", HtmlEntityProvider.Convert(8596));
    HtmlEntityProvider.AddSingle(symbols, "leftrightarrows;", HtmlEntityProvider.Convert(8646));
    HtmlEntityProvider.AddSingle(symbols, "leftrightharpoons;", HtmlEntityProvider.Convert(8651));
    HtmlEntityProvider.AddSingle(symbols, "leftrightsquigarrow;", HtmlEntityProvider.Convert(8621));
    HtmlEntityProvider.AddSingle(symbols, "leftthreetimes;", HtmlEntityProvider.Convert(8907));
    HtmlEntityProvider.AddSingle(symbols, "lEg;", HtmlEntityProvider.Convert(10891));
    HtmlEntityProvider.AddSingle(symbols, "leg;", HtmlEntityProvider.Convert(8922));
    HtmlEntityProvider.AddSingle(symbols, "leq;", HtmlEntityProvider.Convert(8804));
    HtmlEntityProvider.AddSingle(symbols, "leqq;", HtmlEntityProvider.Convert(8806));
    HtmlEntityProvider.AddSingle(symbols, "leqslant;", HtmlEntityProvider.Convert(10877));
    HtmlEntityProvider.AddSingle(symbols, "les;", HtmlEntityProvider.Convert(10877));
    HtmlEntityProvider.AddSingle(symbols, "lescc;", HtmlEntityProvider.Convert(10920));
    HtmlEntityProvider.AddSingle(symbols, "lesdot;", HtmlEntityProvider.Convert(10879));
    HtmlEntityProvider.AddSingle(symbols, "lesdoto;", HtmlEntityProvider.Convert(10881));
    HtmlEntityProvider.AddSingle(symbols, "lesdotor;", HtmlEntityProvider.Convert(10883));
    HtmlEntityProvider.AddSingle(symbols, "lesg;", HtmlEntityProvider.Convert(8922, 65024));
    HtmlEntityProvider.AddSingle(symbols, "lesges;", HtmlEntityProvider.Convert(10899));
    HtmlEntityProvider.AddSingle(symbols, "lessapprox;", HtmlEntityProvider.Convert(10885));
    HtmlEntityProvider.AddSingle(symbols, "lessdot;", HtmlEntityProvider.Convert(8918));
    HtmlEntityProvider.AddSingle(symbols, "lesseqgtr;", HtmlEntityProvider.Convert(8922));
    HtmlEntityProvider.AddSingle(symbols, "lesseqqgtr;", HtmlEntityProvider.Convert(10891));
    HtmlEntityProvider.AddSingle(symbols, "lessgtr;", HtmlEntityProvider.Convert(8822));
    HtmlEntityProvider.AddSingle(symbols, "lesssim;", HtmlEntityProvider.Convert(8818));
    HtmlEntityProvider.AddSingle(symbols, "lfisht;", HtmlEntityProvider.Convert(10620));
    HtmlEntityProvider.AddSingle(symbols, "lfloor;", HtmlEntityProvider.Convert(8970));
    HtmlEntityProvider.AddSingle(symbols, "lfr;", HtmlEntityProvider.Convert(120105));
    HtmlEntityProvider.AddSingle(symbols, "lg;", HtmlEntityProvider.Convert(8822));
    HtmlEntityProvider.AddSingle(symbols, "lgE;", HtmlEntityProvider.Convert(10897));
    HtmlEntityProvider.AddSingle(symbols, "lHar;", HtmlEntityProvider.Convert(10594));
    HtmlEntityProvider.AddSingle(symbols, "lhard;", HtmlEntityProvider.Convert(8637));
    HtmlEntityProvider.AddSingle(symbols, "lharu;", HtmlEntityProvider.Convert(8636));
    HtmlEntityProvider.AddSingle(symbols, "lharul;", HtmlEntityProvider.Convert(10602));
    HtmlEntityProvider.AddSingle(symbols, "lhblk;", HtmlEntityProvider.Convert(9604));
    HtmlEntityProvider.AddSingle(symbols, "ljcy;", HtmlEntityProvider.Convert(1113));
    HtmlEntityProvider.AddSingle(symbols, "ll;", HtmlEntityProvider.Convert(8810));
    HtmlEntityProvider.AddSingle(symbols, "llarr;", HtmlEntityProvider.Convert(8647));
    HtmlEntityProvider.AddSingle(symbols, "llcorner;", HtmlEntityProvider.Convert(8990));
    HtmlEntityProvider.AddSingle(symbols, "llhard;", HtmlEntityProvider.Convert(10603));
    HtmlEntityProvider.AddSingle(symbols, "lltri;", HtmlEntityProvider.Convert(9722));
    HtmlEntityProvider.AddSingle(symbols, "lmidot;", HtmlEntityProvider.Convert(320));
    HtmlEntityProvider.AddSingle(symbols, "lmoust;", HtmlEntityProvider.Convert(9136));
    HtmlEntityProvider.AddSingle(symbols, "lmoustache;", HtmlEntityProvider.Convert(9136));
    HtmlEntityProvider.AddSingle(symbols, "lnap;", HtmlEntityProvider.Convert(10889));
    HtmlEntityProvider.AddSingle(symbols, "lnapprox;", HtmlEntityProvider.Convert(10889));
    HtmlEntityProvider.AddSingle(symbols, "lnE;", HtmlEntityProvider.Convert(8808));
    HtmlEntityProvider.AddSingle(symbols, "lne;", HtmlEntityProvider.Convert(10887));
    HtmlEntityProvider.AddSingle(symbols, "lneq;", HtmlEntityProvider.Convert(10887));
    HtmlEntityProvider.AddSingle(symbols, "lneqq;", HtmlEntityProvider.Convert(8808));
    HtmlEntityProvider.AddSingle(symbols, "lnsim;", HtmlEntityProvider.Convert(8934));
    HtmlEntityProvider.AddSingle(symbols, "loang;", HtmlEntityProvider.Convert(10220));
    HtmlEntityProvider.AddSingle(symbols, "loarr;", HtmlEntityProvider.Convert(8701));
    HtmlEntityProvider.AddSingle(symbols, "lobrk;", HtmlEntityProvider.Convert(10214));
    HtmlEntityProvider.AddSingle(symbols, "longleftarrow;", HtmlEntityProvider.Convert(10229));
    HtmlEntityProvider.AddSingle(symbols, "longleftrightarrow;", HtmlEntityProvider.Convert(10231));
    HtmlEntityProvider.AddSingle(symbols, "longmapsto;", HtmlEntityProvider.Convert(10236));
    HtmlEntityProvider.AddSingle(symbols, "longrightarrow;", HtmlEntityProvider.Convert(10230));
    HtmlEntityProvider.AddSingle(symbols, "looparrowleft;", HtmlEntityProvider.Convert(8619));
    HtmlEntityProvider.AddSingle(symbols, "looparrowright;", HtmlEntityProvider.Convert(8620));
    HtmlEntityProvider.AddSingle(symbols, "lopar;", HtmlEntityProvider.Convert(10629));
    HtmlEntityProvider.AddSingle(symbols, "lopf;", HtmlEntityProvider.Convert(120157));
    HtmlEntityProvider.AddSingle(symbols, "loplus;", HtmlEntityProvider.Convert(10797));
    HtmlEntityProvider.AddSingle(symbols, "lotimes;", HtmlEntityProvider.Convert(10804));
    HtmlEntityProvider.AddSingle(symbols, "lowast;", HtmlEntityProvider.Convert(8727));
    HtmlEntityProvider.AddSingle(symbols, "lowbar;", HtmlEntityProvider.Convert(95));
    HtmlEntityProvider.AddSingle(symbols, "loz;", HtmlEntityProvider.Convert(9674));
    HtmlEntityProvider.AddSingle(symbols, "lozenge;", HtmlEntityProvider.Convert(9674));
    HtmlEntityProvider.AddSingle(symbols, "lozf;", HtmlEntityProvider.Convert(10731));
    HtmlEntityProvider.AddSingle(symbols, "lpar;", HtmlEntityProvider.Convert(40));
    HtmlEntityProvider.AddSingle(symbols, "lparlt;", HtmlEntityProvider.Convert(10643));
    HtmlEntityProvider.AddSingle(symbols, "lrarr;", HtmlEntityProvider.Convert(8646));
    HtmlEntityProvider.AddSingle(symbols, "lrcorner;", HtmlEntityProvider.Convert(8991));
    HtmlEntityProvider.AddSingle(symbols, "lrhar;", HtmlEntityProvider.Convert(8651));
    HtmlEntityProvider.AddSingle(symbols, "lrhard;", HtmlEntityProvider.Convert(10605));
    HtmlEntityProvider.AddSingle(symbols, "lrm;", HtmlEntityProvider.Convert(8206));
    HtmlEntityProvider.AddSingle(symbols, "lrtri;", HtmlEntityProvider.Convert(8895));
    HtmlEntityProvider.AddSingle(symbols, "lsaquo;", HtmlEntityProvider.Convert(8249));
    HtmlEntityProvider.AddSingle(symbols, "lscr;", HtmlEntityProvider.Convert(120001));
    HtmlEntityProvider.AddSingle(symbols, "lsh;", HtmlEntityProvider.Convert(8624));
    HtmlEntityProvider.AddSingle(symbols, "lsim;", HtmlEntityProvider.Convert(8818));
    HtmlEntityProvider.AddSingle(symbols, "lsime;", HtmlEntityProvider.Convert(10893));
    HtmlEntityProvider.AddSingle(symbols, "lsimg;", HtmlEntityProvider.Convert(10895));
    HtmlEntityProvider.AddSingle(symbols, "lsqb;", HtmlEntityProvider.Convert(91));
    HtmlEntityProvider.AddSingle(symbols, "lsquo;", HtmlEntityProvider.Convert(8216));
    HtmlEntityProvider.AddSingle(symbols, "lsquor;", HtmlEntityProvider.Convert(8218));
    HtmlEntityProvider.AddSingle(symbols, "lstrok;", HtmlEntityProvider.Convert(322));
    HtmlEntityProvider.AddBoth(symbols, "lt;", HtmlEntityProvider.Convert(60));
    HtmlEntityProvider.AddSingle(symbols, "ltcc;", HtmlEntityProvider.Convert(10918));
    HtmlEntityProvider.AddSingle(symbols, "ltcir;", HtmlEntityProvider.Convert(10873));
    HtmlEntityProvider.AddSingle(symbols, "ltdot;", HtmlEntityProvider.Convert(8918));
    HtmlEntityProvider.AddSingle(symbols, "lthree;", HtmlEntityProvider.Convert(8907));
    HtmlEntityProvider.AddSingle(symbols, "ltimes;", HtmlEntityProvider.Convert(8905));
    HtmlEntityProvider.AddSingle(symbols, "ltlarr;", HtmlEntityProvider.Convert(10614));
    HtmlEntityProvider.AddSingle(symbols, "ltquest;", HtmlEntityProvider.Convert(10875));
    HtmlEntityProvider.AddSingle(symbols, "ltri;", HtmlEntityProvider.Convert(9667));
    HtmlEntityProvider.AddSingle(symbols, "ltrie;", HtmlEntityProvider.Convert(8884));
    HtmlEntityProvider.AddSingle(symbols, "ltrif;", HtmlEntityProvider.Convert(9666));
    HtmlEntityProvider.AddSingle(symbols, "ltrPar;", HtmlEntityProvider.Convert(10646));
    HtmlEntityProvider.AddSingle(symbols, "lurdshar;", HtmlEntityProvider.Convert(10570));
    HtmlEntityProvider.AddSingle(symbols, "luruhar;", HtmlEntityProvider.Convert(10598));
    HtmlEntityProvider.AddSingle(symbols, "lvertneqq;", HtmlEntityProvider.Convert(8808, 65024));
    HtmlEntityProvider.AddSingle(symbols, "lvnE;", HtmlEntityProvider.Convert(8808, 65024));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigL()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Lacute;", HtmlEntityProvider.Convert(313));
    HtmlEntityProvider.AddSingle(symbols, "Lambda;", HtmlEntityProvider.Convert(923));
    HtmlEntityProvider.AddSingle(symbols, "Lang;", HtmlEntityProvider.Convert(10218));
    HtmlEntityProvider.AddSingle(symbols, "Laplacetrf;", HtmlEntityProvider.Convert(8466));
    HtmlEntityProvider.AddSingle(symbols, "Larr;", HtmlEntityProvider.Convert(8606));
    HtmlEntityProvider.AddSingle(symbols, "Lcaron;", HtmlEntityProvider.Convert(317));
    HtmlEntityProvider.AddSingle(symbols, "Lcedil;", HtmlEntityProvider.Convert(315));
    HtmlEntityProvider.AddSingle(symbols, "Lcy;", HtmlEntityProvider.Convert(1051));
    HtmlEntityProvider.AddSingle(symbols, "LeftAngleBracket;", HtmlEntityProvider.Convert(10216));
    HtmlEntityProvider.AddSingle(symbols, "LeftArrow;", HtmlEntityProvider.Convert(8592));
    HtmlEntityProvider.AddSingle(symbols, "Leftarrow;", HtmlEntityProvider.Convert(8656));
    HtmlEntityProvider.AddSingle(symbols, "LeftArrowBar;", HtmlEntityProvider.Convert(8676));
    HtmlEntityProvider.AddSingle(symbols, "LeftArrowRightArrow;", HtmlEntityProvider.Convert(8646));
    HtmlEntityProvider.AddSingle(symbols, "LeftCeiling;", HtmlEntityProvider.Convert(8968));
    HtmlEntityProvider.AddSingle(symbols, "LeftDoubleBracket;", HtmlEntityProvider.Convert(10214));
    HtmlEntityProvider.AddSingle(symbols, "LeftDownTeeVector;", HtmlEntityProvider.Convert(10593));
    HtmlEntityProvider.AddSingle(symbols, "LeftDownVector;", HtmlEntityProvider.Convert(8643));
    HtmlEntityProvider.AddSingle(symbols, "LeftDownVectorBar;", HtmlEntityProvider.Convert(10585));
    HtmlEntityProvider.AddSingle(symbols, "LeftFloor;", HtmlEntityProvider.Convert(8970));
    HtmlEntityProvider.AddSingle(symbols, "LeftRightArrow;", HtmlEntityProvider.Convert(8596));
    HtmlEntityProvider.AddSingle(symbols, "Leftrightarrow;", HtmlEntityProvider.Convert(8660));
    HtmlEntityProvider.AddSingle(symbols, "LeftRightVector;", HtmlEntityProvider.Convert(10574));
    HtmlEntityProvider.AddSingle(symbols, "LeftTee;", HtmlEntityProvider.Convert(8867));
    HtmlEntityProvider.AddSingle(symbols, "LeftTeeArrow;", HtmlEntityProvider.Convert(8612));
    HtmlEntityProvider.AddSingle(symbols, "LeftTeeVector;", HtmlEntityProvider.Convert(10586));
    HtmlEntityProvider.AddSingle(symbols, "LeftTriangle;", HtmlEntityProvider.Convert(8882));
    HtmlEntityProvider.AddSingle(symbols, "LeftTriangleBar;", HtmlEntityProvider.Convert(10703));
    HtmlEntityProvider.AddSingle(symbols, "LeftTriangleEqual;", HtmlEntityProvider.Convert(8884));
    HtmlEntityProvider.AddSingle(symbols, "LeftUpDownVector;", HtmlEntityProvider.Convert(10577));
    HtmlEntityProvider.AddSingle(symbols, "LeftUpTeeVector;", HtmlEntityProvider.Convert(10592));
    HtmlEntityProvider.AddSingle(symbols, "LeftUpVector;", HtmlEntityProvider.Convert(8639));
    HtmlEntityProvider.AddSingle(symbols, "LeftUpVectorBar;", HtmlEntityProvider.Convert(10584));
    HtmlEntityProvider.AddSingle(symbols, "LeftVector;", HtmlEntityProvider.Convert(8636));
    HtmlEntityProvider.AddSingle(symbols, "LeftVectorBar;", HtmlEntityProvider.Convert(10578));
    HtmlEntityProvider.AddSingle(symbols, "LessEqualGreater;", HtmlEntityProvider.Convert(8922));
    HtmlEntityProvider.AddSingle(symbols, "LessFullEqual;", HtmlEntityProvider.Convert(8806));
    HtmlEntityProvider.AddSingle(symbols, "LessGreater;", HtmlEntityProvider.Convert(8822));
    HtmlEntityProvider.AddSingle(symbols, "LessLess;", HtmlEntityProvider.Convert(10913));
    HtmlEntityProvider.AddSingle(symbols, "LessSlantEqual;", HtmlEntityProvider.Convert(10877));
    HtmlEntityProvider.AddSingle(symbols, "LessTilde;", HtmlEntityProvider.Convert(8818));
    HtmlEntityProvider.AddSingle(symbols, "Lfr;", HtmlEntityProvider.Convert(120079));
    HtmlEntityProvider.AddSingle(symbols, "LJcy;", HtmlEntityProvider.Convert(1033));
    HtmlEntityProvider.AddSingle(symbols, "Ll;", HtmlEntityProvider.Convert(8920));
    HtmlEntityProvider.AddSingle(symbols, "Lleftarrow;", HtmlEntityProvider.Convert(8666));
    HtmlEntityProvider.AddSingle(symbols, "Lmidot;", HtmlEntityProvider.Convert(319));
    HtmlEntityProvider.AddSingle(symbols, "LongLeftArrow;", HtmlEntityProvider.Convert(10229));
    HtmlEntityProvider.AddSingle(symbols, "Longleftarrow;", HtmlEntityProvider.Convert(10232));
    HtmlEntityProvider.AddSingle(symbols, "LongLeftRightArrow;", HtmlEntityProvider.Convert(10231));
    HtmlEntityProvider.AddSingle(symbols, "Longleftrightarrow;", HtmlEntityProvider.Convert(10234));
    HtmlEntityProvider.AddSingle(symbols, "LongRightArrow;", HtmlEntityProvider.Convert(10230));
    HtmlEntityProvider.AddSingle(symbols, "Longrightarrow;", HtmlEntityProvider.Convert(10233));
    HtmlEntityProvider.AddSingle(symbols, "Lopf;", HtmlEntityProvider.Convert(120131));
    HtmlEntityProvider.AddSingle(symbols, "LowerLeftArrow;", HtmlEntityProvider.Convert(8601));
    HtmlEntityProvider.AddSingle(symbols, "LowerRightArrow;", HtmlEntityProvider.Convert(8600));
    HtmlEntityProvider.AddSingle(symbols, "Lscr;", HtmlEntityProvider.Convert(8466));
    HtmlEntityProvider.AddSingle(symbols, "Lsh;", HtmlEntityProvider.Convert(8624));
    HtmlEntityProvider.AddSingle(symbols, "Lstrok;", HtmlEntityProvider.Convert(321));
    HtmlEntityProvider.AddBoth(symbols, "LT;", HtmlEntityProvider.Convert(60));
    HtmlEntityProvider.AddSingle(symbols, "Lt;", HtmlEntityProvider.Convert(8810));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleM()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "macr;", HtmlEntityProvider.Convert(175));
    HtmlEntityProvider.AddSingle(symbols, "male;", HtmlEntityProvider.Convert(9794));
    HtmlEntityProvider.AddSingle(symbols, "malt;", HtmlEntityProvider.Convert(10016));
    HtmlEntityProvider.AddSingle(symbols, "maltese;", HtmlEntityProvider.Convert(10016));
    HtmlEntityProvider.AddSingle(symbols, "map;", HtmlEntityProvider.Convert(8614));
    HtmlEntityProvider.AddSingle(symbols, "mapsto;", HtmlEntityProvider.Convert(8614));
    HtmlEntityProvider.AddSingle(symbols, "mapstodown;", HtmlEntityProvider.Convert(8615));
    HtmlEntityProvider.AddSingle(symbols, "mapstoleft;", HtmlEntityProvider.Convert(8612));
    HtmlEntityProvider.AddSingle(symbols, "mapstoup;", HtmlEntityProvider.Convert(8613));
    HtmlEntityProvider.AddSingle(symbols, "marker;", HtmlEntityProvider.Convert(9646));
    HtmlEntityProvider.AddSingle(symbols, "mcomma;", HtmlEntityProvider.Convert(10793));
    HtmlEntityProvider.AddSingle(symbols, "mcy;", HtmlEntityProvider.Convert(1084));
    HtmlEntityProvider.AddSingle(symbols, "mdash;", HtmlEntityProvider.Convert(8212));
    HtmlEntityProvider.AddSingle(symbols, "mDDot;", HtmlEntityProvider.Convert(8762));
    HtmlEntityProvider.AddSingle(symbols, "measuredangle;", HtmlEntityProvider.Convert(8737));
    HtmlEntityProvider.AddSingle(symbols, "mfr;", HtmlEntityProvider.Convert(120106));
    HtmlEntityProvider.AddSingle(symbols, "mho;", HtmlEntityProvider.Convert(8487));
    HtmlEntityProvider.AddBoth(symbols, "micro;", HtmlEntityProvider.Convert(181));
    HtmlEntityProvider.AddSingle(symbols, "mid;", HtmlEntityProvider.Convert(8739));
    HtmlEntityProvider.AddSingle(symbols, "midast;", HtmlEntityProvider.Convert(42));
    HtmlEntityProvider.AddSingle(symbols, "midcir;", HtmlEntityProvider.Convert(10992));
    HtmlEntityProvider.AddBoth(symbols, "middot;", HtmlEntityProvider.Convert(183));
    HtmlEntityProvider.AddSingle(symbols, "minus;", HtmlEntityProvider.Convert(8722));
    HtmlEntityProvider.AddSingle(symbols, "minusb;", HtmlEntityProvider.Convert(8863));
    HtmlEntityProvider.AddSingle(symbols, "minusd;", HtmlEntityProvider.Convert(8760));
    HtmlEntityProvider.AddSingle(symbols, "minusdu;", HtmlEntityProvider.Convert(10794));
    HtmlEntityProvider.AddSingle(symbols, "mlcp;", HtmlEntityProvider.Convert(10971));
    HtmlEntityProvider.AddSingle(symbols, "mldr;", HtmlEntityProvider.Convert(8230));
    HtmlEntityProvider.AddSingle(symbols, "mnplus;", HtmlEntityProvider.Convert(8723));
    HtmlEntityProvider.AddSingle(symbols, "models;", HtmlEntityProvider.Convert(8871));
    HtmlEntityProvider.AddSingle(symbols, "mopf;", HtmlEntityProvider.Convert(120158));
    HtmlEntityProvider.AddSingle(symbols, "mp;", HtmlEntityProvider.Convert(8723));
    HtmlEntityProvider.AddSingle(symbols, "mscr;", HtmlEntityProvider.Convert(120002));
    HtmlEntityProvider.AddSingle(symbols, "mstpos;", HtmlEntityProvider.Convert(8766));
    HtmlEntityProvider.AddSingle(symbols, "mu;", HtmlEntityProvider.Convert(956));
    HtmlEntityProvider.AddSingle(symbols, "multimap;", HtmlEntityProvider.Convert(8888));
    HtmlEntityProvider.AddSingle(symbols, "mumap;", HtmlEntityProvider.Convert(8888));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigM()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Map;", HtmlEntityProvider.Convert(10501));
    HtmlEntityProvider.AddSingle(symbols, "Mcy;", HtmlEntityProvider.Convert(1052));
    HtmlEntityProvider.AddSingle(symbols, "MediumSpace;", HtmlEntityProvider.Convert(8287));
    HtmlEntityProvider.AddSingle(symbols, "Mellintrf;", HtmlEntityProvider.Convert(8499));
    HtmlEntityProvider.AddSingle(symbols, "Mfr;", HtmlEntityProvider.Convert(120080));
    HtmlEntityProvider.AddSingle(symbols, "MinusPlus;", HtmlEntityProvider.Convert(8723));
    HtmlEntityProvider.AddSingle(symbols, "Mopf;", HtmlEntityProvider.Convert(120132));
    HtmlEntityProvider.AddSingle(symbols, "Mscr;", HtmlEntityProvider.Convert(8499));
    HtmlEntityProvider.AddSingle(symbols, "Mu;", HtmlEntityProvider.Convert(924));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleN()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "nabla;", HtmlEntityProvider.Convert(8711));
    HtmlEntityProvider.AddSingle(symbols, "nacute;", HtmlEntityProvider.Convert(324));
    HtmlEntityProvider.AddSingle(symbols, "nang;", HtmlEntityProvider.Convert(8736, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nap;", HtmlEntityProvider.Convert(8777));
    HtmlEntityProvider.AddSingle(symbols, "napE;", HtmlEntityProvider.Convert(10864, 824));
    HtmlEntityProvider.AddSingle(symbols, "napid;", HtmlEntityProvider.Convert(8779, 824));
    HtmlEntityProvider.AddSingle(symbols, "napos;", HtmlEntityProvider.Convert(329));
    HtmlEntityProvider.AddSingle(symbols, "napprox;", HtmlEntityProvider.Convert(8777));
    HtmlEntityProvider.AddSingle(symbols, "natur;", HtmlEntityProvider.Convert(9838));
    HtmlEntityProvider.AddSingle(symbols, "natural;", HtmlEntityProvider.Convert(9838));
    HtmlEntityProvider.AddSingle(symbols, "naturals;", HtmlEntityProvider.Convert(8469));
    HtmlEntityProvider.AddBoth(symbols, "nbsp;", HtmlEntityProvider.Convert(160 /*0xA0*/));
    HtmlEntityProvider.AddSingle(symbols, "nbump;", HtmlEntityProvider.Convert(8782, 824));
    HtmlEntityProvider.AddSingle(symbols, "nbumpe;", HtmlEntityProvider.Convert(8783, 824));
    HtmlEntityProvider.AddSingle(symbols, "ncap;", HtmlEntityProvider.Convert(10819));
    HtmlEntityProvider.AddSingle(symbols, "ncaron;", HtmlEntityProvider.Convert(328));
    HtmlEntityProvider.AddSingle(symbols, "ncedil;", HtmlEntityProvider.Convert(326));
    HtmlEntityProvider.AddSingle(symbols, "ncong;", HtmlEntityProvider.Convert(8775));
    HtmlEntityProvider.AddSingle(symbols, "ncongdot;", HtmlEntityProvider.Convert(10861, 824));
    HtmlEntityProvider.AddSingle(symbols, "ncup;", HtmlEntityProvider.Convert(10818));
    HtmlEntityProvider.AddSingle(symbols, "ncy;", HtmlEntityProvider.Convert(1085));
    HtmlEntityProvider.AddSingle(symbols, "ndash;", HtmlEntityProvider.Convert(8211));
    HtmlEntityProvider.AddSingle(symbols, "ne;", HtmlEntityProvider.Convert(8800));
    HtmlEntityProvider.AddSingle(symbols, "nearhk;", HtmlEntityProvider.Convert(10532));
    HtmlEntityProvider.AddSingle(symbols, "neArr;", HtmlEntityProvider.Convert(8663));
    HtmlEntityProvider.AddSingle(symbols, "nearr;", HtmlEntityProvider.Convert(8599));
    HtmlEntityProvider.AddSingle(symbols, "nearrow;", HtmlEntityProvider.Convert(8599));
    HtmlEntityProvider.AddSingle(symbols, "nedot;", HtmlEntityProvider.Convert(8784, 824));
    HtmlEntityProvider.AddSingle(symbols, "nequiv;", HtmlEntityProvider.Convert(8802));
    HtmlEntityProvider.AddSingle(symbols, "nesear;", HtmlEntityProvider.Convert(10536));
    HtmlEntityProvider.AddSingle(symbols, "nesim;", HtmlEntityProvider.Convert(8770, 824));
    HtmlEntityProvider.AddSingle(symbols, "nexist;", HtmlEntityProvider.Convert(8708));
    HtmlEntityProvider.AddSingle(symbols, "nexists;", HtmlEntityProvider.Convert(8708));
    HtmlEntityProvider.AddSingle(symbols, "nfr;", HtmlEntityProvider.Convert(120107));
    HtmlEntityProvider.AddSingle(symbols, "ngE;", HtmlEntityProvider.Convert(8807, 824));
    HtmlEntityProvider.AddSingle(symbols, "nge;", HtmlEntityProvider.Convert(8817));
    HtmlEntityProvider.AddSingle(symbols, "ngeq;", HtmlEntityProvider.Convert(8817));
    HtmlEntityProvider.AddSingle(symbols, "ngeqq;", HtmlEntityProvider.Convert(8807, 824));
    HtmlEntityProvider.AddSingle(symbols, "ngeqslant;", HtmlEntityProvider.Convert(10878, 824));
    HtmlEntityProvider.AddSingle(symbols, "nges;", HtmlEntityProvider.Convert(10878, 824));
    HtmlEntityProvider.AddSingle(symbols, "nGg;", HtmlEntityProvider.Convert(8921, 824));
    HtmlEntityProvider.AddSingle(symbols, "ngsim;", HtmlEntityProvider.Convert(8821));
    HtmlEntityProvider.AddSingle(symbols, "nGt;", HtmlEntityProvider.Convert(8811, 8402));
    HtmlEntityProvider.AddSingle(symbols, "ngt;", HtmlEntityProvider.Convert(8815));
    HtmlEntityProvider.AddSingle(symbols, "ngtr;", HtmlEntityProvider.Convert(8815));
    HtmlEntityProvider.AddSingle(symbols, "nGtv;", HtmlEntityProvider.Convert(8811, 824));
    HtmlEntityProvider.AddSingle(symbols, "nhArr;", HtmlEntityProvider.Convert(8654));
    HtmlEntityProvider.AddSingle(symbols, "nharr;", HtmlEntityProvider.Convert(8622));
    HtmlEntityProvider.AddSingle(symbols, "nhpar;", HtmlEntityProvider.Convert(10994));
    HtmlEntityProvider.AddSingle(symbols, "ni;", HtmlEntityProvider.Convert(8715));
    HtmlEntityProvider.AddSingle(symbols, "nis;", HtmlEntityProvider.Convert(8956));
    HtmlEntityProvider.AddSingle(symbols, "nisd;", HtmlEntityProvider.Convert(8954));
    HtmlEntityProvider.AddSingle(symbols, "niv;", HtmlEntityProvider.Convert(8715));
    HtmlEntityProvider.AddSingle(symbols, "njcy;", HtmlEntityProvider.Convert(1114));
    HtmlEntityProvider.AddSingle(symbols, "nlArr;", HtmlEntityProvider.Convert(8653));
    HtmlEntityProvider.AddSingle(symbols, "nlarr;", HtmlEntityProvider.Convert(8602));
    HtmlEntityProvider.AddSingle(symbols, "nldr;", HtmlEntityProvider.Convert(8229));
    HtmlEntityProvider.AddSingle(symbols, "nlE;", HtmlEntityProvider.Convert(8806, 824));
    HtmlEntityProvider.AddSingle(symbols, "nle;", HtmlEntityProvider.Convert(8816));
    HtmlEntityProvider.AddSingle(symbols, "nLeftarrow;", HtmlEntityProvider.Convert(8653));
    HtmlEntityProvider.AddSingle(symbols, "nleftarrow;", HtmlEntityProvider.Convert(8602));
    HtmlEntityProvider.AddSingle(symbols, "nLeftrightarrow;", HtmlEntityProvider.Convert(8654));
    HtmlEntityProvider.AddSingle(symbols, "nleftrightarrow;", HtmlEntityProvider.Convert(8622));
    HtmlEntityProvider.AddSingle(symbols, "nleq;", HtmlEntityProvider.Convert(8816));
    HtmlEntityProvider.AddSingle(symbols, "nleqq;", HtmlEntityProvider.Convert(8806, 824));
    HtmlEntityProvider.AddSingle(symbols, "nleqslant;", HtmlEntityProvider.Convert(10877, 824));
    HtmlEntityProvider.AddSingle(symbols, "nles;", HtmlEntityProvider.Convert(10877, 824));
    HtmlEntityProvider.AddSingle(symbols, "nless;", HtmlEntityProvider.Convert(8814));
    HtmlEntityProvider.AddSingle(symbols, "nLl;", HtmlEntityProvider.Convert(8920, 824));
    HtmlEntityProvider.AddSingle(symbols, "nlsim;", HtmlEntityProvider.Convert(8820));
    HtmlEntityProvider.AddSingle(symbols, "nLt;", HtmlEntityProvider.Convert(8810, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nlt;", HtmlEntityProvider.Convert(8814));
    HtmlEntityProvider.AddSingle(symbols, "nltri;", HtmlEntityProvider.Convert(8938));
    HtmlEntityProvider.AddSingle(symbols, "nltrie;", HtmlEntityProvider.Convert(8940));
    HtmlEntityProvider.AddSingle(symbols, "nLtv;", HtmlEntityProvider.Convert(8810, 824));
    HtmlEntityProvider.AddSingle(symbols, "nmid;", HtmlEntityProvider.Convert(8740));
    HtmlEntityProvider.AddSingle(symbols, "nopf;", HtmlEntityProvider.Convert(120159));
    HtmlEntityProvider.AddBoth(symbols, "not;", HtmlEntityProvider.Convert(172));
    HtmlEntityProvider.AddSingle(symbols, "notin;", HtmlEntityProvider.Convert(8713));
    HtmlEntityProvider.AddSingle(symbols, "notindot;", HtmlEntityProvider.Convert(8949, 824));
    HtmlEntityProvider.AddSingle(symbols, "notinE;", HtmlEntityProvider.Convert(8953, 824));
    HtmlEntityProvider.AddSingle(symbols, "notinva;", HtmlEntityProvider.Convert(8713));
    HtmlEntityProvider.AddSingle(symbols, "notinvb;", HtmlEntityProvider.Convert(8951));
    HtmlEntityProvider.AddSingle(symbols, "notinvc;", HtmlEntityProvider.Convert(8950));
    HtmlEntityProvider.AddSingle(symbols, "notni;", HtmlEntityProvider.Convert(8716));
    HtmlEntityProvider.AddSingle(symbols, "notniva;", HtmlEntityProvider.Convert(8716));
    HtmlEntityProvider.AddSingle(symbols, "notnivb;", HtmlEntityProvider.Convert(8958));
    HtmlEntityProvider.AddSingle(symbols, "notnivc;", HtmlEntityProvider.Convert(8957));
    HtmlEntityProvider.AddSingle(symbols, "npar;", HtmlEntityProvider.Convert(8742));
    HtmlEntityProvider.AddSingle(symbols, "nparallel;", HtmlEntityProvider.Convert(8742));
    HtmlEntityProvider.AddSingle(symbols, "nparsl;", HtmlEntityProvider.Convert(11005, 8421));
    HtmlEntityProvider.AddSingle(symbols, "npart;", HtmlEntityProvider.Convert(8706, 824));
    HtmlEntityProvider.AddSingle(symbols, "npolint;", HtmlEntityProvider.Convert(10772));
    HtmlEntityProvider.AddSingle(symbols, "npr;", HtmlEntityProvider.Convert(8832));
    HtmlEntityProvider.AddSingle(symbols, "nprcue;", HtmlEntityProvider.Convert(8928));
    HtmlEntityProvider.AddSingle(symbols, "npre;", HtmlEntityProvider.Convert(10927, 824));
    HtmlEntityProvider.AddSingle(symbols, "nprec;", HtmlEntityProvider.Convert(8832));
    HtmlEntityProvider.AddSingle(symbols, "npreceq;", HtmlEntityProvider.Convert(10927, 824));
    HtmlEntityProvider.AddSingle(symbols, "nrArr;", HtmlEntityProvider.Convert(8655));
    HtmlEntityProvider.AddSingle(symbols, "nrarr;", HtmlEntityProvider.Convert(8603));
    HtmlEntityProvider.AddSingle(symbols, "nrarrc;", HtmlEntityProvider.Convert(10547, 824));
    HtmlEntityProvider.AddSingle(symbols, "nrarrw;", HtmlEntityProvider.Convert(8605, 824));
    HtmlEntityProvider.AddSingle(symbols, "nRightarrow;", HtmlEntityProvider.Convert(8655));
    HtmlEntityProvider.AddSingle(symbols, "nrightarrow;", HtmlEntityProvider.Convert(8603));
    HtmlEntityProvider.AddSingle(symbols, "nrtri;", HtmlEntityProvider.Convert(8939));
    HtmlEntityProvider.AddSingle(symbols, "nrtrie;", HtmlEntityProvider.Convert(8941));
    HtmlEntityProvider.AddSingle(symbols, "nsc;", HtmlEntityProvider.Convert(8833));
    HtmlEntityProvider.AddSingle(symbols, "nsccue;", HtmlEntityProvider.Convert(8929));
    HtmlEntityProvider.AddSingle(symbols, "nsce;", HtmlEntityProvider.Convert(10928, 824));
    HtmlEntityProvider.AddSingle(symbols, "nscr;", HtmlEntityProvider.Convert(120003));
    HtmlEntityProvider.AddSingle(symbols, "nshortmid;", HtmlEntityProvider.Convert(8740));
    HtmlEntityProvider.AddSingle(symbols, "nshortparallel;", HtmlEntityProvider.Convert(8742));
    HtmlEntityProvider.AddSingle(symbols, "nsim;", HtmlEntityProvider.Convert(8769));
    HtmlEntityProvider.AddSingle(symbols, "nsime;", HtmlEntityProvider.Convert(8772));
    HtmlEntityProvider.AddSingle(symbols, "nsimeq;", HtmlEntityProvider.Convert(8772));
    HtmlEntityProvider.AddSingle(symbols, "nsmid;", HtmlEntityProvider.Convert(8740));
    HtmlEntityProvider.AddSingle(symbols, "nspar;", HtmlEntityProvider.Convert(8742));
    HtmlEntityProvider.AddSingle(symbols, "nsqsube;", HtmlEntityProvider.Convert(8930));
    HtmlEntityProvider.AddSingle(symbols, "nsqsupe;", HtmlEntityProvider.Convert(8931));
    HtmlEntityProvider.AddSingle(symbols, "nsub;", HtmlEntityProvider.Convert(8836));
    HtmlEntityProvider.AddSingle(symbols, "nsubE;", HtmlEntityProvider.Convert(10949, 824));
    HtmlEntityProvider.AddSingle(symbols, "nsube;", HtmlEntityProvider.Convert(8840));
    HtmlEntityProvider.AddSingle(symbols, "nsubset;", HtmlEntityProvider.Convert(8834, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nsubseteq;", HtmlEntityProvider.Convert(8840));
    HtmlEntityProvider.AddSingle(symbols, "nsubseteqq;", HtmlEntityProvider.Convert(10949, 824));
    HtmlEntityProvider.AddSingle(symbols, "nsucc;", HtmlEntityProvider.Convert(8833));
    HtmlEntityProvider.AddSingle(symbols, "nsucceq;", HtmlEntityProvider.Convert(10928, 824));
    HtmlEntityProvider.AddSingle(symbols, "nsup;", HtmlEntityProvider.Convert(8837));
    HtmlEntityProvider.AddSingle(symbols, "nsupE;", HtmlEntityProvider.Convert(10950, 824));
    HtmlEntityProvider.AddSingle(symbols, "nsupe;", HtmlEntityProvider.Convert(8841));
    HtmlEntityProvider.AddSingle(symbols, "nsupset;", HtmlEntityProvider.Convert(8835, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nsupseteq;", HtmlEntityProvider.Convert(8841));
    HtmlEntityProvider.AddSingle(symbols, "nsupseteqq;", HtmlEntityProvider.Convert(10950, 824));
    HtmlEntityProvider.AddSingle(symbols, "ntgl;", HtmlEntityProvider.Convert(8825));
    HtmlEntityProvider.AddBoth(symbols, "ntilde;", HtmlEntityProvider.Convert(241));
    HtmlEntityProvider.AddSingle(symbols, "ntlg;", HtmlEntityProvider.Convert(8824));
    HtmlEntityProvider.AddSingle(symbols, "ntriangleleft;", HtmlEntityProvider.Convert(8938));
    HtmlEntityProvider.AddSingle(symbols, "ntrianglelefteq;", HtmlEntityProvider.Convert(8940));
    HtmlEntityProvider.AddSingle(symbols, "ntriangleright;", HtmlEntityProvider.Convert(8939));
    HtmlEntityProvider.AddSingle(symbols, "ntrianglerighteq;", HtmlEntityProvider.Convert(8941));
    HtmlEntityProvider.AddSingle(symbols, "nu;", HtmlEntityProvider.Convert(957));
    HtmlEntityProvider.AddSingle(symbols, "num;", HtmlEntityProvider.Convert(35));
    HtmlEntityProvider.AddSingle(symbols, "numero;", HtmlEntityProvider.Convert(8470));
    HtmlEntityProvider.AddSingle(symbols, "numsp;", HtmlEntityProvider.Convert(8199));
    HtmlEntityProvider.AddSingle(symbols, "nvap;", HtmlEntityProvider.Convert(8781, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nVDash;", HtmlEntityProvider.Convert(8879));
    HtmlEntityProvider.AddSingle(symbols, "nVdash;", HtmlEntityProvider.Convert(8878));
    HtmlEntityProvider.AddSingle(symbols, "nvDash;", HtmlEntityProvider.Convert(8877));
    HtmlEntityProvider.AddSingle(symbols, "nvdash;", HtmlEntityProvider.Convert(8876));
    HtmlEntityProvider.AddSingle(symbols, "nvge;", HtmlEntityProvider.Convert(8805, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nvgt;", HtmlEntityProvider.Convert(62, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nvHarr;", HtmlEntityProvider.Convert(10500));
    HtmlEntityProvider.AddSingle(symbols, "nvinfin;", HtmlEntityProvider.Convert(10718));
    HtmlEntityProvider.AddSingle(symbols, "nvlArr;", HtmlEntityProvider.Convert(10498));
    HtmlEntityProvider.AddSingle(symbols, "nvle;", HtmlEntityProvider.Convert(8804, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nvlt;", HtmlEntityProvider.Convert(60, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nvltrie;", HtmlEntityProvider.Convert(8884, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nvrArr;", HtmlEntityProvider.Convert(10499));
    HtmlEntityProvider.AddSingle(symbols, "nvrtrie;", HtmlEntityProvider.Convert(8885, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nvsim;", HtmlEntityProvider.Convert(8764, 8402));
    HtmlEntityProvider.AddSingle(symbols, "nwarhk;", HtmlEntityProvider.Convert(10531));
    HtmlEntityProvider.AddSingle(symbols, "nwArr;", HtmlEntityProvider.Convert(8662));
    HtmlEntityProvider.AddSingle(symbols, "nwarr;", HtmlEntityProvider.Convert(8598));
    HtmlEntityProvider.AddSingle(symbols, "nwarrow;", HtmlEntityProvider.Convert(8598));
    HtmlEntityProvider.AddSingle(symbols, "nwnear;", HtmlEntityProvider.Convert(10535));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigN()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Nacute;", HtmlEntityProvider.Convert(323));
    HtmlEntityProvider.AddSingle(symbols, "Ncaron;", HtmlEntityProvider.Convert(327));
    HtmlEntityProvider.AddSingle(symbols, "Ncedil;", HtmlEntityProvider.Convert(325));
    HtmlEntityProvider.AddSingle(symbols, "NegativeMediumSpace;", HtmlEntityProvider.Convert(8203));
    HtmlEntityProvider.AddSingle(symbols, "NegativeThickSpace;", HtmlEntityProvider.Convert(8203));
    HtmlEntityProvider.AddSingle(symbols, "NegativeThinSpace;", HtmlEntityProvider.Convert(8203));
    HtmlEntityProvider.AddSingle(symbols, "NegativeVeryThinSpace;", HtmlEntityProvider.Convert(8203));
    HtmlEntityProvider.AddSingle(symbols, "NestedGreaterGreater;", HtmlEntityProvider.Convert(8811));
    HtmlEntityProvider.AddSingle(symbols, "NestedLessLess;", HtmlEntityProvider.Convert(8810));
    HtmlEntityProvider.AddSingle(symbols, "Ncy;", HtmlEntityProvider.Convert(1053));
    HtmlEntityProvider.AddSingle(symbols, "Nfr;", HtmlEntityProvider.Convert(120081));
    HtmlEntityProvider.AddSingle(symbols, "NoBreak;", HtmlEntityProvider.Convert(8288));
    HtmlEntityProvider.AddSingle(symbols, "NonBreakingSpace;", HtmlEntityProvider.Convert(160 /*0xA0*/));
    HtmlEntityProvider.AddSingle(symbols, "Nopf;", HtmlEntityProvider.Convert(8469));
    HtmlEntityProvider.AddSingle(symbols, "NewLine;", HtmlEntityProvider.Convert(10));
    HtmlEntityProvider.AddSingle(symbols, "NJcy;", HtmlEntityProvider.Convert(1034));
    HtmlEntityProvider.AddSingle(symbols, "Not;", HtmlEntityProvider.Convert(10988));
    HtmlEntityProvider.AddSingle(symbols, "NotCongruent;", HtmlEntityProvider.Convert(8802));
    HtmlEntityProvider.AddSingle(symbols, "NotCupCap;", HtmlEntityProvider.Convert(8813));
    HtmlEntityProvider.AddSingle(symbols, "NotDoubleVerticalBar;", HtmlEntityProvider.Convert(8742));
    HtmlEntityProvider.AddSingle(symbols, "NotElement;", HtmlEntityProvider.Convert(8713));
    HtmlEntityProvider.AddSingle(symbols, "NotEqual;", HtmlEntityProvider.Convert(8800));
    HtmlEntityProvider.AddSingle(symbols, "NotEqualTilde;", HtmlEntityProvider.Convert(8770, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotExists;", HtmlEntityProvider.Convert(8708));
    HtmlEntityProvider.AddSingle(symbols, "NotGreater;", HtmlEntityProvider.Convert(8815));
    HtmlEntityProvider.AddSingle(symbols, "NotGreaterEqual;", HtmlEntityProvider.Convert(8817));
    HtmlEntityProvider.AddSingle(symbols, "NotGreaterFullEqual;", HtmlEntityProvider.Convert(8807, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotGreaterGreater;", HtmlEntityProvider.Convert(8811, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotGreaterLess;", HtmlEntityProvider.Convert(8825));
    HtmlEntityProvider.AddSingle(symbols, "NotGreaterSlantEqual;", HtmlEntityProvider.Convert(10878, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotGreaterTilde;", HtmlEntityProvider.Convert(8821));
    HtmlEntityProvider.AddSingle(symbols, "NotHumpDownHump;", HtmlEntityProvider.Convert(8782, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotHumpEqual;", HtmlEntityProvider.Convert(8783, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotLeftTriangle;", HtmlEntityProvider.Convert(8938));
    HtmlEntityProvider.AddSingle(symbols, "NotLeftTriangleBar;", HtmlEntityProvider.Convert(10703, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotLeftTriangleEqual;", HtmlEntityProvider.Convert(8940));
    HtmlEntityProvider.AddSingle(symbols, "NotLess;", HtmlEntityProvider.Convert(8814));
    HtmlEntityProvider.AddSingle(symbols, "NotLessEqual;", HtmlEntityProvider.Convert(8816));
    HtmlEntityProvider.AddSingle(symbols, "NotLessGreater;", HtmlEntityProvider.Convert(8824));
    HtmlEntityProvider.AddSingle(symbols, "NotLessLess;", HtmlEntityProvider.Convert(8810, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotLessSlantEqual;", HtmlEntityProvider.Convert(10877, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotLessTilde;", HtmlEntityProvider.Convert(8820));
    HtmlEntityProvider.AddSingle(symbols, "NotNestedGreaterGreater;", HtmlEntityProvider.Convert(10914, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotNestedLessLess;", HtmlEntityProvider.Convert(10913, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotPrecedes;", HtmlEntityProvider.Convert(8832));
    HtmlEntityProvider.AddSingle(symbols, "NotPrecedesEqual;", HtmlEntityProvider.Convert(10927, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotPrecedesSlantEqual;", HtmlEntityProvider.Convert(8928));
    HtmlEntityProvider.AddSingle(symbols, "NotReverseElement;", HtmlEntityProvider.Convert(8716));
    HtmlEntityProvider.AddSingle(symbols, "NotRightTriangle;", HtmlEntityProvider.Convert(8939));
    HtmlEntityProvider.AddSingle(symbols, "NotRightTriangleBar;", HtmlEntityProvider.Convert(10704, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotRightTriangleEqual;", HtmlEntityProvider.Convert(8941));
    HtmlEntityProvider.AddSingle(symbols, "NotSquareSubset;", HtmlEntityProvider.Convert(8847, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotSquareSubsetEqual;", HtmlEntityProvider.Convert(8930));
    HtmlEntityProvider.AddSingle(symbols, "NotSquareSuperset;", HtmlEntityProvider.Convert(8848, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotSquareSupersetEqual;", HtmlEntityProvider.Convert(8931));
    HtmlEntityProvider.AddSingle(symbols, "NotSubset;", HtmlEntityProvider.Convert(8834, 8402));
    HtmlEntityProvider.AddSingle(symbols, "NotSubsetEqual;", HtmlEntityProvider.Convert(8840));
    HtmlEntityProvider.AddSingle(symbols, "NotSucceeds;", HtmlEntityProvider.Convert(8833));
    HtmlEntityProvider.AddSingle(symbols, "NotSucceedsEqual;", HtmlEntityProvider.Convert(10928, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotSucceedsSlantEqual;", HtmlEntityProvider.Convert(8929));
    HtmlEntityProvider.AddSingle(symbols, "NotSucceedsTilde;", HtmlEntityProvider.Convert(8831, 824));
    HtmlEntityProvider.AddSingle(symbols, "NotSuperset;", HtmlEntityProvider.Convert(8835, 8402));
    HtmlEntityProvider.AddSingle(symbols, "NotSupersetEqual;", HtmlEntityProvider.Convert(8841));
    HtmlEntityProvider.AddSingle(symbols, "NotTilde;", HtmlEntityProvider.Convert(8769));
    HtmlEntityProvider.AddSingle(symbols, "NotTildeEqual;", HtmlEntityProvider.Convert(8772));
    HtmlEntityProvider.AddSingle(symbols, "NotTildeFullEqual;", HtmlEntityProvider.Convert(8775));
    HtmlEntityProvider.AddSingle(symbols, "NotTildeTilde;", HtmlEntityProvider.Convert(8777));
    HtmlEntityProvider.AddSingle(symbols, "NotVerticalBar;", HtmlEntityProvider.Convert(8740));
    HtmlEntityProvider.AddSingle(symbols, "Nscr;", HtmlEntityProvider.Convert(119977));
    HtmlEntityProvider.AddBoth(symbols, "Ntilde;", HtmlEntityProvider.Convert(209));
    HtmlEntityProvider.AddSingle(symbols, "Nu;", HtmlEntityProvider.Convert(925));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleO()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "oacute;", HtmlEntityProvider.Convert(243));
    HtmlEntityProvider.AddSingle(symbols, "oast;", HtmlEntityProvider.Convert(8859));
    HtmlEntityProvider.AddSingle(symbols, "ocir;", HtmlEntityProvider.Convert(8858));
    HtmlEntityProvider.AddBoth(symbols, "ocirc;", HtmlEntityProvider.Convert(244));
    HtmlEntityProvider.AddSingle(symbols, "ocy;", HtmlEntityProvider.Convert(1086));
    HtmlEntityProvider.AddSingle(symbols, "odash;", HtmlEntityProvider.Convert(8861));
    HtmlEntityProvider.AddSingle(symbols, "odblac;", HtmlEntityProvider.Convert(337));
    HtmlEntityProvider.AddSingle(symbols, "odiv;", HtmlEntityProvider.Convert(10808));
    HtmlEntityProvider.AddSingle(symbols, "odot;", HtmlEntityProvider.Convert(8857));
    HtmlEntityProvider.AddSingle(symbols, "odsold;", HtmlEntityProvider.Convert(10684));
    HtmlEntityProvider.AddSingle(symbols, "oelig;", HtmlEntityProvider.Convert(339));
    HtmlEntityProvider.AddSingle(symbols, "ofcir;", HtmlEntityProvider.Convert(10687));
    HtmlEntityProvider.AddSingle(symbols, "ofr;", HtmlEntityProvider.Convert(120108));
    HtmlEntityProvider.AddSingle(symbols, "ogon;", HtmlEntityProvider.Convert(731));
    HtmlEntityProvider.AddBoth(symbols, "ograve;", HtmlEntityProvider.Convert(242));
    HtmlEntityProvider.AddSingle(symbols, "ogt;", HtmlEntityProvider.Convert(10689));
    HtmlEntityProvider.AddSingle(symbols, "ohbar;", HtmlEntityProvider.Convert(10677));
    HtmlEntityProvider.AddSingle(symbols, "ohm;", HtmlEntityProvider.Convert(937));
    HtmlEntityProvider.AddSingle(symbols, "oint;", HtmlEntityProvider.Convert(8750));
    HtmlEntityProvider.AddSingle(symbols, "olarr;", HtmlEntityProvider.Convert(8634));
    HtmlEntityProvider.AddSingle(symbols, "olcir;", HtmlEntityProvider.Convert(10686));
    HtmlEntityProvider.AddSingle(symbols, "olcross;", HtmlEntityProvider.Convert(10683));
    HtmlEntityProvider.AddSingle(symbols, "oline;", HtmlEntityProvider.Convert(8254));
    HtmlEntityProvider.AddSingle(symbols, "olt;", HtmlEntityProvider.Convert(10688));
    HtmlEntityProvider.AddSingle(symbols, "omacr;", HtmlEntityProvider.Convert(333));
    HtmlEntityProvider.AddSingle(symbols, "omega;", HtmlEntityProvider.Convert(969));
    HtmlEntityProvider.AddSingle(symbols, "omicron;", HtmlEntityProvider.Convert(959));
    HtmlEntityProvider.AddSingle(symbols, "omid;", HtmlEntityProvider.Convert(10678));
    HtmlEntityProvider.AddSingle(symbols, "ominus;", HtmlEntityProvider.Convert(8854));
    HtmlEntityProvider.AddSingle(symbols, "oopf;", HtmlEntityProvider.Convert(120160));
    HtmlEntityProvider.AddSingle(symbols, "opar;", HtmlEntityProvider.Convert(10679));
    HtmlEntityProvider.AddSingle(symbols, "operp;", HtmlEntityProvider.Convert(10681));
    HtmlEntityProvider.AddSingle(symbols, "oplus;", HtmlEntityProvider.Convert(8853));
    HtmlEntityProvider.AddSingle(symbols, "or;", HtmlEntityProvider.Convert(8744));
    HtmlEntityProvider.AddSingle(symbols, "orarr;", HtmlEntityProvider.Convert(8635));
    HtmlEntityProvider.AddSingle(symbols, "ord;", HtmlEntityProvider.Convert(10845));
    HtmlEntityProvider.AddSingle(symbols, "order;", HtmlEntityProvider.Convert(8500));
    HtmlEntityProvider.AddSingle(symbols, "orderof;", HtmlEntityProvider.Convert(8500));
    HtmlEntityProvider.AddBoth(symbols, "ordf;", HtmlEntityProvider.Convert(170));
    HtmlEntityProvider.AddBoth(symbols, "ordm;", HtmlEntityProvider.Convert(186));
    HtmlEntityProvider.AddSingle(symbols, "origof;", HtmlEntityProvider.Convert(8886));
    HtmlEntityProvider.AddSingle(symbols, "oror;", HtmlEntityProvider.Convert(10838));
    HtmlEntityProvider.AddSingle(symbols, "orslope;", HtmlEntityProvider.Convert(10839));
    HtmlEntityProvider.AddSingle(symbols, "orv;", HtmlEntityProvider.Convert(10843));
    HtmlEntityProvider.AddSingle(symbols, "oS;", HtmlEntityProvider.Convert(9416));
    HtmlEntityProvider.AddSingle(symbols, "oscr;", HtmlEntityProvider.Convert(8500));
    HtmlEntityProvider.AddBoth(symbols, "oslash;", HtmlEntityProvider.Convert(248));
    HtmlEntityProvider.AddSingle(symbols, "osol;", HtmlEntityProvider.Convert(8856));
    HtmlEntityProvider.AddBoth(symbols, "otilde;", HtmlEntityProvider.Convert(245));
    HtmlEntityProvider.AddSingle(symbols, "otimes;", HtmlEntityProvider.Convert(8855));
    HtmlEntityProvider.AddSingle(symbols, "otimesas;", HtmlEntityProvider.Convert(10806));
    HtmlEntityProvider.AddBoth(symbols, "ouml;", HtmlEntityProvider.Convert(246));
    HtmlEntityProvider.AddSingle(symbols, "ovbar;", HtmlEntityProvider.Convert(9021));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigO()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "Oacute;", HtmlEntityProvider.Convert(211));
    HtmlEntityProvider.AddBoth(symbols, "Ocirc;", HtmlEntityProvider.Convert(212));
    HtmlEntityProvider.AddSingle(symbols, "Ocy;", HtmlEntityProvider.Convert(1054));
    HtmlEntityProvider.AddSingle(symbols, "Odblac;", HtmlEntityProvider.Convert(336));
    HtmlEntityProvider.AddSingle(symbols, "OElig;", HtmlEntityProvider.Convert(338));
    HtmlEntityProvider.AddSingle(symbols, "Ofr;", HtmlEntityProvider.Convert(120082));
    HtmlEntityProvider.AddBoth(symbols, "Ograve;", HtmlEntityProvider.Convert(210));
    HtmlEntityProvider.AddSingle(symbols, "Omacr;", HtmlEntityProvider.Convert(332));
    HtmlEntityProvider.AddSingle(symbols, "Omega;", HtmlEntityProvider.Convert(937));
    HtmlEntityProvider.AddSingle(symbols, "Omicron;", HtmlEntityProvider.Convert(927));
    HtmlEntityProvider.AddSingle(symbols, "Oopf;", HtmlEntityProvider.Convert(120134));
    HtmlEntityProvider.AddSingle(symbols, "OpenCurlyDoubleQuote;", HtmlEntityProvider.Convert(8220));
    HtmlEntityProvider.AddSingle(symbols, "OpenCurlyQuote;", HtmlEntityProvider.Convert(8216));
    HtmlEntityProvider.AddSingle(symbols, "Or;", HtmlEntityProvider.Convert(10836));
    HtmlEntityProvider.AddSingle(symbols, "Oscr;", HtmlEntityProvider.Convert(119978));
    HtmlEntityProvider.AddBoth(symbols, "Oslash;", HtmlEntityProvider.Convert(216));
    HtmlEntityProvider.AddBoth(symbols, "Otilde;", HtmlEntityProvider.Convert(213));
    HtmlEntityProvider.AddSingle(symbols, "Otimes;", HtmlEntityProvider.Convert(10807));
    HtmlEntityProvider.AddBoth(symbols, "Ouml;", HtmlEntityProvider.Convert(214));
    HtmlEntityProvider.AddSingle(symbols, "OverBar;", HtmlEntityProvider.Convert(8254));
    HtmlEntityProvider.AddSingle(symbols, "OverBrace;", HtmlEntityProvider.Convert(9182));
    HtmlEntityProvider.AddSingle(symbols, "OverBracket;", HtmlEntityProvider.Convert(9140));
    HtmlEntityProvider.AddSingle(symbols, "OverParenthesis;", HtmlEntityProvider.Convert(9180));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleP()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "pfr;", HtmlEntityProvider.Convert(120109));
    HtmlEntityProvider.AddSingle(symbols, "par;", HtmlEntityProvider.Convert(8741));
    HtmlEntityProvider.AddBoth(symbols, "para;", HtmlEntityProvider.Convert(182));
    HtmlEntityProvider.AddSingle(symbols, "parallel;", HtmlEntityProvider.Convert(8741));
    HtmlEntityProvider.AddSingle(symbols, "parsim;", HtmlEntityProvider.Convert(10995));
    HtmlEntityProvider.AddSingle(symbols, "parsl;", HtmlEntityProvider.Convert(11005));
    HtmlEntityProvider.AddSingle(symbols, "part;", HtmlEntityProvider.Convert(8706));
    HtmlEntityProvider.AddSingle(symbols, "pcy;", HtmlEntityProvider.Convert(1087));
    HtmlEntityProvider.AddSingle(symbols, "percnt;", HtmlEntityProvider.Convert(37));
    HtmlEntityProvider.AddSingle(symbols, "period;", HtmlEntityProvider.Convert(46));
    HtmlEntityProvider.AddSingle(symbols, "permil;", HtmlEntityProvider.Convert(8240));
    HtmlEntityProvider.AddSingle(symbols, "perp;", HtmlEntityProvider.Convert(8869));
    HtmlEntityProvider.AddSingle(symbols, "pertenk;", HtmlEntityProvider.Convert(8241));
    HtmlEntityProvider.AddSingle(symbols, "phi;", HtmlEntityProvider.Convert(966));
    HtmlEntityProvider.AddSingle(symbols, "phiv;", HtmlEntityProvider.Convert(981));
    HtmlEntityProvider.AddSingle(symbols, "phmmat;", HtmlEntityProvider.Convert(8499));
    HtmlEntityProvider.AddSingle(symbols, "phone;", HtmlEntityProvider.Convert(9742));
    HtmlEntityProvider.AddSingle(symbols, "pi;", HtmlEntityProvider.Convert(960));
    HtmlEntityProvider.AddSingle(symbols, "pitchfork;", HtmlEntityProvider.Convert(8916));
    HtmlEntityProvider.AddSingle(symbols, "piv;", HtmlEntityProvider.Convert(982));
    HtmlEntityProvider.AddSingle(symbols, "planck;", HtmlEntityProvider.Convert(8463));
    HtmlEntityProvider.AddSingle(symbols, "planckh;", HtmlEntityProvider.Convert(8462));
    HtmlEntityProvider.AddSingle(symbols, "plankv;", HtmlEntityProvider.Convert(8463));
    HtmlEntityProvider.AddSingle(symbols, "plus;", HtmlEntityProvider.Convert(43));
    HtmlEntityProvider.AddSingle(symbols, "plusacir;", HtmlEntityProvider.Convert(10787));
    HtmlEntityProvider.AddSingle(symbols, "plusb;", HtmlEntityProvider.Convert(8862));
    HtmlEntityProvider.AddSingle(symbols, "pluscir;", HtmlEntityProvider.Convert(10786));
    HtmlEntityProvider.AddSingle(symbols, "plusdo;", HtmlEntityProvider.Convert(8724));
    HtmlEntityProvider.AddSingle(symbols, "plusdu;", HtmlEntityProvider.Convert(10789));
    HtmlEntityProvider.AddSingle(symbols, "pluse;", HtmlEntityProvider.Convert(10866));
    HtmlEntityProvider.AddBoth(symbols, "plusmn;", HtmlEntityProvider.Convert(177));
    HtmlEntityProvider.AddSingle(symbols, "plussim;", HtmlEntityProvider.Convert(10790));
    HtmlEntityProvider.AddSingle(symbols, "plustwo;", HtmlEntityProvider.Convert(10791));
    HtmlEntityProvider.AddSingle(symbols, "pm;", HtmlEntityProvider.Convert(177));
    HtmlEntityProvider.AddSingle(symbols, "pointint;", HtmlEntityProvider.Convert(10773));
    HtmlEntityProvider.AddSingle(symbols, "popf;", HtmlEntityProvider.Convert(120161));
    HtmlEntityProvider.AddBoth(symbols, "pound;", HtmlEntityProvider.Convert(163));
    HtmlEntityProvider.AddSingle(symbols, "pr;", HtmlEntityProvider.Convert(8826));
    HtmlEntityProvider.AddSingle(symbols, "prap;", HtmlEntityProvider.Convert(10935));
    HtmlEntityProvider.AddSingle(symbols, "prcue;", HtmlEntityProvider.Convert(8828));
    HtmlEntityProvider.AddSingle(symbols, "prE;", HtmlEntityProvider.Convert(10931));
    HtmlEntityProvider.AddSingle(symbols, "pre;", HtmlEntityProvider.Convert(10927));
    HtmlEntityProvider.AddSingle(symbols, "prec;", HtmlEntityProvider.Convert(8826));
    HtmlEntityProvider.AddSingle(symbols, "precapprox;", HtmlEntityProvider.Convert(10935));
    HtmlEntityProvider.AddSingle(symbols, "preccurlyeq;", HtmlEntityProvider.Convert(8828));
    HtmlEntityProvider.AddSingle(symbols, "preceq;", HtmlEntityProvider.Convert(10927));
    HtmlEntityProvider.AddSingle(symbols, "precnapprox;", HtmlEntityProvider.Convert(10937));
    HtmlEntityProvider.AddSingle(symbols, "precneqq;", HtmlEntityProvider.Convert(10933));
    HtmlEntityProvider.AddSingle(symbols, "precnsim;", HtmlEntityProvider.Convert(8936));
    HtmlEntityProvider.AddSingle(symbols, "precsim;", HtmlEntityProvider.Convert(8830));
    HtmlEntityProvider.AddSingle(symbols, "prime;", HtmlEntityProvider.Convert(8242));
    HtmlEntityProvider.AddSingle(symbols, "primes;", HtmlEntityProvider.Convert(8473));
    HtmlEntityProvider.AddSingle(symbols, "prnap;", HtmlEntityProvider.Convert(10937));
    HtmlEntityProvider.AddSingle(symbols, "prnE;", HtmlEntityProvider.Convert(10933));
    HtmlEntityProvider.AddSingle(symbols, "prnsim;", HtmlEntityProvider.Convert(8936));
    HtmlEntityProvider.AddSingle(symbols, "prod;", HtmlEntityProvider.Convert(8719));
    HtmlEntityProvider.AddSingle(symbols, "profalar;", HtmlEntityProvider.Convert(9006));
    HtmlEntityProvider.AddSingle(symbols, "profline;", HtmlEntityProvider.Convert(8978));
    HtmlEntityProvider.AddSingle(symbols, "profsurf;", HtmlEntityProvider.Convert(8979));
    HtmlEntityProvider.AddSingle(symbols, "prop;", HtmlEntityProvider.Convert(8733));
    HtmlEntityProvider.AddSingle(symbols, "propto;", HtmlEntityProvider.Convert(8733));
    HtmlEntityProvider.AddSingle(symbols, "prsim;", HtmlEntityProvider.Convert(8830));
    HtmlEntityProvider.AddSingle(symbols, "prurel;", HtmlEntityProvider.Convert(8880));
    HtmlEntityProvider.AddSingle(symbols, "pscr;", HtmlEntityProvider.Convert(120005));
    HtmlEntityProvider.AddSingle(symbols, "psi;", HtmlEntityProvider.Convert(968));
    HtmlEntityProvider.AddSingle(symbols, "puncsp;", HtmlEntityProvider.Convert(8200));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigP()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "PartialD;", HtmlEntityProvider.Convert(8706));
    HtmlEntityProvider.AddSingle(symbols, "Pcy;", HtmlEntityProvider.Convert(1055));
    HtmlEntityProvider.AddSingle(symbols, "Pfr;", HtmlEntityProvider.Convert(120083));
    HtmlEntityProvider.AddSingle(symbols, "Phi;", HtmlEntityProvider.Convert(934));
    HtmlEntityProvider.AddSingle(symbols, "Pi;", HtmlEntityProvider.Convert(928));
    HtmlEntityProvider.AddSingle(symbols, "PlusMinus;", HtmlEntityProvider.Convert(177));
    HtmlEntityProvider.AddSingle(symbols, "Poincareplane;", HtmlEntityProvider.Convert(8460));
    HtmlEntityProvider.AddSingle(symbols, "Popf;", HtmlEntityProvider.Convert(8473));
    HtmlEntityProvider.AddSingle(symbols, "Pr;", HtmlEntityProvider.Convert(10939));
    HtmlEntityProvider.AddSingle(symbols, "Precedes;", HtmlEntityProvider.Convert(8826));
    HtmlEntityProvider.AddSingle(symbols, "PrecedesEqual;", HtmlEntityProvider.Convert(10927));
    HtmlEntityProvider.AddSingle(symbols, "PrecedesSlantEqual;", HtmlEntityProvider.Convert(8828));
    HtmlEntityProvider.AddSingle(symbols, "PrecedesTilde;", HtmlEntityProvider.Convert(8830));
    HtmlEntityProvider.AddSingle(symbols, "Prime;", HtmlEntityProvider.Convert(8243));
    HtmlEntityProvider.AddSingle(symbols, "Product;", HtmlEntityProvider.Convert(8719));
    HtmlEntityProvider.AddSingle(symbols, "Proportion;", HtmlEntityProvider.Convert(8759));
    HtmlEntityProvider.AddSingle(symbols, "Proportional;", HtmlEntityProvider.Convert(8733));
    HtmlEntityProvider.AddSingle(symbols, "Pscr;", HtmlEntityProvider.Convert(119979));
    HtmlEntityProvider.AddSingle(symbols, "Psi;", HtmlEntityProvider.Convert(936));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleQ()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "qfr;", HtmlEntityProvider.Convert(120110));
    HtmlEntityProvider.AddSingle(symbols, "qint;", HtmlEntityProvider.Convert(10764));
    HtmlEntityProvider.AddSingle(symbols, "qopf;", HtmlEntityProvider.Convert(120162));
    HtmlEntityProvider.AddSingle(symbols, "qprime;", HtmlEntityProvider.Convert(8279));
    HtmlEntityProvider.AddSingle(symbols, "qscr;", HtmlEntityProvider.Convert(120006));
    HtmlEntityProvider.AddSingle(symbols, "quaternions;", HtmlEntityProvider.Convert(8461));
    HtmlEntityProvider.AddSingle(symbols, "quatint;", HtmlEntityProvider.Convert(10774));
    HtmlEntityProvider.AddSingle(symbols, "quest;", HtmlEntityProvider.Convert(63 /*0x3F*/));
    HtmlEntityProvider.AddSingle(symbols, "questeq;", HtmlEntityProvider.Convert(8799));
    HtmlEntityProvider.AddBoth(symbols, "quot;", HtmlEntityProvider.Convert(34));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigQ()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Qfr;", HtmlEntityProvider.Convert(120084));
    HtmlEntityProvider.AddSingle(symbols, "Qopf;", HtmlEntityProvider.Convert(8474));
    HtmlEntityProvider.AddSingle(symbols, "Qscr;", HtmlEntityProvider.Convert(119980));
    HtmlEntityProvider.AddBoth(symbols, "QUOT;", HtmlEntityProvider.Convert(34));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleR()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "rAarr;", HtmlEntityProvider.Convert(8667));
    HtmlEntityProvider.AddSingle(symbols, "race;", HtmlEntityProvider.Convert(8765, 817));
    HtmlEntityProvider.AddSingle(symbols, "racute;", HtmlEntityProvider.Convert(341));
    HtmlEntityProvider.AddSingle(symbols, "radic;", HtmlEntityProvider.Convert(8730));
    HtmlEntityProvider.AddSingle(symbols, "raemptyv;", HtmlEntityProvider.Convert(10675));
    HtmlEntityProvider.AddSingle(symbols, "rang;", HtmlEntityProvider.Convert(10217));
    HtmlEntityProvider.AddSingle(symbols, "rangd;", HtmlEntityProvider.Convert(10642));
    HtmlEntityProvider.AddSingle(symbols, "range;", HtmlEntityProvider.Convert(10661));
    HtmlEntityProvider.AddSingle(symbols, "rangle;", HtmlEntityProvider.Convert(10217));
    HtmlEntityProvider.AddBoth(symbols, "raquo;", HtmlEntityProvider.Convert(187));
    HtmlEntityProvider.AddSingle(symbols, "rArr;", HtmlEntityProvider.Convert(8658));
    HtmlEntityProvider.AddSingle(symbols, "rarr;", HtmlEntityProvider.Convert(8594));
    HtmlEntityProvider.AddSingle(symbols, "rarrap;", HtmlEntityProvider.Convert(10613));
    HtmlEntityProvider.AddSingle(symbols, "rarrb;", HtmlEntityProvider.Convert(8677));
    HtmlEntityProvider.AddSingle(symbols, "rarrbfs;", HtmlEntityProvider.Convert(10528));
    HtmlEntityProvider.AddSingle(symbols, "rarrc;", HtmlEntityProvider.Convert(10547));
    HtmlEntityProvider.AddSingle(symbols, "rarrfs;", HtmlEntityProvider.Convert(10526));
    HtmlEntityProvider.AddSingle(symbols, "rarrhk;", HtmlEntityProvider.Convert(8618));
    HtmlEntityProvider.AddSingle(symbols, "rarrlp;", HtmlEntityProvider.Convert(8620));
    HtmlEntityProvider.AddSingle(symbols, "rarrpl;", HtmlEntityProvider.Convert(10565));
    HtmlEntityProvider.AddSingle(symbols, "rarrsim;", HtmlEntityProvider.Convert(10612));
    HtmlEntityProvider.AddSingle(symbols, "rarrtl;", HtmlEntityProvider.Convert(8611));
    HtmlEntityProvider.AddSingle(symbols, "rarrw;", HtmlEntityProvider.Convert(8605));
    HtmlEntityProvider.AddSingle(symbols, "rAtail;", HtmlEntityProvider.Convert(10524));
    HtmlEntityProvider.AddSingle(symbols, "ratail;", HtmlEntityProvider.Convert(10522));
    HtmlEntityProvider.AddSingle(symbols, "ratio;", HtmlEntityProvider.Convert(8758));
    HtmlEntityProvider.AddSingle(symbols, "rationals;", HtmlEntityProvider.Convert(8474));
    HtmlEntityProvider.AddSingle(symbols, "rBarr;", HtmlEntityProvider.Convert(10511));
    HtmlEntityProvider.AddSingle(symbols, "rbarr;", HtmlEntityProvider.Convert(10509));
    HtmlEntityProvider.AddSingle(symbols, "rbbrk;", HtmlEntityProvider.Convert(10099));
    HtmlEntityProvider.AddSingle(symbols, "rbrace;", HtmlEntityProvider.Convert(125));
    HtmlEntityProvider.AddSingle(symbols, "rbrack;", HtmlEntityProvider.Convert(93));
    HtmlEntityProvider.AddSingle(symbols, "rbrke;", HtmlEntityProvider.Convert(10636));
    HtmlEntityProvider.AddSingle(symbols, "rbrksld;", HtmlEntityProvider.Convert(10638));
    HtmlEntityProvider.AddSingle(symbols, "rbrkslu;", HtmlEntityProvider.Convert(10640));
    HtmlEntityProvider.AddSingle(symbols, "rcaron;", HtmlEntityProvider.Convert(345));
    HtmlEntityProvider.AddSingle(symbols, "rcedil;", HtmlEntityProvider.Convert(343));
    HtmlEntityProvider.AddSingle(symbols, "rceil;", HtmlEntityProvider.Convert(8969));
    HtmlEntityProvider.AddSingle(symbols, "rcub;", HtmlEntityProvider.Convert(125));
    HtmlEntityProvider.AddSingle(symbols, "rcy;", HtmlEntityProvider.Convert(1088));
    HtmlEntityProvider.AddSingle(symbols, "rdca;", HtmlEntityProvider.Convert(10551));
    HtmlEntityProvider.AddSingle(symbols, "rdldhar;", HtmlEntityProvider.Convert(10601));
    HtmlEntityProvider.AddSingle(symbols, "rdquo;", HtmlEntityProvider.Convert(8221));
    HtmlEntityProvider.AddSingle(symbols, "rdquor;", HtmlEntityProvider.Convert(8221));
    HtmlEntityProvider.AddSingle(symbols, "rdsh;", HtmlEntityProvider.Convert(8627));
    HtmlEntityProvider.AddSingle(symbols, "real;", HtmlEntityProvider.Convert(8476));
    HtmlEntityProvider.AddSingle(symbols, "realine;", HtmlEntityProvider.Convert(8475));
    HtmlEntityProvider.AddSingle(symbols, "realpart;", HtmlEntityProvider.Convert(8476));
    HtmlEntityProvider.AddSingle(symbols, "reals;", HtmlEntityProvider.Convert(8477));
    HtmlEntityProvider.AddSingle(symbols, "rect;", HtmlEntityProvider.Convert(9645));
    HtmlEntityProvider.AddBoth(symbols, "reg;", HtmlEntityProvider.Convert(174));
    HtmlEntityProvider.AddSingle(symbols, "rfisht;", HtmlEntityProvider.Convert(10621));
    HtmlEntityProvider.AddSingle(symbols, "rfloor;", HtmlEntityProvider.Convert(8971));
    HtmlEntityProvider.AddSingle(symbols, "rfr;", HtmlEntityProvider.Convert(120111));
    HtmlEntityProvider.AddSingle(symbols, "rHar;", HtmlEntityProvider.Convert(10596));
    HtmlEntityProvider.AddSingle(symbols, "rhard;", HtmlEntityProvider.Convert(8641));
    HtmlEntityProvider.AddSingle(symbols, "rharu;", HtmlEntityProvider.Convert(8640));
    HtmlEntityProvider.AddSingle(symbols, "rharul;", HtmlEntityProvider.Convert(10604));
    HtmlEntityProvider.AddSingle(symbols, "rho;", HtmlEntityProvider.Convert(961));
    HtmlEntityProvider.AddSingle(symbols, "rhov;", HtmlEntityProvider.Convert(1009));
    HtmlEntityProvider.AddSingle(symbols, "rightarrow;", HtmlEntityProvider.Convert(8594));
    HtmlEntityProvider.AddSingle(symbols, "rightarrowtail;", HtmlEntityProvider.Convert(8611));
    HtmlEntityProvider.AddSingle(symbols, "rightharpoondown;", HtmlEntityProvider.Convert(8641));
    HtmlEntityProvider.AddSingle(symbols, "rightharpoonup;", HtmlEntityProvider.Convert(8640));
    HtmlEntityProvider.AddSingle(symbols, "rightleftarrows;", HtmlEntityProvider.Convert(8644));
    HtmlEntityProvider.AddSingle(symbols, "rightleftharpoons;", HtmlEntityProvider.Convert(8652));
    HtmlEntityProvider.AddSingle(symbols, "rightrightarrows;", HtmlEntityProvider.Convert(8649));
    HtmlEntityProvider.AddSingle(symbols, "rightsquigarrow;", HtmlEntityProvider.Convert(8605));
    HtmlEntityProvider.AddSingle(symbols, "rightthreetimes;", HtmlEntityProvider.Convert(8908));
    HtmlEntityProvider.AddSingle(symbols, "ring;", HtmlEntityProvider.Convert(730));
    HtmlEntityProvider.AddSingle(symbols, "risingdotseq;", HtmlEntityProvider.Convert(8787));
    HtmlEntityProvider.AddSingle(symbols, "rlarr;", HtmlEntityProvider.Convert(8644));
    HtmlEntityProvider.AddSingle(symbols, "rlhar;", HtmlEntityProvider.Convert(8652));
    HtmlEntityProvider.AddSingle(symbols, "rlm;", HtmlEntityProvider.Convert(8207));
    HtmlEntityProvider.AddSingle(symbols, "rmoust;", HtmlEntityProvider.Convert(9137));
    HtmlEntityProvider.AddSingle(symbols, "rmoustache;", HtmlEntityProvider.Convert(9137));
    HtmlEntityProvider.AddSingle(symbols, "rnmid;", HtmlEntityProvider.Convert(10990));
    HtmlEntityProvider.AddSingle(symbols, "roang;", HtmlEntityProvider.Convert(10221));
    HtmlEntityProvider.AddSingle(symbols, "roarr;", HtmlEntityProvider.Convert(8702));
    HtmlEntityProvider.AddSingle(symbols, "robrk;", HtmlEntityProvider.Convert(10215));
    HtmlEntityProvider.AddSingle(symbols, "ropar;", HtmlEntityProvider.Convert(10630));
    HtmlEntityProvider.AddSingle(symbols, "ropf;", HtmlEntityProvider.Convert(120163));
    HtmlEntityProvider.AddSingle(symbols, "roplus;", HtmlEntityProvider.Convert(10798));
    HtmlEntityProvider.AddSingle(symbols, "rotimes;", HtmlEntityProvider.Convert(10805));
    HtmlEntityProvider.AddSingle(symbols, "rpar;", HtmlEntityProvider.Convert(41));
    HtmlEntityProvider.AddSingle(symbols, "rpargt;", HtmlEntityProvider.Convert(10644));
    HtmlEntityProvider.AddSingle(symbols, "rppolint;", HtmlEntityProvider.Convert(10770));
    HtmlEntityProvider.AddSingle(symbols, "rrarr;", HtmlEntityProvider.Convert(8649));
    HtmlEntityProvider.AddSingle(symbols, "rsaquo;", HtmlEntityProvider.Convert(8250));
    HtmlEntityProvider.AddSingle(symbols, "rscr;", HtmlEntityProvider.Convert(120007));
    HtmlEntityProvider.AddSingle(symbols, "rsh;", HtmlEntityProvider.Convert(8625));
    HtmlEntityProvider.AddSingle(symbols, "rsqb;", HtmlEntityProvider.Convert(93));
    HtmlEntityProvider.AddSingle(symbols, "rsquo;", HtmlEntityProvider.Convert(8217));
    HtmlEntityProvider.AddSingle(symbols, "rsquor;", HtmlEntityProvider.Convert(8217));
    HtmlEntityProvider.AddSingle(symbols, "rthree;", HtmlEntityProvider.Convert(8908));
    HtmlEntityProvider.AddSingle(symbols, "rtimes;", HtmlEntityProvider.Convert(8906));
    HtmlEntityProvider.AddSingle(symbols, "rtri;", HtmlEntityProvider.Convert(9657));
    HtmlEntityProvider.AddSingle(symbols, "rtrie;", HtmlEntityProvider.Convert(8885));
    HtmlEntityProvider.AddSingle(symbols, "rtrif;", HtmlEntityProvider.Convert(9656));
    HtmlEntityProvider.AddSingle(symbols, "rtriltri;", HtmlEntityProvider.Convert(10702));
    HtmlEntityProvider.AddSingle(symbols, "ruluhar;", HtmlEntityProvider.Convert(10600));
    HtmlEntityProvider.AddSingle(symbols, "rx;", HtmlEntityProvider.Convert(8478));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigR()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Racute;", HtmlEntityProvider.Convert(340));
    HtmlEntityProvider.AddSingle(symbols, "Rang;", HtmlEntityProvider.Convert(10219));
    HtmlEntityProvider.AddSingle(symbols, "Rarr;", HtmlEntityProvider.Convert(8608));
    HtmlEntityProvider.AddSingle(symbols, "Rarrtl;", HtmlEntityProvider.Convert(10518));
    HtmlEntityProvider.AddSingle(symbols, "RBarr;", HtmlEntityProvider.Convert(10512));
    HtmlEntityProvider.AddSingle(symbols, "Rcaron;", HtmlEntityProvider.Convert(344));
    HtmlEntityProvider.AddSingle(symbols, "Rcedil;", HtmlEntityProvider.Convert(342));
    HtmlEntityProvider.AddSingle(symbols, "Rcy;", HtmlEntityProvider.Convert(1056));
    HtmlEntityProvider.AddSingle(symbols, "Re;", HtmlEntityProvider.Convert(8476));
    HtmlEntityProvider.AddBoth(symbols, "REG;", HtmlEntityProvider.Convert(174));
    HtmlEntityProvider.AddSingle(symbols, "ReverseElement;", HtmlEntityProvider.Convert(8715));
    HtmlEntityProvider.AddSingle(symbols, "ReverseEquilibrium;", HtmlEntityProvider.Convert(8651));
    HtmlEntityProvider.AddSingle(symbols, "ReverseUpEquilibrium;", HtmlEntityProvider.Convert(10607));
    HtmlEntityProvider.AddSingle(symbols, "Rfr;", HtmlEntityProvider.Convert(8476));
    HtmlEntityProvider.AddSingle(symbols, "Rho;", HtmlEntityProvider.Convert(929));
    HtmlEntityProvider.AddSingle(symbols, "RightAngleBracket;", HtmlEntityProvider.Convert(10217));
    HtmlEntityProvider.AddSingle(symbols, "RightArrow;", HtmlEntityProvider.Convert(8594));
    HtmlEntityProvider.AddSingle(symbols, "Rightarrow;", HtmlEntityProvider.Convert(8658));
    HtmlEntityProvider.AddSingle(symbols, "RightArrowBar;", HtmlEntityProvider.Convert(8677));
    HtmlEntityProvider.AddSingle(symbols, "RightArrowLeftArrow;", HtmlEntityProvider.Convert(8644));
    HtmlEntityProvider.AddSingle(symbols, "RightCeiling;", HtmlEntityProvider.Convert(8969));
    HtmlEntityProvider.AddSingle(symbols, "RightDoubleBracket;", HtmlEntityProvider.Convert(10215));
    HtmlEntityProvider.AddSingle(symbols, "RightDownTeeVector;", HtmlEntityProvider.Convert(10589));
    HtmlEntityProvider.AddSingle(symbols, "RightDownVector;", HtmlEntityProvider.Convert(8642));
    HtmlEntityProvider.AddSingle(symbols, "RightDownVectorBar;", HtmlEntityProvider.Convert(10581));
    HtmlEntityProvider.AddSingle(symbols, "RightFloor;", HtmlEntityProvider.Convert(8971));
    HtmlEntityProvider.AddSingle(symbols, "RightTee;", HtmlEntityProvider.Convert(8866));
    HtmlEntityProvider.AddSingle(symbols, "RightTeeArrow;", HtmlEntityProvider.Convert(8614));
    HtmlEntityProvider.AddSingle(symbols, "RightTeeVector;", HtmlEntityProvider.Convert(10587));
    HtmlEntityProvider.AddSingle(symbols, "RightTriangle;", HtmlEntityProvider.Convert(8883));
    HtmlEntityProvider.AddSingle(symbols, "RightTriangleBar;", HtmlEntityProvider.Convert(10704));
    HtmlEntityProvider.AddSingle(symbols, "RightTriangleEqual;", HtmlEntityProvider.Convert(8885));
    HtmlEntityProvider.AddSingle(symbols, "RightUpDownVector;", HtmlEntityProvider.Convert(10575));
    HtmlEntityProvider.AddSingle(symbols, "RightUpTeeVector;", HtmlEntityProvider.Convert(10588));
    HtmlEntityProvider.AddSingle(symbols, "RightUpVector;", HtmlEntityProvider.Convert(8638));
    HtmlEntityProvider.AddSingle(symbols, "RightUpVectorBar;", HtmlEntityProvider.Convert(10580));
    HtmlEntityProvider.AddSingle(symbols, "RightVector;", HtmlEntityProvider.Convert(8640));
    HtmlEntityProvider.AddSingle(symbols, "RightVectorBar;", HtmlEntityProvider.Convert(10579));
    HtmlEntityProvider.AddSingle(symbols, "Ropf;", HtmlEntityProvider.Convert(8477));
    HtmlEntityProvider.AddSingle(symbols, "RoundImplies;", HtmlEntityProvider.Convert(10608));
    HtmlEntityProvider.AddSingle(symbols, "Rrightarrow;", HtmlEntityProvider.Convert(8667));
    HtmlEntityProvider.AddSingle(symbols, "Rscr;", HtmlEntityProvider.Convert(8475));
    HtmlEntityProvider.AddSingle(symbols, "Rsh;", HtmlEntityProvider.Convert(8625));
    HtmlEntityProvider.AddSingle(symbols, "RuleDelayed;", HtmlEntityProvider.Convert(10740));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleS()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "sacute;", HtmlEntityProvider.Convert(347));
    HtmlEntityProvider.AddSingle(symbols, "sbquo;", HtmlEntityProvider.Convert(8218));
    HtmlEntityProvider.AddSingle(symbols, "sc;", HtmlEntityProvider.Convert(8827));
    HtmlEntityProvider.AddSingle(symbols, "scap;", HtmlEntityProvider.Convert(10936));
    HtmlEntityProvider.AddSingle(symbols, "scaron;", HtmlEntityProvider.Convert(353));
    HtmlEntityProvider.AddSingle(symbols, "sccue;", HtmlEntityProvider.Convert(8829));
    HtmlEntityProvider.AddSingle(symbols, "scE;", HtmlEntityProvider.Convert(10932));
    HtmlEntityProvider.AddSingle(symbols, "sce;", HtmlEntityProvider.Convert(10928));
    HtmlEntityProvider.AddSingle(symbols, "scedil;", HtmlEntityProvider.Convert(351));
    HtmlEntityProvider.AddSingle(symbols, "scirc;", HtmlEntityProvider.Convert(349));
    HtmlEntityProvider.AddSingle(symbols, "scnap;", HtmlEntityProvider.Convert(10938));
    HtmlEntityProvider.AddSingle(symbols, "scnE;", HtmlEntityProvider.Convert(10934));
    HtmlEntityProvider.AddSingle(symbols, "scnsim;", HtmlEntityProvider.Convert(8937));
    HtmlEntityProvider.AddSingle(symbols, "scpolint;", HtmlEntityProvider.Convert(10771));
    HtmlEntityProvider.AddSingle(symbols, "scsim;", HtmlEntityProvider.Convert(8831));
    HtmlEntityProvider.AddSingle(symbols, "scy;", HtmlEntityProvider.Convert(1089));
    HtmlEntityProvider.AddSingle(symbols, "sdot;", HtmlEntityProvider.Convert(8901));
    HtmlEntityProvider.AddSingle(symbols, "sdotb;", HtmlEntityProvider.Convert(8865));
    HtmlEntityProvider.AddSingle(symbols, "sdote;", HtmlEntityProvider.Convert(10854));
    HtmlEntityProvider.AddSingle(symbols, "searhk;", HtmlEntityProvider.Convert(10533));
    HtmlEntityProvider.AddSingle(symbols, "seArr;", HtmlEntityProvider.Convert(8664));
    HtmlEntityProvider.AddSingle(symbols, "searr;", HtmlEntityProvider.Convert(8600));
    HtmlEntityProvider.AddSingle(symbols, "searrow;", HtmlEntityProvider.Convert(8600));
    HtmlEntityProvider.AddBoth(symbols, "sect;", HtmlEntityProvider.Convert(167));
    HtmlEntityProvider.AddSingle(symbols, "semi;", HtmlEntityProvider.Convert(59));
    HtmlEntityProvider.AddSingle(symbols, "seswar;", HtmlEntityProvider.Convert(10537));
    HtmlEntityProvider.AddSingle(symbols, "setminus;", HtmlEntityProvider.Convert(8726));
    HtmlEntityProvider.AddSingle(symbols, "setmn;", HtmlEntityProvider.Convert(8726));
    HtmlEntityProvider.AddSingle(symbols, "sext;", HtmlEntityProvider.Convert(10038));
    HtmlEntityProvider.AddSingle(symbols, "sfr;", HtmlEntityProvider.Convert(120112));
    HtmlEntityProvider.AddSingle(symbols, "sfrown;", HtmlEntityProvider.Convert(8994));
    HtmlEntityProvider.AddSingle(symbols, "sharp;", HtmlEntityProvider.Convert(9839));
    HtmlEntityProvider.AddSingle(symbols, "shchcy;", HtmlEntityProvider.Convert(1097));
    HtmlEntityProvider.AddSingle(symbols, "shcy;", HtmlEntityProvider.Convert(1096));
    HtmlEntityProvider.AddSingle(symbols, "shortmid;", HtmlEntityProvider.Convert(8739));
    HtmlEntityProvider.AddSingle(symbols, "shortparallel;", HtmlEntityProvider.Convert(8741));
    HtmlEntityProvider.AddBoth(symbols, "shy;", HtmlEntityProvider.Convert(173));
    HtmlEntityProvider.AddSingle(symbols, "sigma;", HtmlEntityProvider.Convert(963));
    HtmlEntityProvider.AddSingle(symbols, "sigmaf;", HtmlEntityProvider.Convert(962));
    HtmlEntityProvider.AddSingle(symbols, "sigmav;", HtmlEntityProvider.Convert(962));
    HtmlEntityProvider.AddSingle(symbols, "sim;", HtmlEntityProvider.Convert(8764));
    HtmlEntityProvider.AddSingle(symbols, "simdot;", HtmlEntityProvider.Convert(10858));
    HtmlEntityProvider.AddSingle(symbols, "sime;", HtmlEntityProvider.Convert(8771));
    HtmlEntityProvider.AddSingle(symbols, "simeq;", HtmlEntityProvider.Convert(8771));
    HtmlEntityProvider.AddSingle(symbols, "simg;", HtmlEntityProvider.Convert(10910));
    HtmlEntityProvider.AddSingle(symbols, "simgE;", HtmlEntityProvider.Convert(10912));
    HtmlEntityProvider.AddSingle(symbols, "siml;", HtmlEntityProvider.Convert(10909));
    HtmlEntityProvider.AddSingle(symbols, "simlE;", HtmlEntityProvider.Convert(10911));
    HtmlEntityProvider.AddSingle(symbols, "simne;", HtmlEntityProvider.Convert(8774));
    HtmlEntityProvider.AddSingle(symbols, "simplus;", HtmlEntityProvider.Convert(10788));
    HtmlEntityProvider.AddSingle(symbols, "simrarr;", HtmlEntityProvider.Convert(10610));
    HtmlEntityProvider.AddSingle(symbols, "slarr;", HtmlEntityProvider.Convert(8592));
    HtmlEntityProvider.AddSingle(symbols, "smallsetminus;", HtmlEntityProvider.Convert(8726));
    HtmlEntityProvider.AddSingle(symbols, "smashp;", HtmlEntityProvider.Convert(10803));
    HtmlEntityProvider.AddSingle(symbols, "smeparsl;", HtmlEntityProvider.Convert(10724));
    HtmlEntityProvider.AddSingle(symbols, "smid;", HtmlEntityProvider.Convert(8739));
    HtmlEntityProvider.AddSingle(symbols, "smile;", HtmlEntityProvider.Convert(8995));
    HtmlEntityProvider.AddSingle(symbols, "smt;", HtmlEntityProvider.Convert(10922));
    HtmlEntityProvider.AddSingle(symbols, "smte;", HtmlEntityProvider.Convert(10924));
    HtmlEntityProvider.AddSingle(symbols, "smtes;", HtmlEntityProvider.Convert(10924, 65024));
    HtmlEntityProvider.AddSingle(symbols, "softcy;", HtmlEntityProvider.Convert(1100));
    HtmlEntityProvider.AddSingle(symbols, "sol;", HtmlEntityProvider.Convert(47));
    HtmlEntityProvider.AddSingle(symbols, "solb;", HtmlEntityProvider.Convert(10692));
    HtmlEntityProvider.AddSingle(symbols, "solbar;", HtmlEntityProvider.Convert(9023));
    HtmlEntityProvider.AddSingle(symbols, "sopf;", HtmlEntityProvider.Convert(120164));
    HtmlEntityProvider.AddSingle(symbols, "spades;", HtmlEntityProvider.Convert(9824));
    HtmlEntityProvider.AddSingle(symbols, "spadesuit;", HtmlEntityProvider.Convert(9824));
    HtmlEntityProvider.AddSingle(symbols, "spar;", HtmlEntityProvider.Convert(8741));
    HtmlEntityProvider.AddSingle(symbols, "sqcap;", HtmlEntityProvider.Convert(8851));
    HtmlEntityProvider.AddSingle(symbols, "sqcaps;", HtmlEntityProvider.Convert(8851, 65024));
    HtmlEntityProvider.AddSingle(symbols, "sqcup;", HtmlEntityProvider.Convert(8852));
    HtmlEntityProvider.AddSingle(symbols, "sqcups;", HtmlEntityProvider.Convert(8852, 65024));
    HtmlEntityProvider.AddSingle(symbols, "sqsub;", HtmlEntityProvider.Convert(8847));
    HtmlEntityProvider.AddSingle(symbols, "sqsube;", HtmlEntityProvider.Convert(8849));
    HtmlEntityProvider.AddSingle(symbols, "sqsubset;", HtmlEntityProvider.Convert(8847));
    HtmlEntityProvider.AddSingle(symbols, "sqsubseteq;", HtmlEntityProvider.Convert(8849));
    HtmlEntityProvider.AddSingle(symbols, "sqsup;", HtmlEntityProvider.Convert(8848));
    HtmlEntityProvider.AddSingle(symbols, "sqsupe;", HtmlEntityProvider.Convert(8850));
    HtmlEntityProvider.AddSingle(symbols, "sqsupset;", HtmlEntityProvider.Convert(8848));
    HtmlEntityProvider.AddSingle(symbols, "sqsupseteq;", HtmlEntityProvider.Convert(8850));
    HtmlEntityProvider.AddSingle(symbols, "squ;", HtmlEntityProvider.Convert(9633));
    HtmlEntityProvider.AddSingle(symbols, "square;", HtmlEntityProvider.Convert(9633));
    HtmlEntityProvider.AddSingle(symbols, "squarf;", HtmlEntityProvider.Convert(9642));
    HtmlEntityProvider.AddSingle(symbols, "squf;", HtmlEntityProvider.Convert(9642));
    HtmlEntityProvider.AddSingle(symbols, "srarr;", HtmlEntityProvider.Convert(8594));
    HtmlEntityProvider.AddSingle(symbols, "sscr;", HtmlEntityProvider.Convert(120008));
    HtmlEntityProvider.AddSingle(symbols, "ssetmn;", HtmlEntityProvider.Convert(8726));
    HtmlEntityProvider.AddSingle(symbols, "ssmile;", HtmlEntityProvider.Convert(8995));
    HtmlEntityProvider.AddSingle(symbols, "sstarf;", HtmlEntityProvider.Convert(8902));
    HtmlEntityProvider.AddSingle(symbols, "star;", HtmlEntityProvider.Convert(9734));
    HtmlEntityProvider.AddSingle(symbols, "starf;", HtmlEntityProvider.Convert(9733));
    HtmlEntityProvider.AddSingle(symbols, "straightepsilon;", HtmlEntityProvider.Convert(1013));
    HtmlEntityProvider.AddSingle(symbols, "straightphi;", HtmlEntityProvider.Convert(981));
    HtmlEntityProvider.AddSingle(symbols, "strns;", HtmlEntityProvider.Convert(175));
    HtmlEntityProvider.AddSingle(symbols, "sub;", HtmlEntityProvider.Convert(8834));
    HtmlEntityProvider.AddSingle(symbols, "subdot;", HtmlEntityProvider.Convert(10941));
    HtmlEntityProvider.AddSingle(symbols, "subE;", HtmlEntityProvider.Convert(10949));
    HtmlEntityProvider.AddSingle(symbols, "sube;", HtmlEntityProvider.Convert(8838));
    HtmlEntityProvider.AddSingle(symbols, "subedot;", HtmlEntityProvider.Convert(10947));
    HtmlEntityProvider.AddSingle(symbols, "submult;", HtmlEntityProvider.Convert(10945));
    HtmlEntityProvider.AddSingle(symbols, "subnE;", HtmlEntityProvider.Convert(10955));
    HtmlEntityProvider.AddSingle(symbols, "subne;", HtmlEntityProvider.Convert(8842));
    HtmlEntityProvider.AddSingle(symbols, "subplus;", HtmlEntityProvider.Convert(10943));
    HtmlEntityProvider.AddSingle(symbols, "subrarr;", HtmlEntityProvider.Convert(10617));
    HtmlEntityProvider.AddSingle(symbols, "subset;", HtmlEntityProvider.Convert(8834));
    HtmlEntityProvider.AddSingle(symbols, "subseteq;", HtmlEntityProvider.Convert(8838));
    HtmlEntityProvider.AddSingle(symbols, "subseteqq;", HtmlEntityProvider.Convert(10949));
    HtmlEntityProvider.AddSingle(symbols, "subsetneq;", HtmlEntityProvider.Convert(8842));
    HtmlEntityProvider.AddSingle(symbols, "subsetneqq;", HtmlEntityProvider.Convert(10955));
    HtmlEntityProvider.AddSingle(symbols, "subsim;", HtmlEntityProvider.Convert(10951));
    HtmlEntityProvider.AddSingle(symbols, "subsub;", HtmlEntityProvider.Convert(10965));
    HtmlEntityProvider.AddSingle(symbols, "subsup;", HtmlEntityProvider.Convert(10963));
    HtmlEntityProvider.AddSingle(symbols, "succ;", HtmlEntityProvider.Convert(8827));
    HtmlEntityProvider.AddSingle(symbols, "succapprox;", HtmlEntityProvider.Convert(10936));
    HtmlEntityProvider.AddSingle(symbols, "succcurlyeq;", HtmlEntityProvider.Convert(8829));
    HtmlEntityProvider.AddSingle(symbols, "succeq;", HtmlEntityProvider.Convert(10928));
    HtmlEntityProvider.AddSingle(symbols, "succnapprox;", HtmlEntityProvider.Convert(10938));
    HtmlEntityProvider.AddSingle(symbols, "succneqq;", HtmlEntityProvider.Convert(10934));
    HtmlEntityProvider.AddSingle(symbols, "succnsim;", HtmlEntityProvider.Convert(8937));
    HtmlEntityProvider.AddSingle(symbols, "succsim;", HtmlEntityProvider.Convert(8831));
    HtmlEntityProvider.AddSingle(symbols, "sum;", HtmlEntityProvider.Convert(8721));
    HtmlEntityProvider.AddSingle(symbols, "sung;", HtmlEntityProvider.Convert(9834));
    HtmlEntityProvider.AddSingle(symbols, "sup;", HtmlEntityProvider.Convert(8835));
    HtmlEntityProvider.AddBoth(symbols, "sup1;", HtmlEntityProvider.Convert(185));
    HtmlEntityProvider.AddBoth(symbols, "sup2;", HtmlEntityProvider.Convert(178));
    HtmlEntityProvider.AddBoth(symbols, "sup3;", HtmlEntityProvider.Convert(179));
    HtmlEntityProvider.AddSingle(symbols, "supdot;", HtmlEntityProvider.Convert(10942));
    HtmlEntityProvider.AddSingle(symbols, "supdsub;", HtmlEntityProvider.Convert(10968));
    HtmlEntityProvider.AddSingle(symbols, "supE;", HtmlEntityProvider.Convert(10950));
    HtmlEntityProvider.AddSingle(symbols, "supe;", HtmlEntityProvider.Convert(8839));
    HtmlEntityProvider.AddSingle(symbols, "supedot;", HtmlEntityProvider.Convert(10948));
    HtmlEntityProvider.AddSingle(symbols, "suphsol;", HtmlEntityProvider.Convert(10185));
    HtmlEntityProvider.AddSingle(symbols, "suphsub;", HtmlEntityProvider.Convert(10967));
    HtmlEntityProvider.AddSingle(symbols, "suplarr;", HtmlEntityProvider.Convert(10619));
    HtmlEntityProvider.AddSingle(symbols, "supmult;", HtmlEntityProvider.Convert(10946));
    HtmlEntityProvider.AddSingle(symbols, "supnE;", HtmlEntityProvider.Convert(10956));
    HtmlEntityProvider.AddSingle(symbols, "supne;", HtmlEntityProvider.Convert(8843));
    HtmlEntityProvider.AddSingle(symbols, "supplus;", HtmlEntityProvider.Convert(10944));
    HtmlEntityProvider.AddSingle(symbols, "supset;", HtmlEntityProvider.Convert(8835));
    HtmlEntityProvider.AddSingle(symbols, "supseteq;", HtmlEntityProvider.Convert(8839));
    HtmlEntityProvider.AddSingle(symbols, "supseteqq;", HtmlEntityProvider.Convert(10950));
    HtmlEntityProvider.AddSingle(symbols, "supsetneq;", HtmlEntityProvider.Convert(8843));
    HtmlEntityProvider.AddSingle(symbols, "supsetneqq;", HtmlEntityProvider.Convert(10956));
    HtmlEntityProvider.AddSingle(symbols, "supsim;", HtmlEntityProvider.Convert(10952));
    HtmlEntityProvider.AddSingle(symbols, "supsub;", HtmlEntityProvider.Convert(10964));
    HtmlEntityProvider.AddSingle(symbols, "supsup;", HtmlEntityProvider.Convert(10966));
    HtmlEntityProvider.AddSingle(symbols, "swarhk;", HtmlEntityProvider.Convert(10534));
    HtmlEntityProvider.AddSingle(symbols, "swArr;", HtmlEntityProvider.Convert(8665));
    HtmlEntityProvider.AddSingle(symbols, "swarr;", HtmlEntityProvider.Convert(8601));
    HtmlEntityProvider.AddSingle(symbols, "swarrow;", HtmlEntityProvider.Convert(8601));
    HtmlEntityProvider.AddSingle(symbols, "swnwar;", HtmlEntityProvider.Convert(10538));
    HtmlEntityProvider.AddBoth(symbols, "szlig;", HtmlEntityProvider.Convert(223));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigS()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Sacute;", HtmlEntityProvider.Convert(346));
    HtmlEntityProvider.AddSingle(symbols, "Sc;", HtmlEntityProvider.Convert(10940));
    HtmlEntityProvider.AddSingle(symbols, "Scaron;", HtmlEntityProvider.Convert(352));
    HtmlEntityProvider.AddSingle(symbols, "Scedil;", HtmlEntityProvider.Convert(350));
    HtmlEntityProvider.AddSingle(symbols, "Scirc;", HtmlEntityProvider.Convert(348));
    HtmlEntityProvider.AddSingle(symbols, "Scy;", HtmlEntityProvider.Convert(1057));
    HtmlEntityProvider.AddSingle(symbols, "Sfr;", HtmlEntityProvider.Convert(120086));
    HtmlEntityProvider.AddSingle(symbols, "SHCHcy;", HtmlEntityProvider.Convert(1065));
    HtmlEntityProvider.AddSingle(symbols, "SHcy;", HtmlEntityProvider.Convert(1064));
    HtmlEntityProvider.AddSingle(symbols, "ShortDownArrow;", HtmlEntityProvider.Convert(8595));
    HtmlEntityProvider.AddSingle(symbols, "ShortLeftArrow;", HtmlEntityProvider.Convert(8592));
    HtmlEntityProvider.AddSingle(symbols, "ShortRightArrow;", HtmlEntityProvider.Convert(8594));
    HtmlEntityProvider.AddSingle(symbols, "ShortUpArrow;", HtmlEntityProvider.Convert(8593));
    HtmlEntityProvider.AddSingle(symbols, "Sigma;", HtmlEntityProvider.Convert(931));
    HtmlEntityProvider.AddSingle(symbols, "SmallCircle;", HtmlEntityProvider.Convert(8728));
    HtmlEntityProvider.AddSingle(symbols, "SOFTcy;", HtmlEntityProvider.Convert(1068));
    HtmlEntityProvider.AddSingle(symbols, "Sopf;", HtmlEntityProvider.Convert(120138));
    HtmlEntityProvider.AddSingle(symbols, "Sqrt;", HtmlEntityProvider.Convert(8730));
    HtmlEntityProvider.AddSingle(symbols, "Square;", HtmlEntityProvider.Convert(9633));
    HtmlEntityProvider.AddSingle(symbols, "SquareIntersection;", HtmlEntityProvider.Convert(8851));
    HtmlEntityProvider.AddSingle(symbols, "SquareSubset;", HtmlEntityProvider.Convert(8847));
    HtmlEntityProvider.AddSingle(symbols, "SquareSubsetEqual;", HtmlEntityProvider.Convert(8849));
    HtmlEntityProvider.AddSingle(symbols, "SquareSuperset;", HtmlEntityProvider.Convert(8848));
    HtmlEntityProvider.AddSingle(symbols, "SquareSupersetEqual;", HtmlEntityProvider.Convert(8850));
    HtmlEntityProvider.AddSingle(symbols, "SquareUnion;", HtmlEntityProvider.Convert(8852));
    HtmlEntityProvider.AddSingle(symbols, "Sscr;", HtmlEntityProvider.Convert(119982));
    HtmlEntityProvider.AddSingle(symbols, "Star;", HtmlEntityProvider.Convert(8902));
    HtmlEntityProvider.AddSingle(symbols, "Sub;", HtmlEntityProvider.Convert(8912));
    HtmlEntityProvider.AddSingle(symbols, "Subset;", HtmlEntityProvider.Convert(8912));
    HtmlEntityProvider.AddSingle(symbols, "SubsetEqual;", HtmlEntityProvider.Convert(8838));
    HtmlEntityProvider.AddSingle(symbols, "Succeeds;", HtmlEntityProvider.Convert(8827));
    HtmlEntityProvider.AddSingle(symbols, "SucceedsEqual;", HtmlEntityProvider.Convert(10928));
    HtmlEntityProvider.AddSingle(symbols, "SucceedsSlantEqual;", HtmlEntityProvider.Convert(8829));
    HtmlEntityProvider.AddSingle(symbols, "SucceedsTilde;", HtmlEntityProvider.Convert(8831));
    HtmlEntityProvider.AddSingle(symbols, "SuchThat;", HtmlEntityProvider.Convert(8715));
    HtmlEntityProvider.AddSingle(symbols, "Sum;", HtmlEntityProvider.Convert(8721));
    HtmlEntityProvider.AddSingle(symbols, "Sup;", HtmlEntityProvider.Convert(8913));
    HtmlEntityProvider.AddSingle(symbols, "Superset;", HtmlEntityProvider.Convert(8835));
    HtmlEntityProvider.AddSingle(symbols, "SupersetEqual;", HtmlEntityProvider.Convert(8839));
    HtmlEntityProvider.AddSingle(symbols, "Supset;", HtmlEntityProvider.Convert(8913));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleT()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "target;", HtmlEntityProvider.Convert(8982));
    HtmlEntityProvider.AddSingle(symbols, "tau;", HtmlEntityProvider.Convert(964));
    HtmlEntityProvider.AddSingle(symbols, "tbrk;", HtmlEntityProvider.Convert(9140));
    HtmlEntityProvider.AddSingle(symbols, "tcaron;", HtmlEntityProvider.Convert(357));
    HtmlEntityProvider.AddSingle(symbols, "tcedil;", HtmlEntityProvider.Convert(355));
    HtmlEntityProvider.AddSingle(symbols, "tcy;", HtmlEntityProvider.Convert(1090));
    HtmlEntityProvider.AddSingle(symbols, "tdot;", HtmlEntityProvider.Convert(8411));
    HtmlEntityProvider.AddSingle(symbols, "telrec;", HtmlEntityProvider.Convert(8981));
    HtmlEntityProvider.AddSingle(symbols, "tfr;", HtmlEntityProvider.Convert(120113));
    HtmlEntityProvider.AddSingle(symbols, "there4;", HtmlEntityProvider.Convert(8756));
    HtmlEntityProvider.AddSingle(symbols, "therefore;", HtmlEntityProvider.Convert(8756));
    HtmlEntityProvider.AddSingle(symbols, "theta;", HtmlEntityProvider.Convert(952));
    HtmlEntityProvider.AddSingle(symbols, "thetasym;", HtmlEntityProvider.Convert(977));
    HtmlEntityProvider.AddSingle(symbols, "thetav;", HtmlEntityProvider.Convert(977));
    HtmlEntityProvider.AddSingle(symbols, "thickapprox;", HtmlEntityProvider.Convert(8776));
    HtmlEntityProvider.AddSingle(symbols, "thicksim;", HtmlEntityProvider.Convert(8764));
    HtmlEntityProvider.AddSingle(symbols, "thinsp;", HtmlEntityProvider.Convert(8201));
    HtmlEntityProvider.AddSingle(symbols, "thkap;", HtmlEntityProvider.Convert(8776));
    HtmlEntityProvider.AddSingle(symbols, "thksim;", HtmlEntityProvider.Convert(8764));
    HtmlEntityProvider.AddBoth(symbols, "thorn;", HtmlEntityProvider.Convert(254));
    HtmlEntityProvider.AddSingle(symbols, "tilde;", HtmlEntityProvider.Convert(732));
    HtmlEntityProvider.AddBoth(symbols, "times;", HtmlEntityProvider.Convert(215));
    HtmlEntityProvider.AddSingle(symbols, "timesb;", HtmlEntityProvider.Convert(8864));
    HtmlEntityProvider.AddSingle(symbols, "timesbar;", HtmlEntityProvider.Convert(10801));
    HtmlEntityProvider.AddSingle(symbols, "timesd;", HtmlEntityProvider.Convert(10800));
    HtmlEntityProvider.AddSingle(symbols, "tint;", HtmlEntityProvider.Convert(8749));
    HtmlEntityProvider.AddSingle(symbols, "toea;", HtmlEntityProvider.Convert(10536));
    HtmlEntityProvider.AddSingle(symbols, "top;", HtmlEntityProvider.Convert(8868));
    HtmlEntityProvider.AddSingle(symbols, "topbot;", HtmlEntityProvider.Convert(9014));
    HtmlEntityProvider.AddSingle(symbols, "topcir;", HtmlEntityProvider.Convert(10993));
    HtmlEntityProvider.AddSingle(symbols, "topf;", HtmlEntityProvider.Convert(120165));
    HtmlEntityProvider.AddSingle(symbols, "topfork;", HtmlEntityProvider.Convert(10970));
    HtmlEntityProvider.AddSingle(symbols, "tosa;", HtmlEntityProvider.Convert(10537));
    HtmlEntityProvider.AddSingle(symbols, "tprime;", HtmlEntityProvider.Convert(8244));
    HtmlEntityProvider.AddSingle(symbols, "trade;", HtmlEntityProvider.Convert(8482));
    HtmlEntityProvider.AddSingle(symbols, "triangle;", HtmlEntityProvider.Convert(9653));
    HtmlEntityProvider.AddSingle(symbols, "triangledown;", HtmlEntityProvider.Convert(9663));
    HtmlEntityProvider.AddSingle(symbols, "triangleleft;", HtmlEntityProvider.Convert(9667));
    HtmlEntityProvider.AddSingle(symbols, "trianglelefteq;", HtmlEntityProvider.Convert(8884));
    HtmlEntityProvider.AddSingle(symbols, "triangleq;", HtmlEntityProvider.Convert(8796));
    HtmlEntityProvider.AddSingle(symbols, "triangleright;", HtmlEntityProvider.Convert(9657));
    HtmlEntityProvider.AddSingle(symbols, "trianglerighteq;", HtmlEntityProvider.Convert(8885));
    HtmlEntityProvider.AddSingle(symbols, "tridot;", HtmlEntityProvider.Convert(9708));
    HtmlEntityProvider.AddSingle(symbols, "trie;", HtmlEntityProvider.Convert(8796));
    HtmlEntityProvider.AddSingle(symbols, "triminus;", HtmlEntityProvider.Convert(10810));
    HtmlEntityProvider.AddSingle(symbols, "triplus;", HtmlEntityProvider.Convert(10809));
    HtmlEntityProvider.AddSingle(symbols, "trisb;", HtmlEntityProvider.Convert(10701));
    HtmlEntityProvider.AddSingle(symbols, "tritime;", HtmlEntityProvider.Convert(10811));
    HtmlEntityProvider.AddSingle(symbols, "trpezium;", HtmlEntityProvider.Convert(9186));
    HtmlEntityProvider.AddSingle(symbols, "tscr;", HtmlEntityProvider.Convert(120009));
    HtmlEntityProvider.AddSingle(symbols, "tscy;", HtmlEntityProvider.Convert(1094));
    HtmlEntityProvider.AddSingle(symbols, "tshcy;", HtmlEntityProvider.Convert(1115));
    HtmlEntityProvider.AddSingle(symbols, "tstrok;", HtmlEntityProvider.Convert(359));
    HtmlEntityProvider.AddSingle(symbols, "twixt;", HtmlEntityProvider.Convert(8812));
    HtmlEntityProvider.AddSingle(symbols, "twoheadleftarrow;", HtmlEntityProvider.Convert(8606));
    HtmlEntityProvider.AddSingle(symbols, "twoheadrightarrow;", HtmlEntityProvider.Convert(8608));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigT()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Tab;", HtmlEntityProvider.Convert(9));
    HtmlEntityProvider.AddSingle(symbols, "Tau;", HtmlEntityProvider.Convert(932));
    HtmlEntityProvider.AddSingle(symbols, "Tcaron;", HtmlEntityProvider.Convert(356));
    HtmlEntityProvider.AddSingle(symbols, "Tcedil;", HtmlEntityProvider.Convert(354));
    HtmlEntityProvider.AddSingle(symbols, "Tcy;", HtmlEntityProvider.Convert(1058));
    HtmlEntityProvider.AddSingle(symbols, "Tfr;", HtmlEntityProvider.Convert(120087));
    HtmlEntityProvider.AddSingle(symbols, "Therefore;", HtmlEntityProvider.Convert(8756));
    HtmlEntityProvider.AddSingle(symbols, "Theta;", HtmlEntityProvider.Convert(920));
    HtmlEntityProvider.AddSingle(symbols, "ThickSpace;", HtmlEntityProvider.Convert(8287, 8202));
    HtmlEntityProvider.AddSingle(symbols, "ThinSpace;", HtmlEntityProvider.Convert(8201));
    HtmlEntityProvider.AddBoth(symbols, "THORN;", HtmlEntityProvider.Convert(222));
    HtmlEntityProvider.AddSingle(symbols, "Tilde;", HtmlEntityProvider.Convert(8764));
    HtmlEntityProvider.AddSingle(symbols, "TildeEqual;", HtmlEntityProvider.Convert(8771));
    HtmlEntityProvider.AddSingle(symbols, "TildeFullEqual;", HtmlEntityProvider.Convert(8773));
    HtmlEntityProvider.AddSingle(symbols, "TildeTilde;", HtmlEntityProvider.Convert(8776));
    HtmlEntityProvider.AddSingle(symbols, "Topf;", HtmlEntityProvider.Convert(120139));
    HtmlEntityProvider.AddSingle(symbols, "TRADE;", HtmlEntityProvider.Convert(8482));
    HtmlEntityProvider.AddSingle(symbols, "TripleDot;", HtmlEntityProvider.Convert(8411));
    HtmlEntityProvider.AddSingle(symbols, "Tscr;", HtmlEntityProvider.Convert(119983));
    HtmlEntityProvider.AddSingle(symbols, "TScy;", HtmlEntityProvider.Convert(1062));
    HtmlEntityProvider.AddSingle(symbols, "TSHcy;", HtmlEntityProvider.Convert(1035));
    HtmlEntityProvider.AddSingle(symbols, "Tstrok;", HtmlEntityProvider.Convert(358));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleU()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "uacute;", HtmlEntityProvider.Convert(250));
    HtmlEntityProvider.AddSingle(symbols, "uArr;", HtmlEntityProvider.Convert(8657));
    HtmlEntityProvider.AddSingle(symbols, "uarr;", HtmlEntityProvider.Convert(8593));
    HtmlEntityProvider.AddSingle(symbols, "ubrcy;", HtmlEntityProvider.Convert(1118));
    HtmlEntityProvider.AddSingle(symbols, "ubreve;", HtmlEntityProvider.Convert(365));
    HtmlEntityProvider.AddBoth(symbols, "ucirc;", HtmlEntityProvider.Convert(251));
    HtmlEntityProvider.AddSingle(symbols, "ucy;", HtmlEntityProvider.Convert(1091));
    HtmlEntityProvider.AddSingle(symbols, "udarr;", HtmlEntityProvider.Convert(8645));
    HtmlEntityProvider.AddSingle(symbols, "udblac;", HtmlEntityProvider.Convert(369));
    HtmlEntityProvider.AddSingle(symbols, "udhar;", HtmlEntityProvider.Convert(10606));
    HtmlEntityProvider.AddSingle(symbols, "ufisht;", HtmlEntityProvider.Convert(10622));
    HtmlEntityProvider.AddSingle(symbols, "ufr;", HtmlEntityProvider.Convert(120114));
    HtmlEntityProvider.AddBoth(symbols, "ugrave;", HtmlEntityProvider.Convert(249));
    HtmlEntityProvider.AddSingle(symbols, "uHar;", HtmlEntityProvider.Convert(10595));
    HtmlEntityProvider.AddSingle(symbols, "uharl;", HtmlEntityProvider.Convert(8639));
    HtmlEntityProvider.AddSingle(symbols, "uharr;", HtmlEntityProvider.Convert(8638));
    HtmlEntityProvider.AddSingle(symbols, "uhblk;", HtmlEntityProvider.Convert(9600));
    HtmlEntityProvider.AddSingle(symbols, "ulcorn;", HtmlEntityProvider.Convert(8988));
    HtmlEntityProvider.AddSingle(symbols, "ulcorner;", HtmlEntityProvider.Convert(8988));
    HtmlEntityProvider.AddSingle(symbols, "ulcrop;", HtmlEntityProvider.Convert(8975));
    HtmlEntityProvider.AddSingle(symbols, "ultri;", HtmlEntityProvider.Convert(9720));
    HtmlEntityProvider.AddSingle(symbols, "umacr;", HtmlEntityProvider.Convert(363));
    HtmlEntityProvider.AddBoth(symbols, "uml;", HtmlEntityProvider.Convert(168));
    HtmlEntityProvider.AddSingle(symbols, "uogon;", HtmlEntityProvider.Convert(371));
    HtmlEntityProvider.AddSingle(symbols, "uopf;", HtmlEntityProvider.Convert(120166));
    HtmlEntityProvider.AddSingle(symbols, "uparrow;", HtmlEntityProvider.Convert(8593));
    HtmlEntityProvider.AddSingle(symbols, "updownarrow;", HtmlEntityProvider.Convert(8597));
    HtmlEntityProvider.AddSingle(symbols, "upharpoonleft;", HtmlEntityProvider.Convert(8639));
    HtmlEntityProvider.AddSingle(symbols, "upharpoonright;", HtmlEntityProvider.Convert(8638));
    HtmlEntityProvider.AddSingle(symbols, "uplus;", HtmlEntityProvider.Convert(8846));
    HtmlEntityProvider.AddSingle(symbols, "upsi;", HtmlEntityProvider.Convert(965));
    HtmlEntityProvider.AddSingle(symbols, "upsih;", HtmlEntityProvider.Convert(978));
    HtmlEntityProvider.AddSingle(symbols, "upsilon;", HtmlEntityProvider.Convert(965));
    HtmlEntityProvider.AddSingle(symbols, "upuparrows;", HtmlEntityProvider.Convert(8648));
    HtmlEntityProvider.AddSingle(symbols, "urcorn;", HtmlEntityProvider.Convert(8989));
    HtmlEntityProvider.AddSingle(symbols, "urcorner;", HtmlEntityProvider.Convert(8989));
    HtmlEntityProvider.AddSingle(symbols, "urcrop;", HtmlEntityProvider.Convert(8974));
    HtmlEntityProvider.AddSingle(symbols, "uring;", HtmlEntityProvider.Convert(367));
    HtmlEntityProvider.AddSingle(symbols, "urtri;", HtmlEntityProvider.Convert(9721));
    HtmlEntityProvider.AddSingle(symbols, "uscr;", HtmlEntityProvider.Convert(120010));
    HtmlEntityProvider.AddSingle(symbols, "utdot;", HtmlEntityProvider.Convert(8944));
    HtmlEntityProvider.AddSingle(symbols, "utilde;", HtmlEntityProvider.Convert(361));
    HtmlEntityProvider.AddSingle(symbols, "utri;", HtmlEntityProvider.Convert(9653));
    HtmlEntityProvider.AddSingle(symbols, "utrif;", HtmlEntityProvider.Convert(9652));
    HtmlEntityProvider.AddSingle(symbols, "uuarr;", HtmlEntityProvider.Convert(8648));
    HtmlEntityProvider.AddBoth(symbols, "uuml;", HtmlEntityProvider.Convert(252));
    HtmlEntityProvider.AddSingle(symbols, "uwangle;", HtmlEntityProvider.Convert(10663));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigU()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "Uacute;", HtmlEntityProvider.Convert(218));
    HtmlEntityProvider.AddSingle(symbols, "Uarr;", HtmlEntityProvider.Convert(8607));
    HtmlEntityProvider.AddSingle(symbols, "Uarrocir;", HtmlEntityProvider.Convert(10569));
    HtmlEntityProvider.AddSingle(symbols, "Ubrcy;", HtmlEntityProvider.Convert(1038));
    HtmlEntityProvider.AddSingle(symbols, "Ubreve;", HtmlEntityProvider.Convert(364));
    HtmlEntityProvider.AddBoth(symbols, "Ucirc;", HtmlEntityProvider.Convert(219));
    HtmlEntityProvider.AddSingle(symbols, "Ucy;", HtmlEntityProvider.Convert(1059));
    HtmlEntityProvider.AddSingle(symbols, "Udblac;", HtmlEntityProvider.Convert(368));
    HtmlEntityProvider.AddSingle(symbols, "Ufr;", HtmlEntityProvider.Convert(120088));
    HtmlEntityProvider.AddBoth(symbols, "Ugrave;", HtmlEntityProvider.Convert(217));
    HtmlEntityProvider.AddSingle(symbols, "Umacr;", HtmlEntityProvider.Convert(362));
    HtmlEntityProvider.AddSingle(symbols, "UnderBar;", HtmlEntityProvider.Convert(95));
    HtmlEntityProvider.AddSingle(symbols, "UnderBrace;", HtmlEntityProvider.Convert(9183));
    HtmlEntityProvider.AddSingle(symbols, "UnderBracket;", HtmlEntityProvider.Convert(9141));
    HtmlEntityProvider.AddSingle(symbols, "UnderParenthesis;", HtmlEntityProvider.Convert(9181));
    HtmlEntityProvider.AddSingle(symbols, "Union;", HtmlEntityProvider.Convert(8899));
    HtmlEntityProvider.AddSingle(symbols, "UnionPlus;", HtmlEntityProvider.Convert(8846));
    HtmlEntityProvider.AddSingle(symbols, "Uogon;", HtmlEntityProvider.Convert(370));
    HtmlEntityProvider.AddSingle(symbols, "Uopf;", HtmlEntityProvider.Convert(120140));
    HtmlEntityProvider.AddSingle(symbols, "UpArrow;", HtmlEntityProvider.Convert(8593));
    HtmlEntityProvider.AddSingle(symbols, "Uparrow;", HtmlEntityProvider.Convert(8657));
    HtmlEntityProvider.AddSingle(symbols, "UpArrowBar;", HtmlEntityProvider.Convert(10514));
    HtmlEntityProvider.AddSingle(symbols, "UpArrowDownArrow;", HtmlEntityProvider.Convert(8645));
    HtmlEntityProvider.AddSingle(symbols, "UpDownArrow;", HtmlEntityProvider.Convert(8597));
    HtmlEntityProvider.AddSingle(symbols, "Updownarrow;", HtmlEntityProvider.Convert(8661));
    HtmlEntityProvider.AddSingle(symbols, "UpEquilibrium;", HtmlEntityProvider.Convert(10606));
    HtmlEntityProvider.AddSingle(symbols, "UpperLeftArrow;", HtmlEntityProvider.Convert(8598));
    HtmlEntityProvider.AddSingle(symbols, "UpperRightArrow;", HtmlEntityProvider.Convert(8599));
    HtmlEntityProvider.AddSingle(symbols, "Upsi;", HtmlEntityProvider.Convert(978));
    HtmlEntityProvider.AddSingle(symbols, "Upsilon;", HtmlEntityProvider.Convert(933));
    HtmlEntityProvider.AddSingle(symbols, "UpTee;", HtmlEntityProvider.Convert(8869));
    HtmlEntityProvider.AddSingle(symbols, "UpTeeArrow;", HtmlEntityProvider.Convert(8613));
    HtmlEntityProvider.AddSingle(symbols, "Uring;", HtmlEntityProvider.Convert(366));
    HtmlEntityProvider.AddSingle(symbols, "Uscr;", HtmlEntityProvider.Convert(119984));
    HtmlEntityProvider.AddSingle(symbols, "Utilde;", HtmlEntityProvider.Convert(360));
    HtmlEntityProvider.AddBoth(symbols, "Uuml;", HtmlEntityProvider.Convert(220));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleV()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "vangrt;", HtmlEntityProvider.Convert(10652));
    HtmlEntityProvider.AddSingle(symbols, "varepsilon;", HtmlEntityProvider.Convert(1013));
    HtmlEntityProvider.AddSingle(symbols, "varkappa;", HtmlEntityProvider.Convert(1008));
    HtmlEntityProvider.AddSingle(symbols, "varnothing;", HtmlEntityProvider.Convert(8709));
    HtmlEntityProvider.AddSingle(symbols, "varphi;", HtmlEntityProvider.Convert(981));
    HtmlEntityProvider.AddSingle(symbols, "varpi;", HtmlEntityProvider.Convert(982));
    HtmlEntityProvider.AddSingle(symbols, "varpropto;", HtmlEntityProvider.Convert(8733));
    HtmlEntityProvider.AddSingle(symbols, "vArr;", HtmlEntityProvider.Convert(8661));
    HtmlEntityProvider.AddSingle(symbols, "varr;", HtmlEntityProvider.Convert(8597));
    HtmlEntityProvider.AddSingle(symbols, "varrho;", HtmlEntityProvider.Convert(1009));
    HtmlEntityProvider.AddSingle(symbols, "varsigma;", HtmlEntityProvider.Convert(962));
    HtmlEntityProvider.AddSingle(symbols, "varsubsetneq;", HtmlEntityProvider.Convert(8842, 65024));
    HtmlEntityProvider.AddSingle(symbols, "varsubsetneqq;", HtmlEntityProvider.Convert(10955, 65024));
    HtmlEntityProvider.AddSingle(symbols, "varsupsetneq;", HtmlEntityProvider.Convert(8843, 65024));
    HtmlEntityProvider.AddSingle(symbols, "varsupsetneqq;", HtmlEntityProvider.Convert(10956, 65024));
    HtmlEntityProvider.AddSingle(symbols, "vartheta;", HtmlEntityProvider.Convert(977));
    HtmlEntityProvider.AddSingle(symbols, "vartriangleleft;", HtmlEntityProvider.Convert(8882));
    HtmlEntityProvider.AddSingle(symbols, "vartriangleright;", HtmlEntityProvider.Convert(8883));
    HtmlEntityProvider.AddSingle(symbols, "vBar;", HtmlEntityProvider.Convert(10984));
    HtmlEntityProvider.AddSingle(symbols, "vBarv;", HtmlEntityProvider.Convert(10985));
    HtmlEntityProvider.AddSingle(symbols, "vcy;", HtmlEntityProvider.Convert(1074));
    HtmlEntityProvider.AddSingle(symbols, "vDash;", HtmlEntityProvider.Convert(8872));
    HtmlEntityProvider.AddSingle(symbols, "vdash;", HtmlEntityProvider.Convert(8866));
    HtmlEntityProvider.AddSingle(symbols, "vee;", HtmlEntityProvider.Convert(8744));
    HtmlEntityProvider.AddSingle(symbols, "veebar;", HtmlEntityProvider.Convert(8891));
    HtmlEntityProvider.AddSingle(symbols, "veeeq;", HtmlEntityProvider.Convert(8794));
    HtmlEntityProvider.AddSingle(symbols, "vellip;", HtmlEntityProvider.Convert(8942));
    HtmlEntityProvider.AddSingle(symbols, "verbar;", HtmlEntityProvider.Convert(124));
    HtmlEntityProvider.AddSingle(symbols, "vert;", HtmlEntityProvider.Convert(124));
    HtmlEntityProvider.AddSingle(symbols, "vfr;", HtmlEntityProvider.Convert(120115));
    HtmlEntityProvider.AddSingle(symbols, "vltri;", HtmlEntityProvider.Convert(8882));
    HtmlEntityProvider.AddSingle(symbols, "vnsub;", HtmlEntityProvider.Convert(8834, 8402));
    HtmlEntityProvider.AddSingle(symbols, "vnsup;", HtmlEntityProvider.Convert(8835, 8402));
    HtmlEntityProvider.AddSingle(symbols, "vopf;", HtmlEntityProvider.Convert(120167));
    HtmlEntityProvider.AddSingle(symbols, "vprop;", HtmlEntityProvider.Convert(8733));
    HtmlEntityProvider.AddSingle(symbols, "vrtri;", HtmlEntityProvider.Convert(8883));
    HtmlEntityProvider.AddSingle(symbols, "vscr;", HtmlEntityProvider.Convert(120011));
    HtmlEntityProvider.AddSingle(symbols, "vsubnE;", HtmlEntityProvider.Convert(10955, 65024));
    HtmlEntityProvider.AddSingle(symbols, "vsubne;", HtmlEntityProvider.Convert(8842, 65024));
    HtmlEntityProvider.AddSingle(symbols, "vsupnE;", HtmlEntityProvider.Convert(10956, 65024));
    HtmlEntityProvider.AddSingle(symbols, "vsupne;", HtmlEntityProvider.Convert(8843, 65024));
    HtmlEntityProvider.AddSingle(symbols, "vzigzag;", HtmlEntityProvider.Convert(10650));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigV()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Vbar;", HtmlEntityProvider.Convert(10987));
    HtmlEntityProvider.AddSingle(symbols, "Vcy;", HtmlEntityProvider.Convert(1042));
    HtmlEntityProvider.AddSingle(symbols, "VDash;", HtmlEntityProvider.Convert(8875));
    HtmlEntityProvider.AddSingle(symbols, "Vdash;", HtmlEntityProvider.Convert(8873));
    HtmlEntityProvider.AddSingle(symbols, "Vdashl;", HtmlEntityProvider.Convert(10982));
    HtmlEntityProvider.AddSingle(symbols, "Vee;", HtmlEntityProvider.Convert(8897));
    HtmlEntityProvider.AddSingle(symbols, "Verbar;", HtmlEntityProvider.Convert(8214));
    HtmlEntityProvider.AddSingle(symbols, "Vert;", HtmlEntityProvider.Convert(8214));
    HtmlEntityProvider.AddSingle(symbols, "VerticalBar;", HtmlEntityProvider.Convert(8739));
    HtmlEntityProvider.AddSingle(symbols, "VerticalLine;", HtmlEntityProvider.Convert(124));
    HtmlEntityProvider.AddSingle(symbols, "VerticalSeparator;", HtmlEntityProvider.Convert(10072));
    HtmlEntityProvider.AddSingle(symbols, "VerticalTilde;", HtmlEntityProvider.Convert(8768));
    HtmlEntityProvider.AddSingle(symbols, "VeryThinSpace;", HtmlEntityProvider.Convert(8202));
    HtmlEntityProvider.AddSingle(symbols, "Vfr;", HtmlEntityProvider.Convert(120089));
    HtmlEntityProvider.AddSingle(symbols, "Vopf;", HtmlEntityProvider.Convert(120141));
    HtmlEntityProvider.AddSingle(symbols, "Vscr;", HtmlEntityProvider.Convert(119985));
    HtmlEntityProvider.AddSingle(symbols, "Vvdash;", HtmlEntityProvider.Convert(8874));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleW()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "wcirc;", HtmlEntityProvider.Convert(373));
    HtmlEntityProvider.AddSingle(symbols, "wedbar;", HtmlEntityProvider.Convert(10847));
    HtmlEntityProvider.AddSingle(symbols, "wedge;", HtmlEntityProvider.Convert(8743));
    HtmlEntityProvider.AddSingle(symbols, "wedgeq;", HtmlEntityProvider.Convert(8793));
    HtmlEntityProvider.AddSingle(symbols, "weierp;", HtmlEntityProvider.Convert(8472));
    HtmlEntityProvider.AddSingle(symbols, "wfr;", HtmlEntityProvider.Convert(120116));
    HtmlEntityProvider.AddSingle(symbols, "wopf;", HtmlEntityProvider.Convert(120168));
    HtmlEntityProvider.AddSingle(symbols, "wp;", HtmlEntityProvider.Convert(8472));
    HtmlEntityProvider.AddSingle(symbols, "wr;", HtmlEntityProvider.Convert(8768));
    HtmlEntityProvider.AddSingle(symbols, "wreath;", HtmlEntityProvider.Convert(8768));
    HtmlEntityProvider.AddSingle(symbols, "wscr;", HtmlEntityProvider.Convert(120012));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigW()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Wcirc;", HtmlEntityProvider.Convert(372));
    HtmlEntityProvider.AddSingle(symbols, "Wedge;", HtmlEntityProvider.Convert(8896));
    HtmlEntityProvider.AddSingle(symbols, "Wfr;", HtmlEntityProvider.Convert(120090));
    HtmlEntityProvider.AddSingle(symbols, "Wopf;", HtmlEntityProvider.Convert(120142));
    HtmlEntityProvider.AddSingle(symbols, "Wscr;", HtmlEntityProvider.Convert(119986));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleX()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "xcap;", HtmlEntityProvider.Convert(8898));
    HtmlEntityProvider.AddSingle(symbols, "xcirc;", HtmlEntityProvider.Convert(9711));
    HtmlEntityProvider.AddSingle(symbols, "xcup;", HtmlEntityProvider.Convert(8899));
    HtmlEntityProvider.AddSingle(symbols, "xdtri;", HtmlEntityProvider.Convert(9661));
    HtmlEntityProvider.AddSingle(symbols, "xfr;", HtmlEntityProvider.Convert(120117));
    HtmlEntityProvider.AddSingle(symbols, "xhArr;", HtmlEntityProvider.Convert(10234));
    HtmlEntityProvider.AddSingle(symbols, "xharr;", HtmlEntityProvider.Convert(10231));
    HtmlEntityProvider.AddSingle(symbols, "xi;", HtmlEntityProvider.Convert(958));
    HtmlEntityProvider.AddSingle(symbols, "xlArr;", HtmlEntityProvider.Convert(10232));
    HtmlEntityProvider.AddSingle(symbols, "xlarr;", HtmlEntityProvider.Convert(10229));
    HtmlEntityProvider.AddSingle(symbols, "xmap;", HtmlEntityProvider.Convert(10236));
    HtmlEntityProvider.AddSingle(symbols, "xnis;", HtmlEntityProvider.Convert(8955));
    HtmlEntityProvider.AddSingle(symbols, "xodot;", HtmlEntityProvider.Convert(10752));
    HtmlEntityProvider.AddSingle(symbols, "xopf;", HtmlEntityProvider.Convert(120169));
    HtmlEntityProvider.AddSingle(symbols, "xoplus;", HtmlEntityProvider.Convert(10753));
    HtmlEntityProvider.AddSingle(symbols, "xotime;", HtmlEntityProvider.Convert(10754));
    HtmlEntityProvider.AddSingle(symbols, "xrArr;", HtmlEntityProvider.Convert(10233));
    HtmlEntityProvider.AddSingle(symbols, "xrarr;", HtmlEntityProvider.Convert(10230));
    HtmlEntityProvider.AddSingle(symbols, "xscr;", HtmlEntityProvider.Convert(120013));
    HtmlEntityProvider.AddSingle(symbols, "xsqcup;", HtmlEntityProvider.Convert(10758));
    HtmlEntityProvider.AddSingle(symbols, "xuplus;", HtmlEntityProvider.Convert(10756));
    HtmlEntityProvider.AddSingle(symbols, "xutri;", HtmlEntityProvider.Convert(9651));
    HtmlEntityProvider.AddSingle(symbols, "xvee;", HtmlEntityProvider.Convert(8897));
    HtmlEntityProvider.AddSingle(symbols, "xwedge;", HtmlEntityProvider.Convert(8896));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigX()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Xfr;", HtmlEntityProvider.Convert(120091));
    HtmlEntityProvider.AddSingle(symbols, "Xi;", HtmlEntityProvider.Convert(926));
    HtmlEntityProvider.AddSingle(symbols, "Xopf;", HtmlEntityProvider.Convert(120143));
    HtmlEntityProvider.AddSingle(symbols, "Xscr;", HtmlEntityProvider.Convert(119987));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleY()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "yacute;", HtmlEntityProvider.Convert(253));
    HtmlEntityProvider.AddSingle(symbols, "yacy;", HtmlEntityProvider.Convert(1103));
    HtmlEntityProvider.AddSingle(symbols, "ycirc;", HtmlEntityProvider.Convert(375));
    HtmlEntityProvider.AddSingle(symbols, "ycy;", HtmlEntityProvider.Convert(1099));
    HtmlEntityProvider.AddBoth(symbols, "yen;", HtmlEntityProvider.Convert(165));
    HtmlEntityProvider.AddSingle(symbols, "yfr;", HtmlEntityProvider.Convert(120118));
    HtmlEntityProvider.AddSingle(symbols, "yicy;", HtmlEntityProvider.Convert(1111));
    HtmlEntityProvider.AddSingle(symbols, "yopf;", HtmlEntityProvider.Convert(120170));
    HtmlEntityProvider.AddSingle(symbols, "yscr;", HtmlEntityProvider.Convert(120014));
    HtmlEntityProvider.AddSingle(symbols, "yucy;", HtmlEntityProvider.Convert(1102));
    HtmlEntityProvider.AddBoth(symbols, "yuml;", HtmlEntityProvider.Convert((int) byte.MaxValue));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigY()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddBoth(symbols, "Yacute;", HtmlEntityProvider.Convert(221));
    HtmlEntityProvider.AddSingle(symbols, "YAcy;", HtmlEntityProvider.Convert(1071));
    HtmlEntityProvider.AddSingle(symbols, "Ycirc;", HtmlEntityProvider.Convert(374));
    HtmlEntityProvider.AddSingle(symbols, "Ycy;", HtmlEntityProvider.Convert(1067));
    HtmlEntityProvider.AddSingle(symbols, "Yfr;", HtmlEntityProvider.Convert(120092));
    HtmlEntityProvider.AddSingle(symbols, "YIcy;", HtmlEntityProvider.Convert(1031));
    HtmlEntityProvider.AddSingle(symbols, "Yopf;", HtmlEntityProvider.Convert(120144));
    HtmlEntityProvider.AddSingle(symbols, "Yscr;", HtmlEntityProvider.Convert(119988));
    HtmlEntityProvider.AddSingle(symbols, "YUcy;", HtmlEntityProvider.Convert(1070));
    HtmlEntityProvider.AddSingle(symbols, "Yuml;", HtmlEntityProvider.Convert(376));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolLittleZ()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "zacute;", HtmlEntityProvider.Convert(378));
    HtmlEntityProvider.AddSingle(symbols, "zcaron;", HtmlEntityProvider.Convert(382));
    HtmlEntityProvider.AddSingle(symbols, "zcy;", HtmlEntityProvider.Convert(1079));
    HtmlEntityProvider.AddSingle(symbols, "zdot;", HtmlEntityProvider.Convert(380));
    HtmlEntityProvider.AddSingle(symbols, "zeetrf;", HtmlEntityProvider.Convert(8488));
    HtmlEntityProvider.AddSingle(symbols, "zeta;", HtmlEntityProvider.Convert(950));
    HtmlEntityProvider.AddSingle(symbols, "zfr;", HtmlEntityProvider.Convert(120119));
    HtmlEntityProvider.AddSingle(symbols, "zhcy;", HtmlEntityProvider.Convert(1078));
    HtmlEntityProvider.AddSingle(symbols, "zigrarr;", HtmlEntityProvider.Convert(8669));
    HtmlEntityProvider.AddSingle(symbols, "zopf;", HtmlEntityProvider.Convert(120171));
    HtmlEntityProvider.AddSingle(symbols, "zscr;", HtmlEntityProvider.Convert(120015));
    HtmlEntityProvider.AddSingle(symbols, "zwj;", HtmlEntityProvider.Convert(8205));
    HtmlEntityProvider.AddSingle(symbols, "zwnj;", HtmlEntityProvider.Convert(8204));
    return symbols;
  }

  private Dictionary<string, string> GetSymbolBigZ()
  {
    Dictionary<string, string> symbols = new Dictionary<string, string>();
    HtmlEntityProvider.AddSingle(symbols, "Zacute;", HtmlEntityProvider.Convert(377));
    HtmlEntityProvider.AddSingle(symbols, "Zcaron;", HtmlEntityProvider.Convert(381));
    HtmlEntityProvider.AddSingle(symbols, "Zcy;", HtmlEntityProvider.Convert(1047));
    HtmlEntityProvider.AddSingle(symbols, "Zdot;", HtmlEntityProvider.Convert(379));
    HtmlEntityProvider.AddSingle(symbols, "ZeroWidthSpace;", HtmlEntityProvider.Convert(8203));
    HtmlEntityProvider.AddSingle(symbols, "Zeta;", HtmlEntityProvider.Convert(918));
    HtmlEntityProvider.AddSingle(symbols, "Zfr;", HtmlEntityProvider.Convert(8488));
    HtmlEntityProvider.AddSingle(symbols, "ZHcy;", HtmlEntityProvider.Convert(1046));
    HtmlEntityProvider.AddSingle(symbols, "Zopf;", HtmlEntityProvider.Convert(8484));
    HtmlEntityProvider.AddSingle(symbols, "Zscr;", HtmlEntityProvider.Convert(119989));
    return symbols;
  }

  public string GetSymbol(string name)
  {
    string symbol = (string) null;
    Dictionary<string, string> dictionary;
    if (!string.IsNullOrEmpty(name) && this._entities.TryGetValue(name[0], out dictionary))
      dictionary.TryGetValue(name, out symbol);
    return symbol;
  }

  private static string Convert(int code) => char.ConvertFromUtf32(code);

  private static string Convert(int leading, int trailing)
  {
    return char.ConvertFromUtf32(leading) + char.ConvertFromUtf32(trailing);
  }

  public static bool IsInvalidNumber(int code)
  {
    return code >= 55296 && code <= 57343 /*0xDFFF*/ || code < 0 || code > 1114111;
  }

  public static bool IsInCharacterTable(int code)
  {
    return code == 0 || code == 13 || code == 128 /*0x80*/ || code == 129 || code == 130 || code == 131 || code == 132 || code == 133 || code == 134 || code == 135 || code == 136 || code == 137 || code == 138 || code == 139 || code == 140 || code == 141 || code == 142 || code == 143 || code == 144 /*0x90*/ || code == 145 || code == 146 || code == 147 || code == 148 || code == 149 || code == 150 || code == 151 || code == 152 || code == 153 || code == 154 || code == 155 || code == 156 || code == 157 || code == 158 || code == 159;
  }

  public static string GetSymbolFromTable(int code)
  {
    switch (code)
    {
      case 0:
        return HtmlEntityProvider.Convert(65533);
      case 13:
        return HtmlEntityProvider.Convert(13);
      case 128 /*0x80*/:
        return HtmlEntityProvider.Convert(8364);
      case 129:
        return HtmlEntityProvider.Convert(129);
      case 130:
        return HtmlEntityProvider.Convert(8218);
      case 131:
        return HtmlEntityProvider.Convert(402);
      case 132:
        return HtmlEntityProvider.Convert(8222);
      case 133:
        return HtmlEntityProvider.Convert(8230);
      case 134:
        return HtmlEntityProvider.Convert(8224);
      case 135:
        return HtmlEntityProvider.Convert(8225);
      case 136:
        return HtmlEntityProvider.Convert(710);
      case 137:
        return HtmlEntityProvider.Convert(8240);
      case 138:
        return HtmlEntityProvider.Convert(352);
      case 139:
        return HtmlEntityProvider.Convert(8249);
      case 140:
        return HtmlEntityProvider.Convert(338);
      case 141:
        return HtmlEntityProvider.Convert(141);
      case 142:
        return HtmlEntityProvider.Convert(381);
      case 143:
        return HtmlEntityProvider.Convert(143);
      case 144 /*0x90*/:
        return HtmlEntityProvider.Convert(144 /*0x90*/);
      case 145:
        return HtmlEntityProvider.Convert(8216);
      case 146:
        return HtmlEntityProvider.Convert(8217);
      case 147:
        return HtmlEntityProvider.Convert(8220);
      case 148:
        return HtmlEntityProvider.Convert(8221);
      case 149:
        return HtmlEntityProvider.Convert(8226);
      case 150:
        return HtmlEntityProvider.Convert(8211);
      case 151:
        return HtmlEntityProvider.Convert(8212);
      case 152:
        return HtmlEntityProvider.Convert(732);
      case 153:
        return HtmlEntityProvider.Convert(8482);
      case 154:
        return HtmlEntityProvider.Convert(353);
      case 155:
        return HtmlEntityProvider.Convert(8250);
      case 156:
        return HtmlEntityProvider.Convert(339);
      case 157:
        return HtmlEntityProvider.Convert(157);
      case 158:
        return HtmlEntityProvider.Convert(382);
      case 159:
        return HtmlEntityProvider.Convert(376);
      default:
        return (string) null;
    }
  }

  public static bool IsInInvalidRange(int code)
  {
    return code >= 1 && code <= 8 || code >= 14 && code <= 31 /*0x1F*/ || code >= (int) sbyte.MaxValue && code <= 159 || code >= 64976 && code <= 65007 || code == 11 || code == 65534 || code == (int) ushort.MaxValue || code == 131070 || code == 196606 || code == 131071 /*0x01FFFF*/ || code == 196607 /*0x02FFFF*/ || code == 262142 || code == 262143 /*0x03FFFF*/ || code == 327678 || code == 327679 /*0x04FFFF*/ || code == 393214 || code == 393215 /*0x05FFFF*/ || code == 458750 || code == 458751 /*0x06FFFF*/ || code == 524286 || code == 524287 /*0x07FFFF*/ || code == 589822 || code == 589823 /*0x08FFFF*/ || code == 655358 || code == 655359 /*0x09FFFF*/ || code == 720894 || code == 720895 /*0x0AFFFF*/ || code == 786430 || code == 786431 /*0x0BFFFF*/ || code == 851966 || code == 851967 /*0x0CFFFF*/ || code == 917502 || code == 917503 /*0x0DFFFF*/ || code == 983038 || code == 983039 /*0x0EFFFF*/ || code == 1048574 || code == 1048575 /*0x0FFFFF*/ || code == 1114110 || code == 1114111;
  }

  private static void AddSingle(Dictionary<string, string> symbols, string key, string value)
  {
    symbols.Add(key, value);
  }

  private static void AddBoth(Dictionary<string, string> symbols, string key, string value)
  {
    symbols.Add(key, value);
    symbols.Add(key.Remove(key.Length - 1), value);
  }
}
