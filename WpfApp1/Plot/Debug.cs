﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Fluent;

namespace WpfApp1;

public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
{
   private static DebugProc DebugMessageDelegate = OnDebugMessage;
   private static void OnDebugMessage(
       DebugSource source,     // Source of the debugging message.
       DebugType type,         // Type of the debugging message.
       int id,                 // ID associated with the message.
       DebugSeverity severity, // Severity of the message.
       int length,             // Length of the string in pMessage.
       IntPtr pMessage,        // Pointer to message string.
       IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
   {
      // In order to access the string pointed to by pMessage, you can use Marshal
      // class to copy its contents to a C# string without unsafe code. You can
      // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
      string message = Marshal.PtrToStringAnsi(pMessage, length);
      // The rest of the function is up to you to implement, however a debug output
      // is always useful.
      Debug.WriteLine($"[{severity} source={source} type={type} id={id}] \n\n{message}\n\n");
      // Potentially, you may want to throw from the function for certain severity
      // messages.
      if (type == DebugType.DebugTypeError)
      {
         throw new Exception(message);
      }
   }
}
