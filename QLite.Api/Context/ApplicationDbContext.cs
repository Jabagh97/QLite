
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLite.Data.Models;
using QLite.Data.Models.Auth;

namespace QLiteDataApi.Context;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountLanguage> AccountLanguages { get; set; }




    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Country> Countries { get; set; }


    public virtual DbSet<Design> Designs { get; set; }

    public virtual DbSet<DesignTarget> DesignTargets { get; set; }

    public virtual DbSet<Desk> Desks { get; set; }

    public virtual DbSet<DeskCreatableService> DeskCreatableServices { get; set; }

    public virtual DbSet<DeskMacroSchedule> DeskMacroSchedules { get; set; }

    public virtual DbSet<DeskStatus> DeskStatuses { get; set; }

    public virtual DbSet<DeskTransferableService> DeskTransferableServices { get; set; }

    public virtual DbSet<KappRelation> KappRelations { get; set; }


    public virtual DbSet<KappSessionStep> KappSessionSteps { get; set; }

    public virtual DbSet<KappSetting> KappSettings { get; set; }


    public virtual DbSet<KappWorkflow> KappWorkflows { get; set; }

    public virtual DbSet<KioskApplication> KioskApplications { get; set; }

    public virtual DbSet<KioskApplicationType> KioskApplicationTypes { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Macro> Macros { get; set; }

    public virtual DbSet<MacroRule> MacroRules { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<QorchSession> QorchSessions { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<RestartProfile> RestartProfiles { get; set; }

    public virtual DbSet<Segment> Segments { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<SubProvince> SubProvinces { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketPool> TicketPools { get; set; }

    public virtual DbSet<TicketPoolProfile> TicketPoolProfiles { get; set; }

    public virtual DbSet<TicketState> TicketStates { get; set; }

    public virtual DbSet<UploadBo> UploadBos { get; set; }

  
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlLite("Data Source=queue.db");
    //    }
    //}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Account");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Account");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.Mail).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<AccountLanguage>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("AccountLanguage");

            entity.HasIndex(e => e.Account, "iAccount_AccountLanguage");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_AccountLanguage");

            entity.HasIndex(e => e.Language, "iLanguage_AccountLanguage");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.AccountLanguages)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_AccountLanguage_Account");

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.AccountLanguages)
                .HasForeignKey(d => d.Language)
                .HasConstraintName("FK_AccountLanguage_Language");
        });


      


        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Branch");

            entity.HasIndex(e => e.Account, "iAccount_Branch");

            entity.HasIndex(e => e.Country, "iCountry_Branch");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Branch");

            entity.HasIndex(e => e.Province, "iProvince_Branch");

            entity.HasIndex(e => e.SubProvince, "iSubProvince_Branch");

            entity.HasIndex(e => e.TicketPoolProfile, "iTicketPoolProfile_Branch");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Address2).HasMaxLength(100);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.BranchCode).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Terminal).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.Branches)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_Branch_Account");

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.Branches)
                .HasForeignKey(d => d.Country)
                .HasConstraintName("FK_Branch_Country");

            entity.HasOne(d => d.ProvinceNavigation).WithMany(p => p.Branches)
                .HasForeignKey(d => d.Province)
                .HasConstraintName("FK_Branch_Province");

            entity.HasOne(d => d.SubProvinceNavigation).WithMany(p => p.Branches)
                .HasForeignKey(d => d.SubProvince)
                .HasConstraintName("FK_Branch_SubProvince");

            entity.HasOne(d => d.TicketPoolProfileNavigation).WithMany(p => p.Branches)
                .HasForeignKey(d => d.TicketPoolProfile)
                .HasConstraintName("FK_Branch_TicketPoolProfile");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Country");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Country");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.LangCode).HasMaxLength(100);
            entity.Property(e => e.Mask).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneCode).HasMaxLength(100);
        });

      

        modelBuilder.Entity<Design>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Design");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Design");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.DesignTag).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.WfStep).HasMaxLength(100);
        });

        modelBuilder.Entity<DesignTarget>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("DesignTarget");

            entity.HasIndex(e => e.Design, "iDesign_DesignTarget");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_DesignTarget");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.DesignNavigation).WithMany(p => p.DesignTargets)
                .HasForeignKey(d => d.Design)
                .HasConstraintName("FK_DesignTarget_Design");
        });

        modelBuilder.Entity<Desk>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Desk");

            entity.HasIndex(e => e.Branch, "iBranch_Desk");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Desk");

            entity.HasIndex(e => e.Pano, "iPano_Desk");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.ActiveUser).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.DisplayNo).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.LastStateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.Desks)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_Desk_Branch");

            entity.HasOne(d => d.PanoNavigation).WithMany(p => p.Desks)
                .HasForeignKey(d => d.Pano)
                .HasConstraintName("FK_Desk_Pano");
        });

        modelBuilder.Entity<DeskCreatableService>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.HasIndex(e => e.Branch, "iBranch_DeskCreatableServices");

            entity.HasIndex(e => e.Desk, "iDesk_DeskCreatableServices");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_DeskCreatableServices");

            entity.HasIndex(e => e.ServiceType, "iServiceType_DeskCreatableServices");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.DeskCreatableServices)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_DeskCreatableServices_Branch");

            entity.HasOne(d => d.DeskNavigation).WithMany(p => p.DeskCreatableServices)
                .HasForeignKey(d => d.Desk)
                .HasConstraintName("FK_DeskCreatableServices_Desk");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.DeskCreatableServices)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_DeskCreatableServices_ServiceType");
        });

        modelBuilder.Entity<DeskMacroSchedule>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("DeskMacroSchedule");

            entity.HasIndex(e => e.Branch, "iBranch_DeskMacroSchedule");

            entity.HasIndex(e => e.Desk, "iDesk_DeskMacroSchedule");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_DeskMacroSchedule");

            entity.HasIndex(e => e.Macro, "iMacro_DeskMacroSchedule");

            entity.HasIndex(e => e.User, "iUser_DeskMacroSchedule");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.DeskMacroSchedules)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_DeskMacroSchedule_Branch");

            entity.HasOne(d => d.DeskNavigation).WithMany(p => p.DeskMacroSchedules)
                .HasForeignKey(d => d.Desk)
                .HasConstraintName("FK_DeskMacroSchedule_Desk");

            entity.HasOne(d => d.MacroNavigation).WithMany(p => p.DeskMacroSchedules)
                .HasForeignKey(d => d.Macro)
                .HasConstraintName("FK_DeskMacroSchedule_Macro");

            //entity.HasOne(d => d.UserNavigation).WithMany(p => p.DeskMacroSchedules)
            //    .HasForeignKey(d => d.User)
            //    .HasConstraintName("FK_DeskMacroSchedule_User");
        });

        modelBuilder.Entity<DeskStatus>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("DeskStatus");

            entity.HasIndex(e => e.Desk, "iDesk_DeskStatus");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_DeskStatus");

            entity.HasIndex(e => e.User, "iUser_DeskStatus");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.StateEndTime).HasColumnType("datetime");
            entity.Property(e => e.StateStartTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<DeskTransferableService>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.HasIndex(e => e.Branch, "iBranch_DeskTransferableServices");

            entity.HasIndex(e => e.Desk, "iDesk_DeskTransferableServices");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_DeskTransferableServices");

            entity.HasIndex(e => e.ServiceType, "iServiceType_DeskTransferableServices");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.DeskTransferableServices)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_DeskTransferableServices_Branch");

            entity.HasOne(d => d.DeskNavigation).WithMany(p => p.DeskTransferableServices)
                .HasForeignKey(d => d.Desk)
                .HasConstraintName("FK_DeskTransferableServices_Desk");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.DeskTransferableServices)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_DeskTransferableServices_ServiceType");
        });

        modelBuilder.Entity<KappRelation>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KappRelation");

            entity.HasIndex(e => e.Child, "iChild_KappRelation");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_KappRelation");

            entity.HasIndex(e => e.Parent, "iParent_KappRelation");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.ChildNavigation).WithMany(p => p.KappRelationChildNavigations)
                .HasForeignKey(d => d.Child)
                .HasConstraintName("FK_KappRelation_Child");

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.KappRelationParentNavigations)
                .HasForeignKey(d => d.Parent)
                .HasConstraintName("FK_KappRelation_Parent");
        });

       

        modelBuilder.Entity<KappSessionStep>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KappSessionStep");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_KappSessionStep");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Step).HasMaxLength(100);
            entity.Property(e => e.SubStep).HasMaxLength(100);
        });

        modelBuilder.Entity<KappSetting>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.HasIndex(e => e.Account, "iAccount_KappSettings");

            entity.HasIndex(e => e.Branch, "iBranch_KappSettings");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_KappSettings");

            entity.HasIndex(e => e.KioskApplication, "iKioskApplication_KappSettings");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Parameter).HasMaxLength(100);
            entity.Property(e => e.ParameterValue).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.KappSettings)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_KappSettings_Account");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.KappSettings)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_KappSettings_Branch");

            entity.HasOne(d => d.KioskApplicationNavigation).WithMany(p => p.KappSettings)
                .HasForeignKey(d => d.KioskApplication)
                .HasConstraintName("FK_KappSettings_KioskApplication");
        });

       


        modelBuilder.Entity<AppUser>(entity =>
        {
           

        });

        modelBuilder.Entity<KappWorkflow>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KappWorkflow");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_KappWorkflow");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<KioskApplication>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KioskApplication");

            entity.HasIndex(e => e.Account, "iAccount_KioskApplication");

            entity.HasIndex(e => e.Branch, "iBranch_KioskApplication");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_KioskApplication");

            entity.HasIndex(e => e.KappWorkflow, "iKappWorkflow_KioskApplication");

            entity.HasIndex(e => e.KioskApplicationType, "iKioskApplicationType_KioskApplication");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.DesignTag).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.HwId).HasMaxLength(100);
            entity.Property(e => e.KappName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.PlatformAuthClientId).HasMaxLength(100);
            entity.Property(e => e.PlatformAuthClientSecret).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.KioskApplications)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_KioskApplication_Account");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.KioskApplications)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_KioskApplication_Branch");

            entity.HasOne(d => d.KappWorkflowNavigation).WithMany(p => p.KioskApplications)
                .HasForeignKey(d => d.KappWorkflow)
                .HasConstraintName("FK_KioskApplication_KappWorkflow");

            entity.HasOne(d => d.KioskApplicationTypeNavigation).WithMany(p => p.KioskApplications)
                .HasForeignKey(d => d.KioskApplicationType)
                .HasConstraintName("FK_KioskApplication_KioskApplicationType");
        });

        modelBuilder.Entity<KioskApplicationType>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KioskApplicationType");

            entity.HasIndex(e => e.Account, "iAccount_KioskApplicationType");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_KioskApplicationType");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.KioskApplicationTypes)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_KioskApplicationType_Account");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Language");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Language");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.CultureInfo).HasMaxLength(100);
            entity.Property(e => e.EnglishName).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.LangCode).HasMaxLength(100);
            entity.Property(e => e.LocalName).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Macro>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Macro");

            entity.HasIndex(e => e.Account, "iAccount_Macro");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Macro");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.Macros)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_Macro_Account");
        });

        modelBuilder.Entity<MacroRule>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("MacroRule");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_MacroRule");

            entity.HasIndex(e => e.Macro, "iMacro_MacroRule");

            entity.HasIndex(e => e.Segment, "iSegment_MacroRule");

            entity.HasIndex(e => e.ServiceType, "iServiceType_MacroRule");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.MacroNavigation).WithMany(p => p.MacroRules)
                .HasForeignKey(d => d.Macro)
                .HasConstraintName("FK_MacroRule_Macro");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.MacroRules)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_MacroRule_Segment");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.MacroRules)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_MacroRule_ServiceType");
        });

      
     

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Province");

            entity.HasIndex(e => e.Country, "iCountry_Province");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Province");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.Provinces)
                .HasForeignKey(d => d.Country)
                .HasConstraintName("FK_Province_Country");
        });

        modelBuilder.Entity<QorchSession>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("QorchSession");

            entity.HasIndex(e => e.Account, "iAccount_QorchSession");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_QorchSession");

            entity.HasIndex(e => e.KioskApplication, "iKioskApplication_QorchSession");

            entity.HasIndex(e => e.Segment, "iSegment_QorchSession");

            entity.HasIndex(e => e.ServiceType, "iServiceType_QorchSession");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.CurrentStep).HasMaxLength(100);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.EndTimeUtc).HasColumnType("datetime");
            entity.Property(e => e.Error).HasMaxLength(400);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.InputInfo).HasMaxLength(100);
            entity.Property(e => e.InputType).HasMaxLength(100);
            entity.Property(e => e.InputValue).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.StartTimeUtc).HasColumnType("datetime");

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.QorchSessions)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_QorchSession_Account");

            entity.HasOne(d => d.KioskApplicationNavigation).WithMany(p => p.QorchSessions)
                .HasForeignKey(d => d.KioskApplication)
                .HasConstraintName("FK_QorchSession_KioskApplication");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.QorchSessions)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_QorchSession_Segment");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.QorchSessions)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_QorchSession_ServiceType");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Resource");

            entity.HasIndex(e => e.Account, "iAccount_Resource");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Resource");

            entity.HasIndex(e => e.Language, "iLanguage_Resource");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Parameter).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.Resources)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_Resource_Account");

            entity.HasOne(d => d.LanguageNavigation).WithMany(p => p.Resources)
                .HasForeignKey(d => d.Language)
                .HasConstraintName("FK_Resource_Language");
        });

        modelBuilder.Entity<RestartProfile>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("RestartProfile");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_RestartProfile");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RestartTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Segment>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Segment");

            entity.HasIndex(e => e.Account, "iAccount_Segment");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Segment");

            entity.HasIndex(e => e.Parent, "iParent_Segment");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Prefix).HasMaxLength(50);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.Segments)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_Segment_Account");

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.InverseParentNavigation)
                .HasForeignKey(d => d.Parent)
                .HasConstraintName("FK_Segment_Parent");
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("ServiceType");

            entity.HasIndex(e => e.Account, "iAccount_ServiceType");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_ServiceType");

            entity.HasIndex(e => e.Parent, "iParent_ServiceType");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.ServiceTypes)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_ServiceType_Account");

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.InverseParentNavigation)
                .HasForeignKey(d => d.Parent)
                .HasConstraintName("FK_ServiceType_Parent");
        });

        modelBuilder.Entity<SubProvince>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("SubProvince");

            entity.HasIndex(e => e.Country, "iCountry_SubProvince");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_SubProvince");

            entity.HasIndex(e => e.Province, "iProvince_SubProvince");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CountryNavigation).WithMany(p => p.SubProvinces)
                .HasForeignKey(d => d.Country)
                .HasConstraintName("FK_SubProvince_Country");

            entity.HasOne(d => d.ProvinceNavigation).WithMany(p => p.SubProvinces)
                .HasForeignKey(d => d.Province)
                .HasConstraintName("FK_SubProvince_Province");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Ticket");

            entity.HasIndex(e => e.Branch, "iBranch_Ticket");

            entity.HasIndex(e => e.CurrentDesk, "iCurrentDesk_Ticket");

            entity.HasIndex(e => e.Desk, "iDesk_Ticket");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Ticket");

            entity.HasIndex(e => e.Segment, "iSegment_Ticket");

            entity.HasIndex(e => e.ServiceType, "iServiceType_Ticket");

            entity.HasIndex(e => e.ToDesk, "iToDesk_Ticket");

            entity.HasIndex(e => e.ToServiceType, "iToServiceType_Ticket");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CardNo).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.CustomerInfo).HasMaxLength(100);
            entity.Property(e => e.CustomerNo).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.LangCode).HasMaxLength(4);
            entity.Property(e => e.LastOprTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.NationalId).HasMaxLength(100);
            entity.Property(e => e.SegmentName).HasMaxLength(100);
            entity.Property(e => e.ServiceTypeName).HasMaxLength(100);
            entity.Property(e => e.TicketNote).HasMaxLength(100);

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_Ticket_Branch");

            entity.HasOne(d => d.CurrentDeskNavigation).WithMany(p => p.TicketCurrentDeskNavigations)
                .HasForeignKey(d => d.CurrentDesk)
                .HasConstraintName("FK_Ticket_CurrentDesk");

            entity.HasOne(d => d.DeskNavigation).WithMany(p => p.TicketDeskNavigations)
                .HasForeignKey(d => d.Desk)
                .HasConstraintName("FK_Ticket_Desk");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_Ticket_Segment");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.TicketServiceTypeNavigations)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_Ticket_ServiceType");

            entity.HasOne(d => d.ToDeskNavigation).WithMany(p => p.TicketToDeskNavigations)
                .HasForeignKey(d => d.ToDesk)
                .HasConstraintName("FK_Ticket_ToDesk");

            entity.HasOne(d => d.ToServiceTypeNavigation).WithMany(p => p.TicketToServiceTypeNavigations)
                .HasForeignKey(d => d.ToServiceType)
                .HasConstraintName("FK_Ticket_ToServiceType");
        });

        modelBuilder.Entity<TicketPool>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("TicketPool");

            entity.HasIndex(e => e.Account, "iAccount_TicketPool");

            entity.HasIndex(e => e.Branch, "iBranch_TicketPool");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_TicketPool");

            entity.HasIndex(e => e.KioskApplication, "iKioskApplication_TicketPool");

            entity.HasIndex(e => e.Segment, "iSegment_TicketPool");

            entity.HasIndex(e => e.ServiceType, "iServiceType_TicketPool");

            entity.HasIndex(e => e.TicketPoolProfile, "iTicketPoolProfile_TicketPool");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.BreakEndTime).HasColumnType("datetime");
            entity.Property(e => e.BreakStartTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.MaxWaitingTicketCountControlTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.ServiceCode).HasMaxLength(1);
            entity.Property(e => e.ServiceEndTime).HasColumnType("datetime");
            entity.Property(e => e.ServiceStartTime).HasColumnType("datetime");

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.TicketPools)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_TicketPool_Account");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.TicketPools)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_TicketPool_Branch");

            entity.HasOne(d => d.KioskApplicationNavigation).WithMany(p => p.TicketPools)
                .HasForeignKey(d => d.KioskApplication)
                .HasConstraintName("FK_TicketPool_KioskApplication");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.TicketPools)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_TicketPool_Segment");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.TicketPools)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_TicketPool_ServiceType");

            entity.HasOne(d => d.TicketPoolProfileNavigation).WithMany(p => p.TicketPools)
                .HasForeignKey(d => d.TicketPoolProfile)
                .HasConstraintName("FK_TicketPool_TicketPoolProfile");
        });

        modelBuilder.Entity<TicketPoolProfile>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("TicketPoolProfile");

            entity.HasIndex(e => e.Account, "iAccount_TicketPoolProfile");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_TicketPoolProfile");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.TicketPoolProfiles)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_TicketPoolProfile_Account");
        });

        modelBuilder.Entity<TicketState>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("TicketState");

            entity.HasIndex(e => e.Branch, "iBranch_TicketState");

            entity.HasIndex(e => e.Desk, "iDesk_TicketState");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_TicketState");

            entity.HasIndex(e => e.Segment, "iSegment_TicketState");

            entity.HasIndex(e => e.ServiceType, "iServiceType_TicketState");

            entity.HasIndex(e => e.Ticket, "iTicket_TicketState");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CallingRuleDescription).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.KioskAppId).HasColumnName("KioskAppID");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.SegmentName).HasMaxLength(100);
            entity.Property(e => e.ServiceTypeName).HasMaxLength(100);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.TicketStates)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_TicketState_Branch");

            entity.HasOne(d => d.DeskNavigation).WithMany(p => p.TicketStates)
                .HasForeignKey(d => d.Desk)
                .HasConstraintName("FK_TicketState_Desk");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.TicketStates)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_TicketState_Segment");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.TicketStates)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_TicketState_ServiceType");

            entity.HasOne(d => d.TicketNavigation).WithMany(p => p.TicketStates)
                .HasForeignKey(d => d.Ticket)
                .HasConstraintName("FK_TicketState_Ticket");
        });

        modelBuilder.Entity<UploadBo>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("UploadBO");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_UploadBO");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
        });





        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
