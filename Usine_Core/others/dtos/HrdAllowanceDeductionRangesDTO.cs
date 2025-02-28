namespace Usine_Core.others.dtos
{
    public class HrdAllowanceDeductionRangesDTO
    {
        public int? AllowanceId { get; set; }
        public long Lineid { get; set; }
        public double? FromValue { get; set; }
        public double? ToValue { get; set; }
        public double? Valu { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }
    }
}
