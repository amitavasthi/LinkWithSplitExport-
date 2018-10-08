using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCore.Items
{
    public class UserCollection : BaseClasses.BaseCollection<User>
    {
        #region Constructor

        public UserCollection(BaseClasses.BaseCore owner, string tableName,
            StorageMethodType storageMethod = StorageMethodType.Database)
            : base(owner, tableName, true, storageMethod)
        { }

        public UserCollection(
            BaseClasses.BaseCore owner,
            string tableName,
            string databaseProvider,
            string connectionString,
            StorageMethodType storageMethod = StorageMethodType.Database
        )
            : base(owner, tableName, databaseProvider, connectionString, true, storageMethod)
        { }

        #endregion


        #region Methods

        /// <summary>
        /// Validates the login details of an user.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns></returns>
        public User Valid(string username, string password, bool isEncrypted = false)
        {
            User result = base.GetSingle(
                new string[] { "Name", "Password" },
                new object[] { username, isEncrypted ? password : GetMD5Hash(password) }
            );

            return result;
        }

        /// <summary>
        /// Returns a MD5 hash as string.
        /// </summary>
        /// <param name="input">String to encrypt.</param>
        /// <returns>Hash as string.</returns>
        public string GetMD5Hash(string input)
        {
            // Check if there is an string to encrypt.
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // The MD5 encryption provider.
            MD5 md5 = new MD5CryptoServiceProvider();

            // Convert string to byte array.
            byte[] textToHash = Encoding.Default.GetBytes(input);

            // Calculate MD5 Hash. 
            byte[] result = md5.ComputeHash(textToHash);

            // Convert byte array to string.
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public bool HasPermission(Guid idUser, int idPermission)
        {
            // Get the user's role.
            object idRole = this.Owner.UserRoles.GetValue(
                "IdRole",
                "IdUser",
                idUser
            );

            if (idRole == null)
                return false;

            // Get the role permission by the role's id and permission id.
            RolePermission rolePermission = this.Owner.RolePermissions.GetSingle(
                new string[] { "IdRole", "Permission" },
                new object[] { idRole, idPermission }
            );

            if (rolePermission == null)
                return false;

            return true;
        }

        #endregion
    }
}
