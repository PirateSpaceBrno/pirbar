using PirBanka.Server.Models.Db;
using PirBanka.Server.Models.Get;
using System;
using System.Net;

namespace PirBanka.Server.Controllers
{
    internal static class HttpAuth
    {
        /// <summary>
        /// Authenticates HTTP request.
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="accessLevel"></param>
        /// <returns></returns>
        internal static Authentication AuthenticateHttpRequest(NetworkCredential auth, AccessLevel minAccessLevel, int identityId = 0, int accountId = 0)
        {
            if (auth != null)
            {
                var authentication = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"content='{TextHelper.SHA512(auth.Password)}'");
                var authLevel = GetAuthLevel(auth, authentication, identityId, accountId);

                // Compare minimal access level with authetication access level
                if (authLevel >= minAccessLevel)
                {
                    return authentication;
                }
            }

            return null;
        }

        internal static Token CreateToken(NetworkCredential auth, Authentication authentication)
        {
            if (auth != null && auth.Domain == "creds" && !string.IsNullOrEmpty(auth.UserName))
            {
                if (authentication != null)
                {
                    try
                    {
                        Server.db.BeginTransaction();

                        Identity ident = Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");

                        var token = Guid.NewGuid().ToString();
                        var now = DateTime.Now;
                        var expiration = now.AddMinutes(15);

                        // create new token with 15 minutes expiration
                        Authentication newToken = new Authentication()
                        {
                            identity = ident.id,
                            content = TextHelper.SHA512(token),
                            expiration = expiration,
                            created = now
                        };
                        Server.db.Insert(DatabaseHelper.Tables.authentications, newToken);
                        newToken = Server.db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"content='{TextHelper.SHA512(token)}'");

                        Server.db.CompleteTransaction();

                        return new Token() { token = token, expiration = expiration, authentications_id = newToken.id };
                    }
                    catch (Exception ex)
                    {
                        Server.db.AbortTransaction();
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return null;
        }

        private static AccessLevel GetAuthLevel(NetworkCredential auth, Authentication authentication, int identityId = 0, int accountId = 0)
        {
            if (authentication != null)
            {
                // Check authentication expiration
                if (authentication.expiration != null)
                {
                    var expiration = (DateTime)authentication.expiration;
                    if (expiration.Subtract(DateTime.Now).TotalSeconds <= 0)
                    {
                        return AccessLevel.Expired;
                    }
                    else
                    {
                        // If validation is not expired, extend it by next 15 minutes
                        try
                        {
                            Server.db.BeginTransaction();
                            authentication.expiration = DateTime.Now.AddMinutes(15);
                            Server.db.Update(DatabaseHelper.Tables.authentications, authentication);
                            Server.db.CompleteTransaction();
                        }
                        catch (Exception ex)
                        {
                            Server.db.AbortTransaction();
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                // Check authentication level
                switch (auth.Domain)
                {
                    // Credentials can be for Identity or Account (in Account case - username is empty)
                    case "creds":
                        if (authentication.account == null)
                        {
                            var identity = Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");
                            if (identity != null && authentication.identity == identity.id && identity.admin)
                            {
                                return AccessLevel.Administrator;
                            }
                            else if (identity != null && authentication.identity == identity.id)
                            {
                                if (identityId == 0 || (identityId != 0 && identityId == identity.id))
                                {
                                    return AccessLevel.Identity;
                                }
                            }
                        }
                        else
                        {
                            var account = Server.db.Get<Account>(DatabaseHelper.Tables.accounts, $"id={authentication.account}");
                            if (accountId == 0 || (accountId != 0 && accountId == account.id))
                            {
                                return AccessLevel.Account;
                            }
                        }
                        break;

                    // Token is for identity only
                    case "token":
                        if (authentication.account == null)
                        {
                            var identity = Server.db.Get<Identity>(DatabaseHelper.Tables.identities, $"id='{authentication.identity}'");
                            if (identity != null && identity.admin)
                            {
                                return AccessLevel.Administrator;
                            }
                            else if (identity != null)
                            {
                                if (identityId == 0 || (identityId != 0 && identityId == authentication.identity))
                                {
                                    return AccessLevel.Identity;
                                }
                            }

                        }
                        break;
                }
            }

            return AccessLevel.None;
        }

        internal enum AccessLevel
        {
            None,
            Expired,
            Account,
            Identity,
            Administrator
        }
    }
}
