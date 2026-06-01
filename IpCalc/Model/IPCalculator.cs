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
            return (IPAddress.None, null, "Некорректный формат IP (должно быть 4 октета)");

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
            return (IPAddress.None, "Введите маску ");

        // 1. Базовый парсинг в IP-адрес
        if (!IPAddress.TryParse(mask, out IPAddress ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            return (IPAddress.None, "Не является маской IPv4");

        // 2. Строгая проверка формата (ровно 4 числа без сокращений типа "255.255")
        var octets = mask.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (octets.Length != 4 || octets.Any(o => !byte.TryParse(o, out _)))
            return (IPAddress.None, "Некорректный формат маски (должно быть 4 октета)");

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

    public static (IPAddress netmask, string errormsg) CidrToSubnet(int cidrValue)
    {

        if (cidrValue < 1 || cidrValue > 32)
            return (IPAddress.None, "Недопустимое значение CIDR");

        // Крайний случай для /0, чтобы избежать сдвига uint на 32 бита (что в C# вернет uint.MaxValue)
        if (cidrValue == 0) return (IPAddress.Any, string.Empty);

        // Вычисляем маску путем сдвига бит и инвертирования
        uint mask = uint.MaxValue << (32 - cidrValue);
        byte[] bytes = BitConverter.GetBytes(mask);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return (new IPAddress(bytes), string.Empty);
    }

    public static IPAddress GetNetworkAddress(IPAddress ip, IPAddress mask)
    {
        byte[] ipBytes = ip.GetAddressBytes();
        byte[] maskBytes = mask.GetAddressBytes();
        byte[] result = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        return new IPAddress(result);
    }

    public static IPAddress GetBroadcastAddress(IPAddress ip, IPAddress mask)
    {
        byte[] ipBytes = ip.GetAddressBytes();
        byte[] maskBytes = mask.GetAddressBytes();
        byte[] result = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
        }

        return new IPAddress(result);
    }

    public static IPAddress GetStartHost(IPAddress ip, IPAddress mask)
    {
        byte[] netBytes = GetNetworkAddress(ip, mask).GetAddressBytes();
        int cidr = GetCidrFromMask(mask);

        //для масок 31 и 32 первый хост = адрес сети
        if (cidr >= 31) return new IPAddress(netBytes);

        netBytes[3] += 1;
        return new IPAddress(netBytes);
    }


    public static IPAddress GetLastHost(IPAddress ip, IPAddress mask)
    {
        byte[] broadcastBytes = GetBroadcastAddress(ip, mask).GetAddressBytes();
        int cidr = GetCidrFromMask(mask);
        if (cidr >= 31) return new IPAddress(broadcastBytes);
        broadcastBytes[3] -= 1;
        return new IPAddress(broadcastBytes);
    }

    public static double GetHostsCount(IPAddress mask)
    {
        int cidr = GetCidrFromMask(mask);
        return cidr
            switch
        {
            32 => 1,
            31 => 2,
            _ => Math.Pow(2, 32 - GetCidrFromMask(mask)) - 2
        };
    }

    public static string GetNetworkClass(IPAddress ip)
    {
        return ip.GetAddressBytes()[0] switch
        {
            >= 1 and <= 126   => "A",
            127               => "A (Loopback)",
            >= 128 and <= 191 => "B",
            >= 192 and <= 223 => "C",
            >= 224 and <= 239 => "D (Multicast)",
            _                 => "E (Experimental)"
        };


    }
    private static int GetCidrFromMask(IPAddress mask)
    {
        byte[] maskBytes = mask.GetAddressBytes();
        if (BitConverter.IsLittleEndian) Array.Reverse(maskBytes);
        uint maskValue = BitConverter.ToUInt32(maskBytes, 0);
        int count = 0;
        while (maskValue > 0)
        {
            count += (int)(maskValue & 1);
            maskValue >>= 1;
        }

        return count;
    }

    public static IPAddress GetDefaultMaskByClass(IPAddress address)
    {
        if (address == null) return IPAddress.Parse("255.255.255.0");
        byte firstByte = address.GetAddressBytes()[0];

        return address.GetAddressBytes()[0] switch
        {
            >= 1 and <= 126   => IPAddress.Parse("255.0.0.0"),
            >= 128 and <= 191 => IPAddress.Parse("255.255.0.0"),
            >= 192 and <= 223 => IPAddress.Parse("255.255.255.0"),
            _                 => IPAddress.Parse("255.255.255.255")
        };
    }
}