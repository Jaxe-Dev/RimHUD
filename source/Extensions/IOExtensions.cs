using System.IO;

namespace RimHUD.Extensions
{
  public static class IOExtensions
  {
    public static T Refreshed<T>(this T self) where T : FileSystemInfo
    {
      self.Refresh();
      return self;
    }

    public static bool ExistsNow(this FileSystemInfo self) => self.Refreshed().Exists;
    public static string NameWithoutExtension(this FileInfo self) => Path.GetFileNameWithoutExtension(self.Name);
  }
}
