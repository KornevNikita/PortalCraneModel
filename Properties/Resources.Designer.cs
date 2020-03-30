// Decompiled with JetBrains decompiler
// Type: PortalCraneModel.Properties.Resources
// Assembly: PortalCraneModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BAA13FC9-345E-43AF-A47A-80EBEB1AFDE9
// Assembly location: C:\Users\Nikita\Desktop\PortalCraneModel\x64\Release\PortalCraneModel.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PortalCraneModel.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Properties.Resources.resourceMan == null)
          Properties.Resources.resourceMan = new ResourceManager("PortalCraneModel.Properties.Resources", typeof (Properties.Resources).Assembly);
        return Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return Properties.Resources.resourceCulture;
      }
      set
      {
        Properties.Resources.resourceCulture = value;
      }
    }
  }
}
