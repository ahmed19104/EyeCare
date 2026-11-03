using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Domain.Entities;
using EyeCare.Domain.Identity;
using System.Linq.Expressions;

namespace EyeCare.Application.Identity.RefreshTokens.Specifications;

public sealed class RefreshTokenHashSpecification
    : ISpecification<RefreshToken>
{
    public RefreshTokenHashSpecification(string hash)
    {
        Criteria = token =>
            token.TokenHash == hash;
    }

    public Expression<Func<RefreshToken, bool>> Criteria { get; }
    public List<Expression<Func<RefreshToken, object>>> Includes { get; } = new List<Expression<Func<RefreshToken, object>>>();
    public Expression<Func<RefreshToken, object>>?
    OrderByDescending
    { get; } = null;
}
