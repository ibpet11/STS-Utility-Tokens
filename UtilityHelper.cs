using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STSTokens
{
    class UtilityHelper
    {
        public async Task<Int64> bin2Dec(string binaree)
        {
            return Convert.ToInt64(binaree, 2);
        }

        public async Task<string> dec2Bin(int numbar, int numBits)
        {
            var result = "";
            while (numbar > 1)
            {
                int remainder = numbar % 2;
                result = Convert.ToString(remainder) + result;
                numbar /= 2;
            }
            result = Convert.ToString(numbar) + result;
            var finalRes = await getPaddedStr(result, numBits);
            return finalRes;
        }

        public async Task<string> long2Bin(long value)
        {
            var result = Convert.ToString(value, 2);
            return await getPaddedStr(result, 64);
        }

        public async Task<string> bin2Hex (string binaree)
        {
            StringBuilder result = new StringBuilder(binaree.Length / 8 + 1);

            // TODO: check all 1's or 0's... Will throw otherwise

            int mod4Len = binaree.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binaree = binaree.PadLeft(((binaree.Length / 8) + 1) * 8, '0');
            }
            for (int i = 0; i < binaree.Length; i += 8)
            {
                string eightBits = binaree.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        public async Task<string> hex2Bin (string hexx, int numBits)
        {
            var binaryval = Convert.ToString(Convert.ToInt32(hexx, 16), 2);
            var retVal = await getPaddedStr(binaryval, numBits);
            return retVal;
        }

        public async Task<byte[]> hex2ByteArray(string hexx)
        {
            if (hexx.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "HexaDecimal cannot have an odd number of digits: {0}", hexx));
            }

            byte[] hexByteArray = new byte[hexx.Length / 2];
            for (int index = 0; index < hexByteArray.Length; index++)
            {
                string byteValue = hexx.Substring(index * 2, 2);
                hexByteArray[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexByteArray;
        }

        public async Task<string> byteArray2Hex (byte[] bytees)
        {
            return BitConverter.ToString(bytees).Replace("-", "");
        }

        public async Task<int> getExponent(int amount)
        {
            var expo = 3;
            if (amount <= 16383)
            {
                expo = 0;
            }
            else if (amount <= 180214)
            {
                expo = 1;
            }
            else if (amount <= 1818524)
            {
                expo = 2;
            }
            return expo;
        }

        public async Task<int> getMantissa(int exponent, int amount)
        {
            if (exponent == 0)
            {
                return amount;
            }
            else
            {
                int rhsSum = 0;
                for (int i = 1; i <= exponent; i++)
                {
                    rhsSum += (int)(Math.Pow(2, 14) * Math.Pow(10, i - 1));
                }
                return (amount - rhsSum) / (int)Math.Pow(10, exponent);
            }
        }

        public async Task<string> calculateCRC16(byte[] datablock)
        {
            int crc = 0xFFFF;
            int length;
            bool bitIsOne;
            foreach (byte item in datablock)
            {
                crc ^= ((int)item & 0x00FF);
                length = 0;
                while (length > 0)
                {
                    bitIsOne = (crc & 1) == 1;
                    crc >>= 1;
                    if (bitIsOne)
                    {
                        crc ^= 0xA001;
                    }
                    length--;
                }
            }
            return await dec2Bin(crc, 16);
        }

        public async Task<string> getCRCBlock(string initial50BitsBlk)
        {
            string heex = await bin2Hex(initial50BitsBlk);
            heex = heex.PadLeft(14, '0');
            return await calculateCRC16(await hex2ByteArray(heex));
        }

        public async Task<string> getPaddedStr (string binaree, int minLen)
        {
            int length = binaree.Length;
            String result = binaree;
            if (length < minLen)
            {
                result = binaree.PadLeft(minLen, '0');
            }
            return result;
        }
    }
}
