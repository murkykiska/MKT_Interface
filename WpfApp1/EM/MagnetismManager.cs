using System.Runtime.InteropServices;

namespace WpfApp1.EM;

public class MagnetismManager
{
   public MagnetismManager() {}

   [DllImport("..\\..\\..\\EM\\Modern Computer Technologies.dll")]
   public static extern void makeDirectTask(string cfgFileName, double left, double right, int nRecivers, string reciversFileName);

   [DllImport(@"..\..\..\EM\Modern Computer Technologies.dll")]
   public static extern void makeReverseTask(string cfgFileName, string reciversFileName, string ansFileName, double alpha);

} 