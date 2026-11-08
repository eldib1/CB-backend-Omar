namespace ClutchBids.Domain.DTOs.Reporting;

public class ReviewUserReportRequestDto
{
    public long ReportId { get; set; }
    public int Status { get; set; }
    public string AdminNotes { get; set; }
    public int? AdminAction { get; set; }
}
