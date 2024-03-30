namespace AdminPortal.Constants
{
    public class EndPoints
    {
        public static string AdminGetData(string modelName) =>
    $"api/Admin/GetData?modelName={modelName}";


        public static string GetdropDowns(string modelName) =>
  $"api/Admin/GetDropDowns?modelName={modelName}";


        public static string AdminDelete = "api/Admin/Delete";
        public static string AdminCreate = "api/Admin/Create";

        public static string AdminEdit = "api/Admin/Edit";

        public static string AdminGetCollection(string tabName, string modelName, string Oid) => $"api/Admin/GetDbSetCollection?tabName={tabName}&modelName={modelName}&Oid={Oid}";

        public static string AdminDeleteFromCollection = "api/Admin/DeleteFromDbSetCollection";


    }
}
