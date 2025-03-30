using System; // Базовое пространство имен для работы с примитивными типами данных и основными классами .NET
using System.IO; // Пространство имен для работы с файлами и потоками данных
using System.Windows.Forms; // Пространство имен для создания графического интерфейса Windows
using Org.BouncyCastle.Crypto; // Основное пространство имен BouncyCastle для криптографии
using Org.BouncyCastle.Crypto.Engines; // Содержит реализации криптографических алгоритмов (в т.ч. IDEA)
using Org.BouncyCastle.Crypto.Modes; // Содержит режимы шифрования (CBC и др.)
using Org.BouncyCastle.Crypto.Paddings; // Содержит алгоритмы заполнения блоков данных
using Org.BouncyCastle.Crypto.Parameters; // Содержит классы для передачи параметров криптографическим алгоритмам
using Org.BouncyCastle.Security; // Содержит службы безопасности (например, для генерации случайных чисел)

namespace IdeaCbcApp
{
    // Главный класс программы с точкой входа
    static class Program
    {
        [STAThread] // Атрибут, указывающий, что приложение поддерживает однопоточную модель COM
        static void Main()
        {
            // Настройка стилей визуального отображения элементов управления
            Application.EnableVisualStyles();
            
            // Настройка режима рендеринга текста, совместимого с используемыми шрифтами
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Запуск главной формы приложения (класс MainForm должен быть определен в другом файле)
            Application.Run(new MainForm());
        }
    }

    // Класс с методами для шифрования/дешифрования файлов по алгоритму IDEA в режиме CBC
    class IdeaCbcExample
    {
        // Метод для генерации случайного вектора инициализации (IV)
        // IV используется в режиме CBC для обеспечения уникальности каждого процесса шифрования
        private static byte[] GenerateRandomIV()
        {
            // SecureRandom - криптографически стойкий генератор случайных чисел из BouncyCastle
            var random = new SecureRandom();
            
            // IDEA - блочный шифр с размером блока 8 байт, поэтому и IV имеет такую же длину
            byte[] iv = new byte[8];
            
            // Заполнение массива случайными значениями
            random.NextBytes(iv);
            return iv;
        }

        // Метод для шифрования файла с использованием IDEA в режиме CBC
        // inputFile - путь к исходному файлу
        // outputFile - путь к файлу, который будет содержать зашифрованные данные
        // key - ключ шифрования (должен быть 16 байт для IDEA)
        public static void EncryptFile(string inputFile, string outputFile, byte[] key)
        {
            // Чтение всех байтов из исходного файла
            byte[] inputBytes = File.ReadAllBytes(inputFile);
            
            // Генерация случайного вектора инициализации
            byte[] iv = GenerateRandomIV();

            // Создание объекта для шифрования:
            // PaddedBufferedBlockCipher - обертка для обеспечения буферизации и заполнения неполных блоков
            // CbcBlockCipher - реализация режима шифрования CBC (Cipher Block Chaining)
            // IdeaEngine - реализация алгоритма шифрования IDEA
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
            
            // Инициализация шифра для шифрования (true = шифрование)
            // ParametersWithIV - объект, содержащий ключ и вектор инициализации
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

            // Создание буфера для зашифрованных данных с достаточным размером
            byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
            
            // Шифрование основных данных
            int length = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outputBytes, 0);
            
            // Завершение шифрования и обработка последнего блока
            cipher.DoFinal(outputBytes, length);

            // Запись IV и зашифрованных данных в выходной файл
            using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                // Сначала запись IV (нужен для дешифрования)
                outputStream.Write(iv, 0, iv.Length);
                // Затем запись зашифрованных данных
                outputStream.Write(outputBytes, 0, outputBytes.Length);
            }
        }

        // Метод для дешифрования файла
        // inputFile - путь к зашифрованному файлу
        // outputFile - путь к файлу, который будет содержать дешифрованные данные
        // key - ключ шифрования (тот же, что использовался при шифровании)
        public static void DecryptFile(string inputFile, string outputFile, byte[] key)
        {
            // Чтение всех байтов из зашифрованного файла
            byte[] inputBytes = File.ReadAllBytes(inputFile);

            // Извлечение вектора инициализации (первые 8 байт)
            byte[] iv = new byte[8];
            Array.Copy(inputBytes, 0, iv, 0, iv.Length);

            // Извлечение зашифрованных данных (все байты после IV)
            byte[] cipherText = new byte[inputBytes.Length - iv.Length];
            Array.Copy(inputBytes, iv.Length, cipherText, 0, cipherText.Length);

            // Создание объекта для дешифрования (аналогично шифрованию)
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
            
            // Инициализация шифра для дешифрования (false = дешифрование)
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

            // Создание буфера для дешифрованных данных
            byte[] outputBytes = new byte[cipher.GetOutputSize(cipherText.Length)];
            
            // Дешифрование основных данных
            int length = cipher.ProcessBytes(cipherText, 0, cipherText.Length, outputBytes, 0);
            
            // Завершение дешифрования и обработка последнего блока
            // finalLength содержит количество байт, записанных в буфер после DoFinal
            int finalLength = cipher.DoFinal(outputBytes, length);
            
            // Создание нового массива только для фактических дешифрованных данных
            // (без лишних нулей, которые могут быть в выходном буфере)
            byte[] result = new byte[length + finalLength];
            Array.Copy(outputBytes, 0, result, 0, length + finalLength);

            // Запись дешифрованных данных в выходной файл
            File.WriteAllBytes(outputFile, result);
        }
    }
}
