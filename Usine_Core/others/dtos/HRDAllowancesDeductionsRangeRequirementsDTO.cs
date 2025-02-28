using System.Collections.Generic;

namespace Usine_Core.others.dtos
{
    public class HRDAllowancesDeductionsRangeRequirementsDTO
    {
        public List<HrdAllowancesDeductionsDTO> AllowancesDeductions { get; set; }
        public List<HrdAllowanceDeductionRangesDTO> AllowanceDeductionRanges { get; set; }
    }
}
