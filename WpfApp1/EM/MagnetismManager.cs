using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace WpfApp1.EM;

public class MagnetismManager
{
   public static void MakeDirect(string directtaskCfg, double leftX, double rightX, int i, string recsTxt)
   {
      string direct = @"../../../EM/direct.exe";

      using (var s = new StreamWriter(@"../../../EM/direct_info.txt", new FileStreamOptions{ Mode = FileMode.OpenOrCreate }))
      {
         s.WriteLine(directtaskCfg);
         s.WriteLine(leftX);
         s.WriteLine(rightX);
         s.WriteLine(i);
         s.WriteLine(recsTxt);
      }

      ProcessStartInfo info = new(direct, "direct_info.txt");
      info.CreateNoWindow = true;

      try
      {
         using (Process exeProcess = Process.Start(info))
         {
            exeProcess.WaitForExit();
         }
      }
      catch
      {
         throw new Exception("Pepega");
      }
   }

   public static void MakeReverse(string directtaskCfg, string recsTxt, string ansTxt, double alpha)
   {
      string reverse = @"..\..\..\EM\reverse.exe";

      using (StreamWriter s = new StreamWriter("..\\..\\..\\EM\\reverse_info.txt", new FileStreamOptions(){Mode = FileMode.CreateNew}))
      {
         s.WriteLine(directtaskCfg);
         s.WriteLine(recsTxt);
         s.WriteLine(ansTxt);
         s.WriteLine(alpha);
      }

      ProcessStartInfo info = new(reverse, "reverse_info.txt");
      info.CreateNoWindow = true;

      try
      {
         using (Process exeProcess = Process.Start(info))
         {
            exeProcess.WaitForExit();
         }
      }
      catch
      {
         throw new Exception("agepeP");
      }
   }
} 