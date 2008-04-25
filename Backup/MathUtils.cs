using System;
using System.Collections;
using System.Text;
using System.Globalization;

namespace ea
{

    /// <summary>
    /// Static Math functions that I find very useful
    /// </summary>
    public class MathUtils
    {

        /// <summary>
        /// Keeps track of if you use big endian or little endian math
        /// </summary>
        private static Endian defaultEndian = Endian.Big;
        /// <summary>
        /// Allows setting of the default endian value used in the hex functions, be careful with this...
        /// </summary>
        public static Endian DefaultEndian
        {
            get { return defaultEndian; }
            set { defaultEndian = value; }
        }

        /// <summary>
        /// Should be used for functions that require a random number, this way you won't keep getting the same "random" numbers
        /// </summary>
        static System.Random rand = new System.Random();

        /// <summary>
        /// Used with the Hex() function
        /// </summary>
        public enum ByteLength
        {
            Byte = 1,
            Word = 2,
            DWord = 4,
            QWord = 8,
            Word16 = 16,
            Word32 = 32
        }

        /// <summary>
        /// Specify if a byte array is little or big endian
        /// </summary>
        public enum Endian
        {
            Big = 1,
            Little = 2
        }

        /// <summary>
        /// Round a number, uses standard rounding rules: <= .5 rounds down > .5 rounds up
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static int Round(double Number)
        {
            return ((int)(Number + .4));
        }

        /// <summary>
        /// Tells you if a number is ODD
        /// </summary>
        /// <param name="Number">The number to test</param>
        /// <returns>TRUE if the number is ODD, FALSE if it is EVEN</returns>
        public static bool Odd(long Number)
        {
            return (!Even(Number));
        }

        /// <summary>
        /// Tells you if a number is EVEN
        /// </summary>
        /// <param name="Number">The number to test</param>
        /// <returns>TRUE if the number is EVEN, FALSE if it is ODD</returns>
        public static bool Even(long Number)
        {
            return ((double)Number / 2 == ((int)Number / 2));
        }

        /// <summary>
        /// Converts a byte array to a number
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="ReturnValue">The unsigned value of the byte array</param>
        public static void ByteArrayToNumber(byte[] ByteArray, out ulong ReturnValue)
        { ByteArrayToNumber(ByteArray, 0, ByteArray.Length, defaultEndian, out ReturnValue); }
        /// <summary>
        /// Converts a byte array to a number
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="ReturnValue">The signed value of the byte array</param>
        public static void ByteArrayToNumber(byte[] ByteArray, out long ReturnValue)
        { ByteArrayToNumber(ByteArray, 0, ByteArray.Length, defaultEndian, out ReturnValue); }
        /// <summary>
        /// Convert a byte array to a number
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="Index">Where to start in the array</param>
        /// <param name="Length">How big the number is in bytes (must be less than or equal to 8)</param>
        /// <param name="ReturnValue">The unsigned value of the byte array</param>
        public static void ByteArrayToNumber(byte[] ByteArray, int Index, int Length, out ulong ReturnValue)
        { ByteArrayToNumber(ByteArray, Index, Length, defaultEndian, out ReturnValue); }
        /// <summary>
        /// Convert a byte array to a number
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="Index">Where to start in the array</param>
        /// <param name="Length">How big the number is in bytes (must be less than or equal to 8)</param>
        /// <param name="ReturnValue">The signed value of the byte array</param>
        public static void ByteArrayToNumber(byte[] ByteArray, int Index, int Length, out long ReturnValue)
        { ByteArrayToNumber(ByteArray, Index, Length, defaultEndian, out ReturnValue); }
        /// <summary>
        /// Convert a byte array to a number
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="Index">Where to start in the array</param>
        /// <param name="Length">How big the number is in bytes (must be less than or equal to 8)</param>
        /// <param name="WhichEndian">Big endian or little endian</param>
        /// <param name="ReturnValue">The signed value of the byte array</param>
        public static void ByteArrayToNumber(byte[] ByteArray, int Index, int Length, Endian WhichEndian, out long ReturnValue)
        { ReturnValue = (long)ByteArrayToNumber(ByteArray, Index, Length, WhichEndian); }
        /// <summary>
        /// Converts a byte array to a number
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="Index">Where to start in the array</param>
        /// <param name="Length">How big the number is in bytes (must be less than or equal to 8)</param>
        /// <param name="WhichEndian">Big endian or little endian</param>
        /// <param name="ReturnValue">The value of the byte array</param>
        public static void ByteArrayToNumber(byte[] ByteArray, int Index, int Length, Endian WhichEndian, out ulong ReturnValue)
        { ReturnValue = ByteArrayToNumber(ByteArray, Index, Length, WhichEndian); }
        /// <summary>
        /// This function does the actual work for the above public functions
        /// </summary>
        /// <param name="ByteArray">Array to Convert</param>
        /// <param name="Index">Where to start in the array</param>
        /// <param name="Length">How big the number is in bytes (must be less than or equal to 8)</param>
        /// <param name="WhichEndian">Big endian or little endian</param>
        /// <returns>An unsigned long</returns>
        public static ulong ByteArrayToNumber(byte[] ByteArray, int Index, int Length, Endian WhichEndian)
        {

            // Bounds checking, the number can't be bigger than 64bits
            if (Length > 8) { Length = 8; }
            if (Length > ByteArray.Length) { Length = ByteArray.Length; }

            // Copy the data into a new array so we can use the BitConverter function	
            byte[] tempValue = new byte[Length];
            Array.Copy(ByteArray, Index, tempValue, 0, Length);
            // Move it to an 8 byte array so it can be converted correctly
            if (WhichEndian != Endian.Little)
            { Array.Reverse(tempValue); }
            byte[] uint64Array = new byte[8];
            Array.Copy(tempValue, 0, uint64Array, 0, Length);
            //Array.Reverse(uint64Array);

            // Signed or unsigned, you be the judge
            return BitConverter.ToUInt64(uint64Array, 0);
        }

        /// <summary>
        /// Convert a number to a byte array
        /// </summary>
        /// <param name="Number">The number to convert</param>
        /// <returns>A byte array (big endian)</returns>
        public static byte[] NumberToByteArray(long Number)
        { return NumberToByteArray(Number, ByteLength.Byte, defaultEndian); }
        /// <summary>
        /// Convert a number to a byte array
        /// </summary>
        /// <param name="Number">The number to convert</param>
        /// <param name="MinimumArraySize">The minimum number of bytes the array will contain, used mostly for negative numbers</param>
        /// <returns>A byte array (big endian)</returns>
        public static byte[] NumberToByteArray(long Number, ByteLength MinimumArraySize)
        { return NumberToByteArray(Number, (int)MinimumArraySize, defaultEndian); }
        /// <summary>
        /// Convert a number to a byte array
        /// </summary>
        /// <param name="Number">The number to convert</param>
        /// <param name="MinimumArraySize">The minimum number of bytes the array will contain, used mostly for negative numbers</param>
        /// <returns>A byte array (big endian)</returns>
        public static byte[] NumberToByteArray(long Number, int MinimumArraySize)
        { return NumberToByteArray(Number, MinimumArraySize, defaultEndian); }
        /// <summary>
        /// Convert a number to a byte array
        /// </summary>
        /// <param name="Number">The number to convert</param>
        /// <param name="WhichEndian">If you want a big endian number or a little endian number</param>
        /// <returns>A byte array </returns>
        public static byte[] NumberToByteArray(long Number, Endian WhichEndian)
        { return NumberToByteArray(Number, ByteLength.Byte, WhichEndian); }
        /// <summary>
        /// Convert a number to a byte array
        /// </summary>
        /// <param name="Number">The number to convert</param>
        /// <param name="MinimumArraySize">The minimum number of bytes the array will contain, used mostly for negative numbers</param>
        /// <param name="WhichEndian">If you want a big endian number or a little endian number</param>
        /// <returns>A byte array </returns>
        public static byte[] NumberToByteArray(long Number, ByteLength MinimumArraySize, Endian WhichEndian)
        { return NumberToByteArray(Number, (int)MinimumArraySize, WhichEndian); }
        /// <summary>
        /// Convert a number to a byte array
        /// </summary>
        /// <param name="Number">The number to convert</param>
        /// <param name="MinimumArraySize">The minimum number of bytes the array will contain, used mostly for negative numbers</param>
        /// <param name="WhichEndian">If you want a big endian number or a little endian number</param>
        /// <returns>A byte array </returns>
        public static byte[] NumberToByteArray(long Number, int MinimumArraySize, Endian WhichEndian)
        {
            ArrayList tempBytes = new ArrayList();
            byte[] finalBytes;

            // Deal with negative numbers
            if (Number == -1)
            {
                for (int i = 0; i < (int)MinimumArraySize; i++)
                { tempBytes.Add((byte)255); }
            }
            else if (Number < -1)
            {
                while (Number != -1 || ((int)MinimumArraySize > tempBytes.Count))
                {
                    if (WhichEndian == Endian.Big)
                    { tempBytes.Insert(0, (byte)(Number & 0xFF)); }
                    else
                    { tempBytes.Add((byte)(Number & 0xFF)); }
                    Number = Number >> 8;
                }
            }
            else
            {
                while (Number > 0 || ((int)MinimumArraySize > tempBytes.Count))
                {
                    if (WhichEndian == Endian.Big)
                    { tempBytes.Insert(0, (byte)(Number & 0xFF)); }
                    else
                    { tempBytes.Add((byte)(Number & 0xFF)); }
                    Number = Number >> 8;
                }
            }

            tempBytes.TrimToSize();
            if ((int)MinimumArraySize > tempBytes.Count)
            { finalBytes = new byte[(int)MinimumArraySize]; }
            else
            { finalBytes = new byte[tempBytes.Count]; }
            tempBytes.CopyTo(finalBytes);
            return finalBytes;
        }


        /// <summary>
        /// Increment a byte array by 1
        /// </summary>
        /// <param name="byteArray">The array to increment</param>
        /// <returns>The incremented array</returns>
        public static byte[] IncrementByteArray(byte[] byteArray)
        { return IncrementByteArray(byteArray, defaultEndian); }
        /// <summary>
        /// Increment a byte array by 1
        /// </summary>
        /// <param name="byteArray">The array to increment</param>
        /// <param name="WhichEndian">Specify if the array is big endian or little endian</param>
        /// <returns>The incremented array</returns>
        public static byte[] IncrementByteArray(byte[] byteArray, Endian WhichEndian)
        {
            // Flip the array to the right way round
            if (WhichEndian == Endian.Little)
            {
                Array.Reverse(byteArray);
            }

            // Increment the array
            for (int i = 0; i < byteArray.Length; i++)
            {
                try
                {
                    byteArray[i] += 1;
                    break;
                }
                catch
                {
                    // There was a carry so add one to the next byte
                }
            }

            // Flip the array back to little endian
            if (WhichEndian == Endian.Little)
            {
                Array.Reverse(byteArray);
            }

            return byteArray;

        }

        /// <summary>
        /// Adds two byte arrays (big endian)
        /// </summary>
        /// <param name="Array1">The First Array</param>
        /// <param name="Array2">The Second Array</param>
        /// <returns>The product of the two arrays</returns>
        public static byte[] AddByteArrays(byte[] Array1, byte[] Array2)
        { return AddByteArrays(Array1, Array2, defaultEndian); }
        /// <summary>
        /// Adds two byte arrays (big endian)
        /// </summary>
        /// <param name="Array1">The First Array</param>
        /// <param name="Array2">The Second Array</param>
        /// <param name="WhichEndian">Specify if the array is big endian or little endian</param>
        /// <returns>The product of the two arrays</returns>
        public static byte[] AddByteArrays(byte[] Array1, byte[] Array2, Endian WhichEndian)
        {
            // The size of the biggest array
            int maxLength = 0;
            if (Array1.Length >= Array2.Length)
            { maxLength = Array1.Length; }
            else
            { maxLength = Array2.Length; }

            // It's easier to add from 0 to n
            if (WhichEndian == Endian.Big)
            {
                Array.Reverse(Array1);
                Array.Reverse(Array2);
            }

            // Create our temp arrays		
            byte[] array1 = new byte[maxLength + 1]; // The extra is in case of a carry from the last addition
            byte[] array2 = new byte[maxLength];
            Array.Copy(Array1, 0, array1, 0, Array1.Length);
            Array.Copy(Array2, 0, array2, 0, Array2.Length);

            // //Debug Start
            //			string line1 = "";
            //			string line2 = "";
            //			for (int i = maxLength - 1; i > -1  ; i--)
            //			{
            //				line1 += array1[i].ToString() + ".";
            //				line2 += array2[i].ToString() + ".";
            //			}
            //			//Console.WriteLine(line1 + "\n" + line2 + " + ");
            // //Debug End

            // Do the actual addition
            int carry = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                // The last byte is going to cause an error, but that's ok we want that
                try
                {
                    // Catch anything over a byte, including any carries from the previous addition
                    carry += array1[i] + array2[i];

                    // Save the result
                    array1[i] = (byte)(carry & 0xFF);
                    array2[i] = array1[i];

                    // Make the carry "byte" sized
                    carry >>= 8;
                    carry &= 0xFF;
                }
                catch
                {
                    // Last byte
                    array1[i] = (byte)(carry & 0xFF);
                }

            }

            // Flip back big endians so they are correctly formated
            if (WhichEndian == Endian.Big)
            {
                Array.Reverse(array1);
                Array.Reverse(array2);
            }

            // Check for any more carries
            if (carry != 0)
            { return array1; }
            else
            { return array2; }
        }


        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(long Number)
        { return Hex(Number, "", ByteLength.Byte, ByteLength.Byte); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="ByteSize">The minimum number of bytes to return</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(long Number, ByteLength ByteSize)
        { return Hex(Number, "", ByteLength.Byte, ByteSize); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(long Number, string Separator)
        { return Hex(Number, Separator, ByteLength.Byte, ByteLength.Byte); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <param name="GroupSize">When using a separator, this sets how many bytes are inbetween each separator</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(long Number, string Separator, ByteLength GroupSize)
        { return Hex(Number, Separator, GroupSize, ByteLength.Byte); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <param name="GroupSize">When using a separator, this sets how many bytes are inbetween each separator</param>
        /// <param name="ByteSize">The minimum number of bytes to return</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(long Number, string Separator, ByteLength GroupSize, ByteLength ByteSize)
        {
            StringBuilder sb = new StringBuilder();

            // Turn it into a Hex string
            sb.AppendFormat(("{0:X" + (int)ByteSize + "}"), Number);

            // Add the separator and grouping, if any
            if (Separator != "")
            {
                int sbLength = sb.Length;
                for (int i = sb.Length - (int)GroupSize; i > 0; i -= (int)GroupSize)
                { sb.Insert(i, Separator); }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(ulong Number)
        { return Hex(Number, "", ByteLength.Byte, ByteLength.Byte); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="ByteSize">The minimum number of bytes to return</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(ulong Number, ByteLength ByteSize)
        { return Hex(Number, "", ByteLength.Byte, ByteSize); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(ulong Number, string Separator)
        { return Hex(Number, Separator, ByteLength.Byte, ByteLength.Byte); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <param name="GroupSize">When using a separator, this sets how many bytes are inbetween each separator</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(ulong Number, string Separator, ByteLength GroupSize)
        { return Hex(Number, Separator, GroupSize, ByteLength.Byte); }
        /// <summary>
        /// Convert a number to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <param name="GroupSize">When using a separator, this sets how many bytes are inbetween each separator</param>
        /// <param name="ByteSize">The minimum number of bytes to return</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(ulong Number, string Separator, ByteLength GroupSize, ByteLength ByteSize)
        {
            StringBuilder sb = new StringBuilder();

            // Turn it into a Hex string
            sb.AppendFormat(("{0:X" + (int)ByteSize + "}"), Number);

            // Add the separator and grouping, if any
            if (Separator != "")
            {
                int sbLength = sb.Length;
                for (int i = sb.Length - (int)GroupSize; i > 0; i -= (int)GroupSize)
                { sb.Insert(i, Separator); }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Convert a byte array to a Hex string
        /// </summary>
        /// <param name="Number">The byte array you want to convert to hex</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(byte[] ByteArray)
        { return Hex(ByteArray, "", ByteLength.Byte, defaultEndian); }
        public static string Hex(byte[] ByteArray, string Separator, ByteLength GroupSize, Endian WhichEndian)
        { return Hex(ByteArray, 0, ByteArray.Length, Separator, GroupSize, WhichEndian); }
        public static string Hex(byte[] ByteArray, string Separator)
        { return Hex(ByteArray, 0, ByteArray.Length, Separator, ByteLength.Byte, defaultEndian); }
        public static string Hex(byte[] ByteArray, string Separator, Endian WhichEndian)
        { return Hex(ByteArray, 0, ByteArray.Length, Separator, ByteLength.Byte, WhichEndian); }
        public static string Hex(byte[] ByteArray, int Index, int Length, string Separator, Endian WhichEndian)
        { return Hex(ByteArray, Index, Length, Separator, ByteLength.Byte, WhichEndian); }
        public static string Hex(byte[] ByteArray, int Index, int Length)
        { return Hex(ByteArray, Index, Length, "", ByteLength.Byte, defaultEndian); }
        public static string Hex(byte[] ByteArray, int Index, int Length, Endian WhichEndian)
        { return Hex(ByteArray, Index, Length, "", ByteLength.Byte, WhichEndian); }
        /// <summary>
        /// Convert a byte array to a Hex string
        /// </summary>
        /// <param name="Number">The number you want to convert to hex</param>
        /// <param name="Separator">The string used to separate value groups</param>
        /// <param name="GroupSize">When using a separator, this sets how many bytes are inbetween each separator</param>
        /// <param name="WhichEndian">If your byte array is little endian and you want it flipped around, tell the function you have an Endian.Little type array</param>
        /// <returns>A Hex formated string</returns>
        public static string Hex(byte[] ByteArray, int Index, int Length, string Separator, ByteLength GroupSize, Endian WhichEndian)
        {
            StringBuilder sb = new StringBuilder();

            // Turn it into a Hex string
            for (int i = Index; i < Index + Length; i++)
            {
                if (WhichEndian == Endian.Big)
                { sb.AppendFormat(("{0:X2}"), ByteArray[i]); }
                else
                { sb.Insert(0, ByteArray[i].ToString("X2")); }
            }

            // Add the separator and grouping, if any
            if (Separator != "")
            {
                int sbLength = sb.Length;
                for (int i = sb.Length - ((int)GroupSize * 2); i > 0; i -= ((int)GroupSize * 2))
                { sb.Insert(i, Separator); }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Convert a hex string to a byte array.  If you input big endian, you'll get big endian out.  If you input little endian, you'll get little endian. 
        /// </summary>
        /// <param name="HexString"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(string HexString)
        {
            if (Math.IEEERemainder(HexString.Length, 2) != 0)
            { HexString = "0" + HexString; }

            byte[] returnArray = new byte[HexString.Length / 2];

            for (int index = 0; index < HexString.Length; index += 2)
            {
                returnArray[index / 2] = byte.Parse(HexString.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return returnArray;
        }


        /// <summary>
        /// Convert a real DateTime to an Oracle formated datetime, this only works if you set your Oracle session to the format used in this function
        /// </summary>
        /// <param name="RealDate"></param>
        /// <returns></returns>
        public static string OracleDateFormat(DateTime RealDate)
        {
            // 08/17/2000 16:32:32
            return "to_date('" + RealDate.ToString("G", DateTimeFormatInfo.InvariantInfo) + "','mm/dd/yyyy hh24:mi:ss')";
        }

        /// <summary>
        /// Convert a time_t value into a dot.net DateTime value
        /// </summary>
        /// <param name="time_t">Unix/Posix Time Value</param>
        /// <returns>Dot.Net DateTime Value</returns>
        public static DateTime ConvertFromTime_T(ulong time_t)
        {
            // Do NOT use ToLocalTime() because it ISN'T local time it's GMT!!!!!!
            DateTime netTimeT = new DateTime(1970, 1, 1).AddSeconds(time_t);
            return netTimeT;
        }

        /// <summary>
        /// Convert a Dot.Net DateTime to a time_t value
        /// </summary>
        /// <param name="DotNetDate">The Dot.Net DateTime value</param>
        /// <returns>The time_t value</returns>
        public static int ConvertToTime_T(DateTime DotNetDate)
        {
            TimeSpan timeTSpan = DotNetDate - new DateTime(1970, 1, 1); ;
            return (int)timeTSpan.TotalSeconds;
        }

        /// <summary>
        /// Applies a simple encryption to a string, the encryption is different everytime for the same string
        /// </summary>
        /// <param name="Data">The unencrypted text</param>
        /// <returns>The encrypted text</returns>
        public static string SimpleEncrypt(string Data)
        {
            StringBuilder sb = new StringBuilder();

            int flip = -1;
            int lastVal = rand.Next(32, 127);
            sb.Append(lastVal.ToString("d3"));
            for (int i = 0; i < Data.Length; i++)
            {
                int val = (int)Data[i];
                val += (lastVal + i) * flip;
                while (val < 32 || val > 127)
                {
                    if (val < 32)
                    { val = 128 - (32 - val); }
                    if (val > 127)
                    { val -= 96; }
                }
                sb.Append(val.ToString("d3"));
                sb.Append(rand.Next(32, 127).ToString("d3"));
                flip *= -1;
                lastVal = val;
            }
            return sb.ToString();

        }

        /// <summary>
        /// Unencrypts text encrypted using SimpleEncrypt
        /// </summary>
        /// <param name="Data">The encrypted text</param>
        /// <returns>The unencrypted text</returns>
        public static string SimpleDecrypt(string Data)
        {
            StringBuilder sb = new StringBuilder();
            int flip = 1;
            int lastVal = int.Parse(Data.Substring(0, 3));
            for (int i = 3; i < Data.Length; i += 6)
            {
                int val = int.Parse(Data.Substring(i, 3));

                char blah = (char)val;
                val += (lastVal + sb.Length) * flip;
                while (val < 32 || val > 127)
                {
                    if (val < 32)
                    { val = 128 - (32 - val); }
                    if (val > 127)
                    { val -= 96; }
                }
                char deblah = (char)val;
                sb.Append((char)val);
                flip *= -1;
                lastVal = int.Parse(Data.Substring(i, 3));
            }
            return sb.ToString();
        }


        /// <summary>
        /// Returns the mirror image of a string
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static string ReverseString(string InputString)
        {
            StringBuilder revStr = new StringBuilder();

            for (int i = 0; i < InputString.Length; i++)
            {
                revStr.Insert(0, InputString.Substring(i, 1));
            }
            return revStr.ToString();
        }

        /// <summary>
        /// Converts a string that would normally crash an SQL Insert into a usable string, e.g. Fixes single quotes, Carrage Return/Line Feeds, etc.
        /// </summary>
        /// <param name="UnsafeString">A string you shouldn't put into a database</param>
        /// <returns></returns>
        public static string DbSafeString(string UnsafeString)
        {
            StringBuilder safeString = new StringBuilder();

            return safeString.ToString();
        }

        /// <summary>
        /// Converts a "safe" string made by the DbSafeString() function back to a naturally formated string.
        /// </summary>
        /// <param name="SafeString">A string you converted to be safe</param>
        /// <returns></returns>
        public static string DbUnSafeString(string SafeString)
        {
            StringBuilder unSafeString = new StringBuilder();

            return unSafeString.ToString();
        }

        /// <summary>
        /// Converts a Binary Coded Decimal byte array to a base-10 number
        /// </summary>
        /// <param name="ByteArray">The BCD byte array representing the number to convert</param>
        /// <returns></returns>
        public static int PackedBCDtoNumber(byte[] ByteArray)
        {
            return PackedBCDtoNumber(ByteArray, 0, ByteArray.Length, MathUtils.Endian.Big);
        }

        /// <summary>
        /// Converts a Binary Coded Decimal byte array to a base-10 number
        /// </summary>
        /// <param name="ByteArray">The BCD byte array representing the number to convert</param>
        /// <param name="WhichEndian">Whether the byte array is little or big endian</param>
        /// <returns></returns>
        public static int PackedBCDtoNumber(byte[] ByteArray, MathUtils.Endian WhichEndian)
        {
            return PackedBCDtoNumber(ByteArray, 0, ByteArray.Length, WhichEndian);
        }

        /// <summary>
        /// Converts a Binary Coded Decimal byte array to a base-10 number
        /// </summary>
        /// <param name="ByteArray">The BCD byte array representing the number to convert</param>
        /// <param name="Index">The starting position from which to extract the data</param>
        /// <param name="Length">How long the BCD portion is</param>
        /// <returns></returns>
        public static int PackedBCDtoNumber(byte[] ByteArray, int Index, int Length)
        {
            return PackedBCDtoNumber(ByteArray, Index, Length, MathUtils.Endian.Big);
        }

        /// <summary>
        /// Converts a Binary Coded Decimal byte array to a base-10 number
        /// </summary>
        /// <param name="ByteArray">The BCD byte array representing the number to convert</param>
        /// <param name="Index">The starting position from which to extract the data</param>
        /// <param name="Length">How long the BCD portion is</param>
        /// <param name="WhichEndian">Describes if the byte array is little or big endian</param>
        /// <returns></returns>
        public static int PackedBCDtoNumber(byte[] ByteArray, int Index, int Length, MathUtils.Endian WhichEndian)
        {
            // Dummy checks
            if (Index >= ByteArray.Length)
            { return -1; }
            if ((Index + Length) > ByteArray.Length)
            { return -1; }

            int returnValue = 0;
            int byteMultiplier = 0;
            int byteMultiplierAddian = 1;

            // Fix byte multiplier if little endian
            if (WhichEndian == MathUtils.Endian.Big)
            {
                byteMultiplier = ByteArray.Length - 1;
                byteMultiplierAddian = -1;
            }
            else
            {
                byteMultiplier = 0;
                byteMultiplierAddian = 1;
            }

            // Extract the number
            for (int i = Index; i < (Index + Length); i++)
            {
                int firstNibble = ByteArray[i] >> 4;
                int secondNibble = ByteArray[i] & 0xF;
                if (firstNibble > 9 || secondNibble > 9)
                { return -1; }
                returnValue += ((firstNibble * 10) + secondNibble) * (int)Math.Pow(100, byteMultiplier);
                byteMultiplier += byteMultiplierAddian;
            }
            return returnValue;
        }


        /// <summary>
        /// Converts a little endian decimal IP Address to a "normal" dotted decimal address.
        /// </summary>
        /// <param name="IpNumber"></param>
        /// <returns></returns>
        public static string IpFromNumber_VIC(ulong IpNumber)
        {
            try
            {
                string hexNum = Hex(IpNumber, MathUtils.ByteLength.QWord);
                string ipNum = "";
                for (int i = 6; i >= 0; i -= 2)
                {
                    string thisNum = "0x" + hexNum.Substring(i, 2);
                    ipNum = ipNum + Convert.ToInt32(thisNum, 16) + ".";
                }
                return ipNum.Substring(0, ipNum.Length - 1);
            }
            catch
            {
                return "Error: Illegal IP Number";
            }
        }

        /// <summary>
        /// Convert a dotted decimal IP address to a little endian number
        /// (I know it says big endian inside the code, ignore that)
        /// </summary>
        /// <param name="IpAddress"></param>
        /// <returns></returns>
        public static ulong NumberFromIp_VIC(string IpAddress)
        {
            System.Net.IPAddress ipFreely;
            try
            {
                ipFreely = System.Net.IPAddress.Parse(IpAddress);
            }
            catch
            {
                return 0;
            }

            int dotCount = 0;
            int lastDot = 0;
            for (dotCount = 0; dotCount < 10; dotCount++)
            {
                int newDot = IpAddress.IndexOf(".", lastDot) + 1;
                if (newDot < lastDot)
                { break; }
                lastDot = newDot;
            }
            if (dotCount != 3)
            {
                return 0;
            }

            byte[] intIp = MathUtils.NumberToByteArray(ipFreely.Address);
            ulong flippedIp = MathUtils.ByteArrayToNumber(intIp, 0, intIp.Length, MathUtils.Endian.Big);
            return flippedIp;

        }

        /// <summary>
        /// Add two numbers of base Base together
        /// </summary>
        /// <param name="Value1"></param>
        /// <param name="Value2"></param>
        /// <param name="Base"></param>
        /// <returns></returns>
        public static string AddBaseN(string Value1, string Value2, Byte Base)
        {
            int carry = 0;
            string result = "";
            int maxDigit = 0;

            if (Value1.Length >= Value2.Length)
            { maxDigit = Value1.Length + 1; }
            else
            { maxDigit = Value2.Length + 1; }

            Value1 = Value1.PadLeft(maxDigit, '0').ToUpper();
            Value2 = Value2.PadLeft(maxDigit, '0').ToUpper();

            for (int digit = 1; digit <= maxDigit; digit++)
            {
                int val1 = ConvertBaseNToDec(Value1.Substring(maxDigit - digit, 1), Base);
                int val2 = ConvertBaseNToDec(Value2.Substring(maxDigit - digit, 1), Base);
                int digitResult = val1 + val2 + carry;
                carry = (int)(digitResult / Base);
                digitResult -= carry * Base;
                result = ConvertDecToBaseN(digitResult, Base) + result;
            }
            result = result.TrimStart('0');
            return result;

        }

        /// <summary>
        /// Turns a base 10 number into a base Base number
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Base"></param>
        /// <returns></returns>
        public static string ConvertDecToBaseN(int Value, Byte Base)
        {
            string baseNumbers = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";
            int remainder = 0;

            if (Base < 2 || Base > 36)
            { return result; }

            Value = Math.Abs(Value);

            do
            {
                remainder = Value - (Base * (int)(Value / Base));
                result = baseNumbers.Substring(remainder, 1) + result;
                Value = (int)(Value / Base);
            } while (Value > 0);

            return result;
        }

        /// <summary>
        /// Turns a base Base number into a base 10 number
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Base"></param>
        /// <returns></returns>
        public static int ConvertBaseNToDec(string Value, Byte Base)
        {
            string baseNumbers = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int returnValue = 0;
            int digit = 1;

            if (Base < 2 || Base > 36)
            { return returnValue; }

            do
            {
                string sDigit = Value.Substring(Value.Length - digit, 1);
                double iDigitValue = baseNumbers.IndexOf(sDigit);
                double pow = Math.Pow(Base, (double)digit - 1);
                returnValue += (int)(iDigitValue * pow);
                digit++;
            } while (digit <= Value.Length);

            return returnValue;

        }

    }
}