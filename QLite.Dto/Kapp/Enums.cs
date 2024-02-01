using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto.Kapp
{
    //Buradaki hatalar kapp seviyesinde olmalı. örneğin sıramatik mantığıyla ilgili bir hata kodu olmalı.
    public enum KappErrorCodes
    {
        ClientError,
        ServerError,
        UiTimeout,
        PlatformDeviceNotFound,
        WorkflowError,
        CancelledByUser,
        UnexpectedError,
        DataPresentError,
        EmergencyStop,
        IntrusionSafety,
        DeviceTimeout,
        RetryCountExceed,
        PrintError,
        DataMissingError,
        PlatformDirValError,
        PlatformDirTimeoutError,
        DeviceDirError,
        NetworkError,
        ServiceSqlAccessError,
        DeserializingError,
        HttpError,
        DirValidationError,
        CussAppProcessError,
        UserNotAuthenticated,
        NotFound,
        GettingClientSecretError,
        DbConnManError,
        MissingConfig,
        KioskNotFound,
        WorkflowConfigError,
        WfEvalError,
        ServiceTypeNotFound,
    }

    public enum WebSocketClientType
    {
        Display,
        MainDisplay,
        Kiosk,
        User
    }

    public enum SubStates
    {
        IdleStep,
        Default,
        EnableFromMediaOutput,
        DisableFromMediaOutput,
        OfferFromFeeder,

    }

    public enum PectabSettingsType
    {
        Pectab,
        Template,
        Logo,
        DataStream
    }

    public enum KioskActivityStatus
    {
        Active,
        Available,
        UnAvailable
    }
}
