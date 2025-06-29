using System.Runtime.InteropServices;

namespace CokeOvenSystem.Services
{
    public static class NativeInterop
    {
        // 使用宽字符版本初始化函数
        [DllImport("coke_oven_system.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int coke_system_init_wide(string dbPath);

        // 原始函数用于非 Windows 平台
        [DllImport("coke_oven_system.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int coke_system_init(string dbPath);

        [DllImport("coke_oven_system.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int record_temperature(
            int coke_oven,
            string time,
            double machine_temp,
            double coke_temp);

        [DllImport("coke_oven_system.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int record_operation(
            int coke_oven,
            string chamber,
            string op_type,
            string time);

        [DllImport("coke_oven_system.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void coke_system_shutdown();

        [DllImport("coke_oven_system.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get_last_error();

        // 根据平台选择合适的初始化函数
        public static int InitSystem(string dbPath)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return coke_system_init_wide(dbPath);
            }
            else
            {
                return coke_system_init(dbPath);
            }
        }
    }
}