using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace Common.AppGlobal
{
    public static class SessionParameters
    {

        public static int CriterioPesquisa { get; set; }
        public static int IndiceTipo { get; set; }
        public static int IndiceCriterio { get; set; }

        private static int _userId = 0;
        /// <summary>
        /// Gets or Sets UserId
        /// </summary>
        public static int UserID
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        private static UserRole _userRole;

        /// <summary>
        /// Gets or Sets UserRole
        /// </summary>
        public static UserRole UserRole
        {
            get
            {
                return _userRole;
            }
            set
            {
                _userRole = value;
            }
        }


        private static string _userName = String.Empty;

        /// <summary>
        /// Gets or Sets UserName
        /// </summary>
        public static string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
    }
}
