using System.ComponentModel.DataAnnotations;
using SharedModules;

namespace IRDA.DAL;

public class PendingStatusReports
{
    public string ReportId{ get; set; }
    public Stages Stage{ get; set; }
    [Range(0, int.MaxValue,ErrorMessage = "Count cannot be negative")]
    public int Count{ get; set; }
    public required string Month{ get; set; }
    public int Year{ get; set; }
}
