using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Common
{
    public class CheckSum
    {
        public static ushort Calculate_CRC(ushort cal_crc, ushort polynomial, byte[] bytes, int length)
        {

            for (int i = 0; i < length; i++)
            {
                cal_crc ^= bytes[i];
                for (int bit_count = 0; bit_count < 8; bit_count++)
                {
                    //crc table
                    if ((cal_crc & 0x001) > 0)
                    {
                        cal_crc >>= 1;
                        cal_crc ^= polynomial;
                    }
                    else
                    {
                        cal_crc >>= 1;
                    }
                }

            }
            return cal_crc;
        }

        public static bool SEMI_CheckSum(string IDWithCheckSum)
        {
            byte[] bytes = Encoding.Default.GetBytes(IDWithCheckSum.ToUpper());
            int nChecksum = 0;
            foreach (var a in bytes)
            {
                nChecksum = ((a - 32 + nChecksum) * 8) % 59;
            }
            return (nChecksum == 0);
        }

        public static bool HPOrificeCheckSum(string Bardcode, char CheckSum)
        {
            if (Bardcode.Length != 8) return false;
            byte[] bytes = Encoding.Default.GetBytes(Bardcode.ToUpper());
            if (CheckSum <= 'z' && CheckSum >= 'a') CheckSum = (char)(CheckSum - 'a' + 'A');
            //int calCheckSum;

            //calCheckSum = Calculate_CRC(0, 26, bytes, bytes.Length);

            long calCheckSum = 0;

            for (int i = 1; i <= 8; i++)
            {
                long longbyte = (long)bytes[i - 1];
                calCheckSum += longbyte << (i);
            }
            calCheckSum %= 26;
            calCheckSum += 65;

            return calCheckSum == (ushort)CheckSum;
        }

    }
}
