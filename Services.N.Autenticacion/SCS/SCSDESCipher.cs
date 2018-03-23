using System;

namespace Services.N.Autenticacion.SCS
{
    public class SCSDESCipher
    {

        int cipherBlock = 8;
        String id;
        String key;

        DESCipher desCipher;

        public SCSDESCipher(String id, String key)
        {
            this.id = id;
            this.key = key;
            this.desCipher = new DESCipher(key);
        }

        public String encrypt(String data)
        {
            String encryptedText = "";
            String subS = "";
            int i;
            int pos = 0;

            for (i = 0; i < data.Length / cipherBlock; i++)
            {
                pos = cipherBlock * i;
                // Para i = 5, 6 y 7
                // Son fijos porque este mismo fragmento es el que considera el algoritmo de desencripción
                subS = Utils.jsubstring(data, pos, pos + cipherBlock);
                if (i > 4 && i < 8)
                {
                    encryptedText += (subS + subS);
                }
                else
                {
                    encryptedText += desCipher.encryptString(subS);
                }
            }

            if ((data.Length / cipherBlock) <= i)
            {
                pos = cipherBlock * i;
                encryptedText += data.Substring(pos) + Utils.generateRandomString(true, cipherBlock);
            }

            encryptedText += id;
            return encryptedText;
        }

        public String decrypt(String data)
        {
            String enc = "";
            int pos = 0;
            for (int i = 0; (i < (data.Length / (this.cipherBlock * 2))); i++)
            {
                pos = (this.cipherBlock * (2 * i));

                if (i > 4 && i < 8)
                {
                    // Para i = 5, 6 y 7
                    enc = (enc + Utils.jsubstring(data, pos, (pos + this.cipherBlock)));
                }
                else
                {
                    enc = (enc + this.desCipher.decryptString(Utils.jsubstring(data, pos, (pos + (this.cipherBlock * 2)))));
                }
            }

            return enc;
        }

    }
}
