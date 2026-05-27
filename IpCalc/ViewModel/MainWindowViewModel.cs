using System.Net;
using IpCalc.Model;

namespace IpCalc.ViewModel;

public class MainWindowViewModel : BaseViewModel
{

    public bool IsMaskInputEnabled
    {
        get => field;
        private set => SetField(ref field, value);
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
                    AddError(error); // Автоматически запишет ошибку для "InputIp"
                    BinaryIp = string.Empty;
                    IsMaskInputEnabled = true;
                }
                else
                {
                    ClearErrors(); // Автоматически очистит ошибки для "InputIp"
                    BinaryIp = address.AsBinary();
                    IsMaskInputEnabled = !cidr.HasValue;
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

    


}