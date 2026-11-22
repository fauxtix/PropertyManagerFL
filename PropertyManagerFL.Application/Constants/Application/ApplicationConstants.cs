namespace PropertyManagerFL.Application.Constants.Application
{
    public static class ApplicationConstants
    {
        public static class Cache
        {
            public const string GetAllBrandsCacheKey = "all-brands";
            public const string GetAllDocumentTypesCacheKey = "all-document-types";

            public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
            {
                return $"all-{entityFullName}-extended-attributes";
            }

            public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
            {
                return $"all-{entityFullName}-extended-attributes-{entityId}";
            }
        }

        public static class MimeTypes
        {
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}