
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Normandy.Legacy.Client.Protocols
{
    public class IniDictionary : Dictionary<String, IniSection>
    {
    }

    public class IniSection : Dictionary<String, String>
    {
    }

    /// <summary>
    /// 时间转换参数
    /// </summary>
    public class ConvertTimeArg
    {
        /// <summary>
        /// 当地时间戳（优先使用时间戳TimeStamp，若时间戳为0，则使用当地时间Time）
        /// </summary>
        public int TimeStamp { get; set; } = 0;

        /// <summary>
        /// 当地时间（优先使用时间戳TimeStamp，若时间戳为0，则使用当地时间Time）
        /// </summary>
        public DateTime Time { get; set; } = new DateTime(1970, 1, 1);

        /// <summary>
        /// 当地时区
        /// </summary>
        public int TimeZone { get; set; } = -28800;

        /// <summary>
        /// 当地令时
        /// </summary>
        public bool IsDayLight { get; set; } = false;

        /// <summary>
        /// 转换地区时区
        /// </summary>
        public int TimeZoneConvert { get; set; } = -28800;

        /// <summary>
        /// 转换地区令时
        /// </summary>
        public bool IsDayLightConvert { get; set; } = false;
    }

    public static class HevoHelper
    {
        internal static void RemoveDirectory(String directory_path)
        {
            var di = new DirectoryInfo(directory_path);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        public static String EncodeClientVersion(String client_version)
        {
            String str = String.Empty;
            String key = "^=,\r\n-~()[];";
            String esc = "^ecrnstpPbBa";
            for (var i = 0; i < client_version.Length; i++)
            {
                var pindex = key.IndexOf(client_version[i]);
                if (pindex != -1)
                {
                    str += esc[0];
                    str += esc[pindex];
                }
                else
                {
                    str += client_version[i];
                }
            }
            return str;
        }

        public static IniDictionary ParseIniString(String ini_content_string)
        {
            IniDictionary ini_ictionary = null;
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.Unicode);
            writer.Write(ini_content_string);
            writer.Flush();
            stream.Position = 0;
            ini_ictionary = ParseIniStream(stream);
            stream.Dispose();
            writer.Dispose();
            return ini_ictionary;
        }

        private static IniDictionary ParseIniStream(Stream stream, ParseIniFlag parse_ini_flag = ParseIniFlag.Original)  //警告：这个函数的注释不要删除！！！！！！
        {
            // 注意，此函数并不处理引号
            const int unknown_scope = 0;
            const int section_scope = 1;
            const int key_scope = 2;
            const int value_scope = 3;

            string string_line;
            var ini_dictionary = new IniDictionary();

            using (var reader = new StreamReader(stream, GbkEncoding))
            {
                IniSection current_section = null;
                while ((string_line = reader.ReadLine()) != null)
                {
                    String section_name = String.Empty;
                    int current_scope = unknown_scope;
                    StringBuilder key = new StringBuilder();
                    StringBuilder value = new StringBuilder();
                    foreach (char c in string_line)
                    {
                        if (current_scope == unknown_scope)
                        {
                            if (c == ' ')
                            {
                                continue;
                            }
                            else if (c == '[')
                            {
                                current_scope = section_scope;
                                continue;
                            }
                            else
                            {
                                current_scope = key_scope;
                                key.Append(c);
                            }
                        }
                        else if (current_scope == section_scope)
                        {
                            if (c == ']' || c == ';')
                            {
                                current_scope = unknown_scope;
                                current_section = new IniSection();
                                if (!ini_dictionary.ContainsKey(section_name))
                                {
                                    ini_dictionary.Add(section_name, current_section);
                                }

                                break;
                            }
                            else
                            {
                                section_name += c;
                            }
                        }
                        else if (current_scope == key_scope)
                        {
                            if (c == '=')
                            {
                                current_scope = value_scope;
                                continue;
                            }
                            else
                            {
                                key.Append(c);
                            }
                        }
                        else if (current_scope == value_scope)
                        {
                            value.Append(c);
                        }
                    }

                    if (current_scope == value_scope)
                    {
                        if (parse_ini_flag == ParseIniFlag.Original)
                        {
                            if (!current_section.ContainsKey(key.ToString()))
                            {
                                current_section.Add(key.ToString(), value.ToString());
                            }
                        }
                        else if (parse_ini_flag == ParseIniFlag.TrimSpace)
                        {
                            if (!current_section.ContainsKey(key.ToString()))
                            {
                                current_section.Add(key.ToString(), value.ToString().Trim());
                            }
                        }
                    }
                }
            }
            return ini_dictionary;
        }

        public enum ParseIniFlag
        {
            Original,
            TrimSpace
        };

        public static IniDictionary ParseIniFile(String ini_file_path, ParseIniFlag parse_ini_flag = ParseIniFlag.Original)
        {
            IniDictionary ini_dic;
            if (!File.Exists(ini_file_path))
            {
                return new IniDictionary();
            }
            try
            {
                using (var stream = File.OpenRead(ini_file_path))
                {
                    ini_dic = ParseIniStream(stream);
                }
            }
            catch (System.Exception)
            {
                return new IniDictionary();
            }
            return ini_dic;
        }

        internal static void DumpToFile(String file_name, byte[] bytes, int offset = 0)
        {
            using (var fs = new FileStream(file_name, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes, offset, bytes.Length - offset);
            };
        }

        /// <summary>
        /// 将时间戳转换成其他地区时间（供请求回来的时间使用，推送回来的时间不允许使用该方法转换）
        /// </summary>
        /// <param name="arg">时间转换参数</param>
        /// <returns></returns>
        public static DateTime TimeToOtherDateTime(ConvertTimeArg arg)
        {
            if (arg.TimeStamp == 0)
            {
                return DateTimeToOtherDateTime(arg);
            }
            else
            {
                return UnixTimeStampToOtherDateTime(arg);
            }
        }

        /// <summary>
        /// 将时间戳转换成其他地区时间（供请求回来的时间使用，推送回来的时间不允许使用该方法转换）
        /// </summary>
        /// <param name="arg">时间转换参数</param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToOtherDateTime(ConvertTimeArg arg)
        {
            var time = UnixTimeStampToDateTime(arg.TimeStamp);
            var total_seconds = (-arg.TimeZoneConvert + (arg.IsDayLightConvert ? 3600 : 0)) - (-arg.TimeZone + (arg.IsDayLight ? 3600 : 0));
            return time.AddSeconds(total_seconds);
        }

        /// <summary>
        /// 将时间戳转换成其他地区时间（供请求回来的时间使用，推送回来的时间不允许使用该方法转换）
        /// </summary>
        /// <param name="arg">时间转换参数</param>
        /// <returns></returns>
        public static DateTime DateTimeToOtherDateTime(ConvertTimeArg arg)
        {
            var total_seconds = (-arg.TimeZoneConvert + (arg.IsDayLightConvert ? 3600 : 0)) - (-arg.TimeZone + (arg.IsDayLight ? 3600 : 0));
            return arg.Time.AddSeconds(total_seconds);
        }

        public static DateTime StringToDatetime(String time)
        {
            return DateTime.ParseExact(time, "yyyyMMddHHmm", CultureInfo.CurrentCulture);
        }

        public static DateTime UnixTimeStampToDateTime(Int64 time_stamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(time_stamp);
            return dtDateTime;
        }

        public static Int64 DateTimeToUnixTimeStamp(DateTime date_time)
        {
            var sTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
            return (long)(date_time - sTime).TotalSeconds;
        }

        /// <summary>
        /// 将时间戳转换成其他地区时间（供推送回来的时间使用，请求回来的时间不允许使用该方法转换）
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static DateTime PushTimeStampToDateTime(ConvertTimeArg arg)
        {
            var time = ToDateTime(arg.TimeStamp, arg.TimeZone, arg.IsDayLight);
            var total_seconds = (-arg.TimeZoneConvert + (arg.IsDayLightConvert ? 3600 : 0)) - (-arg.TimeZone + (arg.IsDayLight ? 3600 : 0));
            return time.AddSeconds(total_seconds);
        }

        public static DateTime ToDateTime(Int64 timestamp, Int32 timezone, bool isdaylight)
        {
            DateTime date = DateTime.SpecifyKind(new DateTime(1970, 1, 1, 0, 0, 0), DateTimeKind.Utc);
            var time = date.AddSeconds(timestamp);
            var total_seconds = -timezone + (isdaylight ? 3600 : 0);
            return time.AddSeconds(total_seconds);
        }

        public static Int64 ToTimeStamp(DateTime time, Int32 timezone, bool isdaylight)
        {
            DateTime start_time = DateTime.SpecifyKind(new DateTime(1970, 1, 1, 0, 0, 0), DateTimeKind.Utc);
            var t = (time - start_time).TotalSeconds;
            var total_seconds = t + timezone - (isdaylight ? 3600 : 0);
            return (Int64)total_seconds;
        }

        public static Int64 ToForeignTimeStamp(DateTime time)
        {
            var t = (Int64)(time - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            return t + 28800;
        }

        public static Int64 DateTimeToLocalUnixTimeStamp(DateTime date_time)
        {
            var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            return (long)(date_time - sTime).TotalSeconds;
        }

        unsafe internal static String GetStringFromBytePointer(byte* ptr, Encoding encoding, int length = int.MaxValue)
        {
            if (length == int.MaxValue)
            {
                var str_length = GetStringLengthFromPointer(ptr);
                var bytes = BytePointerToByteArray(ptr, str_length);
                var str = encoding.GetString(bytes);
                return str;
            }
            else
            {
                var bytes = BytePointerToByteArray(ptr, length);
                var str = encoding.GetString(bytes);
                return str;
            }
        }

        unsafe internal static String GetStringFromBytePointerV2(byte* ptr, Encoding encoding, int length)
        {
            var bytes = BytePointerToByteArray(ptr, length);
            if (IsRight(bytes)) // 如果有FFFFFFF就是有问题
            {
                bytes = BytePointerToByteArrayWithEndChar(ptr, length);
                fixed (byte* start = bytes)
                {
                    var str_length = GetStringLengthFromPointer(start);
                    bytes = BytePointerToByteArray(start, str_length);
                    var str = encoding.GetString(bytes);
                    return str;
                }
            }
            return String.Empty;
        }

        public static bool IsRight(Byte[] bytes)
        {
            return bytes.Any(b => b != 255);
        }

        unsafe internal static Int32 GetStringLengthFromPointer(byte* byte_ptr)
        {
            byte* s = byte_ptr;
            byte* p = s;
            while ((*s) != 0) ++s;
            return (Int32)(s - p);
        }

        unsafe internal static Int32 GetStringLength(byte[] bytes, int start_index)
        {
            fixed (byte* ptr = bytes)
            {
                return GetStringLengthFromPointer(ptr + start_index);
            }
        }

        internal static UInt32 SubBitsToUInt32(BitArray source_bits, int start_bit_index, int sub_bit_count)
        {
            // fill other zero
            var sub_bits = new BitArray(sub_bit_count);
            for (int i = start_bit_index, j = 0; i < start_bit_index + sub_bit_count; ++i, ++j)
            {
                sub_bits[j] = source_bits[i];
            }

            var uint32_array = new Int32[1];
            sub_bits.CopyTo(uint32_array, 0);
            var result = uint32_array[0];
            return (UInt32)result;
        }

        internal static Int32 GetHeadSizeByBit(BitArray hight, BitArray low)
        {
            var sub_bits = new BitArray(low.Count + 2);
            for (int i = 0; i < low.Count; i++)
            {
                sub_bits[i] = low[i];
            }
            sub_bits[low.Count] = hight[hight.Count - 2];
            sub_bits[low.Count + 1] = hight[hight.Count - 1];

            var uint32_array = new Int32[1];
            sub_bits.CopyTo(uint32_array, 0);
            var result = uint32_array[0];
            return result;
        }

        internal static Int32 GetHeadSizeByByte(BitArray hightBytes, BitArray lowBytes)
        {
            var sub_bits = new BitArray(lowBytes.Count + hightBytes.Count);
            for (int i = 0; i < lowBytes.Count; i++)
            {
                sub_bits[i] = lowBytes[i];
            }
            for (int i = 0; i < hightBytes.Count; i++)
            {
                sub_bits[lowBytes.Count + i] = hightBytes[i];
            }
            var uint32_array = new Int32[1];
            sub_bits.CopyTo(uint32_array, 0);
            var result = uint32_array[0];
            return result;
        }

        internal static byte[] SubBytes(byte[] source_bytes, int start_index, int sub_length)
        {
            var sub_bytes = new byte[sub_length];
            Array.Copy(source_bytes, start_index, sub_bytes, 0, sub_length);
            return sub_bytes;
        }

        internal static List<T> BytesToMultipleStruct<T>(byte[] arr, int start_index = 0, int count = int.MaxValue)
                                where T : struct
        {
            var type = typeof(T);
            var struct_size = Marshal.SizeOf(type);
            int object_count = count;
            if (object_count == int.MaxValue)
            {
                object_count = arr.Length / struct_size;
            }
            var struct_objects = new List<T>(object_count);
            unsafe
            {
                fixed (byte* ptr = arr)
                {
                    for (int i = 0; i < object_count; ++i)
                    {
                        var struct_object = (T)Marshal.PtrToStructure(new IntPtr(ptr + start_index + struct_size * i), type);
                        struct_objects.Add(struct_object);
                    }
                }
            }
            return struct_objects;
        }

        internal static T BytesToStruct<T>(byte[] arr, int start_index = 0)
            where T : struct
        {
            unsafe
            {
                var type = typeof(T);
                fixed (byte* ptr = arr)
                {
                    return (T)Marshal.PtrToStructure(new IntPtr(ptr + start_index), type);
                }
            }
        }

        public static byte[] StructToBytes<T>(T struct_object)
            where T : struct
        {
            var struct_size = Marshal.SizeOf(struct_object);
            var bytes = new byte[struct_size];
            var heap_ptr = Marshal.AllocHGlobal(struct_size);
            Marshal.StructureToPtr(struct_object, heap_ptr, true);
            Marshal.Copy(heap_ptr, bytes, 0, struct_size);
            Marshal.FreeHGlobal(heap_ptr);
            return bytes;
        }

        internal static Dictionary<String, String> ParseKeyValueString(String kv_string, bool ignore_no_value = true, params char[] seperators)
        {
            var dictionary = new Dictionary<String, String>();
            var tokens = kv_string.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                //var kv = token.Split('=');
                var index = token.IndexOf('='); //发送的数据有可能是Base64加密，Base64加密数据包含字符'='
                if (index > 0)
                {
                    var key = token.Substring(0, index);
                    var value = token.Substring(index + 1);
                    dictionary[key] = value;
                }
                else
                {
                    if (!ignore_no_value)
                    {
                        dictionary[token] = String.Empty;
                    }
                }
            }
            return dictionary;
        }

        

        internal static String DictionaryToIniString(Dictionary<String, String> dictionary, String seperator = "\n")
        {
            String result = String.Empty;

            foreach (var pair in dictionary)
            {
                result += (pair.Key + "=" + pair.Value + seperator);
            }
            return result;
        }

        public static Encoding GbkEncoding { get; } = Encoding.GetEncoding("GB2312");

        public static byte[] CombineBytes(params byte[][] many_bytes)
        {
            int total_length = many_bytes.Sum(bytes => bytes.Length);

            byte[] result = new byte[total_length];

            int offset = 0;
            foreach (var bytes in many_bytes)
            {
                bytes.CopyTo(result, offset);
                offset += bytes.Length;
            }
            return result;
        }

        public static byte[] EncodeSignature(byte[] source_signature)
        {
            byte[] str_dest = new byte[source_signature.Length / 2];
            Int32 i = 0, j = 0;
            for (i = 0, j = 0; i < source_signature.Length; i++)
            {
                j = 2 * i;
                if (j >= (source_signature.Length - 1))
                {
                    break;
                }

                Int32 n1 = source_signature[j] - 'A';
                Int32 n2 = source_signature[j + 1] - 'A';
                byte cData = (byte)(n2 * 16 + n1);
                str_dest[i] = cData;
            }
            return str_dest;
        }

        internal static String MakeQueryString(IDictionary<String, String> kvs)
        {
            String full_url = String.Empty;
            foreach (var kv in kvs)
            {
                full_url += (kv.Key + "=" + kv.Value + "&");
            }
            if (full_url[full_url.Length - 1] == '&')
            {
                full_url = full_url.Remove(full_url.Length - 1);
            }
            return full_url;
        }

        internal static String MakeUrl(String base_url, Dictionary<String, String> kvs)
        {
            String full_url = base_url;
            full_url += "?";
            full_url += MakeQueryString(kvs);
            return full_url;
        }

        internal static String SelfStockBase64Encode(byte[] data, string encodingName = "ASCII")
        {
            var size = data.Length;
            const byte padding = (byte)'=';
            String encode_dictionary_str = "oPbsG4EvU8gyd02B3q6fIVWXYZaCcMeTKhxnwzmjApRrDtuHkiLlN1O9F5S7JQ+/\0";
            byte[] encode_dictionary = GbkEncoding.GetBytes(encode_dictionary_str);

            byte[] result = new byte[((size + 2) / 3) * 4];

            int current_index = 0;
            int result_current_index = 0;

            while (size > 2)
            {
                result[result_current_index++] = encode_dictionary[data[current_index] >> 2];
                result[result_current_index++] = encode_dictionary[((data[current_index] & 0x03) << 4) + (data[current_index + 1] >> 4)];
                result[result_current_index++] = encode_dictionary[((data[current_index + 1] & 0x0f) << 2) + (data[current_index + 2] >> 6)];
                result[result_current_index++] = encode_dictionary[data[current_index + 2] & 0x3f];
                current_index += 3;
                size -= 3;
            }

            if (size != 0)
            {
                result[result_current_index++] = encode_dictionary[data[current_index] >> 2];
                if (size > 1)
                {
                    result[result_current_index++] = encode_dictionary[((data[current_index] & 0x03) << 4) + (data[current_index + 1] >> 4)];
                    result[result_current_index++] = encode_dictionary[(data[current_index + 1] & 0x0f) << 2];
                    result[result_current_index++] = padding;
                }
                else
                {
                    result[result_current_index++] = encode_dictionary[(data[current_index] & 0x03) << 4];
                    result[result_current_index++] = padding;
                    result[result_current_index++] = padding;
                }
            }
            return Encoding.GetEncoding(encodingName).GetString(result);
        }

        internal static byte[] SelfStockBase64Decode(String data_string)
        {
            byte[] src_data = GbkEncoding.GetBytes(data_string);
            Int32 src_length = src_data.Length;
            Int32 dst_length = src_length + 1;
            byte[] dst_data = new byte[dst_length];

            int dst_index = 0;

            sbyte[] m_sBase64Decode =
            {
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
                13, 53, 14, 16,  5, 57, 18, 59,  9, 55, -1, -1, -1, -1, -1, -1,
                -1, 40, 15, 27, 44,  6, 56,  4, 47, 20, 60, 32, 50, 29, 52, 54,
                 1, 61, 42, 58, 31,  8, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
                -1, 26,  2, 28, 12, 30, 19, 10, 33, 49, 39, 48, 51, 38, 35,  0,
                41, 17, 43,  3, 45, 46,  7, 36, 34, 11, 37, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            };

            var start_index = 0;
            var end_index = start_index + src_length;

            Int32 written = 0;

            bool is_overflow = (dst_data == null) ? true : false;

            while (start_index < end_index && src_data[start_index] != 0)
            {
                var curr = 0;
                Int32 i;
                Int32 bits = 0;
                for (i = 0; i < 4; i++)
                {
                    if (start_index >= end_index)
                    {
                        break;
                    }
                    Int32 nCh = m_sBase64Decode[src_data[start_index]];
                    start_index++;
                    if (nCh == -1)
                    {
                        i--;
                        continue;
                    }
                    curr <<= 6;
                    curr |= nCh;
                    bits += 6;
                }

                if (!is_overflow && written + (bits / 8) > dst_length)
                    is_overflow = true;

                curr <<= 24 - bits;
                for (i = 0; i < bits / 8; i++)
                {
                    if (!is_overflow)
                    {
                        dst_data[dst_index] = (byte)((curr & 0x00ff0000) >> 16);
                        dst_index++;
                    }
                    curr <<= 8;
                    written++;
                }
            }

            dst_length = written;
            return dst_data;
        }

        internal static byte[] SelfStockBase64Decode(String data_string, ref int length)
        {
            byte[] src_data = GbkEncoding.GetBytes(data_string);
            Int32 src_length = src_data.Length;
            length = src_length + 1;
            byte[] dst_data = new byte[length];

            int dst_index = 0;

            sbyte[] m_sBase64Decode =
            {
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
                13, 53, 14, 16,  5, 57, 18, 59,  9, 55, -1, -1, -1, -1, -1, -1,
                -1, 40, 15, 27, 44,  6, 56,  4, 47, 20, 60, 32, 50, 29, 52, 54,
                 1, 61, 42, 58, 31,  8, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
                -1, 26,  2, 28, 12, 30, 19, 10, 33, 49, 39, 48, 51, 38, 35,  0,
                41, 17, 43,  3, 45, 46,  7, 36, 34, 11, 37, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            };

            var start_index = 0;
            var end_index = start_index + src_length;

            Int32 written = 0;

            bool is_overflow = (dst_data == null) ? true : false;

            while (start_index < end_index && src_data[start_index] != 0)
            {
                var curr = 0;
                Int32 i;
                Int32 bits = 0;
                for (i = 0; i < 4; i++)
                {
                    if (start_index >= end_index)
                    {
                        break;
                    }
                    Int32 nCh = m_sBase64Decode[src_data[start_index]];
                    start_index++;
                    if (nCh == -1)
                    {
                        i--;
                        continue;
                    }
                    curr <<= 6;
                    curr |= nCh;
                    bits += 6;
                }

                if (!is_overflow && written + (bits / 8) > length)
                    is_overflow = true;

                curr <<= 24 - bits;
                for (i = 0; i < bits / 8; i++)
                {
                    if (!is_overflow)
                    {
                        dst_data[dst_index] = (byte)((curr & 0x00ff0000) >> 16);
                        dst_index++;
                    }
                    curr <<= 8;
                    written++;
                }
            }

            length = written;
            return dst_data;
        }

        public static byte[] HexinBase64Decode(String data_string, ref Int32 retlength)
        {
            byte[] src_data = GbkEncoding.GetBytes(data_string);
            Int32 src_length = src_data.Length;
            Int32 dst_length = src_length + 1;
            byte[] dst_data = new byte[dst_length];
            var nLen = src_length;
            var scr_index = 0;
            int ch = 0, i = 0, j = 0, k = 0;    // modified by wuping on 2012.2.13 for 源码自动检测
            /* this sucks for threaded environments */

            sbyte[] m_sBase64Decode =
            {
              -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
        52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
        -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
        15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
        -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
            };

            /* run through the whole string, converting as we go */
            while (scr_index < src_data.Length && (ch = src_data[scr_index++]) != '\0' && nLen-- > 0)
            {
                if (ch == '=') { break; }  // 代码检测修复 [3/16/2012 chenggao]

                /* When Base64 gets POSTed, all pluses are interpreted as spaces.
                This line changes them back.  It's not exactly the Base64 spec,
                but it is completely compatible with it (the spec says that
                spaces are invalid).  This will also save many people considerable
                headache.  - Turadg Aleahmad <turadg@wise.berkeley.edu>
                */

                if (ch == ' ') { ch = '+'; }    // 代码检测修复 [3/16/2012 chenggao]

                ch = m_sBase64Decode[ch];
                if (ch < 0) { continue; }       // 代码检测修复 [3/16/2012 chenggao]

                switch (i % 4)
                {
                    case 0:
                        dst_data[j] = (byte)(ch << 2); // vc6->vc10
                        break;

                    case 1:
                        dst_data[j++] |= (byte)(ch >> 4);
                        dst_data[j] = (byte)((ch & 0x0f) << 4);
                        break;

                    case 2:
                        dst_data[j++] |= (byte)(ch >> 2);
                        dst_data[j] = (byte)((ch & 0x03) << 6);
                        break;

                    case 3:
                        dst_data[j++] |= (byte)ch;
                        break;
                }
                i++;
            }

            k = j;
            /* mop things up if we ended on a boundary */
            if (ch == '=')
            {
                switch (i % 4)
                {
                    case 1:
                        // mod by huangguihua on 2011-10-13
                        //delete m_pDecodeBuffer;
                        // mod ends
                        return null;

                    case 2:
                        k++;
                        dst_data[k++] = 0;
                        break;

                    case 3:
                        dst_data[k++] = 0;
                        break;
                }
            }
            retlength = j;
            dst_data[j] = (byte)'\0';
            return dst_data;
        }

        public static String HexinBase64SpecialEncode(byte[] data)
        {
            var result = HexinBase64SpecialEncodeBytes(data);
            return GbkEncoding.GetString(result);
        }

        public static byte[] HexinBase64SpecialEncodeBytes(byte[] data)
        {
            const byte padding = (byte)'=';
            Int32 size = data.Length;
            byte[] result = new byte[((size + 2) / 3) * 4];
            String encode_dictionary_str = "oPbsG4EvU8gyd02B3q6fIVWXYZaCcMeTKhxnwzmjApRrDtuHkiLlN1O9F5S7JQ+/\0";
            byte[] encode_dictionary = GbkEncoding.GetBytes(encode_dictionary_str);
            // modification end.

            int current_index = 0;
            int result_current_index = 0;

            while (size > 2)
            {
                result[result_current_index++] = encode_dictionary[data[current_index] >> 2];
                result[result_current_index++] = encode_dictionary[((data[current_index] & 0x03) << 4) + (data[current_index + 1] >> 4)];
                result[result_current_index++] = encode_dictionary[((data[current_index + 1] & 0x0f) << 2) + (data[current_index + 2] >> 6)];
                result[result_current_index++] = encode_dictionary[data[current_index + 2] & 0x3f];
                current_index += 3;
                size -= 3;
            }

            if (size != 0)
            {
                result[result_current_index++] = encode_dictionary[data[current_index] >> 2];
                if (size > 1)
                {
                    result[result_current_index++] = encode_dictionary[((data[current_index] & 0x03) << 4) + (data[current_index + 1] >> 4)];
                    result[result_current_index++] = encode_dictionary[(data[current_index + 1] & 0x0f) << 2];
                    result[result_current_index++] = padding;
                }
                else
                {
                    result[result_current_index++] = encode_dictionary[(data[current_index] & 0x03) << 4];
                    result[result_current_index++] = padding;
                    result[result_current_index++] = padding;
                }
            }
            return result;
        }

        public static String HexinBase64Encode(byte[] data)
        {
            var size = data.Length;
            const byte padding = (byte)'=';
            String encode_dictionary_str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            byte[] encode_dictionary = GbkEncoding.GetBytes(encode_dictionary_str);

            byte[] result = new byte[((size + 2) / 3) * 4];

            int current_index = 0;
            int result_current_index = 0;

            while (size > 2)
            {
                result[result_current_index++] = encode_dictionary[data[current_index] >> 2];
                result[result_current_index++] = encode_dictionary[((data[current_index] & 0x03) << 4) + (data[current_index + 1] >> 4)];
                result[result_current_index++] = encode_dictionary[((data[current_index + 1] & 0x0f) << 2) + (data[current_index + 2] >> 6)];
                result[result_current_index++] = encode_dictionary[data[current_index + 2] & 0x3f];
                current_index += 3;
                size -= 3;
            }

            if (size != 0)
            {
                result[result_current_index++] = encode_dictionary[data[current_index] >> 2];
                if (size > 1)
                {
                    result[result_current_index++] = encode_dictionary[((data[current_index] & 0x03) << 4) + (data[current_index + 1] >> 4)];
                    result[result_current_index++] = encode_dictionary[(data[current_index + 1] & 0x0f) << 2];
                    result[result_current_index++] = padding;
                }
                else
                {
                    result[result_current_index++] = encode_dictionary[(data[current_index] & 0x03) << 4];
                    result[result_current_index++] = padding;
                    result[result_current_index++] = padding;
                }
            }
            return GbkEncoding.GetString(result);
        }

        internal static byte PutUrl(byte c)
        {
            if (c < 10)
            {
                return Convert.ToByte(Convert.ToByte('0') + c);
            }
            else
            {
                return Convert.ToByte(Convert.ToByte('A') + (c - 10));
            }
        }

        public static String EncodeUrl(String src)
        {
            String ret = String.Empty;
            var buf_cn = new char[2];

            foreach (var c in src)
            {
                if ((c >= 'a' && c <= 'z')
                    || (c >= 'A' && c <= 'Z')
                    || (c >= '0' && c <= '9')
                    || c == '-'
                    || c == '_')
                {
                    ret += c;
                }
                else if (c == ' ')
                {
                    ret += '+';
                }
                else
                {
                    if (c != 0)
                    {
                        ret += '%';
                    }

                    var byte_c = Convert.ToByte(c);

                    buf_cn[0] = Convert.ToChar(PutUrl(Convert.ToByte(byte_c / 16)));
                    buf_cn[1] = Convert.ToChar(PutUrl(Convert.ToByte(byte_c % 16)));
                    ret += new String(buf_cn);
                }
            }
            return ret;
        }

        internal static byte[] HexStringToBytes(String hex_string)
        {
            String fixed_hex_string = hex_string;

            if (fixed_hex_string.Length % 2 != 0)
            {
                fixed_hex_string = "0" + fixed_hex_string;
            }

            var byte_count = fixed_hex_string.Length / 2;
            var bytes = new byte[byte_count];
            for (int i = 0; i < fixed_hex_string.Length / 2; ++i)
            {
                bytes[i] = Convert.ToByte(fixed_hex_string.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        internal static byte[] Encrypt(byte[] original_data, RSAParameters rsa_parameters, bool is_do_oaep_padding)
        {
            try
            {
                byte[] encrypted_data;
                using (var rsa_provider = new RSACryptoServiceProvider())
                {
                    rsa_provider.ImportParameters(rsa_parameters);
                    encrypted_data = rsa_provider.Encrypt(original_data, is_do_oaep_padding);
                }
                return encrypted_data;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        internal static byte[] Decrypt(byte[] encrypted_data, RSAParameters rsa_parameters, bool is_do_oaep_padding)
        {
            try
            {
                byte[] decrypted_data;
                using (var rsa_provider = new RSACryptoServiceProvider())
                {
                    rsa_provider.ImportParameters(rsa_parameters);
                    decrypted_data = rsa_provider.Decrypt(encrypted_data, is_do_oaep_padding);
                }
                return decrypted_data;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        unsafe internal static byte[] BytePointerToByteArray(byte* source, int length)
        {
            byte[] arr = new byte[length];
            Marshal.Copy((IntPtr)source, arr, 0, length);
            return arr;
        }

        unsafe internal static byte[] BytePointerToByteArrayWithEndChar(byte* source, int length)
        {
            byte[] arr = new byte[length + 1];
            Marshal.Copy((IntPtr)source, arr, 0, length);
            arr[length] = 0;
            return arr;
        }
    }
}