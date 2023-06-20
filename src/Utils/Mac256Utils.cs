using System.Security.Cryptography;
using System.Text;

namespace SkSharp.Utils;

public static class Mac256Utils
{
    public static string GetMac256Hash(string challenge, string appId = "msmsgs@msnmsgr.com", string key = "Q1P7W2E4J9R8U3S5")
    {
        string clearText = challenge + appId;
        clearText += new string('0', 8 - clearText.Length % 8);

        string Int32ToHexString(long n)
        {
            string hexChars = "0123456789abcdef";
            StringBuilder hexString = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                hexString.Append(hexChars[(int)(n >> i * 8 + 4) & 15]);
                hexString.Append(hexChars[(int)(n >> i * 8) & 15]);
            }
            return hexString.ToString();
        }

        long Int64Xor(long a, long b)
        {
            string sA = Convert.ToString(a, 2);
            string sB = Convert.ToString(b, 2);
            StringBuilder sC = new StringBuilder();
            StringBuilder sD = new StringBuilder();
            int diff = Math.Abs(sA.Length - sB.Length);
            for (int i = 0; i < diff; i++)
            {
                sD.Append("0");
            }
            if (sA.Length < sB.Length)
            {
                sD.Append(sA);
                sA = sD.ToString();
            }
            else if (sB.Length < sA.Length)
            {
                sD.Append(sB);
                sB = sD.ToString();
            }
            for (int i = 0; i < sA.Length; i++)
            {
                sC.Append(sA[i] == sB[i] ? "0" : "1");
            }
            return Convert.ToInt64(sC.ToString(), 2);
        }

        int[] CS64(int[] pdwData, int[] pInHash)
        {
            const int MODULUS = 2147483647;
            int CS64_a = pInHash[0] & MODULUS;
            int CS64_b = pInHash[1] & MODULUS;
            int CS64_c = pInHash[2] & MODULUS;
            int CS64_d = pInHash[3] & MODULUS;
            int CS64_e = 242854337;
            int pos = 0;
            int qwDatum = 0;
            int qwMAC = 0;
            int qwSum = 0;
            for (int i = 0; i < pdwData.Length / 2; i++)
            {
                qwDatum = pdwData[pos];
                pos += 1;
                qwDatum *= CS64_e;
                qwDatum = qwDatum % MODULUS;
                qwMAC += qwDatum;
                qwMAC *= CS64_a;
                qwMAC += CS64_b;
                qwMAC = qwMAC % MODULUS;
                qwSum += qwMAC;
                qwMAC += pdwData[pos];
                pos += 1;
                qwMAC *= CS64_c;
                qwMAC += CS64_d;
                qwMAC = qwMAC % MODULUS;
                qwSum += qwMAC;
            }
            qwMAC += CS64_b;
            qwMAC = qwMAC % MODULUS;
            qwSum += CS64_d;
            qwSum = qwSum % MODULUS;
            return new int[] { qwMAC, qwSum };
        }

        int cchClearText = clearText.Length / 4;
        int[] pClearText = new int[cchClearText];
        for (int i = 0; i < cchClearText; i++)
        {
            pClearText[i] = 0;
            for (int pos = 0; pos < 4; pos++)
            {
                pClearText[i] += clearText[4 * i + pos] * (int)Math.Pow(256, pos);
            }
        }

        int[] sha256Hash = new int[] { 0, 0, 0, 0 };
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(challenge + key));
            string hash = string.Concat(hashBytes.Select(b => b.ToString("X2")));
            for (int i = 0; i < sha256Hash.Length; i++)
            {
                sha256Hash[i] = 0;
                for (int pos = 0; pos < 4; pos++)
                {
                    int dpos = 8 * i + pos * 2;
                    sha256Hash[i] += int.Parse(hash.Substring(dpos, 2), System.Globalization.NumberStyles.HexNumber) * (int)Math.Pow(256, pos);
                }
            }
        }

        int[] macHash = CS64(pClearText, sha256Hash);
        int[] macParts = new int[] { macHash[0], macHash[1], macHash[0], macHash[1] };

        return string.Concat(sha256Hash.Zip(macParts, (x, y) => Int32ToHexString(Int64Xor(x, y))));
    }
}