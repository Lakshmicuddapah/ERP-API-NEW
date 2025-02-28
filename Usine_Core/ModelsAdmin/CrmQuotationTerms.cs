using System;
using System.Collections.Generic;

namespace Usine_Core.ModelsAdmin
{
    public partial class CrmQuotationTerms
    {
        public int RecordId { get; set; }
        public int Sno { get; set; }
        public string Term { get; set; }

        public virtual CrmQuotationUni Record { get; set; }
    }
}
