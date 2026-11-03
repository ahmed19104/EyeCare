namespace EyeCare.Infrastructure.Billing;

public sealed class RefundOptions
{
    public const string SectionName = "Billing:Refund";

    public int WindowDays { get; set; } = 30;
}
