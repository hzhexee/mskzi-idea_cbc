# Приложение для шифрования файлов с использованием IDEA в режиме CBC

## Описание

Данное приложение реализует шифрование и дешифрование файлов с помощью алгоритма IDEA (International Data Encryption Algorithm) в режиме CBC (Cipher Block Chaining) с использованием библиотеки BouncyCastle.

## Структура кода приложения

Приложение состоит из следующих основных компонентов:

1. **Класс Program** - содержит точку входа приложения и настройку WinForms-интерфейса
2. **Класс IdeaCbcExample** - содержит основную функциональность для шифрования/дешифрования:
   - `GenerateRandomIV()` - создает случайный вектор инициализации
   - `EncryptFile()` - шифрует файл
   - `DecryptFile()` - дешифрует файл

## Детали реализации кода

### Инициализация криптографических компонентов

В коде используется многоуровневая структура объектов BouncyCastle для создания шифра:

```cs
BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
```

Данная структура состоит из:
- **IdeaEngine** - базовый шифр IDEA
- **CbcBlockCipher** - обертка, добавляющая режим CBC к блочному шифру
- **PaddedBufferedBlockCipher** - обертка, добавляющая паддинг и буферизацию данных

### Подготовка криптографических параметров

Для инициализации шифра используется объект `ParametersWithIV`, который инкапсулирует:
- Ключ шифрования (объект `KeyParameter`)
- Вектор инициализации (массив байтов)

```csharp
cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
```
Параметр `true` указывает на операцию шифрования, `false` - дешифрования.

### Процесс шифрования

Процесс шифрования данных в методе `EncryptFile` имеет следующие этапы:

1. Чтение исходных данных из файла:
   ```csharp
   byte[] inputBytes = File.ReadAllBytes(inputFile);
   ```

2. Генерация вектора инициализации:
   ```csharp
   byte[] iv = GenerateRandomIV();
   ```

3. Подготовка шифра:
   ```csharp
   BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
   cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
   ```

4. Шифрование данных:
   ```csharp
   byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
   int length = cipher.ProcessBytes(inputBytes, 0, inputBytes.Length, outputBytes, 0);
   cipher.DoFinal(outputBytes, length);
   ```
   Здесь `ProcessBytes` выполняет основное шифрование, а `DoFinal` обрабатывает последний блок и добавляет паддинг.

5. Сохранение результата:
   ```csharp
   using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
   {
       outputStream.Write(iv, 0, iv.Length);
       outputStream.Write(outputBytes, 0, outputBytes.Length);
   }
   ```
   Сначала записывается вектор инициализации, затем зашифрованные данные.

### Процесс дешифрования

Процесс дешифрования в методе `DecryptFile` включает следующие шаги:

1. Чтение зашифрованных данных:
   ```csharp
   byte[] inputBytes = File.ReadAllBytes(inputFile);
   ```

2. Извлечение вектора инициализации и зашифрованных данных:
   ```csharp
   byte[] iv = new byte[8];
   Array.Copy(inputBytes, 0, iv, 0, iv.Length);
   
   byte[] cipherText = new byte[inputBytes.Length - iv.Length];
   Array.Copy(inputBytes, iv.Length, cipherText, 0, cipherText.Length);
   ```

3. Подготовка шифра для дешифрования:
   ```csharp
   BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new IdeaEngine()));
   cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
   ```

4. Дешифрование данных:
   ```csharp
   byte[] outputBytes = new byte[cipher.GetOutputSize(cipherText.Length)];
   int length = cipher.ProcessBytes(cipherText, 0, cipherText.Length, outputBytes, 0);
   int finalLength = cipher.DoFinal(outputBytes, length);
   ```

5. Извлечение фактических данных (без паддинга):
   ```csharp
   byte[] result = new byte[length + finalLength];
   Array.Copy(outputBytes, 0, result, 0, length + finalLength);
   ```

6. Сохранение расшифрованных данных:
   ```csharp
   File.WriteAllBytes(outputFile, result);
   ```

## Особенности работы с библиотекой BouncyCastle

### Класс BufferedBlockCipher

BufferedBlockCipher обеспечивает буферизацию для блочных шифров. Его важные методы:
- `Init(bool forEncryption, ICipherParameters parameters)` - инициализирует шифр
- `GetOutputSize(int inputLen)` - вычисляет размер выходного буфера
- `ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)` - обрабатывает входные байты
- `DoFinal(byte[] output, int outOff)` - завершает шифрование/дешифрование

### Класс PaddedBufferedBlockCipher

Расширяет BufferedBlockCipher, добавляя паддинг (по умолчанию PKCS7):
- Автоматически добавляет паддинг при шифровании
- Удаляет паддинг при дешифровании

### Класс CbcBlockCipher

Реализует режим CBC для любого блочного шифра:
- Требует вектор инициализации (IV)
- В режиме шифрования каждый блок XOR-ится с предыдущим шифрованным блоком
- Для первого блока используется IV

### Класс IdeaEngine

Реализует алгоритм IDEA:
- Работает с блоками по 8 байт
- Требует ключ длиной 16 байт
- Использует 8.5 раундов преобразований

### Класс SecureRandom

Криптографически стойкий генератор случайных чисел:
- `NextBytes(byte[] bytes)` - заполняет массив случайными значениями

## Работа с файлами в приложении

Для работы с файлами используются стандартные классы .NET:
- `File.ReadAllBytes()` - читает все байты из файла
- `File.WriteAllBytes()` - записывает все байты в файл
- `FileStream` - обеспечивает потоковый доступ к файлу

## Алгоритм IDEA

IDEA (International Data Encryption Algorithm) - блочный шифр, разработанный в ETH Zürich. Основные характеристики:
- Размер блока: 64 бита (8 байт)
- Длина ключа: 128 бит (16 байт)
- Количество раундов: 8.5 (8 полных раундов и 1 выходное преобразование)

IDEA использует комбинацию трёх операций, работающих с 16-битными словами:
- XOR (побитовое исключающее ИЛИ)
- Сложение по модулю 2^16
- Умножение по модулю (2^16 + 1)

## Режим CBC (Cipher Block Chaining)

В режиме CBC каждый блок открытого текста XOR-ится с предыдущим блоком шифротекста перед шифрованием. Это обеспечивает зависимость шифрования каждого блока от всех предыдущих блоков.

Особенности режима CBC:
- Необходим вектор инициализации (IV) для первого блока
- Ошибка в одном блоке шифротекста влияет на расшифровку текущего и следующего блоков
- Обеспечивает высокий уровень безопасности, даже если в открытом тексте есть повторяющиеся блоки

## Безопасность

- Для каждого процесса шифрования генерируется случайный IV
- Ключ должен храниться в секрете и иметь длину 16 байт (128 бит)
- Рекомендуется регулярно менять ключи для обеспечения высокого уровня безопасности

## Примечание

Данное приложение разработано в учебных целях для демонстрации работы алгоритма IDEA в режиме CBC и применения криптографической библиотеки BouncyCastle в C# приложении.
