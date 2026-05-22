using System.Net;
using IpCalc.Model;

namespace IpCalc.ViewModel;

public class MainWindowViewModel : BaseViewModel
{

    public string InputIp
    {
        get => field;
        set
        {
            if (SetField(ref field, value))
            {
                var (address, error) = IpCalculator.ValidateAddress(value);
                IpErrorMsg=error;
                if (string.IsNullOrEmpty(error))
                {
                    BinaryIp = address.AsBinary();
                }
                else
                {
                    BinaryIp = string.Empty;
                }
            }
        }
    }


    public string BinaryIp
    {
        get => field;
        private set => SetField(ref field, value);
    }
    public string BinaryMask { get; set; }

    public string NetworkAddr { get; set; }
    public string BroadcastAddr { get; set; }
    public string StartAddr { get; set; }
    public string EndAddr { get; set; }
    public string HostCount { get; set; }
    public string NetworkClass { get; set; }

    public string IpErrorMsg
    {
        get => field;
        set => SetField(ref field, value);
    }

    public string MaskErrorMsg
    {
        get => field;
        set => SetField(ref field, value);
    }



}