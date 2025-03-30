using System;
using System.IO;
using System.Windows.Forms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace IdeaCbcApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    class IdeaCbcExample
    {
        // Метод для генерации случайного вектора инициализации (IV)
        private static byte[] GenerateRandomIV()
        {
            var random = new SecureRandom();
            byte[] iv = new byte[8]; // IDEA использует 8-байтовый IV
            random.NextBytes(iv);
            return iv;
        }

        // Метод для шифрования данных
        public static void EncryptFile(string inputFile, string outputFile, byte[] key)
        {
            byte[] inputBytes = File.ReadAllBytes(inputFile);
            byte[] iv = GenerateRandomIV();

            // Создаем шифровальщик с режимом CBC и добавляем заполнение PKCS7
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

            byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
            int length = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outputBytes, 0);
            cipher.DoFinal(outputBytes, length);

            // Записываем IV и зашифрованные данные в выходной файл
            using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                outputStream.Write(iv, 0, iv.Length);
                outputStream.Write(outputBytes, 0, outputBytes.Length);
            }
        }

        // Метод для дешифрования данных
        public static void DecryptFile(string inputFile, string outputFile, byte[] key)
        {
            byte[] inputBytes = File.ReadAllBytes(inputFile);

            // Извлекаем IV из входного файла
            byte[] iv = new byte[8];
            Array.Copy(inputBytes, 0, iv, 0, iv.Length);

            // Извлекаем зашифрованные данные
            byte[] cipherText = new byte[inputBytes.Length - iv.Length];
            Array.Copy(inputBytes, iv.Length, cipherText, 0, cipherText.Length);

            // Создаем дешифровальщик с режимом CBC и добавляем заполнение PKCS7
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

            byte[] outputBytes = new byte[cipher.GetOutputSize(cipherText.Length)];
            int length = cipher.ProcessBytes(cipherText, 0, cipherText.Length, outputBytes, 0);
            int finalLength = cipher.DoFinal(outputBytes, length);
            
            // Создаем новый массив только с фактическими данными
            byte[] result = new byte[length + finalLength];
            Array.Copy(outputBytes, 0, result, 0, length + finalLength);

            // Записываем только фактические расшифрованные данные
            File.WriteAllBytes(outputFile, result);
        }
    }
}
