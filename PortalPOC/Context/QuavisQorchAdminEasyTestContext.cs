using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;

namespace PortalPOC.Context;

public partial class QuavisQorchAdminEasyTestContext : DbContext
{
    public QuavisQorchAdminEasyTestContext()
    {
    }

    public QuavisQorchAdminEasyTestContext(DbContextOptions<QuavisQorchAdminEasyTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountLanguage> AccountLanguages { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentSetting> AppointmentSettings { get; set; }

    public virtual DbSet<AuditDataItemPersistent> AuditDataItemPersistents { get; set; }

    public virtual DbSet<AuditedObjectWeakReference> AuditedObjectWeakReferences { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<DashboardDatum> DashboardData { get; set; }

    public virtual DbSet<Design> Designs { get; set; }

    public virtual DbSet<DesignTarget> DesignTargets { get; set; }

    public virtual DbSet<Desk> Desks { get; set; }

    public virtual DbSet<DeskCreatableService> DeskCreatableServices { get; set; }

    public virtual DbSet<DeskMacroSchedule> DeskMacroSchedules { get; set; }

    public virtual DbSet<DeskStatus> DeskStatuses { get; set; }

    public virtual DbSet<DeskTransferableService> DeskTransferableServices { get; set; }

    public virtual DbSet<KappRelation> KappRelations { get; set; }

    public virtual DbSet<KappRole> KappRoles { get; set; }

    public virtual DbSet<KappSessionStep> KappSessionSteps { get; set; }

    public virtual DbSet<KappSetting> KappSettings { get; set; }

    public virtual DbSet<KappUser> KappUsers { get; set; }

    public virtual DbSet<KappWorkflow> KappWorkflows { get; set; }

    public virtual DbSet<KioskApplication> KioskApplications { get; set; }

    public virtual DbSet<KioskApplicationType> KioskApplicationTypes { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Macro> Macros { get; set; }

    public virtual DbSet<MacroRule> MacroRules { get; set; }

    public virtual DbSet<ModelDifference> ModelDifferences { get; set; }

    public virtual DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }

    public virtual DbSet<PermissionPolicyActionPermissionObject> PermissionPolicyActionPermissionObjects { get; set; }

    public virtual DbSet<PermissionPolicyMemberPermissionsObject> PermissionPolicyMemberPermissionsObjects { get; set; }

    public virtual DbSet<PermissionPolicyNavigationPermissionsObject> PermissionPolicyNavigationPermissionsObjects { get; set; }

    public virtual DbSet<PermissionPolicyObjectPermissionsObject> PermissionPolicyObjectPermissionsObjects { get; set; }

    public virtual DbSet<PermissionPolicyRole> PermissionPolicyRoles { get; set; }

    public virtual DbSet<PermissionPolicyTypePermissionsObject> PermissionPolicyTypePermissionsObjects { get; set; }

    public virtual DbSet<PermissionPolicyUser> PermissionPolicyUsers { get; set; }

    public virtual DbSet<PermissionPolicyUserLoginInfo> PermissionPolicyUserLoginInfos { get; set; }

    public virtual DbSet<PermissionPolicyUserUsersPermissionPolicyRoleRole> PermissionPolicyUserUsersPermissionPolicyRoleRoles { get; set; }

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

    public virtual DbSet<VDeskStatus> VDeskStatuses { get; set; }

    public virtual DbSet<VUserPerformanceReport> VUserPerformanceReports { get; set; }

    public virtual DbSet<XpobjectType> XpobjectTypes { get; set; }

    public virtual DbSet<XpweakReference> XpweakReferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Quavis.Qorch.AdminEasyTest;Integrated Security=True;");

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

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Appointment");

            entity.HasIndex(e => e.Branch, "iBranch_Appointment");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_Appointment");

            entity.HasIndex(e => e.Segment, "iSegment_Appointment");

            entity.HasIndex(e => e.ServiceType, "iServiceType_Appointment");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.NationalId)
                .HasMaxLength(100)
                .HasColumnName("NationalID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(100);

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_Appointment_Branch");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_Appointment_Segment");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_Appointment_ServiceType");
        });

        modelBuilder.Entity<AppointmentSetting>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.HasIndex(e => e.Branch, "iBranch_AppointmentSettings");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_AppointmentSettings");

            entity.HasIndex(e => e.ServiceType, "iServiceType_AppointmentSettings");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateUtc).HasColumnType("datetime");
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateUtc).HasColumnType("datetime");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.AppointmentSettings)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_AppointmentSettings_Branch");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.AppointmentSettings)
                .HasForeignKey(d => d.ServiceType)
                .HasConstraintName("FK_AppointmentSettings_ServiceType");
        });

        modelBuilder.Entity<AuditDataItemPersistent>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("AuditDataItemPersistent");

            entity.HasIndex(e => e.AuditedObject, "iAuditedObject_AuditDataItemPersistent");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_AuditDataItemPersistent");

            entity.HasIndex(e => e.ModifiedOn, "iModifiedOn_AuditDataItemPersistent");

            entity.HasIndex(e => e.NewObject, "iNewObject_AuditDataItemPersistent");

            entity.HasIndex(e => e.OldObject, "iOldObject_AuditDataItemPersistent");

            entity.HasIndex(e => e.OperationType, "iOperationType_AuditDataItemPersistent");

            entity.HasIndex(e => e.UserName, "iUserName_AuditDataItemPersistent");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(2048);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.NewValue).HasMaxLength(1024);
            entity.Property(e => e.OldValue).HasMaxLength(1024);
            entity.Property(e => e.OperationType).HasMaxLength(100);
            entity.Property(e => e.PropertyName).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.AuditedObjectNavigation).WithMany(p => p.AuditDataItemPersistents)
                .HasForeignKey(d => d.AuditedObject)
                .HasConstraintName("FK_AuditDataItemPersistent_AuditedObject");

            entity.HasOne(d => d.NewObjectNavigation).WithMany(p => p.AuditDataItemPersistentNewObjectNavigations)
                .HasForeignKey(d => d.NewObject)
                .HasConstraintName("FK_AuditDataItemPersistent_NewObject");

            entity.HasOne(d => d.OldObjectNavigation).WithMany(p => p.AuditDataItemPersistentOldObjectNavigations)
                .HasForeignKey(d => d.OldObject)
                .HasConstraintName("FK_AuditDataItemPersistent_OldObject");
        });

        modelBuilder.Entity<AuditedObjectWeakReference>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("AuditedObjectWeakReference");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.DisplayName).HasMaxLength(250);

            entity.HasOne(d => d.OidNavigation).WithOne(p => p.AuditedObjectWeakReference)
                .HasForeignKey<AuditedObjectWeakReference>(d => d.Oid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditedObjectWeakReference_Oid");
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

        modelBuilder.Entity<DashboardDatum>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_DashboardData");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.Title).HasMaxLength(100);
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

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.DeskMacroSchedules)
                .HasForeignKey(d => d.User)
                .HasConstraintName("FK_DeskMacroSchedule_User");
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

        modelBuilder.Entity<KappRole>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KappRole");

            entity.Property(e => e.Oid).ValueGeneratedNever();

            entity.HasOne(d => d.OidNavigation).WithOne(p => p.KappRole)
                .HasForeignKey<KappRole>(d => d.Oid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KappRole_Oid");
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

        modelBuilder.Entity<KappUser>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("KappUser");

            entity.HasIndex(e => e.Account, "iAccount_KappUser");

            entity.HasIndex(e => e.AuthorizedBranch, "iAuthorizedBranch_KappUser");

            entity.HasIndex(e => e.Branch, "iBranch_KappUser");

            entity.HasIndex(e => e.Desk, "iDesk_KappUser");

            entity.HasIndex(e => e.LastDesk, "iLastDesk_KappUser");

            entity.Property(e => e.Oid).ValueGeneratedNever();

            entity.HasOne(d => d.AccountNavigation).WithMany(p => p.KappUsers)
                .HasForeignKey(d => d.Account)
                .HasConstraintName("FK_KappUser_Account");

            entity.HasOne(d => d.AuthorizedBranchNavigation).WithMany(p => p.KappUserAuthorizedBranchNavigations)
                .HasForeignKey(d => d.AuthorizedBranch)
                .HasConstraintName("FK_KappUser_AuthorizedBranch");

            entity.HasOne(d => d.BranchNavigation).WithMany(p => p.KappUserBranchNavigations)
                .HasForeignKey(d => d.Branch)
                .HasConstraintName("FK_KappUser_Branch");

            entity.HasOne(d => d.DeskNavigation).WithMany(p => p.KappUserDeskNavigations)
                .HasForeignKey(d => d.Desk)
                .HasConstraintName("FK_KappUser_Desk");

            entity.HasOne(d => d.LastDeskNavigation).WithMany(p => p.KappUserLastDeskNavigations)
                .HasForeignKey(d => d.LastDesk)
                .HasConstraintName("FK_KappUser_LastDesk");

            entity.HasOne(d => d.OidNavigation).WithOne(p => p.KappUser)
                .HasForeignKey<KappUser>(d => d.Oid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KappUser_Oid");
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

        modelBuilder.Entity<ModelDifference>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("ModelDifference");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_ModelDifference");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.ContextId).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.UserId).HasMaxLength(100);
        });

        modelBuilder.Entity<ModelDifferenceAspect>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("ModelDifferenceAspect");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_ModelDifferenceAspect");

            entity.HasIndex(e => e.Owner, "iOwner_ModelDifferenceAspect");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.ModelDifferenceAspects)
                .HasForeignKey(d => d.Owner)
                .HasConstraintName("FK_ModelDifferenceAspect_Owner");
        });

        modelBuilder.Entity<PermissionPolicyActionPermissionObject>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyActionPermissionObject");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyActionPermissionObject");

            entity.HasIndex(e => e.Role, "iRole_PermissionPolicyActionPermissionObject");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.ActionId).HasMaxLength(100);
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.PermissionPolicyActionPermissionObjects)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("FK_PermissionPolicyActionPermissionObject_Role");
        });

        modelBuilder.Entity<PermissionPolicyMemberPermissionsObject>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyMemberPermissionsObject");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyMemberPermissionsObject");

            entity.HasIndex(e => e.TypePermissionObject, "iTypePermissionObject_PermissionPolicyMemberPermissionsObject");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

            entity.HasOne(d => d.TypePermissionObjectNavigation).WithMany(p => p.PermissionPolicyMemberPermissionsObjects)
                .HasForeignKey(d => d.TypePermissionObject)
                .HasConstraintName("FK_PermissionPolicyMemberPermissionsObject_TypePermissionObject");
        });

        modelBuilder.Entity<PermissionPolicyNavigationPermissionsObject>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyNavigationPermissionsObject");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyNavigationPermissionsObject");

            entity.HasIndex(e => e.Role, "iRole_PermissionPolicyNavigationPermissionsObject");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.PermissionPolicyNavigationPermissionsObjects)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("FK_PermissionPolicyNavigationPermissionsObject_Role");
        });

        modelBuilder.Entity<PermissionPolicyObjectPermissionsObject>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyObjectPermissionsObject");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyObjectPermissionsObject");

            entity.HasIndex(e => e.TypePermissionObject, "iTypePermissionObject_PermissionPolicyObjectPermissionsObject");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

            entity.HasOne(d => d.TypePermissionObjectNavigation).WithMany(p => p.PermissionPolicyObjectPermissionsObjects)
                .HasForeignKey(d => d.TypePermissionObject)
                .HasConstraintName("FK_PermissionPolicyObjectPermissionsObject_TypePermissionObject");
        });

        modelBuilder.Entity<PermissionPolicyRole>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyRole");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyRole");

            entity.HasIndex(e => e.ObjectType, "iObjectType_PermissionPolicyRole");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ObjectTypeNavigation).WithMany(p => p.PermissionPolicyRoles)
                .HasForeignKey(d => d.ObjectType)
                .HasConstraintName("FK_PermissionPolicyRole_ObjectType");
        });

        modelBuilder.Entity<PermissionPolicyTypePermissionsObject>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyTypePermissionsObject");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyTypePermissionsObject");

            entity.HasIndex(e => e.Role, "iRole_PermissionPolicyTypePermissionsObject");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.PermissionPolicyTypePermissionsObjects)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("FK_PermissionPolicyTypePermissionsObject_Role");
        });

        modelBuilder.Entity<PermissionPolicyUser>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyUser");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_PermissionPolicyUser");

            entity.HasIndex(e => e.ObjectType, "iObjectType_PermissionPolicyUser");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.ObjectTypeNavigation).WithMany(p => p.PermissionPolicyUsers)
                .HasForeignKey(d => d.ObjectType)
                .HasConstraintName("FK_PermissionPolicyUser_ObjectType");
        });

        modelBuilder.Entity<PermissionPolicyUserLoginInfo>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyUserLoginInfo");

            entity.HasIndex(e => new { e.LoginProviderName, e.ProviderUserKey }, "iLoginProviderNameProviderUserKey_PermissionPolicyUserLoginInfo").IsUnique();

            entity.HasIndex(e => e.User, "iUser_PermissionPolicyUserLoginInfo");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.LoginProviderName).HasMaxLength(100);
            entity.Property(e => e.ProviderUserKey).HasMaxLength(100);

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.PermissionPolicyUserLoginInfos)
                .HasForeignKey(d => d.User)
                .HasConstraintName("FK_PermissionPolicyUserLoginInfo_User");
        });

        modelBuilder.Entity<PermissionPolicyUserUsersPermissionPolicyRoleRole>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("PermissionPolicyUserUsers_PermissionPolicyRoleRoles");

            entity.HasIndex(e => new { e.Roles, e.Users }, "iRolesUsers_PermissionPolicyUserUsers_PermissionPolicyRoleRoles").IsUnique();

            entity.HasIndex(e => e.Roles, "iRoles_PermissionPolicyUserUsers_PermissionPolicyRoleRoles");

            entity.HasIndex(e => e.Users, "iUsers_PermissionPolicyUserUsers_PermissionPolicyRoleRoles");

            entity.Property(e => e.Oid)
                .ValueGeneratedNever()
                .HasColumnName("OID");

            entity.HasOne(d => d.RolesNavigation).WithMany(p => p.PermissionPolicyUserUsersPermissionPolicyRoleRoles)
                .HasForeignKey(d => d.Roles)
                .HasConstraintName("FK_PermissionPolicyUserUsers_PermissionPolicyRoleRoles_Roles");

            entity.HasOne(d => d.UsersNavigation).WithMany(p => p.PermissionPolicyUserUsersPermissionPolicyRoleRoles)
                .HasForeignKey(d => d.Users)
                .HasConstraintName("FK_PermissionPolicyUserUsers_PermissionPolicyRoleRoles_Users");
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

        modelBuilder.Entity<VDeskStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_DeskStatus");

            entity.Property(e => e.Account).HasMaxLength(100);
            entity.Property(e => e.AvarageTime).HasPrecision(0);
            entity.Property(e => e.Branch).HasMaxLength(100);
            entity.Property(e => e.TotalTime).HasPrecision(0);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<VUserPerformanceReport>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("V_UserPerformanceReport");

            entity.Property(e => e.AvgProcessTime).HasPrecision(0);
            entity.Property(e => e.AvgWaitingTime).HasPrecision(0);
            entity.Property(e => e.MaxProcessTime).HasPrecision(0);
            entity.Property(e => e.MaxWaitingTime).HasPrecision(0);
            entity.Property(e => e.MinProcessTime).HasPrecision(0);
            entity.Property(e => e.MinWaitingTime).HasPrecision(0);
            entity.Property(e => e.TotalProcessTime).HasColumnType("datetime");
            entity.Property(e => e.TotalWaitingTime).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<XpobjectType>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("XPObjectType");

            entity.HasIndex(e => e.TypeName, "iTypeName_XPObjectType").IsUnique();

            entity.Property(e => e.Oid).HasColumnName("OID");
            entity.Property(e => e.AssemblyName).HasMaxLength(254);
            entity.Property(e => e.TypeName).HasMaxLength(254);
        });

        modelBuilder.Entity<XpweakReference>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("XPWeakReference");

            entity.HasIndex(e => e.Gcrecord, "iGCRecord_XPWeakReference");

            entity.HasIndex(e => e.ObjectType, "iObjectType_XPWeakReference");

            entity.HasIndex(e => e.TargetType, "iTargetType_XPWeakReference");

            entity.Property(e => e.Oid).ValueGeneratedNever();
            entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");
            entity.Property(e => e.TargetKey).HasMaxLength(100);

            entity.HasOne(d => d.ObjectTypeNavigation).WithMany(p => p.XpweakReferenceObjectTypeNavigations)
                .HasForeignKey(d => d.ObjectType)
                .HasConstraintName("FK_XPWeakReference_ObjectType");

            entity.HasOne(d => d.TargetTypeNavigation).WithMany(p => p.XpweakReferenceTargetTypeNavigations)
                .HasForeignKey(d => d.TargetType)
                .HasConstraintName("FK_XPWeakReference_TargetType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
