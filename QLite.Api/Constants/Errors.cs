namespace QLiteDataApi.Constants
{
    public class Errors
    {
        public const string NullModel = "Model instance cannot be null.";

        public const string NullEntity = "Entity to update cannot be null.";

        public const string NullOid = "Primary key 'Oid' not found.";

        public const string EntityNotFound = "Entity not found.";

        public const string SoftDeleteNotSuppurted = "Soft delete is not supported for";

        public const string ErrorGettingDbSet = "Error getting DbSet for type";


        public const string NotCreated = "Failed to create model instance.";


        public const string NoModelType = "Model type not provided.";

        public const string ModelNotFound = "Model type not found.";

        public const string NoMatchingViewModel = "No matching ViewModel type found for inner type";


        public const string NotCollection = "is not of type ICollection<T>.";


        public const string InvalidReportType = "Invalid or unsupported report type";

    }
}
