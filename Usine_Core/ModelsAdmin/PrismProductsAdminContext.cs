using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Usine_Core.ModelsAdmin
{
    public partial class PrismProductsAdminContext : DbContext
    {
        public PrismProductsAdminContext()
        {
        }

        public PrismProductsAdminContext(DbContextOptions<PrismProductsAdminContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CrmEnquiries> CrmEnquiries { get; set; }
        public virtual DbSet<CrmFollowup> CrmFollowup { get; set; }
        public virtual DbSet<CrmOrderApproval> CrmOrderApproval { get; set; }
        public virtual DbSet<CrmOrderDet> CrmOrderDet { get; set; }
        public virtual DbSet<CrmOrderUni> CrmOrderUni { get; set; }
        public virtual DbSet<CrmQuotationDet> CrmQuotationDet { get; set; }
        public virtual DbSet<CrmQuotationTaxes> CrmQuotationTaxes { get; set; }
        public virtual DbSet<CrmQuotationTerms> CrmQuotationTerms { get; set; }
        public virtual DbSet<CrmQuotationUni> CrmQuotationUni { get; set; }
        public virtual DbSet<CrmTickets> CrmTickets { get; set; }
        public virtual DbSet<CustomerBranches> CustomerBranches { get; set; }
        public virtual DbSet<CustomerModules> CustomerModules { get; set; }
        public virtual DbSet<CustomerReceiptsDet> CustomerReceiptsDet { get; set; }
        public virtual DbSet<CustomerReceiptsUni> CustomerReceiptsUni { get; set; }
        public virtual DbSet<CustomerRegistrations> CustomerRegistrations { get; set; }
        public virtual DbSet<ProductDetails> ProductDetails { get; set; }
        public virtual DbSet<ProductModules> ProductModules { get; set; }
        public virtual DbSet<Usraut> Usraut { get; set; }
        public virtual DbSet<VendorDetails> VendorDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
             // optionsBuilder.UseSqlServer("Server=DESKTOP-FND9GDG;Database=Prismadmin;Trusted_Connection=True;");
               optionsBuilder.UseSqlServer("server=192.168.29.53,49792;user id=sa;password=C0rtr@ck3r@2024@0124;database=ERPAdmin;Trusted_Connection=false");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmEnquiries>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmEnqui__D825195E67E1CB0D");

                entity.ToTable("crmEnquiries");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CallerComments)
                    .HasColumnName("callerComments")
                    .HasMaxLength(500)
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

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NextCallDate)
                    .HasColumnName("nextCallDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("productCode")
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

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("datetime");

                entity.Property(e => e.VenodrId).HasColumnName("venodrId");

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CrmFollowup>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmFollo__D825195EB609A508");

                entity.ToTable("crmFollowup");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.CallerComments)
                    .HasColumnName("callerComments")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerComments)
                    .HasColumnName("customerComments")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.EnquiryId).HasColumnName("enquiryId");

                entity.Property(e => e.NextCallDate)
                    .HasColumnName("nextCallDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.NextCallMode)
                    .HasColumnName("nextCallMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.Usrname)
                    .HasColumnName("usrname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Enquiry)
                    .WithMany(p => p.CrmFollowup)
                    .HasForeignKey(d => d.EnquiryId)
                    .HasConstraintName("FK__crmFollow__enqui__5CD6CB2B");
            });

            modelBuilder.Entity<CrmOrderApproval>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmOrderApproval");

                entity.ToTable("crmOrderApproval");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Module)
                    .HasColumnName("module")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Trainingdays)
                    .HasColumnName("trainingdays")
                    .HasColumnType("datetime");

                entity.Property(e => e.VendorId).HasColumnName("vendorId");
            });

            modelBuilder.Entity<CrmOrderDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmOrderDet");

                entity.ToTable("crmOrderDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Module)
                    .HasColumnName("module")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Trainingdays).HasColumnName("trainingdays");

                entity.Property(e => e.VendorId).HasColumnName("vendorId");
            });

            modelBuilder.Entity<CrmOrderUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmOrder__D825195E41E463FF");

                entity.ToTable("crmOrderUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
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

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuotationId).HasColumnName("quotationId");

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

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("vendorId");

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Quotation)
                    .WithMany(p => p.CrmOrderUni)
                    .HasForeignKey(d => d.QuotationId)
                    .HasConstraintName("FK__crmOrderU__quota__3A81B327");
            });

            modelBuilder.Entity<CrmQuotationDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmQuotationDet");

                entity.ToTable("crmQuotationDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Descriptio)
                    .HasColumnName("descriptio")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Module)
                    .HasColumnName("module")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Trainingdays).HasColumnName("trainingdays");

                entity.Property(e => e.VendorId).HasColumnName("vendorId");
            });

            modelBuilder.Entity<CrmQuotationTaxes>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmQuotationTaxes");

                entity.ToTable("crmQuotationTaxes");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

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
                    .HasConstraintName("FK__crmQuotat__recor__06CD04F7");
            });

            modelBuilder.Entity<CrmQuotationTerms>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_crmQuotationTerms");

                entity.ToTable("crmQuotationTerms");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Term)
                    .HasColumnName("term")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Record)
                    .WithMany(p => p.CrmQuotationTerms)
                    .HasForeignKey(d => d.RecordId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__crmQuotat__recor__02FC7413");
            });

            modelBuilder.Entity<CrmQuotationUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__crmQuota__D825195EB901BC41");

                entity.ToTable("crmQuotationUni");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BaseAmt).HasColumnName("baseAmt");

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

                entity.Property(e => e.EnquiryId).HasColumnName("enquiryId");

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasColumnName("productCode")
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

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.Property(e => e.Taxes).HasColumnName("taxes");

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Totalamt).HasColumnName("totalamt");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("vendorId");

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Enquiry)
                    .WithMany(p => p.CrmQuotationUni)
                    .HasForeignKey(d => d.EnquiryId)
                    .HasConstraintName("FK__crmQuotat__enqui__35BCFE0A");
            });

            modelBuilder.Entity<CrmTickets>(entity =>
            {
                entity.HasKey(e => e.TicketId)
                    .HasName("PK__crmTicke__3333C6105EDE429B");

                entity.ToTable("crmTickets");

                entity.Property(e => e.TicketId).HasColumnName("ticketId");

                entity.Property(e => e.ClearedDat)
                    .HasColumnName("clearedDat")
                    .HasColumnType("datetime");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.CustomerIssue)
                    .HasColumnName("customerIssue")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ServiceDescription)
                    .HasColumnName("serviceDescription")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Statu).HasColumnName("statu");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CrmTickets)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__crmTicket__custo__45F365D3");
            });

            modelBuilder.Entity<CustomerBranches>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.BranchId })
                    .HasName("PK__customer__514020A8FB1992D1");

                entity.ToTable("customerBranches");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.BranchId)
                    .HasColumnName("branchId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.BranchHead)
                    .HasColumnName("branchHead")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BranchName)
                    .HasColumnName("branchName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

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

                entity.Property(e => e.Outlets).HasColumnName("outlets");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CustomerModules>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.Sno })
                    .HasName("pk_customerModules");

                entity.ToTable("customerModules");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.ExpDate)
                    .HasColumnName("expDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Module)
                    .HasColumnName("module")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasColumnName("productCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("vendorId");
            });

            modelBuilder.Entity<CustomerReceiptsDet>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.Sno })
                    .HasName("pk_customerReceiptsDet");

                entity.ToTable("customerReceiptsDet");

                entity.Property(e => e.RecordId).HasColumnName("recordId");

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Module)
                    .HasColumnName("module")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CustomerReceiptsUni>(entity =>
            {
                entity.HasKey(e => e.RecordId)
                    .HasName("PK__customer__D825195E0D7DCEB4");

                entity.ToTable("customerReceiptsUni");

                entity.Property(e => e.RecordId)
                    .HasColumnName("recordId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.ModeofPayment).HasColumnName("modeofPayment");

                entity.Property(e => e.ReceiptAmount).HasColumnName("receiptAmount");

                entity.Property(e => e.ReceiptNo)
                    .HasColumnName("receiptNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerReceiptsUni)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__customerR__custo__48CFD27E");
            });

            modelBuilder.Entity<CustomerRegistrations>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK__Customer__B611CB7DB08083D2");

                entity.Property(e => e.CustomerId).HasColumnName("customerId");

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Coins)
                    .HasColumnName("coins")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencySymbol)
                    .HasColumnName("currencySymbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Customer)
                    .HasColumnName("customer")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Dat)
                    .HasColumnName("dat")
                    .HasColumnType("datetime");

                entity.Property(e => e.DefaultUser)
                    .HasColumnName("defaultUser")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExpDate)
                    .HasColumnName("expDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fiscal)
                    .HasColumnName("fiscal")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaxBranches).HasColumnName("maxBranches");

                entity.Property(e => e.MaxOutlets).HasColumnName("maxOutlets");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductId)
                    .HasColumnName("productId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RegDate)
                    .HasColumnName("regDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Schem)
                    .HasColumnName("schem")
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

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("vendorId");

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CustomerRegistrations)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__CustomerR__produ__412EB0B6");
            });

            modelBuilder.Entity<ProductDetails>(entity =>
            {
                entity.HasKey(e => e.ProductCode)
                    .HasName("PK__productD__C2068388A1966D83");

                entity.ToTable("productDetails");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("productCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaxInstallationTime).HasColumnName("maxInstallationTime");

                entity.Property(e => e.MaxTrainingTime).HasColumnName("maxTrainingTime");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.PriceType).HasColumnName("priceType");

                entity.Property(e => e.ProductDescription)
                    .HasColumnName("productDescription")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .HasColumnName("productName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductType)
                    .HasColumnName("productType")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductModules>(entity =>
            {
                entity.HasKey(e => new { e.ProductCode, e.Sno })
                    .HasName("pk_productModules");

                entity.ToTable("productModules");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("productCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sno).HasColumnName("sno");

                entity.Property(e => e.ModuleDescription)
                    .HasColumnName("moduleDescription")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ModuleName)
                    .HasColumnName("moduleName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.TrainingDays).HasColumnName("trainingDays");

                entity.HasOne(d => d.ProductCodeNavigation)
                    .WithMany(p => p.ProductModules)
                    .HasForeignKey(d => d.ProductCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__productMo__produ__30F848ED");
            });

            modelBuilder.Entity<Usraut>(entity =>
            {
                entity.HasKey(e => new { e.Username, e.VendorCode })
                    .HasName("pk_usraut");

                entity.ToTable("usraut");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorCode).HasColumnName("vendorCode");

                entity.Property(e => e.MainUser)
                    .HasColumnName("mainUser")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pos).HasColumnName("pos");

                entity.Property(e => e.Pwd)
                    .HasColumnName("pwd")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rolename)
                    .HasColumnName("rolename")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VendorDetails>(entity =>
            {
                entity.HasKey(e => e.VendorCode)
                    .HasName("PK__vendorDe__0C65ACA4568802A7");

                entity.ToTable("vendorDetails");

                entity.Property(e => e.VendorCode)
                    .HasColumnName("vendorCode")
                    .ValueGeneratedNever();

                entity.Property(e => e.Addr)
                    .HasColumnName("addr")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasColumnName("contactPerson")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

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

                entity.Property(e => e.Gst)
                    .HasColumnName("gst")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MainVendor).HasColumnName("mainVendor");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tel)
                    .HasColumnName("tel")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorName)
                    .HasColumnName("vendorName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Web)
                    .HasColumnName("web")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.MainVendorNavigation)
                    .WithMany(p => p.InverseMainVendorNavigation)
                    .HasForeignKey(d => d.MainVendor)
                    .HasConstraintName("FK__vendorDet__mainV__276EDEB3");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
