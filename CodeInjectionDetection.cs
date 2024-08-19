using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Net_Protector.Class.Advanced
{
    internal class CodeInjectionDetection
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UIntPtr VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, UIntPtr dwLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        public static void DetectCodeInjection()
        {
            IntPtr baseAddress = Process.GetCurrentProcess().MainModule.BaseAddress;
            MEMORY_BASIC_INFORMATION memInfo;

            if (VirtualQuery(baseAddress, out memInfo, (UIntPtr)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))) != UIntPtr.Zero)
            {
                // Verifica se a proteção de memória é EXECUTE_READWRITE, o que pode indicar injeção de código
                if (memInfo.Protect == PAGE_EXECUTE_READWRITE)
                {
                    MessageBox.Show("Possível injeção de código detectada. O aplicativo será encerrado.", "Proteção Avançada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
        }
    }
}
