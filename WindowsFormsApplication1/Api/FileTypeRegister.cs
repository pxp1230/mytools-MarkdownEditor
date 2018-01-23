using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class FileTypeRegister
{
    public static void FileTypeReg(string extension)
    {
        bool isOk = false;
        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension))
        {
            if (key != null)
            {
                isOk = true;
            }
        }
        if (!isOk)
        {
            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", extension.Substring(1), RegistryValueKind.String);
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension.Substring(1)))
            {
                key.CreateSubKey(@"Shell\DefaultIcon").SetValue("", exePath + ",0", RegistryValueKind.ExpandString);
                key.CreateSubKey(@"Shell\Open\Command").SetValue("", exePath + " \"%1\"", RegistryValueKind.ExpandString);
            }
        }
    }
}
