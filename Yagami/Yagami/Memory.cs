using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Yagami
{
    public class Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        public Memory(string processName)
        {
            Process process = Process.GetProcessesByName(processName)[0];
            this.pHandle = Memory.OpenProcess(2035711U, false, process.Id);
            if (this.pHandle == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }
        }
        private byte[] ReadBytes(IntPtr pOffset, uint pSize)
        {
            byte[] result;
            try
            {
                uint flNewProtect;
                Memory.VirtualProtectEx(this.pHandle, pOffset, (UIntPtr)pSize, 4U, out flNewProtect);
                byte[] array = new byte[(int)((IntPtr)((long)((ulong)pSize)))];
                Memory.ReadProcessMemory(this.pHandle, pOffset, array, pSize, 0U);
                Memory.VirtualProtectEx(this.pHandle, pOffset, (UIntPtr)pSize, flNewProtect, out flNewProtect);
                result = array;
            }
            catch
            {
                result = new byte[1];
            }
            return result;
        }
        private bool WriteBytes(IntPtr pOffset, byte[] pBytes)
        {
            bool result;
            try
            {
                uint flNewProtect;
                Memory.VirtualProtectEx(this.pHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), 4U, out flNewProtect);
                bool flag = Memory.WriteProcessMemory(this.pHandle, pOffset, pBytes, (uint)pBytes.Length, 0U);
                Memory.VirtualProtectEx(this.pHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), flNewProtect, out flNewProtect);
                result = flag;
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public unsafe T Read<T>(int address)
        {
            int size = Memory.MarshalCache<T>.Size;
            fixed (byte* ptr = this.ReadBytes((IntPtr)address, (uint)size))
            {
                return Marshal.PtrToStructure<T>((IntPtr)((void*)ptr));
            }
        }
        public T[] Read<T>(int address, int count)
        {
            int size = Memory.MarshalCache<T>.Size;
            T[] array = new T[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = this.Read<T>(address + i * size);
            }
            return array;
        }
        public unsafe void Write<T>(int address, T value)
        {
            byte[] array = new byte[Memory.MarshalCache<T>.Size];
            fixed (byte* ptr = array)
            {
                Marshal.StructureToPtr<T>(value, (IntPtr)((void*)ptr), true);
            }
            this.WriteBytes((IntPtr)address, array);
        }
        public void Dispose()
        {
            if (!Memory.CloseHandle(this.pHandle))
            {
                throw new NullReferenceException();
            }
        }

        private readonly IntPtr pHandle;
        private enum VirtualMemoryProtection : uint
        {
            PAGE_NOACCESS = 1U,
            PAGE_READONLY,
            PAGE_READWRITE = 4U,
            PAGE_WRITECOPY = 8U,
            PAGE_EXECUTE = 16U,
            PAGE_EXECUTE_READ = 32U,
            PAGE_EXECUTE_READWRITE = 64U,
            PAGE_EXECUTE_WRITECOPY = 128U,
            PAGE_GUARD = 256U,
            PAGE_NOCACHE = 512U,
            PROCESS_ALL_ACCESS = 2035711U
        }
        private static class MarshalCache<T>
        {
            unsafe static MarshalCache()
            {
                if (typeof(T) == typeof(bool))
                {
                    Memory.MarshalCache<T>.Size = 1;
                    Memory.MarshalCache<T>.RealType = typeof(T);
                }
                else if (typeof(T).IsEnum)
                {
                    Type enumUnderlyingType = typeof(T).GetEnumUnderlyingType();
                    Memory.MarshalCache<T>.Size = Marshal.SizeOf(enumUnderlyingType);
                    Memory.MarshalCache<T>.RealType = enumUnderlyingType;
                    Memory.MarshalCache<T>.TypeCode = Type.GetTypeCode(enumUnderlyingType);
                }
                else
                {
                    Memory.MarshalCache<T>.Size = Marshal.SizeOf(typeof(T));
                    Memory.MarshalCache<T>.RealType = typeof(T);
                }
                Memory.MarshalCache<T>.TypeRequiresMarshal = Memory.MarshalCache<T>.RealType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((FieldInfo m) => m.GetCustomAttributes(typeof(MarshalAsAttribute), true).Any<object>());
                DynamicMethod dynamicMethod = new DynamicMethod(string.Format("GetPinnedPtr<{0}>", typeof(T).FullName.Replace(".", "<>")), typeof(void*), new Type[]
                {
                typeof(T).MakeByRefType()
                }, typeof(Memory.MarshalCache<>).Module);
                ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
                ilgenerator.Emit(OpCodes.Ldarg_0);
                ilgenerator.Emit(OpCodes.Conv_U);
                ilgenerator.Emit(OpCodes.Ret);
                Memory.MarshalCache<T>.GetUnsafePtr = (Memory.MarshalCache<T>.GetUnsafePtrDelegate)dynamicMethod.CreateDelegate(typeof(Memory.MarshalCache<T>.GetUnsafePtrDelegate));
            }

            public static readonly int Size;

            public static readonly Type RealType;

            public static TypeCode TypeCode = Type.GetTypeCode(typeof(T));

            public static bool TypeRequiresMarshal;

            internal static readonly Memory.MarshalCache<T>.GetUnsafePtrDelegate GetUnsafePtr;

            internal unsafe delegate void* GetUnsafePtrDelegate(ref T value);
        }
    }
}
