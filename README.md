# Реализация системы симметричного блочного шифрования IDEA в режиме CBC

## Описание проекта

Данный проект представляет собой реализацию системы симметричного блочного шифрования, позволяющую шифровать и дешифровать файлы на диске с использованием алгоритма IDEA (International Data Encryption Algorithm) в режиме CBC (Cipher Block Chaining).

## Алгоритм IDEA (International Data Encryption Algorithm)

IDEA - это симметричный блочный шифр, разработанный в 1991 году Джеймсом Мэсси и Сюецзя Лаем. Основные характеристики:

- Размер блока: 64 бита
- Длина ключа: 128 бит
- Количество раундов: 8.5 (8 полных раундов и 1 заключительный полураунд)

### Принцип работы IDEA:

1. **Генерация подключей**: Из 128-битного ключа генерируются 52 подключа по 16 бит каждый.
2. **Обработка данных**: Входной 64-битный блок разделяется на 4 подблока по 16 бит.
3. **Раундовые преобразования**: В каждом раунде применяются операции:
   - Умножение по модулю 2^16+1 (где 0 представляется как 2^16)
   - Сложение по модулю 2^16
   - Побитовое XOR
4. **Смешивание и перестановка**: Подблоки обрабатываются этими операциями в определенном порядке.
5. **Выходное преобразование**: После 8 раундов, выполняется заключительное преобразование (полураунд).

Криптографическая стойкость IDEA основана на сочетании операций из разных алгебраических групп, которые не являются линейными друг относительно друга.

## Режим CBC (Cipher Block Chaining)

CBC - это режим работы блочного шифра, при котором каждый блок открытого текста XOR-ится с результатом шифрования предыдущего блока перед тем, как быть зашифрованным.

### Принцип работы CBC:

1. **Вектор инициализации (IV)**: Для шифрования первого блока используется случайный блок данных (IV).
2. **Шифрование**:
   - Блок открытого текста XOR-ится с предыдущим зашифрованным блоком (или с IV для первого блока).
   - Результат шифруется с помощью блочного шифра (в нашем случае IDEA).
   - Полученный шифротекст становится входом для следующего блока.
3. **Дешифрование**:
   - Блок шифротекста дешифруется с помощью блочного шифра.
   - Результат XOR-ится с предыдущим блоком шифротекста (или с IV для первого блока).

### Преимущества CBC:
- Одинаковые блоки открытого текста шифруются по-разному благодаря зависимости от предыдущих блоков.
- Ошибка в одном блоке шифротекста влияет только на два последовательных блока расшифрованного текста.

### Недостатки CBC:
- Невозможность параллельного шифрования (хотя дешифрование можно распараллелить).
- Необходимость в случайном и уникальном векторе инициализации для каждого сеанса шифрования.

## Требования

`dotnet add package System.Windows.Forms --version 4.0.0`