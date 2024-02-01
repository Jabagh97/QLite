namespace AdminPortal.Constants
{
    public class EndPoints
    {
        public static string AdminGetData(string modelName, string searchValue, string sortColumn, string sortColumnDirection, int skip, int pageSize) =>
    $"api/Admin/GetData?modelName={modelName}&searchValue={searchValue}&sortColumn={sortColumn}&sortColumnDirection={sortColumnDirection}&skip={skip}&pageSize={pageSize}";


        public static string GetdropDowns(string modelName) =>
  $"api/Admin/GetDropDowns?modelName={modelName}";


        public static string AdminDelete = "api/Admin/Delete";
        public static string AdminCreate = "api/Admin/Create";

        public static string AdminEdit = "api/Admin/Edit";


    }
}
