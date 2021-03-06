﻿using System;
using System.Composition;
using System.Linq;
using System.Text;

namespace Science.Cryptography.Ciphers
{
    using Analysis;

    /// <summary>
    /// Represents the Tap Code cipher.
    /// </summary>
    [Export("Tap Code", typeof(ICipher))]
    public class TapCode : ICipher, ISupportsRecognition
    {
        protected readonly char[,] CipherData =
        {
            { 'A', 'B', 'C', 'D', 'E' },
            { 'F', 'G', 'H', 'I', 'J' },
            { 'L', 'M', 'N', 'O', 'P' },
            { 'Q', 'R', 'S', 'T', 'U' },
            { 'V', 'W', 'X', 'Y', 'Z' }
        };


        public string Encrypt(string plaintext)
        {
            return String.Join(" ", plaintext.Select(this.LetterToCode));
        }

        public string Decrypt(string ciphertext)
        {
            int row = 0, column = 0;
            int values = 0;

            StringBuilder result = new StringBuilder();

            foreach (char c in ciphertext)
            {
                // meta character
                if (c == '.')
                {
                    if (values == 0)
                        row++;
                    else if (values == 1)
                        column++;
                }

                // every other characters
                else if (c == ' ')
                {
                    // step dimension
                    values++;

                    // if both dimensions are ready
                    if (values == 2)
                    {
                        result.Append(CipherData[row - 1, column - 1]);
                        
                        row = column = 0;
                        values = 0;
                    }
                }
            }

            if (row != 0 && column != 0)
                result.Append(CipherData[row - 1, column - 1]);

            return result.ToString();
        }


        public bool Recognize(string ciphertext)
        {
            if (ciphertext == null)
                throw new ArgumentNullException(nameof(ciphertext));

            return ciphertext
                .Where(c => !Char.IsWhiteSpace(c))
                .MostOfAll(c => c == '.')
            ;
        }


        private string LetterToCode(char ch)
        {
            ch = ch.ToUpper();

            for (int i = 1; i <= 5; i++)
            for (int j = 1; j <= 5; j++)
                if (CipherData[i - 1, j - 1] == ch)
                    return String.Concat(new String('.', i), ' ', new String('.', j));

            return String.Empty;
        }
    }
}
