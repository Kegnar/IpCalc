using System.Net;

namespace IpCalc.Model;
/// <summary>
/// Расширение класса System.Net.IPAddress.
/// Возвращает двоинчное представление IP
/// </summary>
public static class IpAddressExtensions
{
    public static string AsBinary(this IPAddress ipAddress)
    {
        if (ipAddress == null)
            throw new ArgumentNullException(nameof(ipAddress));

        byte[] addressBytes = ipAddress.GetAddressBytes();
        return string.Join(".", addressBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

}