using QLite.Data.Dto;
using QLite.Data.Dto.Kapp;
using QLite.Data.Model;
using QLite.Data.Model.Kapp;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Data.Adapter.Kapp
{
    public interface IKappDataAdapter
    {
        LanguageDto GetLanguage(string languageOid);
        List<LanguageDto> GetLanguages();
        List<ResourceDto> GetResources(string? langId = null);
        List<KappSettingsDto> GetKappSettings(string parameter);
        T GetKappParameterValue<T>(string parameter, T defaultValue = default);
       
        


        QorchRestApiClient GetApiClient(string kioskHwId);

        
        Segment GetDefaultSegment(bool throwIfNotFound = true);
        KappUser GetUserInfo(string username);
        List<BranchDto> GetBranchList();
        Branch GetBranch(string branchId);
        List<DeskDto> GetDeskList(string branchId);
        List<KappSettingsDto> GetAccountSettings();
        void Test();
        Kiosk GetKiosk(string kioskHwId);
    }
}
