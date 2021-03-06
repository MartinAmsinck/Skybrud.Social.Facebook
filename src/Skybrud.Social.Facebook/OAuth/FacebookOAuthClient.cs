using System;
using System.Collections.Specialized;
using Skybrud.Essentials.Common;
using Skybrud.Social.Facebook.Endpoints.Raw;
using Skybrud.Social.Facebook.Responses.Authentication;
using Skybrud.Social.Facebook.Scopes;
using Skybrud.Social.Http;
using Skybrud.Social.Interfaces.Http;

namespace Skybrud.Social.Facebook.OAuth {

    /// <summary>
    /// Class for handling the raw communication with the Facebook Graph API as well as any OAuth 2.0 communication.
    /// </summary>
    public class FacebookOAuthClient : SocialHttpClient {
        
        #region Properties

        #region OAuth

        /// <summary>
        /// Gets or sets the client ID of the app.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret of the app.
        /// </summary>
        public string ClientSecret { get; set; }
        
        /// <summary>
        /// Gets or sets the redirect URI of your application.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the version of the Facebook Graph API to be used. Defaults to <code>v2.9</code>.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the locale of the client.
        /// </summary>
        public string Locale { get; set; }

        #region Endpoints

        /// <summary>
        /// Gets a reference to the accounts endpoint.
        /// </summary>
        public FacebookAccountsRawEndpoint Accounts { get; private set; }

        /// <summary>
        /// Gets a reference to the albums endpoint.
        /// </summary>
        public FacebookAlbumsRawEndpoint Albums { get; private set; }

        /// <summary>
        /// Gets a reference to the apps endpoint.
        /// </summary>
        public FacebookApplicationsRawEndpoint Applications { get; private set; }

        /// <summary>
        /// Gets a reference to the debug endpoint.
        /// </summary>
        public FacebookDebugRawEndpoint Debug { get; private set; }

        /// <summary>
        /// Gets a reference to the comments endpoint.
        /// </summary>
        public FacebookCommentsRawEndpoint Comments { get; private set; }

        /// <summary>
        /// Gets a reference to the events endpoint.
        /// </summary>
        public FacebookEventsRawEndpoint Events { get; private set; }

        /// <summary>
        /// Gets a reference to the feed endpoint.
        /// </summary>
        public FacebookFeedRawEndpoint Feed { get; private set; }

        /// <summary>
        /// Gets a reference to the likes endpoint.
        /// </summary>
        public FacebookLikesRawEndpoint Likes { get; private set; }

        /// <summary>
        /// Gets a reference to the pages endpoint.
        /// </summary>
        public FacebookPagesRawEndpoint Pages { get; private set; }

        /// <summary>
        /// Gets a reference to the photos endpoint.
        /// </summary>
        public FacebookPhotosRawEndpoint Photos { get; private set; }

        /// <summary>
        /// Gets a reference to the posts endpoint.
        /// </summary>
        public FacebookPostsRawEndpoint Posts { get; private set; }

        /// <summary>
        /// Gets a reference to the posts endpoint.
        /// </summary>
        public FacebookUsersRawEndpoint Users { get; private set; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an OAuth client with empty information.
        /// </summary>
        public FacebookOAuthClient() {
            Version = "v2.9";
            Accounts = new FacebookAccountsRawEndpoint(this);
            Applications = new FacebookApplicationsRawEndpoint(this);
            Debug = new FacebookDebugRawEndpoint(this);
            Comments = new FacebookCommentsRawEndpoint(this);
            Events = new FacebookEventsRawEndpoint(this);
            Feed = new FacebookFeedRawEndpoint(this);
            Likes = new FacebookLikesRawEndpoint(this);
            Pages = new FacebookPagesRawEndpoint(this);
            Photos = new FacebookPhotosRawEndpoint(this);
            Posts = new FacebookPostsRawEndpoint(this);
            Users = new FacebookUsersRawEndpoint(this);
            Albums = new FacebookAlbumsRawEndpoint(this);
        }

        /// <summary>
        /// Initializes an OAuth client with the specified <code>accessToken</code>. Using this initializer, the client
        /// will have no information about your app.
        /// </summary>
        /// <param name="accessToken">A valid access token.</param>
        public FacebookOAuthClient(string accessToken) : this() {
            if (String.IsNullOrWhiteSpace(accessToken)) throw new ArgumentNullException("accessToken");
            AccessToken = accessToken;
        }

        /// <summary>
        /// Initializes an OAuth client with the specified <code>clientId</code> and <code>clientSecret</code>.
        /// </summary>
        /// <param name="clientId">The client ID of the app.</param>
        /// <param name="clientSecret">The client secret of the app.</param>
        public FacebookOAuthClient(string clientId, string clientSecret) : this() {
            if (String.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException("clientId");
            if (String.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException("clientSecret");
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Initializes an OAuth client with the specified <code>clientId</code>, <code>clientSecret</code> and
        /// <code>redirectUri</code>.
        /// </summary>
        /// <param name="clientId">The client ID of the app.</param>
        /// <param name="clientSecret">The client secret of the app.</param>
        /// <param name="redirectUri">The redirect URI of the app.</param>
        public FacebookOAuthClient(string clientId, string clientSecret, string redirectUri) : this() {
            if (String.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException("clientId");
            if (String.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException("clientSecret");
            if (String.IsNullOrWhiteSpace(redirectUri)) throw new ArgumentNullException("redirectUri");
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the authorization URL using the specified state and scope.
        /// </summary>
        /// <param name="state">The state to send to Facebook's OAuth login page.</param>
        /// <param name="scope">The scope of the application.</param>
        /// <returns>Returns an authorization URL based on <code>state</code> and <code>scope</code>.</returns>
        public string GetAuthorizationUrl(string state, FacebookScopeCollection scope) {
            return GetAuthorizationUrl(state, scope == null ? null : scope.ToString());
        }

        /// <summary>
        /// Generates the authorization URL using the specified state and scope.
        /// </summary>
        /// <param name="state">The state to send to Facebook's OAuth login page.</param>
        /// <param name="scope">The scope of the application.</param>
        /// <returns>Returns an authorization URL based on <code>state</code> and <code>scope</code>.</returns>
        public string GetAuthorizationUrl(string state, params string[] scope) {

            // Some validation
            if (String.IsNullOrWhiteSpace(Version)) throw new PropertyNotSetException("Version");
            if (String.IsNullOrWhiteSpace(ClientId)) throw new PropertyNotSetException("ClientId");
            if (String.IsNullOrWhiteSpace(RedirectUri)) throw new PropertyNotSetException("RedirectUri");

            // Do we have a valid "state" ?
            if (String.IsNullOrWhiteSpace(state)) {
                throw new ArgumentNullException("state", "A valid state should be specified as it is part of the security of OAuth 2.0.");
            }

            return String.Format(
                "https://www.facebook.com/" + Version + "/dialog/oauth?client_id={0}&redirect_uri={1}&state={2}&scope={3}",
                ClientId,
                RedirectUri,
                state,
                String.Join(",", scope)
            );
        
        }

        /// <summary>
        /// Exchanges the specified authorization code for an access token.
        /// </summary>
        /// <param name="authCode">The authorization code received from the Facebook OAuth dialog.</param>
        /// <returns>Returns an instance of <see cref="FacebookTokenResponse"/> representing the response.</returns>
        public FacebookTokenResponse GetAccessTokenFromAuthCode(string authCode) {

            // Some validation
            if (String.IsNullOrWhiteSpace(Version)) throw new PropertyNotSetException("Version");
            if (String.IsNullOrWhiteSpace(ClientId)) throw new PropertyNotSetException("ClientId");
            if (String.IsNullOrWhiteSpace(ClientSecret)) throw new PropertyNotSetException("ClientSecret");
            if (String.IsNullOrWhiteSpace(RedirectUri)) throw new PropertyNotSetException("RedirectUri");
            if (String.IsNullOrWhiteSpace(authCode)) throw new ArgumentNullException("authCode");

            // Initialize the query string
            IHttpQueryString query = new SocialHttpQueryString();
            query.Add("client_id", ClientId);
            query.Add("redirect_uri", RedirectUri);
            query.Add("client_secret", ClientSecret);
            query.Add("code", authCode);

            // Make the call to the API
            SocialHttpResponse response = SocialUtils.Http.DoHttpGetRequest("https://graph.facebook.com/" + Version + "/oauth/access_token", query);
            
            // Parse the response
            return FacebookTokenResponse.ParseResponse(response);

        }

        /// <summary>
        /// Attempts to renew a user access token. The specified <code>currentToken</code> must be valid.
        /// </summary>
        /// <param name="currentToken">The current access token.</param>
        /// <returns>Returns an instance of <see cref="FacebookTokenResponse"/> representing the response.</returns>
        public FacebookTokenResponse RenewAccessToken(string currentToken) {

            // Some validation
            if (String.IsNullOrWhiteSpace(Version)) throw new PropertyNotSetException("Version");
            if (String.IsNullOrWhiteSpace(ClientId)) throw new PropertyNotSetException("ClientId");
            if (String.IsNullOrWhiteSpace(ClientSecret)) throw new PropertyNotSetException("ClientSecret");
            if (String.IsNullOrWhiteSpace(currentToken)) throw new ArgumentNullException("currentToken");

            // Initialize the query string
            IHttpQueryString query = new SocialHttpQueryString();
            query.Add("grant_type", "fb_exchange_token");
            query.Add("client_id", ClientId);
            query.Add("client_secret", ClientSecret);
            query.Add("fb_exchange_token", currentToken);

            // Make the call to the API
            SocialHttpResponse response = SocialUtils.Http.DoHttpGetRequest("https://graph.facebook.com/" + Version + "/oauth/access_token", query);

            // Parse the response
            return FacebookTokenResponse.ParseResponse(response);

        }

        /// <summary>
        /// Gets an app access token for for the application. The <see cref="ClientId"/> and <see cref="ClientSecret"/>
        /// properties must be specified for the OAuth client.
        /// </summary>
        /// <returns>Returns an instance of <see cref="FacebookTokenResponse"/> representing the response.</returns>
        public FacebookTokenResponse GetAppAccessToken() {

            // Some validation
            if (String.IsNullOrWhiteSpace(Version)) throw new PropertyNotSetException("Version");
            if (String.IsNullOrWhiteSpace(ClientId)) throw new PropertyNotSetException("ClientId");
            if (String.IsNullOrWhiteSpace(ClientSecret)) throw new PropertyNotSetException("ClientSecret");

            // Initialize the query string
            IHttpQueryString query = new SocialHttpQueryString();
            query.Add("client_id", ClientId);
            query.Add("client_secret", ClientSecret);
            query.Add("grant_type", "client_credentials");

            // Make the call to the API
            SocialHttpResponse response = SocialUtils.Http.DoHttpGetRequest("https://graph.facebook.com/" + Version + "/oauth/access_token", query);

            // Parse the response
            return FacebookTokenResponse.ParseResponse(response);

        }

        protected override void PrepareHttpRequest(SocialHttpRequest request) {

            // Some validation
            if (String.IsNullOrWhiteSpace(Version)) throw new PropertyNotSetException("Version");

            // Append the HTTP scheme and API version if not already specified.
            if (request.Url.StartsWith("/")) {
                request.Url = "https://graph.facebook.com/" + Version + request.Url;
            }
            
            // Append the access token to the query string if present in the client and not already
            // specified in the query string
            if (!request.QueryString.ContainsKey("access_token") && !String.IsNullOrWhiteSpace(AccessToken)) {
                request.QueryString.Add("access_token", AccessToken);
            }

            // Append the locale to the query string if present in the client and not already
            // specified in the query string
            if (!request.QueryString.ContainsKey("locale") && !String.IsNullOrWhiteSpace(Locale)) {
                request.QueryString.Add("locale", Locale);
            }

        }

        #endregion

    }

}