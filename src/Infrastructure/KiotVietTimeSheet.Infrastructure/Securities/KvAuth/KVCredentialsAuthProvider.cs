using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.Infrastructure.Securities.KvAuth
{
    public class KVCredentialsAuthProvider : CredentialsAuthProvider
    {

        public KVCredentialsAuthProvider(IAppSettings appSettings) : base(appSettings)
        {
        }

        public override bool IsAuthorized(IAuthSession session, IAuthTokens tokens, Authenticate request = null)
        {
            return base.IsAuthorized(session, tokens, request);
        }
    }
}
