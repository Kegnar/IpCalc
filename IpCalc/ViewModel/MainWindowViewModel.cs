using System.Net;
using System.Net.NetworkInformation;
using IpCalc.Model;

namespace IpCalc.ViewModel;

public class MainWindowViewModel : BaseViewModel
{
    public string InputMask
    {
        get => field;
        set
        {
            if (SetField(ref field, value))
            {
                var (mask, error) = IpCalculator.ValidateMask(value);
                if (!string.IsNullOrEmpty(error))
                {
                    AddError(error);
                    BinaryMask = string.Empty;
                }
                else
                {
                    ClearErrors();

                    BinaryMask = mask.AsBinary();
                    RecalculateNetwork();
                }
            }
        }
    }

    public string InputIp
    {
        get => field;
        set
        {
            if (SetField(ref field, value))
            {
                var (address, cidr, error) = IpCalculator.ValidateAddress(value);

                if (!string.IsNullOrEmpty(error))
                {
                    AddError(error);
                    BinaryIp = string.Empty;
                    ClearCalculatedFields();

                }
                else
                {
                    ClearErrors();
                    BinaryIp = address.AsBinary();

                    if (cidr.HasValue)
                    {
                        var (mask, _) = IpCalculator.CidrToSubnet(cidr.Value);
                        InputMask = mask.ToString(); // Записываем напрямую в поле, в обход триггеров
                        OnPropertyChanged(nameof(InputMask));
                        RecalculateNetwork();
                    }
                    else
                    {

                        InputMask = IpCalculator.GetDefaultMaskByClass(address).ToString();
                        OnPropertyChanged(nameof(InputMask));
                        RecalculateNetwork();
                    }
                }
                OnPropertyChanged(nameof(IsMaskInputEnabled));
            }
        }
    }

    #region Результаты расчетов в UI
    // Ленивое свойство - работает без явного сеттера
    public bool IsMaskInputEnabled => !string.IsNullOrEmpty(InputIp) && !InputIp.Contains('/');

    public string BinaryIp
    {
        get;
        private set => SetField(ref field, value);
    }
    public string BinaryMask
    {
        get;
        private set => SetField(ref field, value);
    }
    public string NetworkAddr
    {
        get;
        private set => SetField(ref field, value);
    }
    public string BroadcastAddr
    {
        get;
        private set => SetField(ref field, value);
    }
    public string StartAddr
    {
        get;
        private set => SetField(ref field, value);
    }
    public string EndAddr
    {
        get;
        private set => SetField(ref field, value);
    }
    public string HostCount
    {
        get;
        private set => SetField(ref field, value);
    }
    public string NetworkClass
    {
        get;
        private set => SetField(ref field, value);
    }
    #endregion

    #region Вызовы модели
    private void RecalculateNetwork()
    {
        var (ip, _, ipError) = IpCalculator.ValidateAddress(InputIp); //CIDR нинужон
        var (mask, maskError) = IpCalculator.ValidateMask(InputMask);
        if (!string.IsNullOrEmpty(ipError) || !string.IsNullOrEmpty(maskError))
        {
            ClearCalculatedFields();
            return;
        }

        NetworkAddr = IpCalculator.GetNetworkAddress(ip, mask).ToString();
        BroadcastAddr = IpCalculator.GetBroadcastAddress(ip, mask).ToString();
        StartAddr = IpCalculator.GetStartHost(ip, mask).ToString();
        EndAddr = IpCalculator.GetLastHost(ip, mask).ToString();
        HostCount = IpCalculator.GetHostsCount(mask).ToString("N0");
        NetworkClass = IpCalculator.GetNetworkClass(ip);
    }

    private void ClearCalculatedFields()
    {
        NetworkAddr = string.Empty;
        BroadcastAddr = string.Empty;
        StartAddr = string.Empty;
        EndAddr = string.Empty;
        HostCount = string.Empty;
        NetworkClass = string.Empty;
    }
    #endregion

}