using System.ComponentModel.DataAnnotations;

namespace SecurePassword.Models.Common;

/// <summary>
///     Base class for all entities in the database.
///     This class implements the <see cref="IEntity" /> interface.
///     This class is abstract, so it cannot be instantiated.
///     We use this class to avoid code duplication.
/// </summary>
public abstract class BaseEntity : IEntity
{
    /// <summary>
    ///     The primary key of the entity.
    /// </summary>
    [Key]
    public int Id { get; init; }
}