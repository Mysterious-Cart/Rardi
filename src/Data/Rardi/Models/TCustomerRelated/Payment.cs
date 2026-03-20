namespace CHKS.Models;

using CHKS.Domain.Enums;

public class PaymentModel
{
    public PaymentType PaymentType { get; set; } = PaymentType.Bank;
    public decimal Amount { get; set; } = 0;
}
