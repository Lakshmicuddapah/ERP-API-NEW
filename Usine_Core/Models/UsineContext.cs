using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Usine_Core.Controllers;

namespace Usine_Core.Models
{
    public partial class UsineContext : DbContext
    {
        public UsineContext()
        {
        }

        public UsineContext(DbContextOptions<UsineContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountsAssign> AccountsAssign { get; set; }
        public virtual DbSet<AdmTaxes> AdmTaxes { get; set; }
        public virtual DbSet<AdmUserwiseAssigns> AdmUserwiseAssigns { get; set; }
        public virtual DbSet<Admroles> Admroles { get; set; }
        public virtual DbSet<SaleRxPriceList> SaleRxPriceList { get; set; }
        public virtual DbSet<SaleRxDiscountList> SaleRxDiscountList { get; set; }
        public virtual DbSet<CrmDiscountListDet> CrmDiscountListDet { get; set; }
        public virtual DbSet<CrmDiscountListUni> CrmDiscountListUni { get; set; }
        public virtual DbSet<CrmPriceListDet> CrmPriceListDet { get; set; }
        public virtual DbSet<CrmPriceListUni> CrmPriceListUni { get; set; }
        public virtual DbSet<CrmOrdersRxdet> CrmOrdersRxdet { get; set; }
        public virtual DbSet<CrmTeleCallingRx> CrmTeleCallingRx { get; set; }
        public virtual DbSet<CrmOrdersRxuni> CrmOrdersRxuni { get; set; }
        public virtual DbSet<CrmSetup> CrmSetup { get; set; }
        public virtual DbSet<CrmTargetSettings> CrmTargetSettings { get; set; }
        public virtual DbSet<CrmEnquiriesRx> CrmEnquiriesRx { get; set; }
        public virtual DbSet<CrmTaxAssigningDet> CrmTaxAssigningDet { get; set; }
        public virtual DbSet<CrmTaxAssigningUni> CrmTaxAssigningUni { get; set; }
        public virtual DbSet<CrmTeleCallDetails> CrmTeleCallDetails { get; set; }
        public virtual DbSet<CrmQuotationDet> CrmQuotationDet { get; set; }
        public virtual DbSet<CrmQuotationTaxes> CrmQuotationTaxes { get; set; }
        public virtual DbSet<CrmQuotationTerms> CrmQuotationTerms { get; set; }
        public virtual DbSet<CrmQuotationUni> CrmQuotationUni { get; set; }
        public virtual DbSet<CrmSaleOrderDet> CrmSaleOrderDet { get; set; }
        public virtual DbSet<CrmSaleOrderTaxes> CrmSaleOrderTaxes { get; set; }
        public virtual DbSet<CrmSaleOrderTerms> CrmSaleOrderTerms { get; set; }
        public virtual DbSet<CrmSaleOrderUni> CrmSaleOrderUni { get; set; }
        public virtual DbSet<CrmBillSubmissionsDet> CrmBillSubmissionsDet { get; set; }
        public virtual DbSet<CrmBillSubmissionsUni> CrmBillSubmissionsUni { get; set; }
        public virtual DbSet<CrmPostTeleCalling> CrmPostTeleCalling { get; set; }
        public virtual DbSet<FinAccGroups> FinAccGroups { get; set; }
        public virtual DbSet<FinAccounts> FinAccounts { get; set; }
        public virtual DbSet<FinAccountsAssign> FinAccountsAssign { get; set; }
        public virtual DbSet<FinBankCheckings> FinBankCheckings { get; set; }
        public virtual DbSet<Finassets> Finassets { get; set; }
        public virtual DbSet<FinassetsView> FinassetsView { get; set; }
        public virtual DbSet<FinexecDet> FinexecDet { get; set; }
        public virtual DbSet<FinexecDetHistory> FinexecDetHistory { get; set; }
        public virtual DbSet<FinexecUni> FinexecUni { get; set; }
        public virtual DbSet<FinexecUniHistory> FinexecUniHistory { get; set; }
        public virtual DbSet<HrdAllowanceDeductionRanges> HrdAllowanceDeductionRanges { get; set; }
        public virtual DbSet<HrdAllowancesDeductions> HrdAllowancesDeductions { get; set; }
        public virtual DbSet<HrdAllowancesDeductionsEffects> HrdAllowancesDeductionsEffects { get; set; }
        public virtual DbSet<HrdDepartments> HrdDepartments { get; set; }
        public virtual DbSet<HrdDesignations> HrdDesignations { get; set; }
        public virtual DbSet<HrdDesignationsAllowances> HrdDesignationsAllowances { get; set; }
        public virtual DbSet<HrdDesignationsLeaves> HrdDesignationsLeaves { get; set; }
        public virtual DbSet<HrdResumeCurriculum> HrdResumeCurriculum { get; set; }
        public virtual DbSet<HrdResumeExperience> HrdResumeExperience { get; set; }
        public virtual DbSet<HrdResumeUni> HrdResumeUni { get; set; }
        public virtual DbSet<HrdInterviewCandidates> HrdInterviewCandidates { get; set; }
        public virtual DbSet<HrdInterviewsPanel> HrdInterviewsPanel { get; set; }
        public virtual DbSet<HrdInterviewsUni> HrdInterviewsUni { get; set; }
        public virtual DbSet<HrdEmpInOutDetails> HrdEmpInOutDetails { get; set; }
        public virtual DbSet<HrdEmployeeAllowancesDeductions> HrdEmployeeAllowancesDeductions { get; set; }
        public virtual DbSet<HrdEmployeeCurriculum> HrdEmployeeCurriculum { get; set; }
        public virtual DbSet<HrdEmployeeExperience> HrdEmployeeExperience { get; set; }
        public virtual DbSet<HrdEmployeeFamilyDetails> HrdEmployeeFamilyDetails { get; set; }
        public virtual DbSet<HrdEmployeeIdentifications> HrdEmployeeIdentifications { get; set; }
        public virtual DbSet<HrdEmployeeLeaves> HrdEmployeeLeaves { get; set; }
        public virtual DbSet<HrdEmployees> HrdEmployees { get; set; }
        public virtual DbSet<HrdLeaves> HrdLeaves { get; set; }
        public virtual DbSet<HrdShifts> HrdShifts { get; set; }
        public virtual DbSet<HrdAdvances> HrdAdvances { get; set; }
        public virtual DbSet<HrdAttendances> HrdAttendances { get; set; }
        public virtual DbSet<HrdLeaveApplications> HrdLeaveApplications { get; set; }

        public virtual DbSet<InvDepartments> InvDepartments { get; set; }
        public virtual DbSet<InvGroups> InvGroups { get; set; }
        public virtual DbSet<InvLosses> InvLosses { get; set; }
        public virtual DbSet<InvMaterialCompleteDetailsView> InvMaterialCompleteDetailsView { get; set; }
        public virtual DbSet<InvMaterialManagement> InvMaterialManagement { get; set; }
        public virtual DbSet<InvMaterialUnits> InvMaterialUnits { get; set; }
        public virtual DbSet<InvMaterials> InvMaterials { get; set; }
        public virtual DbSet<InvSetup> InvSetup { get; set; }
        public virtual DbSet<InvStores> InvStores { get; set; }
        public virtual DbSet<InvTransactionsDet> InvTransactionsDet { get; set; }
        public virtual DbSet<InvTransactionsUni> InvTransactionsUni { get; set; }
        public virtual DbSet<InvUm> InvUm { get; set; }
        public virtual DbSet<MaiEquipGroups> MaiEquipGroups { get; set; }
        public virtual DbSet<MaiEquipment> MaiEquipment { get; set; }
        public virtual DbSet<MaiEquipmentInsurances> MaiEquipmentInsurances { get; set; }
        public virtual DbSet<MaiEquipmentPmdetails> MaiEquipmentPmdetails { get; set; }
        public virtual DbSet<MaiEquipmentPreventiveMaintenances> MaiEquipmentPreventiveMaintenances { get; set; }
        public virtual DbSet<MaiEquipmentSpecifications> MaiEquipmentSpecifications { get; set; }
        public virtual DbSet<MakeSessions> MakeSessions { get; set; }
        public virtual DbSet<MisCountryMaster> MisCountryMaster { get; set; }
        public virtual DbSet<MisStateMaster> MisStateMaster { get; set; }
        public virtual DbSet<MisTasks> MisTasks { get; set; }
        public virtual DbSet<MisCoveringLetterDetails> MisCoveringLetterDetails { get; set; }
        public virtual DbSet<PartyCreditDebitNotes> PartyCreditDebitNotes { get; set; }
        public virtual DbSet<PartyDeaprtmentDetails> PartyDeaprtmentDetails { get; set; }
        public virtual DbSet<PartyDetails> PartyDetails { get; set; }
        public virtual DbSet<PartyAddresses> PartyAddresses { get; set; }
        public virtual DbSet<PartyDepartments> PartyDepartments { get; set; }

        public virtual DbSet<PartyPaymentDetails> PartyPaymentDetails { get; set; }
        public virtual DbSet<PartyPaymentsdet> PartyPaymentsdet { get; set; }
        public virtual DbSet<PartyPaymentsuni> PartyPaymentsuni { get; set; }
        public virtual DbSet<PartyTransactions> PartyTransactions { get; set; }
        public virtual DbSet<PpcProcessesMaster> PpcProcessesMaster { get; set; }
        public virtual DbSet<PpcBatchPlanningEmployeeAssignings> PpcBatchPlanningEmployeeAssignings { get; set; }
        public virtual DbSet<PpcBatchPlanningProcesses> PpcBatchPlanningProcesses { get; set; }
        public virtual DbSet<PpcBatchPlanningUni> PpcBatchPlanningUni { get; set; }
        public virtual DbSet<PpcMassPlanningDet> PpcMassPlanningDet { get; set; }
        public virtual DbSet<PpcMassPlanningUni> PpcMassPlanningUni { get; set; }
        public virtual DbSet<PpcBatchPlanningSaleOrders> PpcBatchPlanningSaleOrders { get; set; }
        public virtual DbSet<PpcBatchProcessWiseDetailsDet> PpcBatchProcessWiseDetailsDet { get; set; }
        public virtual DbSet<PpcBatchProcessWiseDetailsUni> PpcBatchProcessWiseDetailsUni { get; set; }
        public virtual DbSet<ProItemWiseAttachmentsUni> ProItemWiseAttachmentsUni { get; set; }
        public virtual DbSet<ProItemWiseDesignations> ProItemWiseDesignations { get; set; }
        public virtual DbSet<ProItemWiseEquipment> ProItemWiseEquipment { get; set; }
        public virtual DbSet<ProItemWiseIngredients> ProItemWiseIngredients { get; set; }
        public virtual DbSet<PurPurchaseEnquiryDet> PurPurchaseEnquiryDet { get; set; }
        public virtual DbSet<PurPurchaseEnquiryDetHistory> PurPurchaseEnquiryDetHistory { get; set; }
        public virtual DbSet<PurPurchaseEnquiryNotes> PurPurchaseEnquiryNotes { get; set; }
        public virtual DbSet<PurPurchaseEnquiryNotesHistory> PurPurchaseEnquiryNotesHistory { get; set; }
        public virtual DbSet<PurPurchaseEnquiryUni> PurPurchaseEnquiryUni { get; set; }
        public virtual DbSet<PurPurchaseEnquiryUniHistory> PurPurchaseEnquiryUniHistory { get; set; }
        public virtual DbSet<PurPurchaseOrderDet> PurPurchaseOrderDet { get; set; }
        public virtual DbSet<PurPurchaseOrderTaxes> PurPurchaseOrderTaxes { get; set; }
        public virtual DbSet<PurPurchaseOrderTerms> PurPurchaseOrderTerms { get; set; }
        public virtual DbSet<PurPurchaseOrderUni> PurPurchaseOrderUni { get; set; }
        public virtual DbSet<PurPurchaseRequestDet> PurPurchaseRequestDet { get; set; }
        public virtual DbSet<PurPurchaseRequestDetHistory> PurPurchaseRequestDetHistory { get; set; }
        public virtual DbSet<PurPurchaseRequestUni> PurPurchaseRequestUni { get; set; }
        public virtual DbSet<PurPurchaseRequestUniHistory> PurPurchaseRequestUniHistory { get; set; }
        public virtual DbSet<PurQuotationDet> PurQuotationDet { get; set; }
        public virtual DbSet<PurQuotationTaxes> PurQuotationTaxes { get; set; }
        public virtual DbSet<PurQuotationTerms> PurQuotationTerms { get; set; }
        public virtual DbSet<PurQuotationUni> PurQuotationUni { get; set; }
        public virtual DbSet<PurPurchaseTaxes> PurPurchaseTaxes { get; set; }
        public virtual DbSet<PurPurchasesDet> PurPurchasesDet { get; set; }
        public virtual DbSet<PurPurchasesUni> PurPurchasesUni { get; set; }
        public virtual DbSet<PurPurchaseReturnTaxes> PurPurchaseReturnTaxes { get; set; }
        public virtual DbSet<PurPurchaseReturnsDet> PurPurchaseReturnsDet { get; set; }
        public virtual DbSet<PurPurchaseReturnsUni> PurPurchaseReturnsUni { get; set; }

        public virtual DbSet<PurSetup> PurSetup { get; set; }
        public virtual DbSet<PurSupplierGroups> PurSupplierGroups { get; set; }
        public virtual DbSet<PurTerms> PurTerms { get; set; }
        public virtual DbSet<Purpurchasetypes> Purpurchasetypes { get; set; }
        public virtual DbSet<PurEmails> PurEmails { get; set; }

        //QC
        public virtual DbSet<QcProcessWiseDetails> QcProcessWiseDetails { get; set; }
        public virtual DbSet<QcTestings> QcTestings { get; set; }
        public virtual DbSet<QcTraTestsDet> QcTraTestsDet { get; set; }
        public virtual DbSet<QcTraTestsUni> QcTraTestsUni { get; set; }

        public virtual DbSet<ResOutletMaster> ResOutletMaster { get; set; }
        public virtual DbSet<SalSalesDet> SalSalesDet { get; set; }
        public virtual DbSet<SalSalesTaxes> SalSalesTaxes { get; set; }
        public virtual DbSet<SalSalesUni> SalSalesUni { get; set; }
        public virtual DbSet<SalSaleReturnTaxes> SalSaleReturnTaxes { get; set; }
        public virtual DbSet<SalSaleReturnsDet> SalSaleReturnsDet { get; set; }
        public virtual DbSet<SalSaleReturnsUni> SalSaleReturnsUni { get; set; }
        public virtual DbSet<SalcustomerGroups> SalcustomerGroups { get; set; }
        public virtual DbSet<TotAdvanceDetails> TotAdvanceDetails { get; set; }
        public virtual DbSet<TransactionsAudit> TransactionsAudit { get; set; }
        public virtual DbSet<UserCompleteProfile> UserCompleteProfile { get; set; }
        public virtual DbSet<UserPostingAccess> UserPostingAccess { get; set; }
        public virtual DbSet<UserPostings> UserPostings { get; set; }
        public virtual DbSet<UserPostingsComments> UserPostingsComments { get; set; }
        public virtual DbSet<UserPostingsLikes> UserPostingsLikes { get; set; }
        public virtual DbSet<UsrAut> UsrAut { get; set; }
        public virtual DbSet<TotSalesSupportings> TotSalesSupportings { get; set; }
        public SqlConnection db { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                // optionsBuilder.UseSqlServer("Server=DESKTOP-FND9GDG;Database=Usine;Trusted_Connection=True;");
                String s1, s2, s3, s4;
                try
                {
                    General g = new General();
                    var ser = g.setServerDetails();
                    s1 = ser.servername;
                    s2 = ser.username;
                    s3 = ser.password;
                    s4 = ser.database;


                  
                        if (s2.Trim() == "")
                        {
                            optionsBuilder.UseSqlServer("Server=" + s1 + ";Database=" + s4 + ";Trusted_Connection=True;");
                        }
                        else
                        {
                            optionsBuilder.UseSqlServer("server=" + s1 + ";user id=" + s2 + ";password=" + s3 + ";database=" + s4 + ";Trusted_Connection=false");
                        }
                    
                }
                catch
                {

                }

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountsAssign>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__accounts__32DD162D8CD10F79");

                entity.ToTable("accountsAssign");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.Account).HasColumnName("account");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Module)
                    .HasColumnName("module")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transcode)
                    .HasColumnName("transcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AdmTaxes>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__admTaxes__D825195EE53A652F");

                entity.ToTable("admTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.TaxCode)
                    .HasColumnName("taxCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxPer).HasColumnName("taxPer");
            });

            modelBuilder.Entity<AdmUserwiseAssigns>(entity =>
            {
                entity.HasKey(e => e.sno)
                    .HasName("PK__admUserw__DDDF64468F69A69A");

                entity.ToTable("admUserwiseAssigns");

                entity.Property(e => e.sno).HasColumnName("sno");

                entity.Property(e => e.AssignedId)
                    .HasColumnName("assignedId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AssignedTyp)
                    .HasColumnName("assignedTyp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Admroles>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__admroles__DDDF64464905A6BD");

                entity.ToTable("admroles");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.MenuCode).HasColumnName("menuCode");

                entity.Property(e => e.ModuleCode).HasColumnName("moduleCode");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.RoleName)
                    .HasColumnName("roleName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScreenCode).HasColumnName("screenCode");

                entity.Property(e => e.TransCode).HasColumnName("transCode");
            });

            modelBuilder.Entity<CrmDiscountListDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("crmDiscountListDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.ProductId).HasColumnName("productId");

               

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmDiscountListDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmDiscou__recor__41D8BC2C");
            });

            modelBuilder.Entity<CrmDiscountListUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmDisco__D825195E3F8A4C2C");

                entity.ToTable("crmDiscountListUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.DiscountListName)
                    .HasColumnName("discountListName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effectiveDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Pos).HasColumnName("pos");
            });

            modelBuilder.Entity<CrmPriceListDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("crmPriceListDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.TaxId).HasColumnName("taxId");

               
                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmPriceListDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmPriceL__recor__47919582");
            });

            modelBuilder.Entity<CrmPriceListUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmPrice__D825195E088B777C");

                entity.ToTable("crmPriceListUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Currency).HasColumnName("currency");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnName("effectiveDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.MrpCheck).HasColumnName("mrpCheck");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PriceListName)
                    .HasColumnName("priceListName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });


            modelBuilder.Entity<CrmOrdersRxdet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("crmOrdersRXDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Additional)
                    .HasColumnName("additional")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Coating)
                    .HasColumnName("coating")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Cylindrical)
                    .HasColumnName("cylindrical")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.DespatchedQty).HasColumnName("despatchedQty");

                entity.Property(e => e.Dia)
                    .HasColumnName("dia")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FinalQc).HasColumnName("FinalQC");

                entity.Property(e => e.Hcqty).HasColumnName("HCQty");

                entity.Property(e => e.Hmcqty).HasColumnName("HMCQty");
                entity.Property(e => e.blanksInvQty).HasColumnName("blanksInvQty");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Product)
                    .HasColumnName("product")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.blankBase)
                   .HasColumnName("blankBase")
                   .HasMaxLength(50)
                   .IsUnicode(false);



                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Rxqc).HasColumnName("RXQC");

                entity.Property(e => e.Rxqty).HasColumnName("RXQty");

                entity.Property(e => e.Shade)
                    .HasColumnName("shade")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Side)
                    .HasColumnName("side")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Spherical)
                    .HasColumnName("spherical")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Typ).HasColumnName("typ");
            });

            modelBuilder.Entity<CrmTeleCallingRx>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmTeleC__D825195E8B72690A");

                entity.ToTable("crmTeleCallingRX");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CallPosition).HasColumnName("callPosition");

                entity.Property(e => e.CallerComments)
                    .HasColumnName("callerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Callerid).HasColumnName("callerid");

                entity.Property(e => e.Customer)
                    .HasColumnName("customer")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.CustomerComments)
                    .HasColumnName("customerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NextCallDate)
                    .HasColumnName("nextCallDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.NextCallMode)
                    .HasColumnName("nextCallMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrevCallMode)
                    .HasColumnName("prevCallMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrevcallId).HasColumnName("prevcallId");
                entity.Property(e => e.nextCallId).HasColumnName("nextCallId");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SaleRxPriceList>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("saleRxPriceList");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Coat)
                    .HasColumnName("coat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.PriceListName).HasColumnName("priceListName");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Product)
                    .HasColumnName("product")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxtype)
                    .HasColumnName("taxtype")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });


            modelBuilder.Entity<SaleRxDiscountList>(entity =>
            {
                entity.HasKey(e => e.Sno);

                entity.ToTable("saleRxDiscountList");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Coat)
                    .HasColumnName("coat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.PriceListName)
                    .HasColumnName("priceListName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Product)
                    .HasColumnName("product")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrmOrdersRxuni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmOrder__D825195EA4BA5208");

                entity.ToTable("crmOrdersRXUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Approveddat)
                    .HasColumnName("approveddat")
                    .HasColumnType("datetime");

                entity.Property(e => e.BaseAmt).HasColumnName("baseAmt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Customer)
                    .HasColumnName("customer")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Roundoff).HasColumnName("roundoff");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Typ).HasColumnName("typ");

                entity.Property(e => e.TypeofSale).HasColumnName("typeofSale");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ValidityDate)
                    .HasColumnName("validityDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });



            modelBuilder.Entity<CrmSetup>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__crmSetup__DDDF6446C6391E90");

                entity.ToTable("crmSetup");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Pos)
                    .HasColumnName("pos")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PosDescription)
                    .HasColumnName("posDescription")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SetupCode)
                    .HasColumnName("setupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SetupDescription)
                    .HasColumnName("setupDescription")
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrmTargetSettings>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__crmTarge__32DD162D552EB797");

                entity.ToTable("crmTargetSettings");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Calls).HasColumnName("calls");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Empno).HasColumnName("empno");

                entity.Property(e => e.Mont).HasColumnName("mont");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.Tgt).HasColumnName("tgt");

                entity.Property(e => e.Yea).HasColumnName("yea");

              
            });
            modelBuilder.Entity<CrmEnquiriesRx>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmEnqui__D825195EDAE4C45E");

                entity.ToTable("crmEnquiriesRX");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CallPosition).HasColumnName("callPosition");

                entity.Property(e => e.CallerComments)
                    .HasColumnName("callerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Callerid).HasColumnName("callerid");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Customer)
                    .HasColumnName("customer")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.CustomerComments)
                    .HasColumnName("customerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MainCustomerId).HasColumnName("mainCustomerId");
                entity.Property(e => e.nextCallId).HasColumnName("nextCallId");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NextCallDate)
                    .HasColumnName("nextCallDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.NextCallMode).HasColumnName("nextCallMode");

                entity.Property(e => e.PrevCallMode)
                    .HasColumnName("prevCallMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrevcallId).HasColumnName("prevcallId");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasColumnName("type");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(50)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<CrmTaxAssigningDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("crmTaxAssigningDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.TaxCode)
                    .HasColumnName("taxCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxOn)
                    .HasColumnName("taxOn")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmTaxAssigningDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmTaxAss__recor__53F76C67");
            });

            modelBuilder.Entity<CrmTaxAssigningUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmTaxAs__D825195E3FE2BE61");

                entity.ToTable("crmTaxAssigningUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.TaxName)
                    .HasColumnName("taxName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrmTeleCallDetails>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmTeleC__D825195E0FBD563D");

                entity.ToTable("crmTeleCallDetails");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CallMode)
                    .HasColumnName("callMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Company)
                    .HasColumnName("company")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasColumnName("contactPerson")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.CustomerComments)
                    .HasColumnName("customerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FollowUpMode)
                    .HasColumnName("followUpMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FollowUpdat)
                    .HasColumnName("followUpdat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Impression)
                    .HasColumnName("impression")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PersonalComment)
                    .HasColumnName("personalComment")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PreceedCallId)
                    .HasColumnName("preceedCallId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsrName)
                    .HasColumnName("usrName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrmQuotationDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmQuotationDet");

                entity.ToTable("crmQuotationDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.DiscPer).HasColumnName("discPer");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LeadDays).HasColumnName("leadDays");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmQuotationDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmQuotat__recor__5772F790");
            });

            modelBuilder.Entity<CrmQuotationTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmQuotationTaxes");

                entity.ToTable("crmQuotationTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");
                entity.Property(e => e.LineId).HasColumnName("lineId");
                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.TaxCode)
                    .HasColumnName("taxCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxPer).HasColumnName("taxPer");

                entity.Property(e => e.TaxValue).HasColumnName("taxValue");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmQuotationTaxes)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmQuotat__recor__5B438874");
            });

            modelBuilder.Entity<CrmQuotationTerms>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmQuotationTerms");

                entity.ToTable("crmQuotationTerms");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Term)
                    .HasColumnName("term")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmQuotationTerms)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmQuotat__recor__5E1FF51F");
            });

            modelBuilder.Entity<CrmQuotationUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmQuota__D825195E11A2FF99");

                entity.ToTable("crmQuotationUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.RefEmployee).HasColumnName("refEmployee");

                entity.Property(e => e.RefEnquiryId).HasColumnName("refEnquiryId");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SaleType)
                    .HasColumnName("saleType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrmSaleOrderDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmSaleOrderDet");

                entity.ToTable("crmSaleOrderDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.DiscPer).HasColumnName("discPer");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.ReqdBy)
                    .HasColumnName("reqdBy")
                    .HasColumnType("datetime");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmSaleOrderDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmSaleOr__recor__63D8CE75");
            });

            modelBuilder.Entity<CrmSaleOrderTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmSaleOrderTaxes");

                entity.ToTable("crmSaleOrderTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");
                entity.Property(e => e.LineId).HasColumnName("lineId");
                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Taxcode)
                    .HasColumnName("taxcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmSaleOrderTaxes)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmSaleOr__recor__67A95F59");
            });

            modelBuilder.Entity<CrmSaleOrderTerms>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmSaleOrderTerms");

                entity.ToTable("crmSaleOrderTerms");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Term)
                    .HasColumnName("term")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmSaleOrderTerms)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmSaleOr__recor__6A85CC04");
            });

            modelBuilder.Entity<CrmSaleOrderUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmSaleO__D825195EA496772D");

                entity.ToTable("crmSaleOrderUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConversionFactor).HasColumnName("conversionFactor");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MailCount).HasColumnName("mailCount");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.RefQuotationId).HasColumnName("refQuotationId");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SaleType)
                    .HasColumnName("saleType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.TypeOfOrder)
                    .HasColumnName("typeOfOrder")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<CrmBillSubmissionsDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmBillSubmissionsDet");

                entity.ToTable("crmBillSubmissionsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Billno).HasColumnName("billno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmBillSubmissionsDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmBillSu__recor__442B18F2");
            });

            modelBuilder.Entity<CrmBillSubmissionsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmBillS__D825195E4A290B5E");

                entity.ToTable("crmBillSubmissionsUni");

                entity.Property(e => e.RecordId)
                    .HasColumnName("recordId")
                    .ValueGeneratedNever();

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<CrmPostTeleCalling>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmPostT__D825195EE5A405C2");

                entity.ToTable("crmPostTeleCalling");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CallType).HasColumnName("callType");

                entity.Property(e => e.CallerComments)
                    .HasColumnName("callerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Callerid).HasColumnName("callerid");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.CustomerComments)
                    .HasColumnName("customerComments")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.CustomerName)
                    .HasColumnName("customerName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.NextCallDate)
                    .HasColumnName("nextCallDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.NextCallId).HasColumnName("nextCallId");

                entity.Property(e => e.NextCallMode).HasColumnName("nextCallMode");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.NextCall)
                    .WithMany(p => p.InverseNextCall)
                    .HasForeignKey(d => d.NextCallId)
                    .HasConstraintName("FK__crmPostTe__nextC__48EFCE0F");
            });


            modelBuilder.Entity<FinAccGroups>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__finAccGr__D825195E23F7114B");

                entity.ToTable("finAccGroups");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GroupCode)
                    .HasColumnName("groupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GrpTag)
                    .HasColumnName("grpTag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MGrp)
                    .HasColumnName("mGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SGrp)
                    .HasColumnName("sGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<FinAccounts>(entity =>
            {
                entity.HasKey(e => e.Recordid)
                    .HasName("PK__finAccou__D82414B6BA1FA70E");

                entity.ToTable("finAccounts");

                entity.Property(e => e.Recordid).HasColumnName("recordid");

                entity.Property(e => e.AcChk).HasColumnName("acChk");

                entity.Property(e => e.AcType)
                    .HasColumnName("acType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Accgroup).HasColumnName("accgroup");

                entity.Property(e => e.Accname)
                    .HasColumnName("accname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pin)
                    .HasColumnName("pin")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WebId)
                    .HasColumnName("webId")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FinAccountsAssign>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__finAccou__DDDF6446FD9EC916");

                entity.ToTable("finAccountsAssign");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Accname).HasColumnName("accname");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Customercode).HasColumnName("customercode");

                entity.Property(e => e.TraCode)
                    .HasColumnName("traCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Trans)
                    .HasColumnName("trans")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.AccnameNavigation)
                    .WithMany(p => p.FinAccountsAssign)
                    .HasForeignKey(d => d.Accname)
                    .HasConstraintName("FK__finAccoun__accna__4C8B54C9");
            });

            modelBuilder.Entity<FinBankCheckings>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_finBankCheckings");

                entity.ToTable("finBankCheckings");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Amt).HasColumnName("amt");

                entity.Property(e => e.Bank).HasColumnName("bank");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ClearedDat)
                    .HasColumnName("clearedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Clearedby)
                    .HasColumnName("clearedby")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Details)
                    .HasColumnName("details")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Typ).HasColumnName("typ");

                entity.Property(e => e.Usrname)
                    .HasColumnName("usrname")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Finassets>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__finasset__D825195E2B202C61");

                entity.ToTable("finassets");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.AssetName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .IsRequired()
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Depreciation).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.OpeningValue).HasColumnType("numeric(18, 4)");
            });

            modelBuilder.Entity<FinassetsView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("finassets_view");

                entity.Property(e => e.AssetName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Branchid)
                    .IsRequired()
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Depreciation).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Opedate)
                    .HasColumnName("opedate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Opvalue)
                    .HasColumnName("opvalue")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Presetnvalue)
                    .HasColumnName("presetnvalue")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId)
                    .HasColumnName("recordId")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<FinexecDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_finexecDet");

                entity.ToTable("finexecDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Accname).HasColumnName("accname");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cre).HasColumnName("cre");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deb).HasColumnName("deb");

                entity.HasOne(d => d.AccnameNavigation)
                    .WithMany(p => p.FinexecDet)
                    .HasForeignKey(d => d.Accname)
                    .HasConstraintName("FK__finexecDe__accna__5614BF03");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.FinexecDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__finexecDe__recor__55209ACA");
            });

            modelBuilder.Entity<FinexecDetHistory>(entity =>
            {
                entity.HasKey(e => new { e.AuditId, e.Sno })
                    .HasName("pk_finexecDet_history");

                entity.ToTable("finexecDet_history");

                entity.Property(e => e.AuditId).HasColumnName("auditId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Accname).HasColumnName("accname");

                entity.Property(e => e.AuditDate)
                    .HasColumnName("auditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cre).HasColumnName("cre");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deb).HasColumnName("deb");

                entity.Property(e => e.RecordId).HasColumnName("recordId");
            });

            modelBuilder.Entity<FinexecUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__finexecU__D825195E4D5F28F3");

                entity.ToTable("finexecUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BankDet)
                    .HasColumnName("bankDet")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.detail1)
                    .HasColumnName("detail1")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.detail2)
                    .HasColumnName("detail2")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.detail3)
                    .HasColumnName("detail3")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.detail4)
                    .HasColumnName("detail4")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Narr)
                    .HasColumnName("narr")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.Seq).HasColumnName("seq");

                entity.Property(e => e.Traref)
                    .HasColumnName("traref")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tratype)
                    .HasColumnName("tratype")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vouchertype)
                    .HasColumnName("vouchertype")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FinexecUniHistory>(entity =>
            {
                entity.HasKey(e => e.AuditId)
                    .HasName("PK__finexecU__43D173996DFD566B");

                entity.ToTable("finexecUni_history");

                entity.Property(e => e.AuditId).HasColumnName("auditId");

                entity.Property(e => e.AuditDate)
                    .HasColumnName("auditDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.BankDet)
                    .HasColumnName("bankDet")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Narr)
                    .HasColumnName("narr")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Seq).HasColumnName("seq");

                entity.Property(e => e.Traref)
                    .HasColumnName("traref")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tratype)
                    .HasColumnName("tratype")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vouchertype)
                    .HasColumnName("vouchertype")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.FinexecUniHistory)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__finexecUn__recor__58F12BAE");
            });

            modelBuilder.Entity<HrdAllowanceDeductionRanges>(entity =>
            {
                entity.HasKey(e => e.Lineid)
                    .HasName("PK__hrdAllow__3249818DD5E6142D");

                entity.ToTable("hrdAllowanceDeductionRanges");

                entity.Property(e => e.Lineid).HasColumnName("lineid");

                entity.Property(e => e.AllowanceId).HasColumnName("allowanceId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.FromValue).HasColumnName("fromValue");

                entity.Property(e => e.ToValue).HasColumnName("toValue");

                entity.Property(e => e.Valu).HasColumnName("valu");

                entity.HasOne(d => d.Allowance)
                    .WithMany(p => p.HrdAllowanceDeductionRanges)
                    .HasForeignKey(d => d.AllowanceId)
                    .HasConstraintName("FK__hrdAllowa__allow__636EBA21");
            });

            modelBuilder.Entity<HrdAllowancesDeductions>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdAllow__D825195EE3156D97");

                entity.ToTable("hrdAllowancesDeductions");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Allowance)
                    .HasColumnName("allowance")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AllowanceCheck).HasColumnName("allowanceCheck");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CalcType).HasColumnName("calcType");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.EffectAs).HasColumnName("effectAs");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<HrdAllowancesDeductionsEffects>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_hrdAllowancesDeductionsEffects");

                entity.ToTable("hrdAllowancesDeductionsEffects");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.AllowanceOn).HasColumnName("allowanceOn");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");
            });

            modelBuilder.Entity<HrdDepartments>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdDepar__D825195E14867A7C");

                entity.ToTable("hrdDepartments");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GroupCode)
                    .HasColumnName("groupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GrpTag)
                    .HasColumnName("grpTag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MGrp)
                    .HasColumnName("mGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SGrp)
                    .HasColumnName("sGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<HrdDesignations>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdDesig__D825195EC3CEBB54");

                entity.ToTable("hrdDesignations");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Department).HasColumnName("department");

                entity.Property(e => e.Designation)
                    .HasColumnName("designation")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.HrdDesignations)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK__hrdDesign__depar__68336F3E");
            });

            modelBuilder.Entity<HrdDesignationsAllowances>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdDesig__32489DA54FAD9AA1");

                entity.ToTable("hrdDesignationsAllowances");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.Allowance).HasColumnName("allowance");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Valu).HasColumnName("valu");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdDesignationsAllowances)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdDesign__recor__6CF8245B");
            });

            modelBuilder.Entity<HrdDesignationsLeaves>(entity =>
            {
                entity.HasKey(e => e.Lineid)
                    .HasName("PK__hrdDesig__3249818DB437CB96");

                entity.ToTable("hrdDesignationsLeaves");

                entity.Property(e => e.Lineid).HasColumnName("lineid");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.LeaveId).HasColumnName("leaveId");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Valu).HasColumnName("valu");

                entity.HasOne(d => d.Leave)
                    .WithMany(p => p.HrdDesignationsLeaves)
                    .HasForeignKey(d => d.LeaveId)
                    .HasConstraintName("FK__hrdDesign__leave__70C8B53F");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdDesignationsLeaves)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdDesign__recor__6FD49106");
            });

            modelBuilder.Entity<HrdResumeCurriculum>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_hrdResumeCurriculum");

                entity.ToTable("hrdResumeCurriculum");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Board)
                    .HasColumnName("board")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.FromYear).HasColumnName("fromYear");

                entity.Property(e => e.Qualification)
                    .HasColumnName("qualification")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToYead).HasColumnName("toYead");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdResumeCurriculum)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__hrdResume__recor__370627FE");
            });

            modelBuilder.Entity<HrdResumeExperience>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_hrdResumeExperience");

                entity.ToTable("hrdResumeExperience");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Designation)
                    .HasColumnName("designation")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FromYear).HasColumnName("fromYear");

                entity.Property(e => e.Organisation)
                    .HasColumnName("organisation")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToYead).HasColumnName("toYead");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdResumeExperience)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__hrdResume__recor__39E294A9");
            });

            modelBuilder.Entity<HrdResumeUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdResum__D825195E4541CD11");

                entity.ToTable("hrdResumeUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.AppDate)
                    .HasColumnName("appDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Designation).HasColumnName("designation");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dob)
                    .HasColumnName("dob")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExpectedSalary).HasColumnName("expectedSalary");

                entity.Property(e => e.FatherName)
                    .HasColumnName("fatherName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.MaritalStatus).HasColumnName("maritalStatus");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NameOfCandidate)
                    .HasColumnName("nameOfCandidate")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PermenentId)
                    .HasColumnName("permenentId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SurName)
                    .HasColumnName("surName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HrdInterviewCandidates>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_hrdInterviewCandidates");

                entity.ToTable("hrdInterviewCandidates");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.AppointmentDate)
                    .HasColumnName("appointmentDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.AppointmentStatus).HasColumnName("appointmentStatus");

                entity.Property(e => e.InterviewerComments)
                   .HasColumnName("interviewerComments")
                   .HasMaxLength(250)
                   .IsUnicode(false);
                entity.Property(e => e.AppointmentComments)
                  .HasColumnName("appointmentComments")
                  .HasMaxLength(250)
                  .IsUnicode(false);
                entity.Property(e => e.JoiningComments)
                 .HasColumnName("joiningComments")
                 .HasMaxLength(250)
                 .IsUnicode(false);
               

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.JoiningDate)
                    .HasColumnName("joiningDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.JoiningStatus).HasColumnName("joiningStatus");

                entity.Property(e => e.MaxDateToJoin)
                    .HasColumnName("maxDateToJoin")
                    .HasColumnType("datetime");

                entity.Property(e => e.ResumeId).HasColumnName("resumeId");
                entity.Property(e => e.RefEmpNo).HasColumnName("refEmpNo");
                

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdInterviewCandidates)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__hrdInterv__recor__4DE98D56");
            });

            modelBuilder.Entity<HrdInterviewsPanel>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_hrdInterviewsPanel");

                entity.ToTable("hrdInterviewsPanel");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Empno).HasColumnName("empno");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdInterviewsPanel)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__hrdInterv__recor__4A18FC72");
            });

            modelBuilder.Entity<HrdInterviewsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdInter__D825195EF863442E");

                entity.ToTable("hrdInterviewsUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
               .HasColumnName("descriptio")
               .HasMaxLength(500)
               .IsUnicode(false);


                entity.Property(e => e.Designation).HasColumnName("designation");

                entity.Property(e => e.InterviewDate)
                    .HasColumnName("interviewDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Venue)
                    .HasColumnName("venue")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });


            modelBuilder.Entity<HrdEmpInOutDetails>(entity =>
            {
                entity.HasKey(e => e.Lineid)
                    .HasName("PK__hrdEmpIn__3249818DD2EF3506");

                entity.ToTable("hrdEmpInOutDetails");

                entity.Property(e => e.Lineid).HasColumnName("lineid");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmpId).HasColumnName("empId");

                entity.Property(e => e.FromTime)
                    .HasColumnName("fromTime")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.ToTime)
                    .HasColumnName("toTime")
                    .HasMaxLength(50)
                    .IsUnicode(false);

              
            });

            modelBuilder.Entity<HrdEmployeeAllowancesDeductions>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdEmplo__32489DA503EFA302");

                entity.ToTable("hrdEmployeeAllowancesDeductions");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.Allowance).HasColumnName("allowance");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Valu).HasColumnName("valu");

                entity.HasOne(d => d.AllowanceNavigation)
                    .WithMany(p => p.HrdEmployeeAllowancesDeductions)
                    .HasForeignKey(d => d.Allowance)
                    .HasConstraintName("FK__hrdEmploy__allow__795DFB40");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdEmployeeAllowancesDeductions)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdEmploy__recor__7869D707");
            });

            modelBuilder.Entity<HrdEmployeeCurriculum>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdEmplo__32489DA51D6E5995");

                entity.ToTable("hrdEmployeeCurriculum");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.Board)
                    .HasColumnName("board")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Frmyear).HasColumnName("frmyear");

                entity.Property(e => e.Qual)
                    .HasColumnName("qual")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Toyear).HasColumnName("toyear");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdEmployeeCurriculum)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdEmploy__recor__7C3A67EB");
            });

            modelBuilder.Entity<HrdEmployeeExperience>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdEmplo__32489DA534CBFD30");

                entity.ToTable("hrdEmployeeExperience");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Designation)
                    .HasColumnName("designation")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Frmyear).HasColumnName("frmyear");

                entity.Property(e => e.Organisation)
                    .HasColumnName("organisation")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Toyear).HasColumnName("toyear");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdEmployeeExperience)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdEmploy__recor__7F16D496");
            });

            modelBuilder.Entity<HrdEmployeeFamilyDetails>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdEmplo__32489DA5A3648E1D");

                entity.ToTable("hrdEmployeeFamilyDetails");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Relation)
                    .HasColumnName("relation")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Relativename)
                    .HasColumnName("relativename")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdEmployeeFamilyDetails)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdEmploy__recor__01F34141");
            });

            modelBuilder.Entity<HrdEmployeeIdentifications>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdEmplo__32489DA5EC46272C");

                entity.ToTable("hrdEmployeeIdentifications");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Identit)
                    .HasColumnName("identit")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdEmployeeIdentifications)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdEmploy__recor__04CFADEC");
            });

            modelBuilder.Entity<HrdEmployeeLeaves>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdEmplo__32489DA59EF2A870");

                entity.ToTable("hrdEmployeeLeaves");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.LeaveId).HasColumnName("leaveId");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Valu).HasColumnName("valu");

                entity.HasOne(d => d.Leave)
                    .WithMany(p => p.HrdEmployeeLeaves)
                    .HasForeignKey(d => d.LeaveId)
                    .HasConstraintName("FK__hrdEmploy__leave__08A03ED0");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.HrdEmployeeLeaves)
                    .HasForeignKey(d => d.RecordId)
                    .HasConstraintName("FK__hrdEmploy__recor__07AC1A97");
            });

            modelBuilder.Entity<HrdEmployees>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdEmplo__D825195EFBB0DE26");

                entity.ToTable("hrdEmployees");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Aadhar)
                    .HasColumnName("aadhar")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BankBranch)
                    .HasColumnName("bankBranch")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BankName)
                    .HasColumnName("bankName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BankPay).HasColumnName("bankPay");

                entity.Property(e => e.Bankifscno)
                    .HasColumnName("bankifscno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BasicChk).HasColumnName("basicChk");

                entity.Property(e => e.BasicPay).HasColumnName("basicPay");

                entity.Property(e => e.BloodGrp)
                    .HasColumnName("bloodGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Department).HasColumnName("department");

                entity.Property(e => e.Designation).HasColumnName("designation");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dob)
                    .HasColumnName("dob")
                    .HasColumnType("datetime");

                entity.Property(e => e.Doj)
                    .HasColumnName("doj")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Empname)
                    .HasColumnName("empname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Empno)
                    .HasColumnName("empno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fathername)
                    .HasColumnName("fathername")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.GrandPay).HasColumnName("grandPay");

                entity.Property(e => e.Height)
                    .HasColumnName("height")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idno)
                    .HasColumnName("idno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Idtype)
                    .HasColumnName("idtype")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LeavesScheme)
                    .HasColumnName("leavesScheme")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaritalStatus).HasColumnName("maritalStatus");

                entity.Property(e => e.Mgr).HasColumnName("MGR");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModeofPay).HasColumnName("modeofPay");

                entity.Property(e => e.Paddr)
                    .HasColumnName("paddr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Pan)
                    .HasColumnName("pan")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pcity)
                    .HasColumnName("pcity")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pcountry)
                    .HasColumnName("pcountry")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pdist)
                    .HasColumnName("pdist")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pemail)
                    .HasColumnName("pemail")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pfax)
                    .HasColumnName("pfax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pic)
                    .HasColumnName("pic")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pmobile)
                    .HasColumnName("pmobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pstat)
                    .HasColumnName("pstat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ptel)
                    .HasColumnName("ptel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pwebid)
                    .HasColumnName("pwebid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pzip)
                    .HasColumnName("pzip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Referenc)
                    .HasColumnName("referenc")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SbAc)
                    .HasColumnName("sbAC")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Esi)
                    .HasColumnName("esi")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pf)
                    .HasColumnName("pf")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.HrdEmployees)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK__hrdEmploy__depar__73A521EA");

                entity.HasOne(d => d.DesignationNavigation)
                    .WithMany(p => p.HrdEmployees)
                    .HasForeignKey(d => d.Designation)
                    .HasConstraintName("FK__hrdEmploy__desig__74994623");

                entity.HasOne(d => d.MgrNavigation)
                    .WithMany(p => p.InverseMgrNavigation)
                    .HasForeignKey(d => d.Mgr)
                    .HasConstraintName("FK__hrdEmployee__MGR__758D6A5C");

                
            });

            modelBuilder.Entity<HrdLeaves>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdLeave__D825195E6CE1D011");

                entity.ToTable("hrdLeaves");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Customercode).HasColumnName("customercode");

                entity.Property(e => e.ForwardType).HasColumnName("forwardType");

                entity.Property(e => e.LeaveCode)
                    .HasColumnName("leaveCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LeaveDescription)
                    .HasColumnName("leaveDescription")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PayType).HasColumnName("payType");
            });

            modelBuilder.Entity<HrdShifts>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdShift__D825195E6E99AF05");

                entity.ToTable("hrdShifts");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.FromTime)
                    .HasColumnName("fromTime")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShiftName)
                    .HasColumnName("shiftName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToTime)
                    .HasColumnName("toTime")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HrdAdvances>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdAdvan__D825195E2726E37D");

                entity.ToTable("hrdAdvances");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.AdvanceCredit).HasColumnName("advanceCredit");

                entity.Property(e => e.AdvanceCutOff).HasColumnName("advanceCutOff");

                entity.Property(e => e.AdvanceDebit).HasColumnName("advanceDebit");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Empno).HasColumnName("empno");
            });

            modelBuilder.Entity<HrdAttendances>(entity =>
            {
                entity.HasKey(e => e.LineId)
                    .HasName("PK__hrdAtten__32489DA5303EACB7");

                entity.ToTable("hrdAttendances");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.Attendance)
                    .HasColumnName("attendance")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Empno).HasColumnName("empno");

                entity.Property(e => e.Late)
                    .HasColumnName("late")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ot)
                    .HasColumnName("ot")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HrdLeaveApplications>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__hrdLeave__D825195E2601D55B");

                entity.ToTable("hrdLeaveApplications");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.ApprovalStatus).HasColumnName("approvalStatus");

                entity.Property(e => e.ApprovedBy)
                    .HasColumnName("approvedBy")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Empno).HasColumnName("empno");

                entity.Property(e => e.LeaveFrom)
                    .HasColumnName("leaveFrom")
                    .HasColumnType("datetime");

                entity.Property(e => e.LeaveTo)
                    .HasColumnName("leaveTo")
                    .HasColumnType("datetime");
            });


            modelBuilder.Entity<InvDepartments>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invDepar__D825195E4A9D5905");

                entity.ToTable("invDepartments");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Area)
                    .HasColumnName("area")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InvGroups>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invGroup__D825195E05FBCAD4");

                entity.ToTable("invGroups");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GroupCode)
                    .HasColumnName("groupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GrpTag)
                    .HasColumnName("grpTag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MGrp)
                    .HasColumnName("mGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pic)
                    .HasColumnName("pic")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SGrp)
                    .HasColumnName("sGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<InvLosses>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invLosse__D825195EFEA5E1E7");

                entity.ToTable("invLosses");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Allowableper).HasColumnName("allowableper");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.LossName)
                    .HasColumnName("lossName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InvMaterialCompleteDetailsView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("invMaterialCompleteDetails_view");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Customercode).HasColumnName("customercode");

                entity.Property(e => e.Grpid).HasColumnName("grpid");

                entity.Property(e => e.Grpname)
                    .HasColumnName("grpname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Itemid)
                    .HasColumnName("itemid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Itemname)
                    .HasColumnName("itemname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Um)
                    .HasColumnName("um")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Umid).HasColumnName("umid");
            });

            modelBuilder.Entity<InvMaterialManagement>(entity =>
            {
                entity.HasKey(e => new { e.TransactionId, e.TransactionType, e.Sno });

                entity.ToTable("invMaterialManagement");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.TransactionType).HasColumnName("transactionType");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BatchNo)
                    .HasColumnName("batchNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Department).HasColumnName("department");

                entity.Property(e => e.Descr)
                    .HasColumnName("descr")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Expdate)
                    .HasColumnName("expdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Gin)
                    .HasColumnName("gin")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ItemName).HasColumnName("itemName");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.Manudate)
                    .HasColumnName("manudate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductBatchNo).HasColumnName("productBatchNo");

                entity.Property(e => e.Qtyin).HasColumnName("qtyin");

                entity.Property(e => e.Qtyout).HasColumnName("qtyout");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Stdum).HasColumnName("stdum");

                entity.Property(e => e.Store).HasColumnName("store");
                 
            });

            modelBuilder.Entity<InvMaterialUnits>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("invMaterialUnits");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConversionFactor).HasColumnName("conversionFactor");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.StdUm).HasColumnName("stdUm");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.InvMaterialUnits)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__invMateri__recor__3C1FE2D6");

                entity.HasOne(d => d.UmNavigation)
                    .WithMany(p => p.InvMaterialUnits)
                    .HasForeignKey(d => d.Um)
                    .HasConstraintName("FK__invMaterialU__um__3D14070F");
            });

            modelBuilder.Entity<InvMaterials>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invMater__D825195EF9A79FAB");

                entity.ToTable("invMaterials");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CostingType).HasColumnName("costingType");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Grp).HasColumnName("grp");

                entity.Property(e => e.InventoryReqd).HasColumnName("inventoryReqd");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Itemid)
                    .HasColumnName("itemid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pic)
                    .HasColumnName("pic")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReOrderQty).HasColumnName("reOrderQty");

                entity.Property(e => e.ShelfLifeReqd).HasColumnName("shelfLifeReqd");

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.StdRate).HasColumnName("stdRate");

                entity.Property(e => e.VendorId).HasColumnName("vendorId");

                entity.HasOne(d => d.GrpNavigation)
                    .WithMany(p => p.InvMaterials)
                    .HasForeignKey(d => d.Grp)
                    .HasConstraintName("FK__invMaterial__grp__3943762B");
            });

            modelBuilder.Entity<InvSetup>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__invSetup__DDDF6446E76F42D9");

                entity.ToTable("invSetup");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.SetupCode)
                    .HasColumnName("setupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SetupDescription)
                    .HasColumnName("setupDescription")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.SetupValue)
                    .HasColumnName("setupValue")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InvStores>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invStore__D825197EF72A75E6");

                entity.ToTable("invStores");

                entity.Property(e => e.RecordId).HasColumnName("recordID");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.StoreCode)
                    .HasColumnName("storeCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StoreIncharge)
                    .HasColumnName("storeIncharge")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StoreName)
                    .HasColumnName("storeName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InvTransactionsDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_invTransactionsDet");

                entity.ToTable("invTransactionsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Conversion).HasColumnName("conversion");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Qtyin).HasColumnName("qtyin");

                entity.Property(e => e.Qtyout).HasColumnName("qtyout");

                entity.Property(e => e.Um).HasColumnName("um");

             

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.InvTransactionsDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__invTransa__recor__6339AFF7");
            });

            modelBuilder.Entity<InvTransactionsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invTrans__D825195E96B748D9");

                entity.ToTable("invTransactionsUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TraType)
                    .HasColumnName("traType")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<InvUm>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__invUM__D825195ED35EE5E7");

                entity.ToTable("invUM");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Um)
                    .HasColumnName("um")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MaiEquipGroups>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__maiEquip__D825195ECEEC66A0");

                entity.ToTable("maiEquipGroups");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GroupCode)
                    .HasColumnName("groupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GrpTag)
                    .HasColumnName("grpTag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MGrp).HasColumnName("mGrp");

                entity.Property(e => e.SGrp)
                    .HasColumnName("sGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<MaiEquipment>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__maiEquip__D825195ED39013EE");

                entity.ToTable("maiEquipment");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.AmcDate)
                    .HasColumnName("amcDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.EquipmentCode)
                    .HasColumnName("equipmentCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentGroup).HasColumnName("equipmentGroup");

                entity.Property(e => e.EquipmentName)
                    .HasColumnName("equipmentName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastPmdate)
                    .HasColumnName("lastPMDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.MaxHrs).HasColumnName("maxHrs");

                entity.Property(e => e.MobileCheck).HasColumnName("mobileCheck");

                entity.Property(e => e.PreferableServiceSupplier).HasColumnName("preferableServiceSupplier");

                entity.Property(e => e.PreferableSparesSupplier).HasColumnName("preferableSparesSupplier");

                entity.Property(e => e.Roomno).HasColumnName("roomno");

                entity.HasOne(d => d.EquipmentGroupNavigation)
                    .WithMany(p => p.MaiEquipment)
                    .HasForeignKey(d => d.EquipmentGroup)
                    .HasConstraintName("FK__maiEquipm__equip__6ADAD1BF");
            });

            modelBuilder.Entity<MaiEquipmentInsurances>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__maiEquip__D825195EC4CE55D0");

                entity.ToTable("maiEquipmentInsurances");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Equipid).HasColumnName("equipid");

                entity.Property(e => e.InsureAmt).HasColumnName("insureAmt");

                entity.Property(e => e.InsureFrom)
                    .HasColumnName("insureFrom")
                    .HasColumnType("datetime");

                entity.Property(e => e.InsureTo)
                    .HasColumnName("insureTo")
                    .HasColumnType("datetime");

                entity.Property(e => e.VendorId).HasColumnName("vendorId");

                entity.Property(e => e.VendorName)
                    .HasColumnName("vendorName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Equip)
                    .WithMany(p => p.MaiEquipmentInsurances)
                    .HasForeignKey(d => d.Equipid)
                    .HasConstraintName("FK__maiEquipm__equip__6DB73E6A");

                
            });

            modelBuilder.Entity<MaiEquipmentPmdetails>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__maiEquip__D825195E2BF9555B");

                entity.ToTable("maiEquipmentPMDetails");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Equipid).HasColumnName("equipid");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.HasOne(d => d.Equip)
                    .WithMany(p => p.MaiEquipmentPmdetails)
                    .HasForeignKey(d => d.Equipid)
                    .HasConstraintName("FK__maiEquipm__equip__75586032");
            });

            modelBuilder.Entity<MaiEquipmentPreventiveMaintenances>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_maiEquipmentPreventiveMaintenances");

                entity.ToTable("maiEquipmentPreventiveMaintenances");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.FrequencyInDays).HasColumnName("frequencyInDays");

                entity.Property(e => e.Prevmaintenance)
                    .HasColumnName("prevmaintenance")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MaiEquipmentSpecifications>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_mainEquipmentSpecifications");

                entity.ToTable("maiEquipmentSpecifications");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Specification)
                    .HasColumnName("specification")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Valu)
                    .HasColumnName("valu")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MakeSessions>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__makeSess__DDDF6446AC41335D");

                entity.ToTable("makeSessions");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.KCode)
                    .HasColumnName("kCode")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.LogDate)
                    .HasColumnName("logDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.SessionClose)
                    .HasColumnName("sessionClose")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MisCountryMaster>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__misCount__D825195ECB55A601");

                entity.ToTable("misCountryMaster");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Cntname)
                    .HasColumnName("cntname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Coins)
                    .HasColumnName("coins")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConversionFactor).HasColumnName("conversionFactor");

                entity.Property(e => e.Curr)
                    .HasColumnName("curr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrSymbol)
                    .HasColumnName("currSymbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<MisStateMaster>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__misState__D825195E64949A32");

                entity.ToTable("misStateMaster");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Cntname).HasColumnName("cntname");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GstStart)
                    .HasColumnName("gstStart")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StateName)
                    .HasColumnName("stateName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CntnameNavigation)
                    .WithMany(p => p.MisStateMaster)
                    .HasForeignKey(d => d.Cntname)
                    .HasConstraintName("FK__misStateM__cntna__2630A1B7");
            });

            modelBuilder.Entity<MisTasks>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .HasName("PK__misTasks__DD5D55A2623C64E0");

                entity.ToTable("misTasks");

                entity.Property(e => e.TaskId).HasColumnName("taskID");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ClearDat)
                    .HasColumnName("clearDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.FrmUsr)
                    .HasColumnName("frmUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.ReadDat)
                    .HasColumnName("readDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.RequiredBy)
                    .HasColumnName("requiredBy")
                    .HasColumnType("datetime");

                entity.Property(e => e.Subjec)
                    .HasColumnName("subjec")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ToUsr)
                    .HasColumnName("toUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MisCoveringLetterDetails>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__misCover__32DD162D2E7AAD5F");

                entity.ToTable("misCoveringLetterDetails");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.Body)
                    .HasColumnName("body")
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Img)
                    .HasColumnName("img")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Salutation)
                    .HasColumnName("salutation")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Subjec)
                    .HasColumnName("subjec")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Typ)
                    .HasColumnName("typ")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PartyCreditDebitNotes>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__partyCre__D825195ECB9DF55C");

                entity.ToTable("partyCreditDebitNotes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Amt).HasColumnName("amt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.TransactionType)
                    .HasColumnName("transactionType")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Seq)
                   .HasColumnName("seq")
                   .HasMaxLength(50)
                   .IsUnicode(false);
            });

            modelBuilder.Entity<PartyDeaprtmentDetails>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("partyDeaprtmentDetails");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Details)
                    .HasColumnName("details")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

          
            });

            modelBuilder.Entity<PartyDetails>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__partyDet__D825195E50EBAF91");

                entity.ToTable("partyDetails");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

               

                entity.Property(e => e.AirDestination)
                    .HasColumnName("airDestination")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BankForSecurity).HasColumnName("bankForSecurity");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

              

                entity.Property(e => e.ContactDesignation)
                    .HasColumnName("contactDesignation")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContactEmail)
                    .HasColumnName("contactEmail")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContactMobile)
                    .HasColumnName("contactMobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasColumnName("contactPerson")
                    .HasMaxLength(100)
                    .IsUnicode(false);

             

                entity.Property(e => e.CrAmt).HasColumnName("crAmt");
                entity.Property(e => e.Employee).HasColumnName("employee");

                entity.Property(e => e.CrAmtCheck).HasColumnName("crAmtCheck");

                entity.Property(e => e.CrDay).HasColumnName("crDay");

                entity.Property(e => e.CrDaysCheck).HasColumnName("crDaysCheck");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

              

                entity.Property(e => e.Discountlist)
                    .HasColumnName("discountlist")
                    .HasMaxLength(50)
                    .IsUnicode(false);

               

                entity.Property(e => e.DualType).HasColumnName("dualType");

              

                entity.Property(e => e.EximCheck).HasColumnName("eximCheck");

                

                entity.Property(e => e.LeadTime).HasColumnName("leadTime");

                entity.Property(e => e.MainCustomer).HasColumnName("mainCustomer");

            

                entity.Property(e => e.PartyCode)
                    .HasColumnName("partyCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PartyGroup).HasColumnName("partyGroup");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PartyType)
                    .HasColumnName("partyType")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PartyUserName)
                    .HasColumnName("partyUserName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pricelist)
                    .HasColumnName("pricelist")
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.PartyPwd)
                 .HasColumnName("partyPwd")
                 .HasMaxLength(50)
                 .IsUnicode(false);

                entity.Property(e => e.RestrictMode).HasColumnName("restrictMode");

                entity.Property(e => e.SeaDestination)
                    .HasColumnName("seaDestination")
                    .HasMaxLength(100)
                    .IsUnicode(false);

             

                entity.Property(e => e.Statu).HasColumnName("statu");


                entity.Property(e => e.PrefLanguage)
                  .HasColumnName("prefLanguage")
                  .HasMaxLength(50)
                  .IsUnicode(false);
                entity.Property(e => e.OrderReminder1).HasColumnName("orderReminder1");
                entity.Property(e => e.OrderReminder2).HasColumnName("orderReminder2");
                entity.Property(e => e.OrderReminder3).HasColumnName("orderReminder3");
                entity.Property(e => e.DefaultPurchaseorSaleType)
                 .HasColumnName("defaultPurchaseorSaleType")
                 .HasMaxLength(50)
                 .IsUnicode(false);

                entity.Property(e => e.PaymentReminder1).HasColumnName("paymentReminder1");
                entity.Property(e => e.PaymentReminder2).HasColumnName("paymentReminder2");
                entity.Property(e => e.PaymentReminder3).HasColumnName("paymentReminder3");
                entity.Property(e => e.DefaultPaymentMode)
                .HasColumnName("defaultPaymentMode")
                .HasMaxLength(50)
                .IsUnicode(false);


                entity.Property(e => e.KycAcnumber)
               .HasColumnName("kycAcnumber")
               .HasMaxLength(50)
               .IsUnicode(false);

                entity.Property(e => e.KycAcholder)
             .HasColumnName("kycAcholder")
             .HasMaxLength(50)
             .IsUnicode(false);
                entity.Property(e => e.KycAcbank)
             .HasColumnName("kycAcbank")
             .HasMaxLength(50)
             .IsUnicode(false);
                entity.Property(e => e.KycAcbranch)
             .HasColumnName("kycAcbranch")
             .HasMaxLength(50)
             .IsUnicode(false);
                entity.Property(e => e.KycAcifsc)
             .HasColumnName("kycAcifsc")
             .HasMaxLength(50)
             .IsUnicode(false);

            });

            modelBuilder.Entity<PartyAddresses>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_partyAddresses");

                entity.ToTable("partyAddresses");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Addres)
                    .HasColumnName("addres")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.AddressName)
                    .HasColumnName("addressName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PartyDepartments>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_partyDepartments");

                entity.ToTable("partyDepartments");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentDetails)
                    .HasColumnName("departmentDetails")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Statu).HasColumnName("statu");
            });


            modelBuilder.Entity<PartyPaymentDetails>(entity =>
            {
                entity.HasKey(e => e.Lineid)
                    .HasName("PK__partyPay__3249818D2E00CE1B");

                entity.ToTable("partyPaymentDetails");

                entity.Property(e => e.Lineid).HasColumnName("lineid");

                entity.Property(e => e.AdvanceAmt).HasColumnName("advanceAmt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyId)
                    .HasColumnName("companyId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreditNote).HasColumnName("creditNote");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OtherAmounts).HasColumnName("otherAmounts");

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentAmount).HasColumnName("paymentAmount");

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.TransactionDate)
                    .HasColumnName("transactionDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.TransactionType)
                    .HasColumnName("transactionType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usrname)
                    .HasColumnName("usrname")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PartyPaymentsdet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_partyPaymentsdet");

                entity.ToTable("partyPaymentsdet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BillType)
                    .HasColumnName("billType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Billno).HasColumnName("billno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.PaymentAmt).HasColumnName("paymentAmt");
            });

            modelBuilder.Entity<PartyPaymentsuni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__partyPay__D825195EC73F22D1");

                entity.ToTable("partyPaymentsuni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BaseAmt).HasColumnName("baseAmt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VoucherType)
                    .HasColumnName("voucherType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                
                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ModeOfPayment)
                    .HasColumnName("modeOfPayment")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.Rebate).HasColumnName("rebate");

                entity.Property(e => e.ReceiptAmt).HasColumnName("receiptAmt");

                entity.Property(e => e.RevAccount).HasColumnName("revAccount");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tds).HasColumnName("tds");
            });

            modelBuilder.Entity<PartyTransactions>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__partyTra__D825195EC3F8ED30");

                entity.ToTable("partyTransactions");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreditNote).HasColumnName("creditNote");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentAmount).HasColumnName("paymentAmount");

                entity.Property(e => e.PendingAmount).HasColumnName("pendingAmount");

                entity.Property(e => e.ReturnAmount).HasColumnName("returnAmount");

                entity.Property(e => e.TransactionAmt).HasColumnName("transactionAmt");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");
                entity.Property(e => e.OnTraId).HasColumnName("onTraId");

                entity.Property(e => e.TransactionType)
                    .HasColumnName("transactionType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PpcProcessesMaster>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__ppcProce__D825195E66A4B4AA");

                entity.ToTable("ppcProcessesMaster");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ProcessName)
                    .HasColumnName("processName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.QcRequired).HasColumnName("qcRequired");
            });
            modelBuilder.Entity<PpcBatchPlanningEmployeeAssignings>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno, e.Employee })
                    .HasName("pk_ppcBatchPlanningEmployeeAssignings");

                entity.ToTable("ppcBatchPlanningEmployeeAssignings");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Employee).HasColumnName("employee");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PpcBatchPlanningEmployeeAssignings)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ppcBatchP__recor__184C96B4");
            });

            modelBuilder.Entity<PpcBatchPlanningProcesses>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_ppcBatchPlanningProcesses");

                entity.ToTable("ppcBatchPlanningProcesses");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.FromDate)
                    .HasColumnName("fromDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProcessId).HasColumnName("processId");

                entity.Property(e => e.ProcessIncharge).HasColumnName("processIncharge");

                entity.Property(e => e.QcRequired).HasColumnName("qcRequired");

                entity.Property(e => e.ToDate)
                    .HasColumnName("toDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PpcBatchPlanningProcesses)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ppcBatchP__recor__1387E197");
            });

            modelBuilder.Entity<PpcBatchPlanningUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__ppcBatch__D825195ED41F0A97");

                entity.ToTable("ppcBatchPlanningUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");
                entity.Property(e => e.Pos).HasColumnName("pos");
                entity.Property(e => e.BatchNo)
                    .HasColumnName("batchNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");
                entity.Property(e => e.ItemId).HasColumnName("itemId");
                entity.Property(e => e.Qty).HasColumnName("qty");
                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.FromDate)
                    .HasColumnName("fromDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductionIncharge).HasColumnName("productionIncharge");

                entity.Property(e => e.ToDate)
                    .HasColumnName("toDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<PpcMassPlanningDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_ppcMassPlanningDet");


                entity.ToTable("ppcMassPlanningDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PpcMassPlanningDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ppcMassPl__recor__0BE6BFCF");
            });

            modelBuilder.Entity<PpcMassPlanningUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__ppcMassP__D825195E22A5E546");

                entity.ToTable("ppcMassPlanningUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.FromDate)
                    .HasColumnName("fromDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToDate)
                    .HasColumnName("toDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<PpcBatchPlanningSaleOrders>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Soid, e.LineId })
                    .HasName("pk_ppcBatchPlanningSaleOrders");

                entity.ToTable("ppcBatchPlanningSaleOrders");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Soid).HasColumnName("soid");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Sno).HasColumnName("sno");
            });

            modelBuilder.Entity<PpcBatchProcessWiseDetailsDet>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__ppcBatch__D825195E5CF66AA3");

                entity.ToTable("ppcBatchProcessWiseDetailsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Batchno).HasColumnName("batchno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.JobCardNo).HasColumnName("jobCardNo");

                entity.Property(e => e.LineId).HasColumnName("lineId");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.JobCardNoNavigation)
                    .WithMany(p => p.PpcBatchProcessWiseDetailsDet)
                    .HasForeignKey(d => d.JobCardNo)
                    .HasConstraintName("FK__ppcBatchP__jobCa__23BE4960");
            });

            modelBuilder.Entity<PpcBatchProcessWiseDetailsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__ppcBatch__D825195ECC16286D");

                entity.ToTable("ppcBatchProcessWiseDetailsUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");
                entity.Property(e => e.ProcessId).HasColumnName("processId");
                
                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.JobCardNo)
                    .HasColumnName("jobCardNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessIncharge).HasColumnName("processIncharge");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Um).HasColumnName("um");
            });



            modelBuilder.Entity<ProItemWiseAttachmentsUni>(entity =>
            {
                entity.HasKey(e => e.ItemId)
                    .HasName("PK__proItemW__56A128AA0BB80A52");

                entity.ToTable("proItemWiseAttachmentsUni");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .ValueGeneratedNever();

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.MaxBatchQty).HasColumnName("maxBatchQty");

                entity.Property(e => e.MinBatchQty).HasColumnName("minBatchQty");

                entity.Property(e => e.UmId).HasColumnName("umId");
            });

            modelBuilder.Entity<ProItemWiseDesignations>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__proItemW__32DD162D6C929F7C");

                entity.ToTable("proItemWiseDesignations");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.DesigId).HasColumnName("desigId");

                entity.Property(e => e.Itemid).HasColumnName("itemid");

                entity.Property(e => e.Manhrs).HasColumnName("manhrs");
            });

            modelBuilder.Entity<ProItemWiseEquipment>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__proItemW__32DD162D845A88CD");

                entity.ToTable("proItemWiseEquipment");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.EquipmentId).HasColumnName("equipmentId");

                entity.Property(e => e.Itemid).HasColumnName("itemid");

                entity.Property(e => e.Manhrs).HasColumnName("manhrs");
            });

            modelBuilder.Entity<ProItemWiseIngredients>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__proItemW__32DD162D7C0C9BDA");

                entity.ToTable("proItemWiseIngredients");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Ingredient).HasColumnName("ingredient");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Um).HasColumnName("um");
            });

            modelBuilder.Entity<PurPurchaseEnquiryDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("purPurchaseEnquiryDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Uom).HasColumnName("uom");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurPurchaseEnquiryDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purPurcha__recor__0F183235");
            });

            modelBuilder.Entity<PurPurchaseEnquiryDetHistory>(entity =>
            {
                entity.HasKey(e => new { e.AuditId, e.Sno });

                entity.ToTable("purPurchaseEnquiryDet_history");

                entity.Property(e => e.AuditId).HasColumnName("auditId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.AuditDat)
                    .HasColumnName("auditDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.uom).HasColumnName("uom");
 
            });

            modelBuilder.Entity<PurPurchaseEnquiryNotes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno });

                entity.ToTable("purPurchaseEnquiryNotes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(250)
                    .IsUnicode(false);

              
            });

            modelBuilder.Entity<PurPurchaseEnquiryNotesHistory>(entity =>
            {
                entity.HasKey(e => new { e.AuditId, e.Sno });

                entity.ToTable("purPurchaseEnquiryNotes_history");

                entity.Property(e => e.AuditId).HasColumnName("auditId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.AuditDat)
                    .HasColumnName("auditDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RecordId).HasColumnName("recordId");
 
            });

            modelBuilder.Entity<PurPurchaseEnquiryUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purPurch__D825195E85E8D7F5");

                entity.ToTable("purPurchaseEnquiryUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedUser)
                    .HasColumnName("approvedUser")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ApprvedDat)
                    .HasColumnName("apprvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MailCount).HasColumnName("mailCount");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Supid).HasColumnName("supid");

                entity.Property(e => e.Supplier)
                    .HasColumnName("supplier")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurPurchaseEnquiryUniHistory>(entity =>
            {
                entity.HasKey(e => e.AuditId)
                    .HasName("PK__purPurch__43D1739948541850");

                entity.ToTable("purPurchaseEnquiryUni_history");

                entity.Property(e => e.AuditId).HasColumnName("auditId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedUser)
                    .HasColumnName("approvedUser")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ApprvedDat)
                    .HasColumnName("apprvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditDat)
                    .HasColumnName("auditDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MailCount).HasColumnName("mailCount");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Supid).HasColumnName("supid");

                entity.Property(e => e.Supplier)
                    .HasColumnName("supplier")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

             
            });

            modelBuilder.Entity<PurPurchaseOrderDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseOrderDet");

                entity.ToTable("purPurchaseOrderDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ReqdBy)
                   .HasColumnName("reqdBy")
                   .HasColumnType("datetime");

                entity.Property(e => e.PurRequest).HasColumnName("purRequest");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.PurRequestNavigation)
                    .WithMany(p => p.PurPurchaseOrderDet)
                    .HasForeignKey(d => d.PurRequest)
                    .HasConstraintName("FK__purPurcha__purRe__335592AB");
            });

            modelBuilder.Entity<PurPurchaseOrderTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseOrderTaxes");

                entity.ToTable("purPurchaseOrderTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Taxcode)
                    .HasColumnName("taxcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");
            });

            modelBuilder.Entity<PurPurchaseOrderTerms>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseOrderTerms");

                entity.ToTable("purPurchaseOrderTerms");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Term)
                    .HasColumnName("term")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurPurchaseOrderUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purPurch__D825195E4E685BEC");

                entity.ToTable("purPurchaseOrderUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConversionFactor).HasColumnName("conversionFactor");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MailCount).HasColumnName("mailCount");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.PurchaseType)
                    .HasColumnName("purchaseType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefQuotation)
                    .HasColumnName("refQuotation")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefQuotationId).HasColumnName("refQuotationId");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.TypeOfOrder)
                    .HasColumnName("typeOfOrder")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.Vendorid).HasColumnName("vendorid");

                entity.Property(e => e.Vendorname)
                    .HasColumnName("vendorname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                
            });

            modelBuilder.Entity<PurPurchaseRequestDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseRequestDet");

                entity.ToTable("purPurchaseRequestDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.ApprovedQty).HasColumnName("approvedQty");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Department).HasColumnName("department");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Purpose)
                    .HasColumnName("purpose")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.ReqdBy)
                    .HasColumnName("reqdBy")
                    .HasColumnType("datetime");

                entity.Property(e => e.SuggestedSupplier).HasColumnName("suggestedSupplier");

                entity.Property(e => e.Um).HasColumnName("UM");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurPurchaseRequestDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purPurcha__recor__353DDB1D");
            });

            modelBuilder.Entity<PurPurchaseRequestDetHistory>(entity =>
            {
                entity.HasKey(e => new { e.AuditId, e.Audsno })
                    .HasName("pk_purPurchaseRequestDet_History");

                entity.ToTable("purPurchaseRequestDet_History");

                entity.Property(e => e.AuditId).HasColumnName("auditID");

                entity.Property(e => e.Audsno).HasColumnName("audsno");

                entity.Property(e => e.ApprovedQty).HasColumnName("approvedQty");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AuditDat)
                    .HasColumnName("auditDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Purpose)
                    .HasColumnName("purpose")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.ReqdBy)
                    .HasColumnName("reqdBy")
                    .HasColumnType("datetime");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.SuggestedSupplier).HasColumnName("suggestedSupplier");

                entity.Property(e => e.Um).HasColumnName("UM");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurPurchaseRequestUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purPurch__D825195EEA46D936");

                entity.ToTable("purPurchaseRequestUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Department).HasColumnName("department");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Empno).HasColumnName("empno");

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

            
            });

            modelBuilder.Entity<PurPurchaseRequestUniHistory>(entity =>
            {
                entity.HasKey(e => e.AuditId)
                    .HasName("PK__purPurch__43D173F922218704");

                entity.ToTable("purPurchaseRequestUni_history");

                entity.Property(e => e.AuditId).HasColumnName("auditID");

                entity.Property(e => e.AuditDat)
                    .HasColumnName("auditDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.AuditType).HasColumnName("auditType");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Department).HasColumnName("department");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Empno).HasColumnName("empno");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurQuotationDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purQuotationDet");

                entity.ToTable("purQuotationDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemDescription)
                    .HasColumnName("itemDescription")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.LeadDays).HasColumnName("leadDays");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurQuotationDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purQuotat__recor__381A47C8");
            });

            modelBuilder.Entity<PurQuotationTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purQuotationTaxes");

                entity.ToTable("purQuotationTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.TaxCode)
                    .HasColumnName("taxCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxPer).HasColumnName("taxPer");

                entity.Property(e => e.TaxValue).HasColumnName("taxValue");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurQuotationTaxes)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purQuotat__recor__390E6C01");
            });

            modelBuilder.Entity<PurQuotationTerms>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purQuotationTerms");

                entity.ToTable("purQuotationTerms");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Term)
                    .HasColumnName("term")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurQuotationTerms)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purQuotat__recor__3A02903A");
            });

            modelBuilder.Entity<PurQuotationUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purQuota__D825195EF95770F6");

                entity.ToTable("purQuotationUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PurchaseType)
                    .HasColumnName("purchaseType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefQuotation)
                    .HasColumnName("refQuotation")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefQuotationId).HasColumnName("refQuotationId");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.Vendorid).HasColumnName("vendorid");

                entity.Property(e => e.Vendorname)
                    .HasColumnName("vendorname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<PurPurchaseTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseTaxes");

                entity.ToTable("purPurchaseTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Taxcode)
                    .HasColumnName("taxcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");
            });

            modelBuilder.Entity<PurPurchasesDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchasesDet");

                entity.ToTable("purPurchasesDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Batchno)
                    .HasColumnName("batchno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gin)
                   .HasColumnName("gin")
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Expdate)
                    .HasColumnName("expdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Manudate)
                    .HasColumnName("manudate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Mrp).HasColumnName("mrp");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Store).HasColumnName("store");

                entity.Property(e => e.Um).HasColumnName("um");
            });

            modelBuilder.Entity<PurPurchasesUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purPurch__D825195E7DE2CAE2");

                entity.ToTable("purPurchasesUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyConversion).HasColumnName("currencyConversion");

                entity.Property(e => e.CurrencySymbol)
                    .HasColumnName("currencySymbol")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Invoicedat)
                    .HasColumnName("invoicedat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Invoiceno)
                    .HasColumnName("invoiceno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.PurchaseType)
                    .HasColumnName("purchaseType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QcCheck).HasColumnName("qcCheck");

                entity.Property(e => e.RefPoid).HasColumnName("refPOId");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Settlemode).HasColumnName("settlemode");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Transporter)
                    .HasColumnName("transporter")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vendorid).HasColumnName("vendorid");

                entity.Property(e => e.Vendorname)
                    .HasColumnName("vendorname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurSetup>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__purSetup__DDDF6446AF213E57");

                entity.ToTable("purSetup");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.SetupCode)
                    .HasColumnName("setupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SetupDescription)
                    .HasColumnName("setupDescription")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SetupValue)
                    .HasColumnName("setupValue")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurSupplierGroups>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purSuppl__D825197EA59E5963");

                entity.ToTable("purSupplierGroups");

                entity.Property(e => e.RecordId).HasColumnName("recordID");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GroupCode)
                    .HasColumnName("groupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GrpTag)
                    .HasColumnName("grpTag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MGrp)
                    .HasColumnName("mGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SGrp)
                    .HasColumnName("sGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<PurTerms>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__purTerms__32DD162D167BAEA4");

                entity.ToTable("purTerms");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Term)
                    .HasColumnName("term")
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Purpurchasetypes>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__purpurch__32DD162DFEC8AE1B");

                entity.ToTable("purpurchasetypes");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ImportType).HasColumnName("importType");

                entity.Property(e => e.Purtype)
                    .HasColumnName("purtype")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurEmails>(entity =>
            {
                entity.HasKey(e => e.Sno)
                    .HasName("PK__purEmail__DDDF644677A90DDD");

                entity.ToTable("purEmails");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.SetupCode)
                    .HasColumnName("setupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SetupValue)
                    .HasColumnName("setupValue")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurPurchaseReturnTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseReturnTaxes");

                entity.ToTable("purPurchaseReturnTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Taxcode)
                    .HasColumnName("taxcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurPurchaseReturnTaxes)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purPurcha__recor__7720AD13");
            });

            modelBuilder.Entity<PurPurchaseReturnsDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_purPurchaseReturnsDet");

                entity.ToTable("purPurchaseReturnsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Batchno)
                    .HasColumnName("batchno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Expdate)
                    .HasColumnName("expdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Lotno).HasColumnName("lotno");

                entity.Property(e => e.Manudate)
                    .HasColumnName("manudate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Mrp).HasColumnName("mrp");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.PurPurchaseReturnsDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__purPurcha__recor__74444068");
            });

            modelBuilder.Entity<PurPurchaseReturnsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__purPurch__D825195E2B34A5BB");

                entity.ToTable("purPurchaseReturnsUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyConversion).HasColumnName("currencyConversion");

                entity.Property(e => e.CurrencySymbol)
                    .HasColumnName("currencySymbol")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.RefMir).HasColumnName("refMIR");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Settlemode).HasColumnName("settlemode");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Transporter)
                    .HasColumnName("transporter")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Vendorid).HasColumnName("vendorid");

                entity.Property(e => e.Vendorname)
                    .HasColumnName("vendorname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QcProcessWiseDetails>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__qcProces__D825195EB8F54080");

                entity.ToTable("qcProcessWiseDetails");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Fromdate)
                    .HasColumnName("fromdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProcessId).HasColumnName("processId");

                entity.Property(e => e.BatchNo).HasColumnName("batchNo");
                entity.Property(e => e.QcIncharge).HasColumnName("qcIncharge");

                entity.Property(e => e.RectificationValue).HasColumnName("rectificationValue");

                entity.Property(e => e.Rectified).HasColumnName("rectified");

                entity.Property(e => e.Rejected).HasColumnName("rejected");

                entity.Property(e => e.RejectedValue).HasColumnName("rejectedValue");

                entity.Property(e => e.SamplesCollected).HasColumnName("samplesCollected");

                entity.Property(e => e.Test).HasColumnName("test");

                entity.Property(e => e.Todate)
                    .HasColumnName("todate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<QcTestings>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__qcTestin__D825195E8B59FFC9");

                entity.ToTable("qcTestings");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CheckingType).HasColumnName("checkingType");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.MicroCheck).HasColumnName("microCheck");

                entity.Property(e => e.TestArea)
                    .HasColumnName("testArea")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Testname)
                    .HasColumnName("testname")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QcTraTestsDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_qcTraTestsDet");

                entity.ToTable("qcTraTestsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CheckedQty).HasColumnName("checkedQty");

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Lotno).HasColumnName("lotno");

                entity.Property(e => e.Lottype)
                    .HasColumnName("lottype")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RectificationCost).HasColumnName("rectificationCost");

                entity.Property(e => e.RectifiedQty).HasColumnName("rectifiedQty");

                entity.Property(e => e.RejectedQty).HasColumnName("rejectedQty");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.ValueOfItem).HasColumnName("valueOfItem");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.QcTraTestsDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__qcTraTest__recor__019E3B86");
            });

            modelBuilder.Entity<QcTraTestsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__qcTraTes__D825195E8F51533B");

                entity.ToTable("qcTraTestsUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.InspectedBy).HasColumnName("inspectedBy");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Testid).HasColumnName("testid");

                entity.Property(e => e.Typ)
                    .HasColumnName("typ")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usrname)
                    .HasColumnName("usrname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.QcTraTestsUni)
                    .HasForeignKey(d => d.Testid)
                    .HasConstraintName("FK__qcTraTest__testi__7EC1CEDB");
            });


            modelBuilder.Entity<ResOutletMaster>(entity =>
            {
                entity.HasKey(e => new { e.RestaCode, e.CustomerCode })
                    .HasName("pk_resOutletMaster");

                entity.ToTable("resOutletMaster");

                entity.Property(e => e.RestaCode)
                    .HasColumnName("restaCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.AutoSettleChck).HasColumnName("autoSettleChck");

                entity.Property(e => e.BillingGroup).HasColumnName("billingGroup");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Outlettype).HasColumnName("outlettype");

                entity.Property(e => e.RestaName)
                    .HasColumnName("restaName")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SalSalesDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_salSalesDet");

                entity.ToTable("salSalesDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");
                entity.Property(e => e.RefSoid).HasColumnName("refSoid");
                entity.Property(e => e.RefSoLine).HasColumnName("refSoLine");

                entity.Property(e => e.Batchno)
                    .HasColumnName("batchno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Expdate)
                    .HasColumnName("expdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Manudate)
                    .HasColumnName("manudate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Mrp).HasColumnName("mrp");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Store).HasColumnName("store");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.Property(e => e.WarrentyTill)
                    .HasColumnName("warrentyTill")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.SalSalesDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__salSalesD__recor__025D5595");
            });

            modelBuilder.Entity<SalSalesTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_salSalesTaxes");

                entity.ToTable("salSalesTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.Taxcode)
                    .HasColumnName("taxcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.SalSalesTaxes)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__salSalesT__recor__0539C240");
            });

            modelBuilder.Entity<SalSalesUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__salSales__D825195E60CDAD26");

                entity.ToTable("salSalesUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PassCodeCheck).HasColumnName("passCodeCheck");

                entity.Property(e => e.PassCode)
                    .HasColumnName("passCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyConversion).HasColumnName("currencyConversion");

                entity.Property(e => e.CurrencySymbol)
                    .HasColumnName("currencySymbol")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Dcdat)
                    .HasColumnName("dcdat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Dcno)
                    .HasColumnName("dcno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.RefSoid).HasColumnName("refSOId");

                entity.Property(e => e.SaleType)
                    .HasColumnName("saleType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Settlemode).HasColumnName("settlemode");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Transporter)
                    .HasColumnName("transporter")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SalSaleReturnTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_salSaleReturnTaxes");

                entity.ToTable("salSaleReturnTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Taxcode)
                    .HasColumnName("taxcode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxper).HasColumnName("taxper");

                entity.Property(e => e.Taxvalue).HasColumnName("taxvalue");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.SalSaleReturnTaxes)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__salSaleRe__recor__3E723F9C");
            });

            modelBuilder.Entity<SalSaleReturnsDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_salSaleReturnsDet");

                entity.ToTable("salSaleReturnsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Batchno)
                    .HasColumnName("batchno")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Expdate)
                    .HasColumnName("expdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ItemId).HasColumnName("itemId");

                entity.Property(e => e.ItemName)
                    .HasColumnName("itemName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Lotno).HasColumnName("lotno");

                entity.Property(e => e.Manudate)
                    .HasColumnName("manudate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Mrp).HasColumnName("mrp");

                entity.Property(e => e.Qty).HasColumnName("qty");

                entity.Property(e => e.Rat).HasColumnName("rat");

                entity.Property(e => e.Um).HasColumnName("um");

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.SalSaleReturnsDet)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__salSaleRe__recor__3B95D2F1");
            });

            modelBuilder.Entity<SalSaleReturnsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__salSaleR__D825195EC22B6EEE");

                entity.ToTable("salSaleReturnsUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDat)
                    .HasColumnName("approvedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApprovedUsr)
                    .HasColumnName("approvedUsr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Baseamt).HasColumnName("baseamt");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyConversion).HasColumnName("currencyConversion");

                entity.Property(e => e.CurrencySymbol)
                    .HasColumnName("currencySymbol")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Others).HasColumnName("others");

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.PartyName)
                    .HasColumnName("partyName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.RefInvoice).HasColumnName("refInvoice");

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Settlemode).HasColumnName("settlemode");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmt).HasColumnName("totalAmt");

                entity.Property(e => e.Transporter)
                    .HasColumnName("transporter")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Webid)
                    .HasColumnName("webid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SalcustomerGroups>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__salcusto__D825197E5D18C66C");

                entity.ToTable("salcustomerGroups");

                entity.Property(e => e.RecordId).HasColumnName("recordID");

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Chk).HasColumnName("chk");

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.GroupCode)
                    .HasColumnName("groupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GrpTag)
                    .HasColumnName("grpTag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MGrp)
                    .HasColumnName("mGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SGrp)
                    .HasColumnName("sGrp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Statu).HasColumnName("statu");
            });

            modelBuilder.Entity<TotAdvanceDetails>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__totAdvan__D825195EF8339C8F");

                entity.ToTable("totAdvanceDetails");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.Amt).HasColumnName("amt");

                entity.Property(e => e.Bankdetails)
                    .HasColumnName("bankdetails")
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Detail1)
                    .HasColumnName("detail1")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Detail2)
                    .HasColumnName("detail2")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Detail3)
                    .HasColumnName("detail3")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PartyId).HasColumnName("partyId");

                entity.Property(e => e.Paymentmode)
                    .HasColumnName("paymentmode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrintCount).HasColumnName("printCount");

                entity.Property(e => e.Remarks)
                    .HasColumnName("remarks")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Seq)
                    .HasColumnName("seq")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.Tratype)
                    .HasColumnName("tratype")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsrName)
                    .HasColumnName("usrName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TransactionsAudit>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__transact__32DD162DD55B2E68");

                entity.ToTable("transactions_Audit");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descr)
                    .HasColumnName("descr")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Syscode)
                    .HasColumnName("syscode")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.TraId).HasColumnName("traId");

                entity.Property(e => e.TraModule)
                    .HasColumnName("traModule")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Transact)
                    .HasColumnName("transact")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tratype).HasColumnName("tratype");

                entity.Property(e => e.Usr)
                    .HasColumnName("usr")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserCompleteProfile>(entity =>
            {
                entity.HasKey(e => new { e.UsrName, e.CustomerCode });

                entity.ToTable("userCompleteProfile");

                entity.Property(e => e.UsrName)
                    .HasColumnName("usrName")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.AboutUser)
                    .HasColumnName("aboutUser")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.BannerImage)
                    .HasColumnName("bannerImage")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmployeeNo).HasColumnName("employeeNo");

                entity.Property(e => e.ThumbImage)
                    .HasColumnName("thumbImage")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserProfileName)
                    .HasColumnName("userProfileName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.EmployeeNoNavigation)
                    .WithMany(p => p.UserCompleteProfile)
                    .HasForeignKey(d => d.EmployeeNo)
                    .HasConstraintName("FK__userCompl__emplo__0F4D3C5F");
            });

            modelBuilder.Entity<UserPostingAccess>(entity =>
            {
                entity.HasKey(e => new { e.PostingId, e.UserToAccess })
                    .HasName("pk_userPostingAccess");

                entity.ToTable("userPostingAccess");

                entity.Property(e => e.PostingId).HasColumnName("postingId");

                entity.Property(e => e.UserToAccess)
                    .HasColumnName("userToAccess")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Posting)
                    .WithMany(p => p.UserPostingAccess)
                    .HasForeignKey(d => d.PostingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__userPosti__posti__1411F17C");
            });

            modelBuilder.Entity<UserPostings>(entity =>
            {
                entity.HasKey(e => e.PostingId)
                    .HasName("PK__userPost__48B21C245C172849");

                entity.ToTable("userPostings");

                entity.Property(e => e.PostingId).HasColumnName("postingId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.PostSubject)
                    .HasColumnName("postSubject")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Postcomments).HasColumnName("postcomments");

                entity.Property(e => e.PostcommentsEnable).HasColumnName("postcommentsEnable");

                entity.Property(e => e.Postdetail)
                    .HasColumnName("postdetail")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.Postlikes).HasColumnName("postlikes");

                entity.Property(e => e.UserCode)
                    .HasColumnName("userCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserPostingsComments>(entity =>
            {
                entity.HasKey(e => e.CommentId)
                    .HasName("PK__userPost__CDDE919D2EFD08E6");

                entity.ToTable("userPostingsComments");

                entity.Property(e => e.CommentId).HasColumnName("commentId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.PostingId).HasColumnName("postingId");

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Posting)
                    .WithMany(p => p.UserPostingsComments)
                    .HasForeignKey(d => d.PostingId)
                    .HasConstraintName("FK__userPosti__posti__16EE5E27");
            });

            modelBuilder.Entity<UserPostingsLikes>(entity =>
            {
                entity.HasKey(e => e.LikeId)
                    .HasName("PK__userPost__4FC592DBBA8C1B7F");

                entity.ToTable("userPostingsLikes");

                entity.Property(e => e.LikeId).HasColumnName("likeId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.PostingId).HasColumnName("postingId");

                entity.Property(e => e.UserName)
                    .HasColumnName("userName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Posting)
                    .WithMany(p => p.UserPostingsLikes)
                    .HasForeignKey(d => d.PostingId)
                    .HasConstraintName("FK__userPosti__posti__19CACAD2");
            });

            modelBuilder.Entity<UsrAut>(entity =>
            {
                entity.HasKey(e => new { e.UsrName, e.CustomerCode });

                entity.ToTable("usrAut");

                entity.Property(e => e.UsrName)
                    .HasColumnName("usrName")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MobileFreeEnable).HasColumnName("mobileFreeEnable");

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Pwd)
                    .HasColumnName("pwd")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.RoleName)
                    .HasColumnName("roleName")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.WebFreeEnable).HasColumnName("webFreeEnable");
            });

            modelBuilder.Entity<TotSalesSupportings>(entity =>
            {
                entity.HasKey(e => e.Slno)
                    .HasName("PK__totSales__32DD162DA03D9F95");

                entity.ToTable("totSalesSupportings");

                entity.Property(e => e.Slno).HasColumnName("slno");

                entity.Property(e => e.AccName).HasColumnName("accName");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Amt).HasColumnName("amt");

                entity.Property(e => e.BankDetails)
                    .HasColumnName("bankDetails")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.BillNo).HasColumnName("billNo");

                entity.Property(e => e.BillType)
                    .HasColumnName("billType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Branchid)
                    .HasColumnName("branchid")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode).HasColumnName("customerCode");

                entity.Property(e => e.Descript)
                    .HasColumnName("descript")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.GuestName)
                    .HasColumnName("guestName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoomCheckinid).HasColumnName("roomCheckinid");

                entity.Property(e => e.SettleDate)
                    .HasColumnName("settleDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.SettleMode)
                    .HasColumnName("settleMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Usrname)
                    .HasColumnName("usrname")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
