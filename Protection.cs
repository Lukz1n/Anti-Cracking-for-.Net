using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;

public static class Protection
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool IsDebuggerPresent();

    private static readonly string[] UnsafeProcesses = new string[]
    {
        "dnSpy", "HTTPDebuggerSvc", "x64dbg", "x32dbg", "ida", "CheatEngine"
    };

    public static void Protect()
    {
        CheckForDebugger();
        CheckForUnsafeProcesses();
        CheckAppIntegrity();
    }

    private static void CheckForDebugger()
    {
        if (IsDebuggerPresent())
        {
            MessageBox.Show("Debugger detected. The application will be terminated.", "Net Protection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Environment.Exit(1);
        }
    }

    private static void CheckForUnsafeProcesses()
    {
        foreach (var processName in UnsafeProcesses)
        {
            if (Process.GetProcessesByName(processName).Length > 0)
            {
                MessageBox.Show($"The Process '{processName}'has been detected. The application will be terminated.", ".Net Protection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }
    }

    private static void CheckAppIntegrity()
    {
        string expectedHash = "abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890";
        string filePath = Application.ExecutablePath;

        using (var sha256 = SHA256.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                if (hashString != expectedHash)
                {
                    MessageBox.Show("Application integrity compromised. Application will be terminated.", ".Net Protection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
        }
    }
}
