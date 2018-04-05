using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
            if (IsAdministrator())
            {
                Registry.ClassesRoot.CreateSubKey(extension).SetValue("", extension.Substring(1), RegistryValueKind.String);
                //string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                string exePath = @"C:\Windows\MarkdownEditor.exe";
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension.Substring(1)))
                {
                    key.CreateSubKey(@"Shell\DefaultIcon").SetValue("", exePath + ",0", RegistryValueKind.ExpandString);
                    key.CreateSubKey(@"Shell\Open\Command").SetValue("", exePath + " \"%1\"", RegistryValueKind.ExpandString);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("首次运行时需要以管理员身份运行，以写入注册表关联.md文件。", "首次运行提醒", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }

    /// <summary>
    /// 确定当前主体是否属于具有指定 Administrator 的 Windows 用户组
    /// </summary>
    /// <returns>如果当前主体是指定的 Administrator 用户组的成员，则为 true；否则为 false。</returns>
    public static bool IsAdministrator()
    {
        bool result;
        try
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            result = principal.IsInRole(WindowsBuiltInRole.Administrator);

            //http://www.cnblogs.com/Interkey/p/RunAsAdmin.html
            //AppDomain domain = Thread.GetDomain();
            //domain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            //WindowsPrincipal windowsPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            //result = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            result = false;
        }
        return result;
    }
}
