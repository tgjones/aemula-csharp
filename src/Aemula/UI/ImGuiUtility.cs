using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;

namespace Aemula.UI;

public static unsafe class ImGuiUtility
{
    private const int StackAllocationSizeLimit = 2048;

    public static void SetupDocking()
    {
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

        // Need to reload .ini file as described in https://github.com/mellinoe/veldrid/issues/410
        ImGui.LoadIniSettingsFromDisk(ImGui.GetIO().IniFilename);
    }

    public static bool InputUInt16(string label, ref ushort value)
    {
        var input = value.ToString("X4");

        ImGui.PushItemWidth(38);

        var flags = ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.CharsUppercase | ImGuiInputTextFlags.EnterReturnsTrue;

        var result = false;
        if (ImGui.InputText(label, ref input, 4, flags))
        {
            ushort.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            result = true;
        }

        ImGui.PopItemWidth();

        return result;
    }

    public static bool InputByte(string label, ref byte value)
    {
        var input = value.ToString("X2");

        ImGui.PushItemWidth(22);

        var flags = ImGuiInputTextFlags.CharsHexadecimal | ImGuiInputTextFlags.CharsUppercase | ImGuiInputTextFlags.EnterReturnsTrue;

        var result = false;
        if (ImGui.InputText(label, ref input, 2, flags))
        {
            byte.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
            result = true;
        }

        ImGui.PopItemWidth();

        return result;
    }

    public static bool Combo(string label, ref int current_item, IReadOnlyList<string> items)
    {
        byte* native_label;
        int label_byteCount = 0;
        if (label != null)
        {
            label_byteCount = Encoding.UTF8.GetByteCount(label);
            if (label_byteCount > StackAllocationSizeLimit)
            {
                native_label = Allocate(label_byteCount + 1);
            }
            else
            {
                byte* native_label_stackBytes = stackalloc byte[label_byteCount + 1];
                native_label = native_label_stackBytes;
            }
            int native_label_offset = GetUtf8(label, native_label, label_byteCount);
            native_label[native_label_offset] = 0;
        }
        else { native_label = null; }
        int* items_byteCounts = stackalloc int[items.Count];
        int items_byteCount = 0;
        for (int i = 0; i < items.Count; i++)
        {
            string s = items[i];
            items_byteCounts[i] = Encoding.UTF8.GetByteCount(s);
            items_byteCount += items_byteCounts[i] + 1;
        }
        byte* native_items_data = stackalloc byte[items_byteCount];
        int offset = 0;
        for (int i = 0; i < items.Count; i++)
        {
            string s = items[i];
            fixed (char* sPtr = s)
            {
                offset += Encoding.UTF8.GetBytes(sPtr, s.Length, native_items_data + offset, items_byteCounts[i]);
                native_items_data[offset] = 0;
                offset += 1;
            }
        }
        byte** native_items = stackalloc byte*[items.Count];
        offset = 0;
        for (int i = 0; i < items.Count; i++)
        {
            native_items[i] = &native_items_data[offset];
            offset += items_byteCounts[i] + 1;
        }
        int popup_max_height_in_items = -1;
        fixed (int* native_current_item = &current_item)
        {
            byte ret = ImGuiNative.igComboStr_arr(native_label, native_current_item, native_items, items.Count, popup_max_height_in_items);
            if (label_byteCount > StackAllocationSizeLimit)
            {
                Free(native_label);
            }
            return ret != 0;
        }
    }

    private static byte* Allocate(int numBytes) => (byte*)Marshal.AllocHGlobal(numBytes);

    private static void Free(byte* ptr) => Marshal.FreeHGlobal((IntPtr)ptr);

    private static int GetUtf8(string s, byte* utf8Bytes, int utf8ByteCount)
    {
        fixed (char* utf16Ptr = s)
        {
            return Encoding.UTF8.GetBytes(utf16Ptr, s.Length, utf8Bytes, utf8ByteCount);
        }
    }

    public static bool ComboEnum<T>(string label, ref T value, string[] options)
        where T : Enum
    {
        var currentItem = Unsafe.As<T, int>(ref value);
        var result = ImGui.Combo(label, ref currentItem, options, options.Length);
        if (result)
        {
            value = Unsafe.As<int, T>(ref currentItem);
        }
        return result;
    }
}
