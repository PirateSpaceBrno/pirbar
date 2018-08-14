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
        internal static Authentication AuthenticateHttpRequest(DatabaseHelper db, NetworkCredential auth, AccessLevel minAccessLevel, int identityId = 0, int accountId = 0)
        {
            if (auth != null)
            {
                var authentication = db.Get<Authentication>(DatabaseHelper.Tables.authentications, $"content='{TextHelper.SHA512(auth.Password)}'");

                if (authentication != null)
                {
                    AccessLevel authLevel = AccessLevel.None;

                    // Check authentication expiration
                    if (authentication.expiration != null)
                    {
                        var expiration = (DateTime)authentication.expiration;
                        if (expiration.Subtract(DateTime.Now).TotalSeconds <= 0)
                        {
                            db.Delete(DatabaseHelper.Tables.authentications, authentication);
                            authLevel = AccessLevel.None;
                        }
                    }

                    // Check authentication level
                    switch (auth.Domain)
                    {
                        // Credentials can be for Identity or Account (in Account case - username is empty)
                        case "creds":
                            if (authentication.account == null)
                            {
                                var identity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");
                                if (identity != null && authentication.identity == identity.id && identity.admin)
                                {
                                    authLevel = AccessLevel.Administrator;
                                }
                                else if (identity != null && authentication.identity == identity.id)
                                {
                                    if (identityId == 0 || (identityId != 0 && identityId == identity.id))
                                    {
                                        authLevel = AccessLevel.Identity;
                                    }
                                }
                            }
                            else
                            {
                                var account = db.Get<Account>(DatabaseHelper.Tables.accounts, $"id={authentication.account}");
                                if (accountId == 0 || (accountId != 0 && accountId == account.id))
                                {
                                    authLevel = AccessLevel.Account;
                                }
                            }
                            break;

                        // Token is for identity only
                        case "token":
                            if (authentication.account == null)
                            {
                                var identity = db.Get<Identity>(DatabaseHelper.Tables.identities, $"id='{authentication.identity}'");
                                if (identity != null && identity.admin)
                                {
                                    authLevel = AccessLevel.Administrator;
                                }
                                else if (identity != null)
                                {
                                    if (identityId == 0 || (identityId != 0 && identityId == authentication.identity))
                                    {
                                        authLevel = AccessLevel.Identity;
                                    }
                                }
                                
                            }
                            break;
                    }

                    // Compare minimal access level with authetication access level
                    if (authLevel >= minAccessLevel)
                    {
                        return authentication;
                    }
                }
            }

            return null;
        }

        internal static Token CreateToken(DatabaseHelper db, NetworkCredential auth)
        {
            if (auth != null && auth.Domain == "creds" && !string.IsNullOrEmpty(auth.UserName))
            {
                var authentication = AuthenticateHttpRequest(db, auth, AccessLevel.Identity);

                if (authentication != null)
                {
                    Identity ident = db.Get<Identity>(DatabaseHelper.Tables.identities, $"name='{auth.UserName}'");

                    try
                    {
                        db.BeginTransaction();

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
                        db.Insert(DatabaseHelper.Tables.authentications, newToken);

                        db.CompleteTransaction();

                        return new Token() { token = token, expiration = expiration };
                    }
                    catch (Exception ex)
                    {
                        db.AbortTransaction();
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return null;
        }

        internal enum AccessLevel
        {
            None,
            Account,
            Identity,
            Administrator
        }
    }
}
