



namespace  QLiteDataApi.QueryFactory
{
    public interface IQueryFactory
    {
        IQuery GetModel(string type);
    }

    public class QueryFactory : IQueryFactory
    {

        public IQuery GetModel(string Type)
        {
            if (Type == null)
            {
                return null;
            }
            if (Type.Equals("VComponent"))
            {
                return  null;

            }
            else
            {
                return new Dynamic();

            }

            return null;

        }

    }
}
