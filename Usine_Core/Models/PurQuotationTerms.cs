﻿using System;
using System.Collections.Generic;

namespace Usine_Core.Models
{
    public partial class PurQuotationTerms
    {
        public int? RecordId { get; set; }
        public int? Sno { get; set; }
        public string Term { get; set; }
        public string BranchId { get; set; }
        public int? CustomerCode { get; set; }

        public virtual PurQuotationUni Record { get; set; }
    }
}
