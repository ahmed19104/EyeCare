

using System.Linq.Expressions;

namespace EyeCare.Application.Abstractions.Persistence;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }

    List<Expression<Func<T, object>>> Includes { get; }
    
    Expression<Func<T, object>>? OrderByDescending { get; }

}
