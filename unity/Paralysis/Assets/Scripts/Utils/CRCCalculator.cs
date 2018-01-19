using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// CRC calculator this class will be used to create
/// short unique network ids
/// </summary>
public class CRCCalculator  {

    private const ushort poly = 4129;
    private const ushort initialValue = 0xffff;

    public static ushort CRCFromVector(Vector3 vector)
    {
        //TODO
        return 0;
    }

    /// <summary>
    /// CRCs from a string.  The longer the string
    /// the longer this calculation will take
    /// </summary>
    /// <returns>The from string.</returns>
    /// <param name="input">Input.</param>
    public static ushort CRCFromString(string input)
    {
        return Crc16Ccitt(HexToBytes(input));
    }

    public static byte[] HexToBytes(string input)
    {
        byte[] result = new byte[input.Length / 2];
        for(int i = 0; i < result.Length; i++)
        {
            result[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
        }
        return result;
    }

    public static ushort Crc16Ccitt(byte[] bytes)
    {
        ushort[] table = new ushort[256];

        ushort temp, a;
        ushort crc = initialValue;
        for (int i = 0; i < table.Length; ++i)
        {
            temp = 0;
            a = (ushort)(i << 8);
            for (int j = 0; j < 8; ++j)
            {
                if (((temp ^ a) & 0x8000) != 0)
                    temp = (ushort)((temp << 1) ^ poly);
                else
                    temp <<= 1;
                a <<= 1;
            }
            table[i] = temp;
        }
        for (int i = 0; i < bytes.Length; ++i)
        {
            crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
        }
        return crc;
    }  
    
}
