using System.Net;

using System.Net.Sockets;

namespace IpCalc.Model;

public static class IpCalculator
{
    //public static (string Network, string Broadcast, string StartAddr, string EndAddr, string Hosts, string NetworkClass) Calculate(string ip, string mask)
    //{
    //    if (!IPAddress.TryParse(ip, out var IpAddr)) throw new ArgumentException("Некорректный адрес");
    //    if (!IPAddress.TryParse(mask, out var netMask)) throw new ArgumentException("Некорректная маска");

    //    var ipBytes = IpAddr.GetAddressBytes();
    //    var maskBytes = netMask.GetAddressBytes();

    //    if ()

    //        return (networkStr, broadcastStr, startIpStr, endIpStr, hostsStr, networkClassStr);
    //}
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

        // Разделяем на IP и маску CIDR
        var parts = input.Split('/');
        var ipPart = parts[0];

        //  Валидация самого IP
        if (!IPAddress.TryParse(ipPart, out var ipAddress))
            return (IPAddress.None, null, "Некорректный IP-адрес");

         // Ограничиваем проверку только IPv4 адресами
        if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
            return (IPAddress.None,null ,"Не является адресом IPv4");

        // Если есть слэш, проверяем CIDR
        if (parts.Length > 1)
        {
            var cidrPart = parts[1];
            if (int.TryParse(cidrPart, out int cidrValue) && cidrValue is >= 0 and <= 32)
            {
                return (ipAddress, cidrValue, string.Empty);
            }
            return (IPAddress.None, null, "Некорректная маска CIDR (0-32)");
        }

        // Если слэша нет, возвращаем только адрес
        return (ipAddress, null, string.Empty);
    }

 
        public static (IPAddress address, string ErrorMsg ) ValidateMask(string mask)
        {
            // 1. Проверяем базовый формат IP-адреса
            if (!IPAddress.TryParse(mask, out IPAddress ipAddress) || ipAddress.AddressFamily != AddressFamily.InterNetwork)
                return (IPAddress.None, "Не является маской IPv4");


            // 2. Преобразуем в 32-битное число (с учетом Reverse для правильного порядка байт)
            byte[] bytes = ipAddress.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            uint maskValue = BitConverter.ToUInt32(bytes, 0);
            

        
            // 3. Битовый трюк: инвертируем маску и прибавляем 1.
            // Если маска корректна, результат операции (NOT mask) + 1 будет равен степени двойки.
            // Операция (x & (x - 1)) == 0 проверяет, является ли число степенью двойки.
            uint inverted = ~maskValue;
            uint nextPowerOfTwo = inverted + 1;

            if ((nextPowerOfTwo & (nextPowerOfTwo - 1)) == 0) return (ipAddress,String.Empty);
            return (IPAddress.None, "Некорректная маска");
        }
    

}