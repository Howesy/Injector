using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Hexus_Injector
{
    class Inject
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
        uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes,
        uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_ALL_ACCESS = PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ;
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;

        public string CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private string process;

        public Inject(string process)
        {
            this.process = process;
        }

        public void InjectDLL(string DLLPath, string DLLName)
        {
                Process[] DesiredProcess = Process.GetProcessesByName(process);
                if (DesiredProcess.Length == 0) throw new Exception(process + " is not open!");
                if (File.Exists(Path.Combine(DLLPath, DLLName)))
                {
                    string CombinedPath = Path.Combine(DLLPath, DLLName);
                    IntPtr LoadLibraryA = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    IntPtr Handle = OpenProcess(PROCESS_ALL_ACCESS, false, DesiredProcess[0].Id);
                    IntPtr Mem = VirtualAllocEx(Handle, IntPtr.Zero, (uint)((CombinedPath.Length + 1)), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                    WriteProcessMemory(Handle, Mem, Encoding.Default.GetBytes(CombinedPath), (uint)((CombinedPath.Length + 1)), out UIntPtr bytesWritten);
                    CreateRemoteThread(Handle, IntPtr.Zero, 0, LoadLibraryA, Mem, 0, IntPtr.Zero);
                } else throw new Exception($"You're missing the {DLLName}");            
        }
    }
}