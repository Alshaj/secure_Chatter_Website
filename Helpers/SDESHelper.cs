namespace S_DES_project4.Helpers
{
    public static class SDESHelper
    {
        private static readonly int[] P10 = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 };
        private static readonly int[] P8 = { 6, 3, 7, 4, 8, 5, 10, 9 };
        private static readonly int[] P4 = { 2, 4, 3, 1 };
        private static readonly int[] IP = { 2, 6, 3, 1, 4, 8, 5, 7 };
        private static readonly int[] IP_INV = { 4, 1, 3, 5, 7, 2, 8, 6 };
        private static readonly int[] EP = { 4, 1, 2, 3, 2, 3, 4, 1 };

        private static readonly int[,] S0 = {
        {1,0,3,2},
        {3,2,1,0},
        {0,2,1,3},
        {3,1,3,2}
    };

        private static readonly int[,] S1 = {
        {0,1,2,3},
        {2,0,1,3},
        {3,0,1,0},
        {2,1,0,3}
    };

        // ------------------- PUBLIC API -------------------
        public static string Encrypt(string plainText, string key)
        {
            var keys = GenerateSubKeys(key);
            string cipher = "";
            foreach (var c in plainText)
            {
                cipher += SDES_EncryptChar(c, keys[0], keys[1]);
            }
            return cipher; // binary string
        }

        public static string Decrypt(string cipherText, string key)
        {
            var keys = GenerateSubKeys(key);
            string plain = "";
            // process 8 bits at a time
            for (int i = 0; i < cipherText.Length; i += 8)
            {
                string byteStr = cipherText.Substring(i, 8);
                plain += SDES_DecryptChar(byteStr, keys[0], keys[1]);
            }
            return plain;
        }

        // ------------------- INTERNAL METHODS -------------------
        private static string SDES_EncryptChar(char input, string K1, string K2)
        {
            string bits = Convert.ToString((int)input, 2).PadLeft(8, '0');
            bits = SDESProcess(bits, K1, K2);
            return bits; // return 8-bit binary string
        }

        private static string SDES_DecryptChar(string bits, string K1, string K2)
        {
            string decryptedBits = SDESProcess(bits, K2, K1); // reverse keys
            int ascii = Convert.ToInt32(decryptedBits, 2);
            return Convert.ToChar(ascii).ToString();
        }

        private static string SDESProcess(string bits, string K1, string K2)
        {
            bits = Permute(bits, IP);
            string left = bits.Substring(0, 4);
            string right = bits.Substring(4, 4);

            string temp = Fk(left, right, K1);
            left = temp.Substring(0, 4);
            right = temp.Substring(4, 4);

            // swap
            var swap = right + left;

            temp = Fk(swap.Substring(0, 4), swap.Substring(4, 4), K2);
            return Permute(temp, IP_INV);
        }

        private static string Fk(string L, string R, string SK)
        {
            string expandedR = Permute(R, EP);
            string xor = XOR(expandedR, SK);
            string left4 = xor.Substring(0, 4);
            string right4 = xor.Substring(4, 4);

            string sbox0 = SBox(left4, S0);
            string sbox1 = SBox(right4, S1);
            string combined = sbox0 + sbox1;

            string p4 = Permute(combined, P4);
            string resultL = XOR(L, p4);
            return resultL + R;
        }

        private static string SBox(string input, int[,] sbox)
        {
            int row = Convert.ToInt32("" + input[0] + input[3], 2);
            int col = Convert.ToInt32("" + input[1] + input[2], 2);
            int val = sbox[row, col];
            return Convert.ToString(val, 2).PadLeft(2, '0');
        }

        private static string XOR(string a, string b)
        {
            char[] res = new char[a.Length];
            for (int i = 0; i < a.Length; i++)
                res[i] = (a[i] == b[i]) ? '0' : '1';
            return new string(res);
        }

        private static string Permute(string input, int[] table)
        {
            char[] res = new char[table.Length];
            for (int i = 0; i < table.Length; i++)
                res[i] = input[table[i] - 1];
            return new string(res);
        }

        private static string[] GenerateSubKeys(string key10)
        {
            string perm10 = Permute(key10, P10);
            string left = perm10.Substring(0, 5);
            string right = perm10.Substring(5, 5);

            // LS-1
            left = left.Substring(1) + left[0];
            right = right.Substring(1) + right[0];

            string K1 = Permute(left + right, P8);

            // LS-2
            left = left.Substring(2) + left.Substring(0, 2);
            right = right.Substring(2) + right.Substring(0, 2);

            string K2 = Permute(left + right, P8);
            return new string[] { K1, K2 };
        }
    }
}
