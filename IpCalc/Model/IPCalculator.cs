using System.Net;

using System.Net.Sockets;

namespace IpCalc.Model;

public static class IpCalculator
{

    public static (string binaryAddr, string ErrorMsg) AddressAsBinary(string ip)
    {
        try
        {
            if (!IPAddress.TryParse(ip, out var ipAddress)) throw new ArgumentException();

            return (ipAddress.AsBinary(), String.Empty);
        }
        catch (ArgumentException)
        {
            return (String.Empty, nameof(AddressAsBinary)); //TODO: придумать что-нибудь вменяемое
        }
    }

    public static (IPAddress address, int? cidr, string ErrorMsg) ValidateAddress(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return (IPAddress.None, null, "Введите адрес");

        // Ограничиваем разделение: максимум 2 части. 
        // Если слэшей больше, parts.Length всё равно будет равен 2, и вторая часть завалит валидацию CIDR.
        var parts = input.Split('/', 2);
        var ipPart = parts[0];

        // 1. Валидация базового формата IP
        if (!IPAddress.TryParse(ipPart, out var ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            return (IPAddress.None, null, "Не является адресом IPv4");

     
        var octets = ipPart.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (octets.Length != 4 || octets.Any(o => !byte.TryParse(o, out _)))
            return (IPAddress.None, null, "Некорректный формат IPv4 (должно быть 4 октета)");

        // 3. Если есть слэш, проверяем CIDR
        if (parts.Length > 1)
        {
            var cidrPart = parts[1];

            // Если после слэша пусто или ввели второй слэш (например, "24/32")
            if (int.TryParse(cidrPart, out int cidrValue) && cidrValue is >= 0 and <= 32)
            {
                return (ipAddress, cidrValue, string.Empty);
            }
            return (IPAddress.None, null, "Некорректная маска CIDR (0-32)");
        }

        // Если слэша нет, возвращаем только адрес
        return (ipAddress, null, string.Empty);
    }


    public static (IPAddress address, string ErrorMsg) ValidateMask(string mask)
    {
        if (string.IsNullOrWhiteSpace(mask))
            return (IPAddress.None, "Введите маску подсети");

        // 1. Базовый парсинг в IP-адрес
        if (!IPAddress.TryParse(mask, out IPAddress ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            return (IPAddress.None, "Не является маской IPv4");

        // 2. Строгая проверка формата (ровно 4 числа без сокращений типа "255.255")
        var octets = mask.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (octets.Length != 4 || octets.Any(o => !byte.TryParse(o, out _)))
            return (IPAddress.None, "Некорректный формат (должно быть 4 октета, например 255.255.255.0)");

        // 3. Преобразуем в 32-битное число
        byte[] bytes = ipAddress.GetAddressBytes();
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        uint maskValue = BitConverter.ToUInt32(bytes, 0);

        // Сетевая маска не может состоять из одних нулей (0.0.0.0)
        if (maskValue == 0)
            return (IPAddress.None, "Маска подсети не может быть 0.0.0.0");

        // 4. Битовая проверка (последовательность единиц, затем нулей)
        uint inverted = ~maskValue;
        uint nextPowerOfTwo = inverted + 1;

        if ((nextPowerOfTwo & (nextPowerOfTwo - 1)) == 0)
            return (ipAddress, string.Empty);

        return (IPAddress.None, "Некорректная маска подсети (нарушена последовательность бит)");
    }


}