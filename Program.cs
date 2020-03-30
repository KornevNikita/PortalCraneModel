// Decompiled with JetBrains decompiler
// Type: PortalCraneModel.Program
// Assembly: PortalCraneModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BAA13FC9-345E-43AF-A47A-80EBEB1AFDE9
// Assembly location: C:\Users\Nikita\Desktop\PortalCraneModel\x64\Release\PortalCraneModel.exe

using System;
using System.Windows.Forms;

namespace PortalCraneModel
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new PortalCraneModel());
    }
  }
}
