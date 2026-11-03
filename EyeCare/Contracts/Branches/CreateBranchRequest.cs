namespace EyeCareAPI.Contracts.Branches
{
    public sealed record CreateBranchRequest(
        string Name,
        string Address,
        string PhoneNumber);
}
