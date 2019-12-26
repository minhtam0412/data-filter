using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SimERP.Data.DBEntities
{
    public partial class DBEntities : DbContext
    {
        public DBEntities()
        {
        }

        public DBEntities(DbContextOptions<DBEntities> options)
            : base(options)
        {
        }

        public virtual DbSet<AggregateCosts> AggregateCosts { get; set; }
        public virtual DbSet<AmountDefine> AmountDefine { get; set; }
        public virtual DbSet<AreaList> AreaList { get; set; }
        public virtual DbSet<AttachFile> AttachFile { get; set; }
        public virtual DbSet<CitiMart> CitiMart { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerCommission> CustomerCommission { get; set; }
        public virtual DbSet<CustomerDelivery> CustomerDelivery { get; set; }
        public virtual DbSet<CustomerDeliveryAddress> CustomerDeliveryAddress { get; set; }
        public virtual DbSet<CustomerDetail> CustomerDetail { get; set; }
        public virtual DbSet<CustomerProduct> CustomerProduct { get; set; }
        public virtual DbSet<CustomerSale> CustomerSale { get; set; }
        public virtual DbSet<CustomerType> CustomerType { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<ExchangeRate> ExchangeRate { get; set; }
        public virtual DbSet<Fiscal> Fiscal { get; set; }
        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<GroupCompany> GroupCompany { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<OptionSystem> OptionSystem { get; set; }
        public virtual DbSet<PackageUnit> PackageUnit { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<PaymentTerm> PaymentTerm { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PriceSpreadsheets> PriceSpreadsheets { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductCifprice> ProductCifprice { get; set; }
        public virtual DbSet<ProductDetail> ProductDetail { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<ReasonInOut> ReasonInOut { get; set; }
        public virtual DbSet<RefNo> RefNo { get; set; }
        public virtual DbSet<ReportColumnView> ReportColumnView { get; set; }
        public virtual DbSet<ReportTotal> ReportTotal { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RolePermission> RolePermission { get; set; }
        public virtual DbSet<SaleInvoice> SaleInvoice { get; set; }
        public virtual DbSet<SaleInvoiceDetail> SaleInvoiceDetail { get; set; }
        public virtual DbSet<SaleReturn> SaleReturn { get; set; }
        public virtual DbSet<SaleReturnDetail> SaleReturnDetail { get; set; }
        public virtual DbSet<Sales> Sales { get; set; }
        public virtual DbSet<ShippedPrice> ShippedPrice { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<Tax> Tax { get; set; }
        public virtual DbSet<TokenRefresh> TokenRefresh { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserEditableData> UserEditableData { get; set; }
        public virtual DbSet<UserPermission> UserPermission { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<VendorProduct> VendorProduct { get; set; }
        public virtual DbSet<VendorType> VendorType { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }

        // Unable to generate entity type for table 'dbo.GiaCIF'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.ProductGiaMinMaxTB'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.GiaVaHTT'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.FiscalStatus'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=192.168.1.8;Initial Catalog=SimERP;Persist Security Info=True;User ID=sim;Password=Sim@123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregateCosts>(entity =>
            {
                entity.ToTable("AggregateCosts", "cal");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.DienGiai).HasMaxLength(255);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LoaiCp)
                    .HasColumnName("LoaiCP")
                    .HasMaxLength(255);

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AmountDefine>(entity =>
            {
                entity.HasKey(e => e.MaSoLuong);

                entity.ToTable("AmountDefine", "cal");

                entity.Property(e => e.MaSoLuong)
                    .HasMaxLength(255)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<AreaList>(entity =>
            {
                entity.ToTable("AreaList", "cal");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GhiChu).HasColumnType("ntext");

                entity.Property(e => e.MaKhuVuc).HasMaxLength(3);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.TenKhuVuc).HasMaxLength(4000);
            });

            modelBuilder.Entity<AttachFile>(entity =>
            {
                entity.HasKey(e => e.AttachId);

                entity.Property(e => e.AttachId).HasColumnName("AttachID");

                entity.Property(e => e.Desctiption).HasMaxLength(500);

                entity.Property(e => e.FileName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FileNameOriginal).HasMaxLength(250);

                entity.Property(e => e.FilePath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FileSize).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.FileTitle).HasMaxLength(250);

                entity.Property(e => e.KeyValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OptionName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CitiMart>(entity =>
            {
                entity.ToTable("CitiMart", "cal");

                entity.Property(e => e.DchiEmail)
                    .HasColumnName("DChiEmail")
                    .HasMaxLength(255);

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.DiaChiGiao).HasMaxLength(255);

                entity.Property(e => e.MaKhuVuc).HasMaxLength(3);

                entity.Property(e => e.Msthue)
                    .HasColumnName("MSThue")
                    .HasMaxLength(40);

                entity.Property(e => e.NgMua).HasMaxLength(100);

                entity.Property(e => e.Nhom).HasMaxLength(50);

                entity.Property(e => e.TenDvi)
                    .HasColumnName("TenDVi")
                    .HasMaxLength(255);

                entity.Property(e => e.TenTat).HasMaxLength(100);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currency", "list");

                entity.Property(e => e.CurrencyId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.SearchString).HasMaxLength(2000);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer", "list");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.BankingName).HasMaxLength(250);

                entity.Property(e => e.BankingNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyAddress).HasMaxLength(250);

                entity.Property(e => e.CompanyName).HasMaxLength(250);

                entity.Property(e => e.CountryId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CustomerTypeList)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DebtCeiling).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FaxNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RepresentativeAddress).HasMaxLength(250);

                entity.Property(e => e.RepresentativeEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RepresentativeName).HasMaxLength(250);

                entity.Property(e => e.RepresentativePhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.TaxNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TrackingNotes).HasColumnType("ntext");
            });

            modelBuilder.Entity<CustomerCommission>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("CustomerCommission", "list");

                entity.Property(e => e.BankAccount)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BankName).HasMaxLength(250);

                entity.Property(e => e.BeneficiaryName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CustomerDelivery>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("CustomerDelivery", "list");

                entity.Property(e => e.CountryId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DeliveryAddress)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.DeliveryPlace)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Latitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Notes).HasMaxLength(250);
            });

            modelBuilder.Entity<CustomerDeliveryAddress>(entity =>
            {
                entity.ToTable("CustomerDeliveryAddress", "cal");

                entity.Property(e => e.DeliveryAdr).HasMaxLength(255);

                entity.Property(e => e.MaKhuVuc).HasMaxLength(10);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CustomerDetail>(entity =>
            {
                entity.HasKey(e => e.CusId);

                entity.ToTable("CustomerDetail", "cal");

                entity.Property(e => e.Adress).HasMaxLength(255);

                entity.Property(e => e.CccongNo)
                    .HasColumnName("CCCongNo")
                    .HasMaxLength(255);

                entity.Property(e => e.ContactName).HasMaxLength(255);

                entity.Property(e => e.CustomerCode).HasMaxLength(255);

                entity.Property(e => e.DeliveryAdr).HasMaxLength(255);

                entity.Property(e => e.DirectorName).HasMaxLength(50);

                entity.Property(e => e.Document)
                    .HasColumnName("document")
                    .HasMaxLength(50);

                entity.Property(e => e.DtlienHe)
                    .HasColumnName("DTLienHe")
                    .HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Employee)
                    .HasColumnName("employee")
                    .HasMaxLength(255);

                entity.Property(e => e.Fax).HasMaxLength(255);

                entity.Property(e => e.FullnameCus).HasMaxLength(255);

                entity.Property(e => e.Giogiao).HasMaxLength(20);

                entity.Property(e => e.HanTt).HasColumnName("HanTT");

                entity.Property(e => e.Khktoan)
                    .HasColumnName("KHKToan")
                    .HasMaxLength(255);

                entity.Property(e => e.KhuVuc).HasMaxLength(10);

                entity.Property(e => e.LienHeCvu)
                    .HasColumnName("LienHeCVu")
                    .HasMaxLength(20);

                entity.Property(e => e.MaKhuVuc).HasMaxLength(10);

                entity.Property(e => e.Manager).HasMaxLength(255);

                entity.Property(e => e.Na).HasColumnName("na");

                entity.Property(e => e.NameCus).HasMaxLength(255);

                entity.Property(e => e.NameCusFs)
                    .HasColumnName("NameCusFS")
                    .HasMaxLength(255);

                entity.Property(e => e.NgLienhe).HasMaxLength(50);

                entity.Property(e => e.NguoiKy).HasMaxLength(50);

                entity.Property(e => e.NguoiMua).HasMaxLength(255);

                entity.Property(e => e.NoiGuiCongno).HasMaxLength(255);

                entity.Property(e => e.Nvid).HasColumnName("NVId");

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.SoTaiKhoan).HasMaxLength(40);

                entity.Property(e => e.TaxCode).HasMaxLength(255);

                entity.Property(e => e.Tel).HasMaxLength(255);

                entity.Property(e => e.TenTat).HasMaxLength(255);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<CustomerProduct>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("CustomerProduct", "list");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<CustomerSale>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("CustomerSale", "list");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);
            });

            modelBuilder.Entity<CustomerType>(entity =>
            {
                entity.ToTable("CustomerType", "list");

                entity.HasIndex(e => e.CustomerTypeCode)
                    .HasName("IX_CustomerCode")
                    .IsUnique();

                entity.Property(e => e.CustomerTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerTypeName).HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.DistrictId).ValueGeneratedNever();

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DistrictName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.ToTable("ExchangeRate", "list");

                entity.Property(e => e.CurrencyId)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.ExchangeRating).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Notes).HasMaxLength(250);
            });

            modelBuilder.Entity<Fiscal>(entity =>
            {
                entity.Property(e => e.ClosePriceDate).HasColumnType("date");

                entity.Property(e => e.FiscalName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.Notes).HasMaxLength(4000);

                entity.Property(e => e.ToDate).HasColumnType("date");
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("Function", "sec");

                entity.Property(e => e.FunctionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.FunctionName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<GroupCompany>(entity =>
            {
                entity.ToTable("GroupCompany", "list");

                entity.Property(e => e.GroupCompanyCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GroupCompanyName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => new { e.Code, e.LangId });

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LangId)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Messages)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("Module", "sec");

                entity.Property(e => e.ModuleId).ValueGeneratedNever();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<OptionSystem>(entity =>
            {
                entity.HasKey(e => e.OptionId);

                entity.Property(e => e.DataType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.OptionName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OptionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<PackageUnit>(entity =>
            {
                entity.ToTable("PackageUnit", "item");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.PackageUnitName).HasMaxLength(100);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Page", "sec");

                entity.Property(e => e.ActionName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ControllerName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FormName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsCheckSecurity)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.PageName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Parameter)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentTerm>(entity =>
            {
                entity.ToTable("PaymentTerm", "pay");

                entity.Property(e => e.PaymentTermId).ValueGeneratedNever();

                entity.Property(e => e.PaymentTermName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission", "sec");

                entity.Property(e => e.FunctionId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PriceSpreadsheets>(entity =>
            {
                entity.ToTable("PriceSpreadsheets", "cal");

                entity.Property(e => e.CpbocXep).HasColumnName("CPBocXep");

                entity.Property(e => e.Cpgiao).HasColumnName("CPGiao");

                entity.Property(e => e.CpluuKho).HasColumnName("CPLuuKho");

                entity.Property(e => e.CpquanLyGt).HasColumnName("CPQuanLyGT");

                entity.Property(e => e.CpquanLyTt).HasColumnName("CPQuanLyTT");

                entity.Property(e => e.CptaiChinh).HasColumnName("CPTaiChinh");

                entity.Property(e => e.CpveKho).HasColumnName("CPVeKho");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DiaChiGh)
                    .HasColumnName("DiaChiGH")
                    .HasMaxLength(255);

                entity.Property(e => e.GiaCif).HasColumnName("GiaCIF");

                entity.Property(e => e.GiaVonGiaoToiKh).HasColumnName("GiaVonGiaoToiKH");

                entity.Property(e => e.IdColorCpbocXep).HasColumnName("IdColorCPBocXep");

                entity.Property(e => e.IdColorCpgiao).HasColumnName("IdColorCPGiao");

                entity.Property(e => e.IdColorCpluuKho).HasColumnName("IdColorCPLuuKho");

                entity.Property(e => e.IdColorCpquanLyGt).HasColumnName("IdColorCPQuanLyGT");

                entity.Property(e => e.IdColorCpquanLyTt).HasColumnName("IdColorCPQuanLyTT");

                entity.Property(e => e.IdColorCptaiChinh).HasColumnName("IdColorCPTaiChinh");

                entity.Property(e => e.IdColorCpveKho).HasColumnName("IdColorCPVeKho");

                entity.Property(e => e.IdColorGiaCif).HasColumnName("IdColorGiaCIF");

                entity.Property(e => e.IdColorTgluuKhoTb).HasColumnName("IdColorTGLuuKhoTB");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.KhuVucGh)
                    .HasColumnName("KhuVucGH")
                    .HasMaxLength(255);

                entity.Property(e => e.LoaiTien).HasMaxLength(5);

                entity.Property(e => e.MaHh)
                    .IsRequired()
                    .HasColumnName("MaHH")
                    .HasMaxLength(8);

                entity.Property(e => e.MaKh)
                    .HasColumnName("MaKH")
                    .HasMaxLength(8);

                entity.Property(e => e.MaKhuVuc).HasMaxLength(10);

                entity.Property(e => e.StoreId).HasDefaultValueSql("((1))");

                entity.Property(e => e.TenHh)
                    .HasColumnName("TenHH")
                    .HasMaxLength(255);

                entity.Property(e => e.TenKh)
                    .HasColumnName("TenKH")
                    .HasMaxLength(255);

                entity.Property(e => e.TgluuKhoTb).HasColumnName("TGLuuKhoTB");

                entity.Property(e => e.ThueNk).HasColumnName("ThueNK");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product", "item");

                entity.Property(e => e.Barcode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ItemType).HasDefaultValueSql("((1))");

                entity.Property(e => e.LargePhoto)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MadeIn)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.PackageUnit).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ProductCategoryList)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ProductNameShort).HasMaxLength(250);

                entity.Property(e => e.ProductType).HasDefaultValueSql("((1))");

                entity.Property(e => e.PurchasePrice).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Rfid)
                    .HasColumnName("RFID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(4000)
                    .IsUnicode(false);

                entity.Property(e => e.StandardCost).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.SupplierNotes).HasMaxLength(500);

                entity.Property(e => e.SupplierProductCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SupplierProductName).HasMaxLength(250);

                entity.Property(e => e.TermCondition).HasMaxLength(2000);

                entity.Property(e => e.ThumbnailPhoto)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.WeightUnit).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory", "item");

                entity.Property(e => e.ProductCategoryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.ParentListId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCategoryCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCategoryName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.SearchString).HasMaxLength(1000);
            });

            modelBuilder.Entity<ProductCifprice>(entity =>
            {
                entity.ToTable("ProductCIFPrice", "cal");

                entity.Property(e => e.Cifnorth).HasColumnName("CIFNorth");

                entity.Property(e => e.Cifsouth).HasColumnName("CIFSouth");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.LoaiTien).HasMaxLength(255);

                entity.Property(e => e.LoaiTienNorth).HasMaxLength(255);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.ToTable("ProductDetail", "cal");

                entity.Property(e => e.Application).HasColumnType("text");

                entity.Property(e => e.CertNo).HasMaxLength(50);

                entity.Property(e => e.ConvertKg).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.CpveKhoBn).HasColumnName("CPVeKhoBN");

                entity.Property(e => e.CpveKhoLh).HasColumnName("CPVeKhoLH");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CtyPhuTrach).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.Etaetd).HasColumnName("ETAETD");

                entity.Property(e => e.FullFcl).HasColumnName("FullFCL");

                entity.Property(e => e.GhiChuTonKho).HasMaxLength(255);

                entity.Property(e => e.GiaCif).HasColumnName("GiaCIF");

                entity.Property(e => e.GiaCifnorth).HasColumnName("GiaCIFNorth");

                entity.Property(e => e.GiaTb).HasColumnName("GiaTB");

                entity.Property(e => e.Group).HasMaxLength(20);

                entity.Property(e => e.IdKhmax).HasColumnName("Id_KHMax");

                entity.Property(e => e.IdKhmin).HasColumnName("Id_KHMin");

                entity.Property(e => e.ImportCode).HasMaxLength(20);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.KhMaxName).HasMaxLength(250);

                entity.Property(e => e.KhMinName).HasMaxLength(250);

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasMaxLength(255);

                entity.Property(e => e.Licenses).HasMaxLength(255);

                entity.Property(e => e.LoaiTien).HasMaxLength(10);

                entity.Property(e => e.LoaiTienNorth)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.National).HasMaxLength(50);

                entity.Property(e => e.NgayGiaMax).HasColumnName("Ngay_GiaMax");

                entity.Property(e => e.NgayGiaMin).HasColumnName("Ngay_GiaMin");

                entity.Property(e => e.Pack)
                    .HasColumnName("pack")
                    .HasMaxLength(50);

                entity.Property(e => e.Producer).HasMaxLength(30);

                entity.Property(e => e.ProductCode).HasMaxLength(8);

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.Property(e => e.ProductNameFull).HasMaxLength(255);

                entity.Property(e => e.ProductNamePlan).HasMaxLength(255);

                entity.Property(e => e.ProductNameTkhq)
                    .HasColumnName("ProductNameTKHQ")
                    .HasMaxLength(255);

                entity.Property(e => e.Remark).HasColumnType("text");

                entity.Property(e => e.Salesman).HasMaxLength(20);

                entity.Property(e => e.SearchString).HasMaxLength(1000);

                entity.Property(e => e.Spktname)
                    .HasColumnName("SPKTName")
                    .HasMaxLength(255);

                entity.Property(e => e.Supplier).HasMaxLength(50);

                entity.Property(e => e.SupplierCode).HasMaxLength(6);

                entity.Property(e => e.TgTonKhoTb).HasColumnName("TgTonKhoTB");

                entity.Property(e => e.Thutuxem).HasColumnName("thutuxem");

                entity.Property(e => e.ThutuxemB).HasColumnName("thutuxemB");

                entity.Property(e => e.Unit).HasMaxLength(10);

                entity.Property(e => e.Vatproduct).HasColumnName("VATProduct");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.ProvinceId).ValueGeneratedNever();

                entity.Property(e => e.CountryId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ReasonInOut>(entity =>
            {
                entity.HasKey(e => e.ReasonId);

                entity.ToTable("ReasonInOut", "inv");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsStockIn)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.ReasonName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<RefNo>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.Property(e => e.RowId).ValueGeneratedNever();

                entity.Property(e => e.FormateString).HasMaxLength(10);

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.RRefType).HasColumnName("rRefType");

                entity.Property(e => e.RefType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SequenceName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.SqlQueryRefNo)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.SqlQueryRefNoSql)
                    .HasColumnName("SqlQueryRefNoSQL")
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportColumnView>(entity =>
            {
                entity.ToTable("ReportColumnView", "cal");

                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ColumnCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColumnName).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ViewType).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ReportTotal>(entity =>
            {
                entity.ToTable("ReportTotal", "cal");

                entity.Property(e => e.CangNuocNgoai).HasMaxLength(250);

                entity.Property(e => e.CangXuatNhap).HasMaxLength(250);

                entity.Property(e => e.ChiCucHaiQuan).HasMaxLength(250);

                entity.Property(e => e.ChungLoaiHangHoaXuatNhap).HasColumnType("ntext");

                entity.Property(e => e.DieuKienGiaoHang)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DieuKienThanhToan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DoanhNghiepXuatNhap).HasMaxLength(250);

                entity.Property(e => e.DonViDoiTac).HasMaxLength(250);

                entity.Property(e => e.Dvt)
                    .HasColumnName("DVT")
                    .HasMaxLength(250);

                entity.Property(e => e.HsCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaDoanhNghiep)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgoaiTeThanhToan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NuocNhapKhau).HasMaxLength(250);

                entity.Property(e => e.NuocXuatKhau).HasMaxLength(250);

                entity.Property(e => e.NuocXuatXu).HasMaxLength(250);

                entity.Property(e => e.PhanLoai)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhanLoaiTrangThai).HasMaxLength(250);

                entity.Property(e => e.PhuongTienVanTai).HasMaxLength(250);

                entity.Property(e => e.SearchDoanhNghiepXuatNhap).HasColumnType("text");

                entity.Property(e => e.SearchDonViDoiTac).HasColumnType("text");

                entity.Property(e => e.SearchNuocXuatXu).HasColumnType("text");

                entity.Property(e => e.SearchString).HasColumnType("text");

                entity.Property(e => e.SoToKhai)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoHang).HasMaxLength(250);

                entity.Property(e => e.TenPhuongTienVanTai).HasMaxLength(250);

                entity.Property(e => e.ThueTtdb).HasColumnName("ThueTTDB");

                entity.Property(e => e.ThueVat).HasColumnName("ThueVAT");

                entity.Property(e => e.ThueXnk).HasColumnName("ThueXNK");

                entity.Property(e => e.TriGiaUsd).HasColumnName("TriGiaUSD");

                entity.Property(e => e.TriGiaVnd).HasColumnName("TriGiaVND");

                entity.Property(e => e.Tsttdb).HasColumnName("TSTTDB");

                entity.Property(e => e.Tsvat).HasColumnName("TSVAT");

                entity.Property(e => e.Tsxnk)
                    .HasColumnName("TSXNK")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TyGiaUsd).HasColumnName("TyGiaUSD");

                entity.Property(e => e.TyGiaVnd).HasColumnName("TyGiaVND");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "sec");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.RoleCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(4000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermission", "sec");
            });

            modelBuilder.Entity<SaleInvoice>(entity =>
            {
                entity.ToTable("SaleInvoice", "sale");

                entity.HasIndex(e => e.SaleInvoiceCode)
                    .HasName("IX_SaleInvoiceUnique")
                    .IsUnique();

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.AmountSub).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ChargeAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CurrencyId)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerAddress).HasMaxLength(250);

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerFax)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerName).HasMaxLength(250);

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DeliveryAddress).HasMaxLength(250);

                entity.Property(e => e.DeliveryNotes).HasMaxLength(250);

                entity.Property(e => e.DeliveryPlace).HasMaxLength(250);

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountItemAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountPercent).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountTotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ExchangeRate).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FeeAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.IsSaleInvoice)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Latitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.RRefCode)
                    .HasColumnName("rRefCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RRefType).HasColumnName("rRefType");

                entity.Property(e => e.ReferenceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SaleInvoiceCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.TaxAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalStandardCost).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<SaleInvoiceDetail>(entity =>
            {
                entity.ToTable("SaleInvoiceDetail", "sale");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ChargeAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountPercent).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountTotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ExpireDate).HasColumnType("date");

                entity.Property(e => e.LotNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ManufactureDate).HasColumnType("date");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ProductCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Quantity).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StandardCost).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TaxAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TaxPercent).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalStandardCost).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<SaleReturn>(entity =>
            {
                entity.ToTable("SaleReturn", "sale");

                entity.HasIndex(e => e.SaleReturnCode)
                    .HasName("IX_SaleReturn_Qnique")
                    .IsUnique();

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.AmountSub).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ChargeAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CustomerAddress).HasMaxLength(250);

                entity.Property(e => e.CustomerCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerName).HasMaxLength(250);

                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DeliveryAddress).HasMaxLength(250);

                entity.Property(e => e.DeliveryNotes).HasMaxLength(250);

                entity.Property(e => e.DeliveryPlace).HasMaxLength(250);

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountItemAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountPercent).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountTotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FeeAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Latitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.RRefCode)
                    .HasColumnName("rRefCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RRefType).HasColumnName("rRefType");

                entity.Property(e => e.ReferenceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SaleReturnCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.TaxAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalStandardCost).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<SaleReturnDetail>(entity =>
            {
                entity.ToTable("SaleReturnDetail", "sale");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ChargeAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountPercent).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiscountTotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ExpireDate).HasColumnType("date");

                entity.Property(e => e.LotNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ManufactureDate).HasColumnType("date");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ProductCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Quantity).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StandardCost).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TaxAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TaxPercent).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalAmount).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalStandardCost).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<Sales>(entity =>
            {
                entity.ToTable("Sales", "cal");

                entity.Property(e => e.GiaId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HanTt).HasColumnName("HanTT");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LoaiNgoaiTe).HasMaxLength(255);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShippedPrice>(entity =>
            {
                entity.ToTable("ShippedPrice", "cal");

                entity.Property(e => e.DienGiai).HasMaxLength(255);

                entity.Property(e => e.GhiChu).HasColumnType("text");

                entity.Property(e => e.GiaVc).HasColumnName("GiaVC");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MaDoGiaVc)
                    .IsRequired()
                    .HasColumnName("MaDoGiaVC")
                    .HasMaxLength(5);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stock", "inv");

                entity.HasIndex(e => e.StockCode)
                    .HasName("IX_Stock")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDefaultForPurchase)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDefaultForSale)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Latitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.SearchString).HasMaxLength(2000);

                entity.Property(e => e.StockCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StockName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store", "cal");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StoreName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.ToTable("Tax", "item");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.TaxCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.TaxPercent).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<TokenRefresh>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Refreshtoken)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Revoked).HasDefaultValueSql("((0))");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit", "item");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.UnitCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "acc");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.AdminCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Avatar)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PageDefault)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordExpire).HasColumnType("date");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.SecondPassword)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SignatureImage)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SystemLanguage)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.UserCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserEditableData>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("UserEditableData", "sec");

                entity.Property(e => e.OwnerListId).IsUnicode(false);
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.ToTable("UserPermission", "sec");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole", "sec");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.ToTable("Vendor", "list");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.BankingName).HasMaxLength(250);

                entity.Property(e => e.BankingNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyAddress).HasMaxLength(250);

                entity.Property(e => e.CompanyName).HasMaxLength(250);

                entity.Property(e => e.CurrencyId)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.DebtCeiling).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FaxNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RepresentativeAddress).HasMaxLength(250);

                entity.Property(e => e.RepresentativeEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RepresentativeName).HasMaxLength(250);

                entity.Property(e => e.RepresentativePhone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.TaxNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TrackingNote).HasColumnType("ntext");

                entity.Property(e => e.VendorCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Website)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VendorProduct>(entity =>
            {
                entity.HasKey(e => e.RowId);

                entity.ToTable("VendorProduct", "list");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.Price).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<VendorType>(entity =>
            {
                entity.ToTable("VendorType", "list");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasMaxLength(250);

                entity.Property(e => e.SearchString)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.VendorTypeCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VendorTypeName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.Property(e => e.WardId).ValueGeneratedNever();

                entity.Property(e => e.WardCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WardName)
                    .IsRequired()
                    .HasMaxLength(250);
            });
        }
    }
}
