using System;

namespace CrowdinSpreader
{
	    /// <summary>
	    /// Summary description for crc16.
	    /// </summary>
	    public class CRC
	    {
		    public static UInt16 GetCrc16(UInt16 Start, Int32[] Value)
		    {
			    UInt16[] crc_16_table = {
				    0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 
				    0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400
			    };
			    UInt16 crc = Start;
			    int r = 0;
			    for (int i = 0; i < Value.Length; ++i) {
				    UInt32 ch = ((UInt32)Value[i]);
				    r = crc_16_table[crc & 0xF];
				    crc = (UInt16)((crc >> 4) & 0x0FFF);
				    crc = (UInt16)(crc ^ r ^ crc_16_table[ch & 0xF]);

				    r = crc_16_table[crc & 0xF];
				    crc = (UInt16)((crc >> 4) & 0x0FFF);
				    crc = (UInt16)(crc ^ r ^ crc_16_table[(ch >> 4) & 0xF]);
			    }
			    return crc;
		    }

            public static Int32 GetCrc32(byte[] SrcBuffer) {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach(byte b in m_Crc32.ComputeHash(SrcBuffer)) {
                    sb.Append(b.ToString("x2").ToLower());
                }
                return Int32.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber); //!TODO: do we really need conversion byte->str->int
            }

            private static CRC32 m_Crc32 = new CRC32();
	    }


    /// <summary>
    /// http://damieng.com/blog/2006/08/08/Calculating_CRC32_in_C_and_NET
    /// </summary>
    public class CRC32 : System.Security.Cryptography.HashAlgorithm
    {
        public const UInt32 DefaultPolynomial = 0xedb88320;
        public const UInt32 DefaultSeed = 0xffffffff;
        private UInt32 crc;
        private UInt32[] table;
        private UInt32 seed;
        

        public CRC32() {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
        }

        public CRC32(UInt32 polynomial, UInt32 seed) {
            table = InitializeTable(polynomial);
            this.seed = seed;
            Initialize();
        }

        public override void Initialize() {
            crc = seed;
        }

        protected override void HashCore(byte[] buffer, int start, int length) {
            crc = CalculateCRC(table, crc, buffer, start, length);
        }

        protected override byte[] HashFinal() {
            byte[] hashBuffer = new byte[4];
            UInt32ToBigEndianBytes(hashBuffer, crc);
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        public override int HashSize {
            get { return 32; }
        }

        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer) {
            return CalculateCRC(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }
        public static UInt32 Compute(String SrcStr) {
            Byte[] buffer = System.Text.Encoding.Unicode.GetBytes(SrcStr);
            return CalculateCRC(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        private static UInt32[] InitializeTable(UInt32 polynomial) {
            UInt32[] createTable = new UInt32[256];
            for (int i = 0; i < 256; i++)
            {
                UInt32 entry = (UInt32)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                }
                createTable[i] = entry;
            }
            return createTable;
        }

        private static UInt32 CalculateCRC(UInt32[] table, UInt32 seed, byte[] buffer, int start, int size)
        {
            UInt32 crc = seed;
            for (int i = start; i < size; i++) {
                unchecked {
                    crc = (crc >> 8) ^ table[(UInt32)buffer[i] ^ crc & (UInt32)0xff];
                }
            }
            return ~crc;
        }

        private void UInt32ToBigEndianBytes(byte[] block, UInt32 x) {
            block[0] = (byte)((x >> 24) & 0xff);
            block[1] = (byte)((x >> 16) & 0xff);
            block[2] = (byte)((x >> 8) & 0xff);
            block[3] = (byte)(x & 0xff);
        }
    }

}


