using System;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Interface for municipal entities that can be managed by the generic repository
    /// Provides type safety and municipal-specific constraints for data management
    /// </summary>
    public interface IMunicipalEntity
    {
        Guid Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? LastModified { get; set; }
        string Municipality { get; set; }
        string Province { get; set; }
        bool IsActive { get; set; }
    }

    /// <summary>
    /// Interface for entities that can be compared for municipal operations
    /// Enables custom sorting and priority management
    /// </summary>
    public interface IMunicipalComparable : IMunicipalEntity, IComparable<IMunicipalComparable>
    {
        int Priority { get; }
        DateTime SubmittedAt { get; }
        int Points { get; }
    }

    /// <summary>
    /// Interface for entities that support municipal workflow operations
    /// Enables operator overloading for municipal business logic
    /// </summary>
    public interface IMunicipalWorkflow
    {
        bool CanMerge { get; }
        bool CanEscalate { get; }
        bool CanAssign { get; }
        string WorkflowStatus { get; }
    }
}
