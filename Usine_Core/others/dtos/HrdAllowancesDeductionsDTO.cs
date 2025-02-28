namespace Usine_Core.others.dtos
{
    public class HrdAllowancesDeductionsDTO
    {
        public int RecordId { get; set; }
        public string Allowance { get; set; }
        public int? AllowanceCheck { get; set; }
        public int? CalcType { get; set; }
        public int? EffectAs { get; set; }
        public int? Statu { get; set; }
        public string Branchid { get; set; }
        public int? CustomerCode { get; set; }
    }
}
