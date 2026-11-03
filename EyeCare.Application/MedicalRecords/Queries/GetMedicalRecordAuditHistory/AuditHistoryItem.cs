using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeCare.Application.MedicalRecords.Queries.GetMedicalRecordAuditHistory
{
    public sealed record AuditHistoryItem(
     Guid Id,
     string Action,
     string? UserId,
     string Changes,
     DateTime CreatedAt);
}
